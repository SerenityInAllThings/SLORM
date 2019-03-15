using System.Collections.Generic;
using SLORM.Application.ValueObjects;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal interface ISQLServerOrderByStatementBuilder
    {
        string GetStatement(ICollection<TableColumn> columnsInTable, ICollection<ColumnOrdering> columnsToOrderings);
    }
}