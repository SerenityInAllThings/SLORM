using SLORM.Application.Enums;
using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.QueryBuilders
{
    internal struct ColumnOrdering
    {
        internal TableColumn Column { get; private set; }
        internal OrderType OrderType { get; private set; }

        internal ColumnOrdering(TableColumn column, OrderType orderType)
        {
            this.Column = column ?? throw new ArgumentNullException(nameof(column));
            this.OrderType = orderType;
        }

        internal ColumnOrdering(TableColumn column, ColumnOrderingRequest request)
        {
            if (column.Name.Trim() != request.ColumnName.Trim())
                throw new ArgumentException($"{nameof(column.Name)} is different from {nameof(request.ColumnName)}");

            this.Column = column ?? throw new ArgumentNullException(nameof(column));
            this.OrderType = request.OrderType;
        }
    }
}
