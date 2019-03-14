using SLORM.Application.Enums;
using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryBuilders
{
    internal struct ColumnFilter
    {
        public TableColumn Column { get; private set; }
        public string Value { get; set; }
        public FilterRigor FilterRigor { get; set; }
        public FilterMethod FilterMethod { get; set; }
    }
}
