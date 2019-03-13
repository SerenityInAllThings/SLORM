using SLORM.Application.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryBuilders
{
    internal interface IQueryBuilderResolver
    {
        IQueryBuilder ResolveFromSQLProvider(SQLProvider provider);
    }
}
