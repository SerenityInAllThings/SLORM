using SLORM.Application.ValueObjects;
using System.Collections.Generic;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal interface ISQLServerSelectStatementBuilder
    {
        Statement GetStatement(ICollection<TableColumn> columnsInTable, ICollection<TableColumn> columnsToGroupBy, ICollection<TableColumn> columnsToCount, ICollection<TableColumn> columnsToSum);
    }
}