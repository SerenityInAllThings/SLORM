using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal class SelectStatementBuilder : ISQLServerSelectStatementBuilder
    {
        public SelectStatementBuilder() { }

        public string GetStatement(ICollection<TableColumn> columnsInTable, ICollection<TableColumn> columnsToGroupBy, ICollection<TableColumn> columnsToCount)
        {
            if (columnsInTable == null)
                throw new ArgumentNullException(nameof(columnsInTable));
            if (columnsToGroupBy == null)
                throw new ArgumentNullException(nameof(columnsToGroupBy));
            if (columnsToCount == null)
                throw new ArgumentNullException(nameof(columnsToCount));

            var queryBuilder = new StringBuilder();
            foreach (var groupByColumn in columnsToGroupBy)
            {
                if (!columnsInTable.Contains(groupByColumn))
                    continue;

                if (queryBuilder.Length == 0)
                    queryBuilder.Append("SELECT");

                queryBuilder.Append($" {groupByColumn.Name},");
            }

            foreach (var columnToCount in columnsToCount)
            {
                if (!columnsInTable.Contains(columnToCount))
                    continue;

                if (queryBuilder.Length == 0)
                    queryBuilder.Append("SELECT");

                queryBuilder.Append($" COUNT({columnToCount.Name}) AS {columnToCount.Name},");
            }
            // Removing last extra comma
            queryBuilder.Length--;
            return queryBuilder.ToString().Trim();
        }
    }
}
