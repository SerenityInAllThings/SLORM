namespace SLORM.Application.QueryBuilders.SQLServer.StatementBuilders
{
    internal interface ISQLServerFromStatementBuilder
    {
        string GetStatement(string tableName);
    }
}