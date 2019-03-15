using System.Collections.Generic;
using SLORM.Application.ValueObjects;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal interface ISQLServerWhereStatementBuilder
    {
        string GetStatement(ICollection<TableColumn> tableColumns, ICollection<ColumnFilter> filterings);
    }
}