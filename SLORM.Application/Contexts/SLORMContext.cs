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
        private readonly string tableName;
        private IDbConnection connection { get; set; }
        private SQLProvider provider { get; set; }
        private IQueryExecutor queryExecutor { get; set; }
        private ICollection<string> columnsToSelect { get; set; }
        private ICollection<TableColumn> columnsInTable { get; set; }

        public SLORMContext(IDbConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.provider = getSQLProviderFromConnectionString();
            this.tableName = getTableNameFromConnectionString();
            this.columnsToSelect = new List<string>();

            var queryExecutorResolver = (IQueryExecutorResolver)Lifecycle.Container.GetInstance(typeof(IQueryExecutorResolver));
            this.queryExecutor = queryExecutorResolver.GetQueryExecutorFromProviderType(this.provider);

            this.columnsInTable = queryExecutor.GetTableColumns(connection, tableName);
        }

        private SQLProvider getSQLProviderFromConnectionString()
        {
            if (isSqlServerConnectionString(connection.ConnectionString))
                return SQLProvider.SQLServer;
            else
                throw new UnknownSQLProviderException(connection.ConnectionString);
        }

        private string getTableNameFromConnectionString()
        {
            switch (provider)
            {
                case SQLProvider.SQLServer:
                    return getTableNameFromSQLServerConnectionString(connection.ConnectionString);
                default:
                    throw new UnknownSQLProviderException(connection.ConnectionString);
            }
        }

        private static string getTableNameFromSQLServerConnectionString(string connectionString)
        {
            const string DatabasePropertyName = "Database";
            const string InitialCatalogPropertyName = "Initial Catalog";

            var connectionStringParts = connectionString.Split(';').Select(p => p.Trim());

            if (connectionString.IndexOf(DatabasePropertyName) != -1)
            {
                var databasePart = connectionStringParts.First(part => part.IndexOf(DatabasePropertyName) == 0);
                return databasePart.Split('=').Last().Trim();
            }
            else if (connectionString.IndexOf(InitialCatalogPropertyName) != -1)
            {
                var databasePart = connectionStringParts.First(part => part.IndexOf(InitialCatalogPropertyName) == 0);
                return databasePart.Split('=').Last().Trim();
            }
            throw new UnknownSQLProviderException(connectionString);
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

        public void Select(params string[] columns)
        {
            foreach (var currentColumn in columns)
                columnsToSelect.Add(currentColumn);
        }
    }
}