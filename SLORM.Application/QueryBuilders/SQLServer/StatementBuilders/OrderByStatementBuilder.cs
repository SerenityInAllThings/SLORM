using SLORM.Application.Enums;
using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal class OrderByStatementBuilder : ISQLServerOrderByStatementBuilder
    {
        private static readonly string columnConnector = ", ";

        public Statement GetStatement(ICollection<TableColumn> columnsInTable, ICollection<ColumnOrdering> columnsToOrderBy)
        {
            if (columnsInTable == null)
                throw new ArgumentNullException(nameof(columnsInTable));

            if (columnsToOrderBy == null)
                throw new ArgumentNullException(nameof(columnsToOrderBy));

            if (columnsToOrderBy.Count() == 0)
                return new Statement(string.Empty, new List<DBParameterKeyValue>());

            var statementText = string.Empty;
            foreach (var currentOrdering in columnsToOrderBy)
            {
                if (!columnsInTable.Contains(currentOrdering.Column))
                    continue;

                if (statementText.Length == 0)
                    statementText += "ORDER BY ";

                statementText += currentOrdering.Column.Name;
                if (currentOrdering.OrderType == OrderType.ASC)
                    statementText += " ASC";
                if (currentOrdering.OrderType == OrderType.DESC)
                    statementText += " DESC";

                statementText += columnConnector;
            }
            statementText = statementText.Substring(0, statementText.Length - columnConnector.Length);

            return new Statement(statementText, new List<DBParameterKeyValue>());
        }

        private string GetParameterName(string baseName)
        {
            return $"@{baseName}_{Guid.NewGuid().ToString("N").Substring(0, 16)}";
        }
    }
}
