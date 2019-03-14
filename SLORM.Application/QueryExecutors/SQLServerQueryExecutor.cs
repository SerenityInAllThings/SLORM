using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
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

        public async Task<ICollection<TableColumn>> GetTableColumns(DbConnection connection, string tableName)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            var columnList = new List<TableColumn>();
            using (var query = queryBuilder.GetTableDescriptionQuery(tableName))
            {
                connection.Open();
                query.Connection = connection;
                var reader = await query.ExecuteReaderAsync();
                
                while(reader.Read())
                {
                    var columnName = reader[columnNameColumnName] as string;
                    var rawColumnType = reader[dataTypeColumnName] as string;
                    var parsedType = dataTypeDeterminator.FromDataTypeField(rawColumnType);
                    columnList.Add(new TableColumn(columnName, parsedType));
                }
            }

            return columnList;
        }
    }
}
