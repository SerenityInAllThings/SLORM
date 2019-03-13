using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryBuilders
{
    internal interface IQueryBuilder
    {
        string GetTableDescriptionQuery(string tableName);
    }
}
