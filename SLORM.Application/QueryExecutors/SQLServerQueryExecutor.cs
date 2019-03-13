using System;
using System.Collections.Generic;
using System.Data;
using SLORM.Application.QueryBuilders;
using SLORM.Application.ValueObjects;

namespace SLORM.Application.QueryExecutors
{
    internal class SQLServerQueryExecutor : IQueryExecutor
    {
        private readonly IQueryBuilder queryBuilder;

        public SQLServerQueryExecutor(IQueryBuilderResolver queryBuilderResolver)
        {
            this.queryBuilder = queryBuilderResolver.ResolveFromSQLProvider(Enums.SQLProvider.SQLServer);
        }

        public ICollection<TableColumn> GetTableColumns(IDbConnection connection, string tableName)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            var query = queryBuilder.GetTableDescriptionQuery(tableName);

            connection.Open();

            return new List<TableColumn>();
        }
    }
}
