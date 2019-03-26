using SLORM.Application.Contexts;
using SLORM.Application.Extensions;
using SLORM.Application.QueryBuilders.SQLServer.StatementBuilders;
using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace SLORM.Application.QueryBuilders.QueryBuilders
{
    internal class SQLServerQueryBuilder : IQueryBuilder
    {
        private static readonly string tableNameParameterName = "@TableName";
        private static readonly string queryCommandTemplate = $@"select * 
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

            tableName = tableName.SanitizeSQL();

            using (SqlCommand queryCommand = new SqlCommand(queryCommandTemplate))
            {
                queryCommand.Parameters.AddWithValue(tableNameParameterName, tableName);

                return queryCommand;
            }
        }

        public DbCommand GetReadQuery(SLORMContext ctx)
        {
            var dbCommand = new SqlCommand();
            var statementList = new List<Statement>();
            statementList.Add(selectStatementBuilder.GetStatement(ctx.ColumnsInTable, ctx.ColumnsToGroupBy, ctx.ColumnsToCount, ctx.ColumnsToSum));
            statementList.Add(fromStatementBuilder.GetStatement(ctx.TableName));
            statementList.Add(whereStatementBuilder.GetStatement(ctx.ColumnsInTable, ctx.ColumnsToFilter));
            statementList.Add(groupByStatementBuilder.GetStatement(ctx.ColumnsInTable, ctx.ColumnsToGroupBy));
            statementList.Add(orderByStatementBuilder.GetStatement(ctx.ColumnsInTable, ctx.ColumnsToOrderBy));

            dbCommand.CommandText = string.Empty;
            var statementParameters = new List<DBParameterKeyValue>();
            foreach (var currentStatement in statementList)
            {
                dbCommand.CommandText += currentStatement.StatementText;
                dbCommand.CommandText += " ";
                foreach (var currentParameter in currentStatement.Parameters)
                    dbCommand.Parameters.AddWithValue(currentParameter.Key, currentParameter.Value);
            }
            return dbCommand;
        }
    }
}
