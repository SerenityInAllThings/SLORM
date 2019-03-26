using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.SQLServerIntegrationTests
{
    internal static class DeleteTableSQLGenerator
    {
        internal static string Generate(string tableName) => $"DROP TABLE {tableName};";
    }
}
