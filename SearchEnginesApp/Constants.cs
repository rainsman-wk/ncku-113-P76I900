using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchEnginesApp
{
    public struct StatusLabel
    {
        public const string Ready = "Ready";
        public const string Busy = "Busy...";
        public const string Error = "Error, click here";
    }
    public class Constants
    {
        public static string Log = Environment.CurrentDirectory + "\\log";
        public static string ReferenceFiles = Environment.CurrentDirectory + "\\ReferenceFile";

        /// <summary>
        /// Delimiter contains reusable delimiters for reuse
        /// </summary>
        public struct Delimiter
        {
            public const char Comma = ',';
            public const char Space = ' ';
            public const char Dot = '.';
            public const char ForwardSlash = '/';
        }
    }


}
