using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal class GroupByStatementBuilder : ISQLServerGroupByStatementBuilder
    {
        private static readonly string columnSeparator = ", ";

        public Statement GetStatement(ICollection<TableColumn> tableColumns, ICollection<TableColumn> groupByColumns)
        {
            if (tableColumns == null || tableColumns.Count == 0 || groupByColumns == null || groupByColumns.Count == 0)
                return new Statement(string.Empty, new List<DBParameterKeyValue>());

            var statementText = string.Empty;
            foreach (var currentGroupByColumn in groupByColumns)
            {
                if (!tableColumns.Contains(currentGroupByColumn))
                    continue;

                if (statementText.Length == 0)
                    statementText += "GROUP BY ";

                statementText += currentGroupByColumn.Name;
                statementText += columnSeparator;
            }
            // Removing last extra separator
            statementText = statementText.Substring(0, statementText.Length - columnSeparator.Length);

            return new Statement(statementText, new List<DBParameterKeyValue>());
        }

        private string GetParameterName(string baseName)
        {
            return $"@{baseName}_{Guid.NewGuid().ToString("N").Substring(0, 16)}";
        }
    }
}
