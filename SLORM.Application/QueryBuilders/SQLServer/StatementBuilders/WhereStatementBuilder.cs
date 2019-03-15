using SLORM.Application.Enums;
using SLORM.Application.Extensions;
using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal class WhereStatementBuilder : ISQLServerWhereStatementBuilder
    {
        private static readonly string filterConnector = "\nAND ";

        public string GetStatement(ICollection<TableColumn> tableColumns, ICollection<ColumnFilter> filterings)
        {
            if (tableColumns == null)
                throw new ArgumentNullException(nameof(tableColumns));

            if (filterings == null)
                throw new ArgumentNullException(nameof(filterings));

            var statement = "";
            foreach (var currentFiltering in filterings)
            {
                if (!tableColumns.Contains(currentFiltering.Column))
                    continue;

                if (statement.Length == 0)
                    statement += "WHERE ";

                switch(currentFiltering.Column.DataType)
                {
                    case ColumnDataType.Date:
                        statement += GetDateTimeFilterClause(currentFiltering);
                        break;
                    case ColumnDataType.Number:
                        statement += GetNumberFilterClause(currentFiltering);
                        break;
                    case ColumnDataType.String:
                        statement += getStringFilterClause(currentFiltering);
                        break;
                    default:
                        // Ignoring not supported data type filters
                        continue; 
                }

                statement += filterConnector;
            }
            // Removing last connector;
            statement = statement.Substring(0, statement.Length - filterConnector.Length);

            return statement.ToString().CleanWhitespacePolution();
        }

        private string getStringFilterClause(ColumnFilter filter)
        {
            var query = $"{filter.Column.Name} ";
            if (filter.FilterRigor == FilterRigor.Contains)
            {
                if (filter.FilterMethod == FilterMethod.Excluding)
                    query += "NOT";
                query += $"LIKE '%{filter.Value}%'";
                
            }
            else if (filter.FilterRigor == FilterRigor.Equals)
            {
                if (filter.FilterMethod == FilterMethod.Excluding)
                    query += "!";
                query += $"= '{filter.Value}'";
            }

            return query;
        }

        private string GetNumberFilterClause(ColumnFilter filter)
        {
            var query = string.Empty;
            if (filter.FilterRigor == FilterRigor.Contains)
            {
                query += $" CONVERT(varchar(64), {filter.Column.Name})";

                if (filter.FilterMethod == FilterMethod.Excluding)
                    query += " NOT";

                query += $" LIKE '%{filter.Value}%'";
            }
            else if (filter.FilterRigor == FilterRigor.Equals)
            {
                query += $" {filter.Column.Name} ";

                if (filter.FilterMethod == FilterMethod.Excluding)
                    query += "!";

                query += $"= {filter.Value}";
            }

            return query;
        }

        private string GetDateTimeFilterClause(ColumnFilter filter)
        {
            var query = string.Empty;
            if (filter.FilterRigor == FilterRigor.Contains)
            {
                query += $" CONVERT(varchar(25), {filter.Column.Name}, 126)";

                if (filter.FilterMethod == FilterMethod.Excluding)
                    query += " NOT";

                query += $" LIKE '%{filter.Value}%'";
            }
            else if (filter.FilterRigor == FilterRigor.Equals)
            {
                query += $" {filter.Column.Name} ";

                if (filter.FilterMethod == FilterMethod.Excluding)
                    query += "!";

                query += $"= '{filter.Value}'";
            }

            return query;
        }
    }
}
