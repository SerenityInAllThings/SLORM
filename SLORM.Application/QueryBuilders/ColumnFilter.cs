using SLORM.Application.Enums;
using SLORM.Application.ValueObjects;
using System;

namespace SLORM.Application.QueryBuilders
{
    internal struct ColumnFilter
    {
        internal TableColumn Column { get; private set; }
        internal string Value { get; private set; }
        internal FilterRigor FilterRigor { get; private set; }
        internal FilterMethod FilterMethod { get; private set; }

        internal ColumnFilter(TableColumn column, string value, FilterRigor filterRigor, FilterMethod filterMethod)
        {
            this.Column = column ?? throw new ArgumentNullException(nameof(column));
            this.Value = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentNullException(nameof(value));
            this.FilterRigor = filterRigor;
            this.FilterMethod = filterMethod;
        }

        internal ColumnFilter(TableColumn column, ColumnFilterRequest filterRequest)
        {
            if (column.Name.Trim() != filterRequest.ColumnName.Trim())
                throw new ArgumentException($"{nameof(column.Name)} is different from {nameof(filterRequest.ColumnName)}");

            this.Column = column ?? throw new ArgumentNullException(nameof(column));
            this.Value = !string.IsNullOrWhiteSpace(filterRequest.Value) ? filterRequest.Value : throw new ArgumentNullException(nameof(filterRequest.Value));
            this.FilterRigor = filterRequest.FilterRigor;
            this.FilterMethod = filterRequest.FilterMethod;
        }
    }
}
