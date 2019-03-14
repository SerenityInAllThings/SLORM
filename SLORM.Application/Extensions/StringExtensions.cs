using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SLORM.Application.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        /// This methods replaces multiple white spaces in the string for a single one.
        /// So the string "a  b c d     e" would become "a b c d e".
        /// </summary>
        /// <param name="currentString"></param>
        /// <returns></returns>
        internal static string CleanWhitespacePolution(this string currentString) => Regex.Replace(currentString, @"\s+", " ");
    }
}
