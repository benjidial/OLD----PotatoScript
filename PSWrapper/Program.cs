using System;
using System.Collections.Generic;
using NegativeFourPotatoes;
using NegativeFourPotatoes.PS;
using NegativeFourPotatoes.PS.Wrapper;

namespace NegativeFourPotatoes.PS.Wrapper
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            string strArgs = String.Empty;
            foreach (string arg in args) strArgs += (arg + " ");
            string strNextArg = String.Empty;
            List<string> cNewArgs = new List<string>();
            bool bInsideQuotes = false;
            foreach (char q in strArgs.ToCharArray())
            {
                if ((q == ' ') && (!bInsideQuotes)) { cNewArgs.Add(strNextArg); strNextArg = String.Empty; }
                else if (q == '"') bInsideQuotes = !bInsideQuotes;
                else strNextArg += q;
            }
            string[] a_strNewArgs = new string[cNewArgs.Count];
            for (int q = 0; q < cNewArgs.Count; q++) a_strNewArgs[q] = cNewArgs[q];
            Code.Hub.Wrapper(a_strNewArgs);
        }
    }
}
