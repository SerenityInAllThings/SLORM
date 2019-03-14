using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SLORM.Application.UnitTests.TestHelpers
{
    internal static class StringExtensions
    {
        internal static bool IsSqlValid(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            var parser = new TSql120Parser(false);
            IList<ParseError> errors;
            using (var reader = new StringReader(str))
            {
                parser.Parse(reader, out errors);
            }
            return errors.Count() == 0;
        }
    }
}
