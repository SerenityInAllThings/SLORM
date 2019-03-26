using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLORM.Application.SQLServerIntegrationTests
{
    internal static class CSharpToSQLTypeConverter
    {
        private static Type[] validTypes = new Type[]
        {
            typeof(int), typeof(long), typeof(float), typeof(double), typeof(string), typeof(bool), typeof(DateTime)
        };

        internal static bool IsTypeValid(Type type) => validTypes.Any(validType => validType == type);
        internal static string GetSQLType(Type cSharpType)
        {
            if (cSharpType == typeof(int))
                return "int";
            if (cSharpType == typeof(long))
                return "bigint";
            if (cSharpType == typeof(float))
                return "float(24)";
            if (cSharpType == typeof(double))
                return "float(53)";
            if (cSharpType == typeof(string))
                return "varchar(1024)";
            if (cSharpType == typeof(bool))
                return "bit";
            if (cSharpType == typeof(DateTime))
                return "datetime2";
            return "varchar(1024)";
        }
    }
}
