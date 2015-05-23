using System;
using System.Collections.Generic;
using System.Text;
using Benji;
using Benji.PSW;
using Benji.PSW.Windows;

namespace Benji.PSW.Windows
{
    class MainProgram
    {
        static void Main(string[ ] args)
        {
            try { Code.Hub.Wrapper(args); } catch (Exception q) { Console.WriteLine(q.ToString()); }
        }
    }
}
