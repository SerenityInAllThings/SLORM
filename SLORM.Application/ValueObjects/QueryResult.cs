using SLORM.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SLORM.Application.ValueObjects
{
    public class QueryResult
    {
        public IList<QueryResultColumn> Columns { get; private set; }
        public IList<QueryResultRow> Rows { get; private set; }
        private ISQLServerDataTypeDeterminator typeDeterminator = new SQLServerDataTypeDeterminator();

        protected QueryResult() { }

        public QueryResult(DbDataReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (!reader.CanGetColumnSchema())
                throw new CannotGetColumnSchemaException();

            var columnSchema = reader.GetColumnSchema();
            Columns = new List<QueryResultColumn>(columnSchema.Count());
            for (var columnIndex = 0; columnIndex < columnSchema.Count(); columnIndex++)
            {
                var currentColumn = columnSchema.ElementAt(columnIndex);
                var parsedColumn = new QueryResultColumn(currentColumn.ColumnName, columnIndex, typeDeterminator.FromDataTypeField(currentColumn.DataTypeName));
                Columns.Add(parsedColumn);
            }

            Rows = new List<QueryResultRow>();
            while (reader.Read())
            {
                var values = new string[columnSchema.Count()];
                for (var columnIndex = 0; columnIndex < columnSchema.Count(); columnIndex++)
                    values[columnIndex] = reader[columnIndex].ToString();
                var parsedRow = new QueryResultRow(values);
                Rows.Add(parsedRow);
            }
        }
    }
}
