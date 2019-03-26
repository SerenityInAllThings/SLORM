using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace SLORM.Application.SQLServerIntegrationTests
{
    internal static class Configuration
    {
        public static readonly string ConnectionString = @"Server=(local)\SQLEXPRESS;Database=Logs;Trusted_Connection=True;";
    }
}
