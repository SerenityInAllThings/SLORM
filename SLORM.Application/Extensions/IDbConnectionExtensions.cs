using System;
using System.Data;
using System.Threading.Tasks;

namespace SLORM.Application.Extensions
{
    internal static class IDbConnectionExtensions
    {
        private static readonly int maximumTimesToWait = 15;
        private static readonly int triesIntervalMs = 1000;

        internal static async Task EnsureConnected(this IDbConnection connection, int tries = 0)
        {
            // TODO: Improve this exception
            if (tries > maximumTimesToWait)
                throw new Exception("It was not possible to connected to DB");

            if (connection.State == ConnectionState.Open)
                return;
            if (connection.State == ConnectionState.Connecting || connection.State == ConnectionState.Executing || connection.State == ConnectionState.Fetching)
            {
                await Task.Delay(triesIntervalMs);
                await connection.EnsureConnected(++tries);
            }
            else
            {
                connection.Open();
            }
        }
    }
}
