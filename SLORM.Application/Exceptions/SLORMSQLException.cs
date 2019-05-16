using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace SLORM.Application.Exceptions
{
    public class SLORMSQLException : Exception
    {
        public string CommandQuery { get; private set; }
        public DbParameterCollection QueryParameters { get; set; }
        public SLORMSQLException(Exception originalException, string commandQuery, DbParameterCollection queryParameters) : base("SQL Query error", originalException)
        {
            CommandQuery = commandQuery;
            QueryParameters = queryParameters;
        }
    }
}
