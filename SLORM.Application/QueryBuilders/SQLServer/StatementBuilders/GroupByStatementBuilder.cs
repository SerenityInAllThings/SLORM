using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal class GroupByStatementBuilder : ISQLServerGroupByStatementBuilder
    {
        private static readonly string columnSeparator = ", ";

        public string GetStatement(ICollection<TableColumn> tableColumns, ICollection<TableColumn> groupByColumns)
        {
            var statement = string.Empty;
            foreach (var currentGroupByColumn in groupByColumns)
            {
                if (!tableColumns.Contains(currentGroupByColumn))
                    continue;

                if (statement.Length == 0)
                    statement += "GROUP BY ";

                statement += currentGroupByColumn.Name;
                statement += columnSeparator;
            }
            // Removing last extra separator
            statement = statement.Substring(0, statement.Length - columnSeparator.Length);

            return statement;
        }
    }
}
