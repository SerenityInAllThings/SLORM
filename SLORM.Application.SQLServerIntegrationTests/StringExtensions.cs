using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SLORM.Application.SQLServerIntegrationTests
{
    internal static class StringExtensions
    {
        internal static string CleanWhitespacePolution(this string currentString) => Regex.Replace(currentString, @"\s+", " ");
    }
}
