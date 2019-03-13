using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryBuilders
{
    internal class SQLServerQueryBuilder : IQueryBuilder
    {
        public string GetTableDescriptionQuery(string tableName)
        {
            var queryCommand = $@"select * 
                 from information_schema.columns 
                 where table_name = '{tableName}'
                 order by ordinal_position";
            return queryCommand;
        }
    }
}
