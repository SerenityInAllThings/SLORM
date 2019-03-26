using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.ValueObjects
{
    public struct QueryResultRow
    {
        public IList<string> Values { get; }
        public QueryResultRow(string[] values)
        {
            Values = new List<string>(values);
        }
    }
}
