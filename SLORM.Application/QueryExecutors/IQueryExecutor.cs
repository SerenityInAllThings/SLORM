using SLORM.Application.Contexts;
using SLORM.Application.ValueObjects;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace SLORM.Application.QueryExecutors
{
    internal interface IQueryExecutor
    {
        Task<ICollection<TableColumn>> GetTableColumns(IDbConnection connection, string tableName);
        Task<QueryResult> Query(SLORMContext context);
    }
}
