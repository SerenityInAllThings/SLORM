using SLORM.Application.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.ValueObjects
{
    public class TableColumn
    {
        public string Name { get; private set; }
        internal ColumnDataType DataType { get; private set; }

        internal TableColumn(string name, ColumnDataType dataType)
        {
            this.Name = name;
            this.DataType = dataType;
        }
    }
}
