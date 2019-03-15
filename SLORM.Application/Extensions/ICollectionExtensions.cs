using SLORM.Application.QueryBuilders;
using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLORM.Application.Extensions
{
    internal static class ICollectionExtensions
    {
        internal static TableColumn GetFromName(this ICollection<TableColumn> tableColumns, string columnName)
        {
            return tableColumns
                .FirstOrDefault(c => c.Name.ToLower() == columnName.ToLower());
        }

        internal static bool Contains(this ICollection<TableColumn> tableColumns, TableColumn column) => tableColumns
            .Any(c => c.Name == column.Name && c.DataType == column.DataType);
    }
}
