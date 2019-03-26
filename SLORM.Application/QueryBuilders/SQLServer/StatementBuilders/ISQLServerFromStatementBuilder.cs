using SLORM.Application.ValueObjects;

namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal interface ISQLServerFromStatementBuilder
    {
        Statement GetStatement(string tableName);
    }
}