using SLORM.Application.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryBuilders
{
    public class ColumnOrderingRequest
    {
        public string ColumnName { get; set; }
        public OrderType OrderType { get; set; }

        public ColumnOrderingRequest() { }

        public ColumnOrderingRequest(string columnName, OrderType orderType)
        {
            this.ColumnName = columnName;
            this.OrderType = orderType;
        }
    }
}
