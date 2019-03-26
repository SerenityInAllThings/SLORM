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
        private static string[] notAllowedStrings = { ";", "'", "--", "xp_", "/*", "*/", "DROP TABLE", "AUX", "CLOCK$", "CONFIG$", "NUL", "PRN",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8" };

        /// <summary>
        /// This methods replaces multiple white spaces in the string for a single one.
        /// So the string "a  b c d     e" would become "a b c d e".
        /// </summary>
        /// <param name="currentString"></param>
        /// <returns></returns>
        internal static string CleanWhitespacePolution(this string currentString) => Regex.Replace(currentString, @"\s+", " ");
        internal static string SanitizeSQL(this string currentString)
        {
            foreach (var currentNotAllowedString in notAllowedStrings)
                currentString = currentString.Replace(currentNotAllowedString, string.Empty);
            return currentString;
        }
    }
}
