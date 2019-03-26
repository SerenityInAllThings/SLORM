using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.ValueObjects
{
    internal struct DBParameterKeyValue
    {
        public string Key { get; private set; }
        public string Value { get; private set; }

        public DBParameterKeyValue(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
