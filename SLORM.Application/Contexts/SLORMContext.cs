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

namespace SLORM.Application.Contexts
{
    public class SLORMContext
    {
        public string TableName { get; private set; }
        internal SQLProvider Provider { get; private set; }
        internal ICollection<TableColumn> ColumnsInTable { get; private set; }
        private DbConnection connection { get; set; }
        private IQueryExecutor queryExecutor { get; set; }

        internal ICollection<TableColumn> ColumnsToGroupBy { get; private set; }
        internal ICollection<TableColumn> ColumnsToCount { get; private set; }
        internal ICollection<ColumnFilter> ColumnsToFilter { get; private set; }
        internal ICollection<ColumnOrdering> ColumnsToOrderBy { get; private set; }

        public SLORMContext(DbConnection connection, string tableName)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.Provider = getSQLProviderFromConnectionString();
            this.TableName = !string.IsNullOrWhiteSpace(tableName) ? tableName : throw new ArgumentNullException(nameof(tableName));
            this.ColumnsToGroupBy = new List<TableColumn>();
            this.ColumnsToCount = new List<TableColumn>();
            this.ColumnsToFilter = new List<ColumnFilter>();
            this.ColumnsToOrderBy = new List<ColumnOrdering>();

            var queryExecutorResolver = (IQueryExecutorResolver)Lifecycle.Container.GetInstance(typeof(IQueryExecutorResolver));
            this.queryExecutor = queryExecutorResolver.GetQueryExecutorFromProviderType(this.Provider);

            this.ColumnsInTable = queryExecutor.GetTableColumns(connection, tableName).Result;
        }

        private SQLProvider getSQLProviderFromConnectionString()
        {
            if (isSqlServerConnectionString(connection.ConnectionString))
                return SQLProvider.SQLServer;
            else
                throw new UnknownSQLProviderException(connection.ConnectionString);
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
            foreach (var currentColumnName in columnNames)
            {
                var column = ColumnsInTable.GetFromName(currentColumnName);
                if (column == null)
                    continue;

                var columnAlreadyIntList = ColumnsToGroupBy.GetFromName(column.Name) != null;
                if (columnAlreadyIntList)
                    continue;

                ColumnsToGroupBy.Add(column);
            }
            return this;
        }

        public SLORMContext Count(params string[] columnNames)
        {
            foreach (var currentColumnName in columnNames)
            {
                var column = ColumnsInTable.GetFromName(currentColumnName);
                if (column == null)
                    continue;

                var columnAlreadyIntList = ColumnsToGroupBy.GetFromName(column.Name) != null;
                if (columnAlreadyIntList)
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

            var columnToFilter = new ColumnFilter(column, filterRequest);
            ColumnsToFilter.Add(columnToFilter);

            return this;
        }

        public SLORMContext Filter(string columnName, string value, FilterRigor rigor, FilterMethod method)
        {
            var filterRequest = new ColumnFilterRequest(columnName, value, rigor, method);
            return Filter(filterRequest);
        }

        public SLORMContext OrderBy(ColumnOrderingRequest orderingRequest)
        {
            var column = ColumnsInTable.GetFromName(orderingRequest.ColumnName);
            if (column == null)
                return this;

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

        public void Query()
        {
            //if (!IsQueryable())
            //    return new InvalidQueryResult();
            queryExecutor.Query(this).Wait();
        }

        public bool IsQueryable()
        {
            if (!ColumnsInTable.Any())
                return false;
            if (!ColumnsToGroupBy.Any() && !ColumnsToCount.Any())
                return false;
            return true;
        }
    }
}