using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.SQLServerIntegrationTests
{
    internal static class InsertIntoSQLGenerator
    {
        internal static string Generate(string tableName, IList<SampleData> data)
        {
            const string insertItemSeparator = ", ";

            var command = $"INSERT INTO {tableName} ";
            command += GetColumnNamesStatement();

            command += " VALUES ";
            foreach (var currentData in data)
                command += (GetValuesStatement(currentData) + insertItemSeparator);

            command = command.Substring(0, command.Length - insertItemSeparator.Length);
            return command;
        }

        private static string GetColumnNamesStatement()
        {
            const string columnSeparator = ", ";
            var statement = "(";
            foreach (var currentProperty in typeof(SampleData).GetProperties())
            {
                if (!CSharpToSQLTypeConverter.IsTypeValid(currentProperty.PropertyType))
                    continue;
                statement += $"{currentProperty.Name}{columnSeparator}";
            }
            // Removing last extra separator
            statement = statement.Substring(0, statement.Length - columnSeparator.Length);
            statement += ")";
            return statement;
        }

        private static string GetValuesStatement(SampleData data)
        {
            const string valuesSeparator = ", ";
            var statement = string.Empty;

            statement += "(";
            foreach (var currentProperty in data.GetType().GetProperties())
            {
                if (!CSharpToSQLTypeConverter.IsTypeValid(currentProperty.PropertyType))
                    continue;

                var propertyType = currentProperty.PropertyType;

                if (propertyType == typeof(int) || propertyType == typeof(long) || propertyType == typeof(float) || propertyType == typeof(double))
                    statement += $"{currentProperty.GetValue(data)}";
                else if (propertyType == typeof(string))
                    statement += $"'{currentProperty.GetValue(data)}'";
                else if (propertyType == typeof(bool))
                    statement += $"{(((bool)currentProperty.GetValue(data)) ? "1" : "0")}";
                else if (propertyType == typeof(DateTime))
                    statement += $"(convert(datetime, '{((DateTime)currentProperty.GetValue(data)).ToString("s", System.Globalization.CultureInfo.InvariantCulture)}', 126))";
                statement += valuesSeparator;
            }
            statement = statement.Substring(0, statement.Length - valuesSeparator.Length);
            statement += ")";
            return statement;
        }
    }
}
