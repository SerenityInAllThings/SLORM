using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.ValueObjects
{
    internal struct Statement
    {
        public string StatementText { get; private set; }
        public ICollection<DBParameterKeyValue> Parameters { get; private set; }

        public Statement(string statement, ICollection<DBParameterKeyValue> parameters)
        {
            this.StatementText = statement;
            this.Parameters = parameters;
        }
    }
}
