using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLORM.Application.SQLServerIntegrationTests
{
    internal static class CreateTableSQLGenerator
    {
        private static string columnDefinitionSeparator = ",\n";

        internal static string GenerateBasedOnType(Type type, string tableName)
        {
            var createTableSQL = $"CREATE TABLE {tableName} (\n";
            var properties = type.GetProperties();
            foreach (var currentProperty in properties)
            {
                if (!CSharpToSQLTypeConverter.IsTypeValid(currentProperty.PropertyType))
                    continue;

                createTableSQL += $"{currentProperty.Name} {CSharpToSQLTypeConverter.GetSQLType(currentProperty.PropertyType)}{columnDefinitionSeparator}";
            }
            createTableSQL = createTableSQL.Substring(0, createTableSQL.Length - columnDefinitionSeparator.Length);
            createTableSQL += "\n)";

            return createTableSQL;
        }
    }
}
