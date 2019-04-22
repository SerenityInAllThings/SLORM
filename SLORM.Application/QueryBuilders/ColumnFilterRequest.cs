using SLORM.Application.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryBuilders
{
    public class ColumnFilterRequest
    {
        public string ColumnName { get; set; }
        public ICollection<string> Values { get; set; }
        public FilterRigor FilterRigor { get; set; }
        public FilterMethod FilterMethod { get; set; }

        public ColumnFilterRequest() { }

        public ColumnFilterRequest(string columnName, ICollection<string> values, FilterRigor filterRigor, FilterMethod filterMethod)
        {
            this.ColumnName = columnName;
            this.Values = values;
            this.FilterRigor = filterRigor;
            this.FilterMethod = filterMethod;
        }
    }
}
