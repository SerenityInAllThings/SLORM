using LightInject;
using SLORM.Application.QueryBuilders;
using SLORM.Application.QueryExecutors;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application
{
    public static class Lifecycle
    {
        internal static readonly string SQLServerKey = "SQLSERVER";
        internal static readonly string MySQLKey = "MYSQL";
        internal static readonly string OracleKey = "ORACLE";

        internal static IServiceContainer Container { get; private set; }

        public static void Initialize()
        {
            var container = new ServiceContainer();

            // Registering general services
            container.Register<IQueryBuilderResolver, QueryBuilderResolver>();
            container.Register<IQueryExecutorResolver, QueryExecutorResolver>();
            // Registering SQL Server services
            container.Register<IQueryBuilder, SQLServerQueryBuilder>(SQLServerKey);
            container.Register<IQueryExecutor, SQLServerQueryExecutor>(SQLServerKey);

            Container = container;
        }
    }
}
