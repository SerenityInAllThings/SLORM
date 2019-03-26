using SLORM.Application.Contexts;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace SLORM.Application.QueryBuilders
{
    internal interface IQueryBuilder
    {
        DbCommand GetTableDescriptionQuery(string tableName);
        DbCommand GetReadQuery(SLORMContext context);
    }
}
