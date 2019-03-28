using LightInject;
using SLORM.Application.QueryBuilders;
using SLORM.Application.QueryBuilders.QueryBuilders;
using SLORM.Application.QueryBuilders.SQLServer.StatementBuilders;
using SLORM.Application.QueryExecutors;
using SLORM.Application.ValueObjects;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SLORM.Application.UnitTests")]
[assembly: InternalsVisibleTo("SLORM.Application.SQLServerIntegrationTests")]
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
            Container.Register<ISQLServerSelectStatementBuilder, SelectStatementBuilder>();
            Container.Register<ISQLServerFromStatementBuilder, FromStatementBuilder>();
            Container.Register<ISQLServerWhereStatementBuilder, WhereStatementBuilder>();
            Container.Register<ISQLServerGroupByStatementBuilder, GroupByStatementBuilder>();
            Container.Register<ISQLServerOrderByStatementBuilder, OrderByStatementBuilder>();

            Container.Register<ISQLServerDataTypeDeterminator, SQLServerDataTypeDeterminator>();
        }
    }
}
