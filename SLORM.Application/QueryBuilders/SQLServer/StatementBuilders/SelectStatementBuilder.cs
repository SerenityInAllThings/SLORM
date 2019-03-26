using SLORM.Application.Extensions;
using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal class SelectStatementBuilder : ISQLServerSelectStatementBuilder
    {
        public SelectStatementBuilder() { }

        public Statement GetStatement(ICollection<TableColumn> columnsInTable, ICollection<TableColumn> columnsToGroupBy, ICollection<TableColumn> columnsToCount, ICollection<TableColumn> columnsToSum)
        {
            if (columnsInTable == null)
                throw new ArgumentNullException(nameof(columnsInTable));
            if (columnsToGroupBy == null)
                throw new ArgumentNullException(nameof(columnsToGroupBy));
            if (columnsToCount == null)
                throw new ArgumentNullException(nameof(columnsToCount));

            var statementTextBuilder = new StringBuilder();
            foreach (var groupByColumn in columnsToGroupBy)
            {
                if (!columnsInTable.Contains(groupByColumn))
                    continue;

                if (statementTextBuilder.Length == 0)
                    statementTextBuilder.Append("SELECT");

                statementTextBuilder.Append($" {groupByColumn.Name.SanitizeSQL()},");
            }

            foreach (var currentColumnToCount in columnsToCount)
            {
                if (!columnsInTable.Contains(currentColumnToCount))
                    continue;

                if (statementTextBuilder.Length == 0)
                    statementTextBuilder.Append("SELECT");

                statementTextBuilder.Append($" COUNT({currentColumnToCount.Name.SanitizeSQL()}) AS {currentColumnToCount.Name.SanitizeSQL()},");
            }

            foreach (var currentColumnToSum in columnsToSum)
            {
                if (!columnsInTable.Contains(currentColumnToSum))
                    continue;

                if (statementTextBuilder.Length == 0)
                    statementTextBuilder.Append("SELECT");

                statementTextBuilder.Append($" SUM({currentColumnToSum.Name.SanitizeSQL()}) AS {currentColumnToSum.Name.SanitizeSQL()},");
            }
            // Removing last extra comma
            statementTextBuilder.Length--;
            return new Statement(statementTextBuilder.ToString().Trim(), new List<DBParameterKeyValue>());
        }

        private string GetParameterName(string baseName)
        {
            return $"@{baseName}_{Guid.NewGuid().ToString("N").Substring(0, 16)}";
        }
    }
}
