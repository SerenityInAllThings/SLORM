using System;
using System.Collections.Generic;
using System.Text;
using SLORM.Application.Enums;
using SLORM.Application.Exceptions;

namespace SLORM.Application.QueryBuilders
{
    internal class QueryBuilderResolver : IQueryBuilderResolver
    {
        public IQueryBuilder ResolveFromSQLProvider(SQLProvider provider)
        {
            switch (provider)
            {
                case SQLProvider.SQLServer:
                    var queryBuilder = (IQueryBuilder)Lifecycle.Container.GetInstance(typeof(IQueryBuilder), Lifecycle.SQLServerKey);
                    return queryBuilder;
                default:
                    throw new UnknownSQLProviderException();
            }
        }
    }
}
