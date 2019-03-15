using SLORM.Application.Enums;
using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal class OrderByStatementBuilder : ISQLServerOrderByStatementBuilder
    {
        private static readonly string columnConnector = ", ";

        public string GetStatement(ICollection<TableColumn> columnsInTable, ICollection<ColumnOrdering> columnsToOrderings)
        {
            var query = string.Empty;
            foreach (var currentOrdering in columnsToOrderings)
            {
                if (!columnsInTable.Contains(currentOrdering.Column))
                    continue;

                if (query.Length == 0)
                    query += "ORDER BY ";

                query += currentOrdering.Column.Name;
                if (currentOrdering.OrderType == OrderType.ASC)
                    query += " ASC";
                if (currentOrdering.OrderType == OrderType.DESC)
                    query += " DESC";

                query += columnConnector;
            }
            query = query.Substring(0, query.Length - columnConnector.Length);

            return query;
        }
    }
}
