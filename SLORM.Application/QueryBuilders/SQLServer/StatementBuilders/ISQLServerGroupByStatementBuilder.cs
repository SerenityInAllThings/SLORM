using System.Collections.Generic;
using SLORM.Application.ValueObjects;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal interface ISQLServerGroupByStatementBuilder
    {
        Statement GetStatement(ICollection<TableColumn> tableColumns, ICollection<TableColumn> groupByColumns);
    }
}