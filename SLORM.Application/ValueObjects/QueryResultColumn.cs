using SLORM.Application.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.ValueObjects
{
    public struct QueryResultColumn
    {
        public string Name { get; private set; }
        public int Index { get; private set; }
        public ColumnDataType Type { get; private set; }

        public QueryResultColumn(string name, int index, ColumnDataType type)
        {
            this.Name = name;
            this.Index = index;
            this.Type = type;
        }
    }
}
