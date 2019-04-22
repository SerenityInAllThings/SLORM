using SLORM.Application.Enums;
using SLORM.Application.Extensions;
using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal class WhereStatementBuilder : ISQLServerWhereStatementBuilder
    {
        private static readonly string filterConnector = "\nAND ";

        public Statement GetStatement(ICollection<TableColumn> tableColumns, ICollection<ColumnFilter> filterings)
        {
            if (tableColumns == null)
                throw new ArgumentNullException(nameof(tableColumns));

            if (filterings == null)
                throw new ArgumentNullException(nameof(filterings));

            var statementParameters = new List<DBParameterKeyValue>();
            var statementText = "";

            if (filterings.Count == 0)
                return new Statement(statementText, statementParameters);

            foreach (var currentFiltering in filterings)
            {
                if (!tableColumns.Contains(currentFiltering.Column))
                    continue;

                if (statementText.Length == 0)
                    statementText += "WHERE ";

                switch (currentFiltering.Column.DataType)
                {
                    case ColumnDataType.Date:
                        var filterDateClause = GetDateTimeFilterClause(currentFiltering);
                        statementText += filterDateClause.StatementText;
                        statementParameters.AddRange(filterDateClause.Parameters);
                        break;
                    case ColumnDataType.Number:
                        var filterNumberClause = GetNumberFilterClause(currentFiltering);
                        statementText += filterNumberClause.StatementText;
                        statementParameters.AddRange(filterNumberClause.Parameters);
                        break;
                    case ColumnDataType.String:
                        var filterStringClause = getStringFilterClause(currentFiltering);
                        statementText += filterStringClause.StatementText;
                        statementParameters.AddRange(filterStringClause.Parameters);
                        break;
                    default:
                        // Ignoring not supported data type filters
                        continue;
                }

                statementText += filterConnector;
            }
            // Removing last connector;
            statementText = statementText.Substring(0, statementText.Length - filterConnector.Length);
            statementText = statementText.ToString().CleanWhitespacePolution();

            return new Statement(statementText, statementParameters);
        }

        private Statement getStringFilterClause(ColumnFilter filter)
        {
            var clauseParameters = new List<DBParameterKeyValue>();
            var clauseText = $"( {filter.Column.Name.SanitizeSQL()} ";

            for (var i = 0; i < filter.Values.Count(); i++)
            {
                var currentValue = filter.Values.ElementAt(i);
                if (i != 0)
                    clauseText += $" OR {filter.Column.Name.SanitizeSQL()} ";

                if (filter.FilterRigor == FilterRigor.Contains)
                {
                    if (filter.FilterMethod == FilterMethod.Excluding)
                        clauseText += "NOT ";

                    var filterValueParameter = new DBParameterKeyValue(GetParameterName("StringFilterValue"), currentValue);
                    clauseParameters.Add(filterValueParameter);
                    clauseText += $"LIKE '%' + {filterValueParameter.Key} + '%'";
                }
                else if (filter.FilterRigor == FilterRigor.Equals)
                {
                    if (filter.FilterMethod == FilterMethod.Excluding)
                        clauseText += "!";

                    var filterValueParameter = new DBParameterKeyValue(GetParameterName("StringFilterValue"), currentValue);
                    clauseParameters.Add(filterValueParameter);
                    clauseText += $"= {filterValueParameter.Key}";
                }
            }
            clauseText += " )";

            return new Statement(clauseText, clauseParameters);
        }

        private Statement GetNumberFilterClause(ColumnFilter filter)
        {
            var clauseParameters = new List<DBParameterKeyValue>();
            var clauseText = string.Empty;
            var sanitizedColumnName = filter.Column.Name.SanitizeSQL();

            for (var i = 0; i < filter.Values.Count(); i++)
            {
                var currentValue = filter.Values.ElementAt(i);
                if (i != 0)
                    clauseText += " OR ";

                if (filter.FilterRigor == FilterRigor.Contains)
                {
                    clauseText += $" CONVERT(varchar(64), {sanitizedColumnName})";

                    if (filter.FilterMethod == FilterMethod.Excluding)
                        clauseText += " NOT";

                    var filterValueParameter = new DBParameterKeyValue(GetParameterName("FilterNumberValue"), currentValue);
                    clauseParameters.Add(filterValueParameter);
                    clauseText += $" LIKE '%' + {filterValueParameter.Key} + '%'";
                }
                else if (filter.FilterRigor == FilterRigor.Equals)
                {
                    clauseText += $" {sanitizedColumnName} ";

                    if (filter.FilterMethod == FilterMethod.Excluding)
                        clauseText += "!";

                    var filterValueParameter = new DBParameterKeyValue(GetParameterName("FilterNumberValue"), currentValue);
                    clauseParameters.Add(filterValueParameter);
                    clauseText += $"= {filterValueParameter.Key}";
                }
            }

            return new Statement(clauseText, clauseParameters);
        }

        private Statement GetDateTimeFilterClause(ColumnFilter filter)
        {
            var clauseParameters = new List<DBParameterKeyValue>();
            var clauseText = string.Empty;

            for (var i = 0; i < filter.Values.Count(); i++)
            {
                var currentValue = filter.Values.ElementAt(i);
                if (i != 0)
                    clauseText += " OR ";

                if (filter.FilterRigor == FilterRigor.Contains)
                {
                    clauseText += $" CONVERT(varchar(25), {filter.Column.Name.SanitizeSQL()}, 126)";

                    if (filter.FilterMethod == FilterMethod.Excluding)
                        clauseText += " NOT";

                    var filterValueParameter = new DBParameterKeyValue(GetParameterName("DateTimeFilterValue"), currentValue);
                    clauseParameters.Add(filterValueParameter);
                    clauseText += $" LIKE '%' + {filterValueParameter.Key} + '%'";
                }
                else if (filter.FilterRigor == FilterRigor.Equals)
                {
                    clauseText += $" {filter.Column.Name.SanitizeSQL()} ";

                    if (filter.FilterMethod == FilterMethod.Excluding)
                        clauseText += "!";

                    var filterValueParameter = new DBParameterKeyValue(GetParameterName("DateTimeFilterValue"), currentValue);
                    clauseParameters.Add(filterValueParameter);
                    clauseText += $"= {filterValueParameter.Key}";
                }
            }

            return new Statement(clauseText, clauseParameters);
        }

        private string GetParameterName(string baseName)
        {
            return $"@{baseName}_{Guid.NewGuid().ToString("N").Substring(0, 16)}";
        }
    }
}
