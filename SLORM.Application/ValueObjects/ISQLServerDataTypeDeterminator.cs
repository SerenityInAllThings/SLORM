using SLORM.Application.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.ValueObjects
{
    internal interface ISQLServerDataTypeDeterminator
    {
        ColumnDataType FromDataTypeField(string dataTypeFieldContent);
    }
}
