using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace SLORM.Application.QueryExecutors
{
    internal interface IQueryExecutor
    {
       ICollection<TableColumn> GetTableColumns(IDbConnection connection, string tableName);
    }
}
