using SLORM.Application.Enums;
using SLORM.Application.Exceptions;
using SLORM.Application.Extensions;
using SLORM.Application.QueryBuilders;
using SLORM.Application.QueryExecutors;
using SLORM.Application.ValueObjects;
using SLORM.Application.ValueObjects.SpecialCases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace SLORM.Application.Contexts
{
    public class SLORMContext : IDisposable
    {
        public string TableName { get; }
        public ICollection<TableColumn> ColumnsInTable { get; }
        internal SQLProvider Provider { get; }
        internal IDbConnection Connection { get; }
        private IQueryExecutor queryExecutor { get; set; }

        internal ICollection<TableColumn> ColumnsToGroupBy { get; } = new List<TableColumn>();
        internal ICollection<TableColumn> ColumnsToCount { get; } = new List<TableColumn>();
        internal ICollection<TableColumn> ColumnsToSum { get; } = new List<TableColumn>();
        internal ICollection<ColumnFilter> ColumnsToFilter { get; } = new List<ColumnFilter>();
        internal ICollection<ColumnOrdering> ColumnsToOrderBy { get; } = new List<ColumnOrdering>();

        public SLORMContext(IDbConnection connection, string tableName)
        {
            this.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.Provider = getSQLProviderFromConnectionString();
            this.TableName = !string.IsNullOrWhiteSpace(tableName) ? tableName : throw new ArgumentNullException(nameof(tableName));

            var queryExecutorResolver = (IQueryExecutorResolver)Lifecycle.Container.GetInstance(typeof(IQueryExecutorResolver));
            this.queryExecutor = queryExecutorResolver.GetQueryExecutorFromProviderType(this.Provider);

            this.ColumnsInTable = queryExecutor.GetTableColumns(connection, tableName).Result;
        }

        private SQLProvider getSQLProviderFromConnectionString()
        {
            if (isSqlServerConnectionString(Connection.ConnectionString))
                return SQLProvider.SQLServer;
            else
                throw new UnknownSQLProviderException(Connection.ConnectionString);
        }

        private bool isSqlServerConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            // This is based on observations of the connection string properties
            // TODO: Improve how we detect if connection string is sql server
            var hasInitialCatalogProperty = connectionString.IndexOf("Initial Catalog") != -1;
            var hasDatabaseProperty = connectionString.IndexOf("Database") != -1;
            var hasUidProperty = connectionString.IndexOf("Uid") != -1;
            return (hasInitialCatalogProperty || hasDatabaseProperty) && !hasUidProperty;
        }

        public SLORMContext GroupBy(params string[] columnNames)
        {
            if (columnNames == null)
                throw new ArgumentNullException(nameof(columnNames));

            foreach (var currentColumnName in columnNames)
            {
                var column = ColumnsInTable.GetFromName(currentColumnName);
                if (column == null)
                    continue;

                var columnAlreadyInList = ColumnsToGroupBy.GetFromName(column.Name) != null;
                if (columnAlreadyInList)
                    continue;

                ColumnsToGroupBy.Add(column);
            }
            return this;
        }

        public SLORMContext Count(params string[] columnNames)
        {
            if (columnNames == null)
                throw new ArgumentNullException(nameof(columnNames));

            foreach (var currentColumnName in columnNames)
            {
                var column = ColumnsInTable.GetFromName(currentColumnName);
                if (column == null)
                    continue;

                var columnAlreadyInList = ColumnsToCount.GetFromName(column.Name) != null;
                if (columnAlreadyInList)
                    continue;

                ColumnsToCount.Add(column);
            }
            return this;
        }

        public SLORMContext Filter(ColumnFilterRequest filterRequest)
        {
            if (filterRequest == null)
                throw new ArgumentNullException(nameof(filterRequest));

            var column = ColumnsInTable.GetFromName(filterRequest.ColumnName);
            if (column == null)
                return this;

            // Filtering incorrect value formats
            if (filterRequest.FilterRigor == FilterRigor.Equals)
                switch (column.DataType)
                {
                    case ColumnDataType.Number:
                        double parsedValue;
                        foreach (var currentValue in filterRequest.Values)
                            if (!double.TryParse(currentValue, out parsedValue))
                                return this;
                        break;
                    case ColumnDataType.Date:
                        DateTime parsedDateValue;
                        foreach (var currentValue in filterRequest.Values)
                            if (!DateTime.TryParse(currentValue, out parsedDateValue))
                                return this;
                        break;
                }

            var columnToFilter = new ColumnFilter(column, filterRequest);
            ColumnsToFilter.Add(columnToFilter);

            return this;
        }

        public SLORMContext Filter(string columnName, ICollection<string> values, FilterRigor rigor, FilterMethod method)
        {
            var filterRequest = new ColumnFilterRequest(columnName, values, rigor, method);
            return Filter(filterRequest);
        }

        public SLORMContext OrderBy(ColumnOrderingRequest orderingRequest)
        {
            var column = ColumnsInTable.GetFromName(orderingRequest.ColumnName);
            if (column == null)
                return this;

            var alreadyGroupedByColumn = ColumnsToGroupBy.GetFromName(column.Name) != null
                   || ColumnsToSum.GetFromName(column.Name) != null
                   || ColumnsToCount.GetFromName(column.Name) != null;
            if (!alreadyGroupedByColumn)
                GroupBy(column.Name);

            var columnOrdering = new ColumnOrdering(column, orderingRequest);
            ColumnsToOrderBy.Add(columnOrdering);

            return this;
        }

        public SLORMContext OrderBy(string columnName, OrderType orderType)
        {
            var orderingRequest = new ColumnOrderingRequest(columnName, orderType);
            return OrderBy(orderingRequest);
        }

        public SLORMContext Sum(string columnName)
        {
            var column = ColumnsInTable.GetFromName(columnName);
            if (column == null || column.DataType != ColumnDataType.Number)
                return this;

            var columnAlreadyInList = ColumnsToSum.GetFromName(column.Name) != null;
            if (columnAlreadyInList)
                return this;

            ColumnsToSum.Add(column);
            return this;
        }

        public async Task<QueryResult> Query(int timeoutInSeconds = 30)
        {
            if (!IsQueryable())
                return new InvalidQueryResult();

            if (timeoutInSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(timeoutInSeconds));

            var result = await queryExecutor.Query(this, timeoutInSeconds);
            return result;
        }

        public bool IsQueryable()
        {
            if (!ColumnsInTable.Any())
                return false;
            if (!ColumnsToGroupBy.Any() && !ColumnsToCount.Any())
                return false;
            return true;
        }

        public async static Task<ICollection<string>> GetDatabasesInServer(IDbConnection connection)
        {
            // TODO: Improve this
            // This method should be in the query builder.

            var tableList = new List<string>();

            await connection.EnsureConnected();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT name FROM master.sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb');";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        tableList.Add(reader[0].ToString());
                }
            }
            return tableList;
        }

        public async Task<ICollection<string>> GetColumnUniqueValues(string columnName)
        {
            // TODO: This method should be in the query builder.

            if (string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentNullException(columnName);

            var uniqueValues = new List<string>();
            if (!ColumnsInTable.Any(c => c.Name == columnName))
                return uniqueValues;

            await Connection.EnsureConnected();
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = $"SELECT DISTINCT({columnName}) FROM {TableName}";
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                        uniqueValues.Add(reader[0].ToString());
            }
            return uniqueValues;
        }

        public static async Task<ICollection<string>> GetTableNamesInDb(IDbConnection connection)
        {
            // This method should be in the query builder.

            var tableNames = new List<string>();
            await connection.EnsureConnected();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES";
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                        tableNames.Add(reader[0].ToString());
            }
            return tableNames;
        }

		public void Dispose()
		{
			Connection.Close();
			Connection.Dispose();
		}
	}
}