using System.Collections.Generic;
using SLORM.Application.ValueObjects;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal interface ISQLServerGroupByStatementBuilder
    {
        string GetStatement(ICollection<TableColumn> tableColumns, ICollection<TableColumn> groupByColumns);
    }
}