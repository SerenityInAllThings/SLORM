using SLORM.Application.Enums;
using System.Runtime.CompilerServices;

namespace SLORM.Application.QueryBuilders
{
    internal interface IQueryBuilderResolver
    {
        IQueryBuilder ResolveFromSQLProvider(SQLProvider provider);
    }
}
