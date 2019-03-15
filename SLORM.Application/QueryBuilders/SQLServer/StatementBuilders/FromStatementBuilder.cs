using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal class FromStatementBuilder : ISQLServerFromStatementBuilder
    {
        public FromStatementBuilder() { }

        public string GetStatement(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            return $"FROM {tableName}";
        }
    }
}
