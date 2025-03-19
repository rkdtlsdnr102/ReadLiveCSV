using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlotCSV
{
    public class RegexChecker
    {
        static public bool IsNumber(string text)
        {
            return Regex.IsMatch(text, "^[0-9]+$");
        }
    }
}
