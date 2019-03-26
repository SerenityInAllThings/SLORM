using System.Collections.Generic;
using SLORM.Application.ValueObjects;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal interface ISQLServerOrderByStatementBuilder
    {
        Statement GetStatement(ICollection<TableColumn> columnsInTable, ICollection<ColumnOrdering> columnsToOrderings);
    }
}