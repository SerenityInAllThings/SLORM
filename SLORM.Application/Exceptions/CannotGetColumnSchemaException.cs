using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.Exceptions
{
    public class CannotGetColumnSchemaException : Exception
    {
        internal CannotGetColumnSchemaException() : base("It was not possible to get the column schema for this query, thus results cannot be parsed.") 
        { }
    }
}
