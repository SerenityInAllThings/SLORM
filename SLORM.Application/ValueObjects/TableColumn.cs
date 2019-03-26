using SLORM.Application.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.ValueObjects
{
    internal class TableColumn
    {
        internal string Name { get; private set; }
        internal ColumnDataType DataType { get; private set; }

        public int Idade { get; set; }

        internal TableColumn(string name, ColumnDataType dataType)
        {
            this.Name = name;
            this.DataType = dataType;
        }
    }
}
