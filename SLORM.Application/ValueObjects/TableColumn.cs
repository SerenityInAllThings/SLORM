using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.ValueObjects
{
    public class TableColumn
    {
        public string Name { get; private set; }

        public TableColumn(string name)
        {
            this.Name = name;
        }
    }
}
