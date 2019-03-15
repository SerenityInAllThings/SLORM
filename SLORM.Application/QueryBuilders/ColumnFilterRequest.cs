using SLORM.Application.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryBuilders
{
    public class ColumnFilterRequest
    {
        public string ColumnName { get; set; }
        public string Value { get; set; }
        public FilterRigor FilterRigor { get; set; }
        public FilterMethod FilterMethod { get; set; }

        public ColumnFilterRequest() { }

        public ColumnFilterRequest(string columnName, string value, FilterRigor filterRigor, FilterMethod filterMethod)
        {
            this.ColumnName = columnName;
            this.Value = value;
            this.FilterRigor = filterRigor;
            this.FilterMethod = filterMethod;
        }
    }
}
