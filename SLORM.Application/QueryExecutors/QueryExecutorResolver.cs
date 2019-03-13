using System;
using System.Collections.Generic;
using System.Text;
using SLORM.Application.Enums;
using SLORM.Application.Exceptions;

namespace SLORM.Application.QueryExecutors
{
    internal class QueryExecutorResolver : IQueryExecutorResolver
    {
        public IQueryExecutor GetQueryExecutorFromProviderType(SQLProvider provider)
        {
            switch (provider)
            {
                case SQLProvider.SQLServer:
                    return (IQueryExecutor)Lifecycle.Container.GetInstance(typeof(IQueryExecutor), Lifecycle.SQLServerKey);
                default:
                    throw new UnknownSQLProviderException();
            }
        }
    }
}
