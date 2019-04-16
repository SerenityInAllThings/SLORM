using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SLORM.Application.Contexts;
using SLORM.Application.Extensions;
using SLORM.Application.QueryBuilders;
using SLORM.Application.ValueObjects;

namespace SLORM.Application.QueryExecutors
{
    internal class SQLServerQueryExecutor : IQueryExecutor
    {
        private static readonly string dataTypeColumnName = "DATA_TYPE";
        private static readonly string columnNameColumnName = "COLUMN_NAME";

        private readonly IQueryBuilder queryBuilder;
        private readonly ISQLServerDataTypeDeterminator dataTypeDeterminator;

        public SQLServerQueryExecutor(IQueryBuilderResolver queryBuilderResolver, ISQLServerDataTypeDeterminator dataTypeDeterminator)
        {
            if (queryBuilderResolver == null)
                throw new ArgumentNullException(nameof(queryBuilderResolver));

            this.queryBuilder = queryBuilderResolver.ResolveFromSQLProvider(Enums.SQLProvider.SQLServer);
            this.dataTypeDeterminator = dataTypeDeterminator ?? throw new ArgumentNullException(nameof(dataTypeDeterminator));
        }

        public async Task<ICollection<TableColumn>> GetTableColumns(IDbConnection connection, string tableName)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            var columnList = new List<TableColumn>();
            using (var query = queryBuilder.GetTableDescriptionQuery(tableName))
            {
                await connection.EnsureConnected();
                query.Connection = (SqlConnection)connection;
                using (var reader = await query.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var columnName = reader[columnNameColumnName] as string;
                        var rawColumnType = reader[dataTypeColumnName] as string;
                        var parsedType = dataTypeDeterminator.FromDataTypeField(rawColumnType);
                        columnList.Add(new TableColumn(columnName, parsedType));
                    }
                }
            }

            return columnList;
        }

        public async Task<QueryResult> Query(SLORMContext context, int timeoutInSeconds)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (timeoutInSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(timeoutInSeconds));

            using (var queryCommand = queryBuilder.GetReadQuery(context))
            {
                await context.Connection.EnsureConnected();
                queryCommand.Connection = (SqlConnection)context.Connection;
                queryCommand.CommandTimeout = timeoutInSeconds;
                var reader = await queryCommand.ExecuteReaderAsync();

                var result = new QueryResult(reader);
                return result;
            }
        }
    }
}
