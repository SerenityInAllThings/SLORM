using SLORM.Application.ValueObjects;
using System.Collections.Generic;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal interface ISQLServerSelectStatementBuilder
    {
        string GetStatement(ICollection<TableColumn> columnsInTable, ICollection<TableColumn> columnsToGroupBy, ICollection<TableColumn> columnsToCount);
    }
}