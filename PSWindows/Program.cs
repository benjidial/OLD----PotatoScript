using System;
using System.Collections.Generic;
using System.Text;
using Benji;
using Benji.PS;
using Benji.PS.Windows;

namespace Benji.PS.Windows
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            string strArgs = String.Empty;
            bool bFirst = true;
            foreach (string arg in args) { if (bFirst) bFirst = false; else strArgs += " "; strArgs += arg; }
            string strNextArg = String.Empty;
            List<string> cNewArgs = new List<string>();
            bool bInsideQuotes = false;
            foreach (char q in strArgs.ToCharArray())
            {
                if ((q == ' ') && (bInsideQuotes = false)) { cNewArgs.Add(strNextArg); strNextArg = String.Empty; }
                else if (q == '"') bInsideQuotes = !bInsideQuotes;
                else strNextArg += q;
            }
            string[] a_strNewArgs = new string[cNewArgs.Count];
            for (int q = 0; q < cNewArgs.Count; q++) a_strNewArgs[q] = cNewArgs[q];
            Code.Hub.Wrapper(a_strNewArgs);
        }
    }
}
