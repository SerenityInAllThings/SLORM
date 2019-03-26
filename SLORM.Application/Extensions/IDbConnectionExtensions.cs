using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace SLORM.Application.Extensions
{
    internal static class IDbConnectionExtensions
    {
        private static readonly int maximumTimesToWait = 15;

        public static async Task EnsureConnected(this IDbConnection connection, int tries = 0)
        {
            // TODO: Improve this exception
            if (tries > maximumTimesToWait)
                throw new Exception("It was not possible to connected to DB");

            if (connection.State == ConnectionState.Open)
                return;
            if (connection.State == ConnectionState.Connecting || connection.State == ConnectionState.Executing || connection.State == ConnectionState.Fetching)
            {
                await Task.Delay(1000);
                await connection.EnsureConnected(++tries);
            }
            else
            {
                connection.Open();
            }
        }
    }
}
