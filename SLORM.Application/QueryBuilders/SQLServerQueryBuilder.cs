using SLORM.Application.Extensions;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace SLORM.Application.QueryBuilders
{
    internal class SQLServerQueryBuilder : IQueryBuilder
    {
        private static readonly string tableNameParameterName = "@TableName";
        private static readonly string queryCommandTemplate =   $@"select * 
                                                                from information_schema.columns 
                                                                where table_name = {tableNameParameterName}
                                                                order by ordinal_position".CleanWhitespacePolution();

        public DbCommand GetTableDescriptionQuery(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            using (SqlCommand queryCommand = new SqlCommand(queryCommandTemplate))
            {
                queryCommand.Parameters.AddWithValue(tableNameParameterName, tableName);

                return queryCommand;
            }
        }
    }
}
