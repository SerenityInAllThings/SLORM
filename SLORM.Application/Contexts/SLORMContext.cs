using SLORM.Application.Enums;
using SLORM.Application.Exceptions;
using SLORM.Application.QueryBuilders;
using SLORM.Application.QueryExecutors;
using SLORM.Application.ValueObjects;
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
        private DbConnection connection { get; set; }
        private SQLProvider provider { get; set; }
        private IQueryExecutor queryExecutor { get; set; }
        private ICollection<TableColumn> columnsInTable { get; set; }
        private ICollection<TableColumn> columnsToGroupBy { get; set; }
        private ICollection<TableColumn> columnsToCount { get; set; }
        private ICollection<TableColumn> columnsToFilter { get; set; }
        private ICollection<TableColumn> columnsToOrderBy { get; set; }

        public SLORMContext(DbConnection connection, string tableName)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.provider = getSQLProviderFromConnectionString();
            this.TableName = !string.IsNullOrWhiteSpace(tableName) ? tableName : throw new ArgumentNullException(nameof(tableName));
            this.columnsToGroupBy = new List<TableColumn>();
            this.columnsToCount = new List<TableColumn>();
            this.columnsToFilter = new List<TableColumn>();
            this.columnsToOrderBy = new List<TableColumn>();

            var queryExecutorResolver = (IQueryExecutorResolver)Lifecycle.Container.GetInstance(typeof(IQueryExecutorResolver));
            this.queryExecutor = queryExecutorResolver.GetQueryExecutorFromProviderType(this.provider);

            this.columnsInTable = queryExecutor.GetTableColumns(connection, tableName).Result;
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

        public SLORMContext GroupBy(params string[] columns)
        {
            foreach (var currentColumn in columns)
            {
                var column = columnsInTable.FirstOrDefault(c => c.Name.ToLower() == currentColumn.ToLower());
                if (column == null)
                    continue;

                var columnAlreadyIntList = columnsToGroupBy.Any(c => c.Name == column.Name);
                if (columnAlreadyIntList)
                    continue;

                columnsToGroupBy.Add(column);
            }
            return this;
        }

        public SLORMContext Count(params string[] columns)
        {
            foreach (var currentColumn in columns)
            {
                var column = columnsInTable.FirstOrDefault(c => c.Name.ToLower() == currentColumn.ToLower());
                if (column == null)
                    continue;

                var columnAlreadyIntList = columnsToCount.Any(c => c.Name == column.Name);
                if (columnAlreadyIntList)
                    continue;

                columnsToCount.Add(column);
            }
            return this;
        }
    }
}