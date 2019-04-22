using SLORM.Application.Enums;
using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;

namespace SLORM.Application.QueryBuilders
{
    internal struct ColumnFilter
    {
        internal TableColumn Column { get; private set; }
        internal ICollection<string> Values { get; private set; }
        internal FilterRigor FilterRigor { get; private set; }
        internal FilterMethod FilterMethod { get; private set; }

        internal ColumnFilter(TableColumn column, ICollection<string> values, FilterRigor filterRigor, FilterMethod filterMethod)
        {
            this.Column = column;
            this.Values = values;
            this.FilterRigor = filterRigor;
            this.FilterMethod = filterMethod;
        }

        internal ColumnFilter(TableColumn column, ColumnFilterRequest filterRequest)
        {
            if (column.Name.Trim() != filterRequest.ColumnName.Trim())
                throw new ArgumentException($"{nameof(column.Name)} is different from {nameof(filterRequest.ColumnName)}");

            this.Column = column;
            this.Values = filterRequest.Values;
            this.FilterRigor = filterRequest.FilterRigor;
            this.FilterMethod = filterRequest.FilterMethod;
        }
    }
}
