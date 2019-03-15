using SLORM.Application.Contexts;
using SLORM.Application.Extensions;
using SLORM.Application.QueryBuilders.SQLServer.StatementBuilders;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace SLORM.Application.QueryBuilders.QueryBuilders
{
    internal class SQLServerQueryBuilder : IQueryBuilder
    {
        private static readonly string generalQueryTemplate = "{0}\n{1}\n{2}\n{3}\n{4};";
        private static readonly string tableNameParameterName = "@TableName";
        private static readonly string queryCommandTemplate =   $@"select * 
                                                                from information_schema.columns 
                                                                where table_name = {tableNameParameterName}
                                                                order by ordinal_position".CleanWhitespacePolution();

        private readonly ISQLServerSelectStatementBuilder selectStatementBuilder;
        private readonly ISQLServerFromStatementBuilder fromStatementBuilder;
        private readonly ISQLServerWhereStatementBuilder whereStatementBuilder;
        private readonly ISQLServerGroupByStatementBuilder groupByStatementBuilder;
        private readonly ISQLServerOrderByStatementBuilder orderByStatementBuilder;

        public SQLServerQueryBuilder(
            ISQLServerSelectStatementBuilder selectStatementBuilder, 
            ISQLServerFromStatementBuilder fromStatementBuilder, 
            ISQLServerWhereStatementBuilder whereStatementBuilder,
            ISQLServerGroupByStatementBuilder groupByStatementBuilder,
            ISQLServerOrderByStatementBuilder orderByStatementBuilder)
        {
            this.selectStatementBuilder = selectStatementBuilder ?? throw new ArgumentNullException(nameof(selectStatementBuilder));
            this.fromStatementBuilder = fromStatementBuilder ?? throw new ArgumentNullException(nameof(fromStatementBuilder));
            this.whereStatementBuilder = whereStatementBuilder ?? throw new ArgumentException(nameof(whereStatementBuilder));
            this.groupByStatementBuilder = groupByStatementBuilder ?? throw new ArgumentNullException(nameof(groupByStatementBuilder));
            this.orderByStatementBuilder = orderByStatementBuilder ?? throw new ArgumentNullException(nameof(orderByStatementBuilder));
        }

        public DbCommand GetTableDescriptionQuery(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            using (SqlCommand queryCommand = new SqlCommand(queryCommandTemplate))
            {
                queryCommand.Parameters.AddWithValue(tableNameParameterName, tableName);

                return queryCommand;
            }
        }

        public string GetReadQuery(SLORMContext ctx)
        {
            var selectStatement = selectStatementBuilder.GetStatement(ctx.ColumnsInTable, ctx.ColumnsToGroupBy, ctx.ColumnsToCount);
            var fromStatement = fromStatementBuilder.GetStatement(ctx.TableName);
            var whereStatement = whereStatementBuilder.GetStatement(ctx.ColumnsInTable, ctx.ColumnsToFilter);
            var groupByStatement = groupByStatementBuilder.GetStatement(ctx.ColumnsInTable, ctx.ColumnsToGroupBy);
            var orderByStatement = orderByStatementBuilder.GetStatement(ctx.ColumnsInTable, ctx.ColumnsToOrderBy);

            var query = string.Format(generalQueryTemplate, selectStatement, fromStatement, whereStatement, groupByStatement, orderByStatement).CleanWhitespacePolution();
            return query;
        }
    }
}
