using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.Exceptions
{
    public class UnknownSQLProviderException : Exception
    {
        private static readonly string errorMessageTemplate = "The provided connection string has an unknown or unsupported SQL Provider: \n{0}";

        internal UnknownSQLProviderException(string connectionString) 
            : base(string.Format(errorMessageTemplate, connectionString))
        { }

        internal UnknownSQLProviderException() : base("Unsupported SQL provider type")
        { }
    }
}
