using SLORM.Application.Extensions;
using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal class FromStatementBuilder : ISQLServerFromStatementBuilder
    {
        public FromStatementBuilder() { }

        public Statement GetStatement(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            var statementText = $"FROM {tableName.SanitizeSQL()}";

            return new Statement(statementText, new List<DBParameterKeyValue>());
        }
    }
}
