using Microsoft.Data.Sqlite;
using SLORM.Application.Contexts;
using System;
using System.Data.SqlClient;
using Xunit;

namespace SLORM.Application.IntegrationTests
{
    public class UnitTest1
    {
        [Fact]
        public void GetTableDataFromRealSQLServer()
        {
            Lifecycle.Initialize();
            var conn = new SqlConnection(@"Server=(local)\SQLEXPRESS;Database=Logs;Trusted_Connection=True;");
            var ctx = new SLORMContext(conn, "Logs");
            //ctx.Select("minhaColunaQUalquer", "Id", "aaaa", "", "DataHora");

            //var conn = new SqliteConnection("Data Source=:memory:;");
            //conn.Open();
            //var command = conn.CreateCommand();
            //command.CommandText = "CREATE TABLE tabelaTop(Id int, nome varchar(32));";
            //command.ExecuteNonQuery();
            //command.CommandText = "INSERT INTO tabelaTop values (1, 'nome1');";
            //command.ExecuteNonQuery();
            //command.CommandText = "Select * from tabelaTop";
            //var reader = command.ExecuteReader();

            //while (reader.Read())
            //{
            //    for (var i = 0; i < reader.FieldCount; i++)
            //        Console.WriteLine(reader[i]);
            //    Console.WriteLine("-----------");
            //}
            //reader.Close();

            //command.CommandText = $@"select * 
            //     from information_schema.columns 
            //     where table_name = 'tabelaTop'
            //     order by ordinal_position";
            //reader = command.ExecuteReader();

            //while (reader.Read())
            //{
            //    for (var i = 0; i < reader.FieldCount; i++)
            //        Console.WriteLine(reader[i]);
            //    Console.WriteLine("-----------");
            //}

            //conn.Close();
        }
    }
}
