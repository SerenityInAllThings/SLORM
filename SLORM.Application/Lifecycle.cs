﻿using LightInject;
using SLORM.Application.QueryBuilders;
using SLORM.Application.QueryExecutors;
using SLORM.Application.ValueObjects;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SLORM.Application.UnitTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
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
            CreateContainer();
            RegisterTypes();
        }

        internal static void CreateContainer()
        {
            Container = new ServiceContainer();
        }

        internal static void RegisterTypes()
        {
            // Registering general services
            Container.Register<IQueryBuilderResolver, QueryBuilderResolver>();
            Container.Register<IQueryExecutorResolver, QueryExecutorResolver>();
            // Registering SQL Server services
            Container.Register<IQueryBuilder, SQLServerQueryBuilder>(SQLServerKey);
            Container.Register<IQueryExecutor, SQLServerQueryExecutor>(SQLServerKey);

            Container.Register<ISQLServerDataTypeDeterminator, SQLServerDataTypeDeterminator>();
        }
    }
}