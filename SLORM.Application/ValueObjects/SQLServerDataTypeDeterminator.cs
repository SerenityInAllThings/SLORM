using SLORM.Application.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SLORM.Application.ValueObjects
{
    internal class SQLServerDataTypeDeterminator : ISQLServerDataTypeDeterminator
    {
        private static string[] stringTypes = new string[]
        {
            "char",
            "varchar",
            "text",
            "nchar",
            "nvarchar",
            "ntext",
            "binary",
            "varbinary",
            "image",
        };

        private static string[] numberTypes = new string[]
        {
            "bit",
            "tinyint",
            "smallint",
            "int",
            "bigint",
            "decimal",
            "numeric",
            "smallmoney",
            "money",
            "float",
            "real",
        };

        private static string[] dateTypes = new string[]
        {
            "datetime",
            "datetime2",
            "smalldatetime",
            "date",
            "time",
            "datetimeoffset",
            "timestamp",
        };

        private static string[] otherTypes = new string[]
        {
            "sql_invariant",
            "uniqueidentifier",
            "xml",
            "cursor",
            "table"
        };

        ColumnDataType ISQLServerDataTypeDeterminator.FromDataTypeField(string dataTypeFieldContent)
        {
            var loweredDataType = dataTypeFieldContent.ToLower();

            if (stringTypes.Contains(loweredDataType))
                return ColumnDataType.String;
            if (numberTypes.Contains(loweredDataType))
                return ColumnDataType.Number;
            if (dateTypes.Contains(loweredDataType))
                return ColumnDataType.Date;
            if (otherTypes.Contains(loweredDataType))
                return ColumnDataType.Other;

            return ColumnDataType.Default;
        }
    }
}
