using SLORM.Application.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryExecutors
{
    internal interface IQueryExecutorResolver
    {
        IQueryExecutor GetQueryExecutorFromProviderType(SQLProvider provider);
    }
}
