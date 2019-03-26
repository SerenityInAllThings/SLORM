using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SLORM.Application.SQLServerIntegrationTests
{
    internal static class TestTableManager
    {
        private static readonly int numberOfTestDataRows = 10000;
        private static readonly int maximumRowsPerInsert = 1000;
        private static string tableName;
        internal static IList<SampleData> TestData = SampleData.GenerateSampleData(numberOfTestDataRows);
        internal static string TableName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(tableName))
                    tableName = $"IntegrationTests_{DateTime.Now.ToFileTime()}";
                return tableName;
            }
        }

        internal static void CreateTestTable()
        {
            var connectionString = Configuration.ConnectionString;
            var dbConnection = new SqlConnection(connectionString);
            dbConnection.Open();
            using (var creationCommand = dbConnection.CreateCommand())
            {
                creationCommand.CommandText = CreateTableSQLGenerator.GenerateBasedOnType(typeof(SampleData), TableName);
                creationCommand.ExecuteNonQuery();
            }
        }

        internal static void InsertTestData()
        {
            var connectionString = Configuration.ConnectionString;
            var dbConnection = new SqlConnection(connectionString);
            dbConnection.Open();
            using (var insertCommand = dbConnection.CreateCommand())
            {
                var requiredInserts = numberOfTestDataRows / maximumRowsPerInsert;
                if (numberOfTestDataRows % maximumRowsPerInsert > 0)
                    requiredInserts++;

                for (var i = 0; i < requiredInserts; i++)
                {
                    var dataToBeInserted = TestData.Skip(i * maximumRowsPerInsert).Take(maximumRowsPerInsert).ToList();
                    insertCommand.CommandText = InsertIntoSQLGenerator.Generate(TableName, dataToBeInserted);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        internal static void DeleteTestTable()
        {
            try
            {
                var connectionString = Configuration.ConnectionString;
                var dbConnection = new SqlConnection(connectionString);
                dbConnection.Open();
                using (var deletionCommand = dbConnection.CreateCommand())
                {
                    deletionCommand.CommandText = DeleteTableSQLGenerator.Generate(TableName);
                    deletionCommand.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Error while deleting test table named {TableName}");
                Console.WriteLine("We recommend deleting it manually");
            }
        }
    }
}
