using System;
using System.Collections.Generic;
using NegativeFourPotatoes;
using NegativeFourPotatoes.PS;

namespace NegativeFourPotatoes.PS
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
            ProcessCommandLine(a_strNewArgs);
        }

        private static Misc.ExitState ProcessCommandLine(string[ ] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Invalid syntax.  Type 'PS /?' for help and examples.");
                return Misc.ExitState.BadSyntax;
            }
            if (args.Length == 1)
            {
                if (args[0] == "/?")
                {
                    Console.WriteLine("Executes PotatoScript code.");
                    Console.WriteLine();
                    Console.WriteLine("PS /F:filename [argument [argument [argument...]]]");
                    Console.WriteLine();
                    Console.WriteLine("  /F:filename  Specifies a file to execute.");
                    Console.WriteLine("  argument     Specifies the argument or arguments to be passed to the file.");
                    Console.WriteLine();
                    Console.WriteLine("Arguments which contain spaces must be put inside double quotes.");
                    Console.WriteLine();
                    Console.WriteLine("For example:");
                    Console.WriteLine();
                    Console.WriteLine("PS /F:Adder.psc 1 2");
                    Console.WriteLine("PS /F:Printer.psc \"Hello, world!\"");
                    Console.WriteLine("PS /F:SpaceInvaders.psc");
                    Console.WriteLine("PS /F:RepeatText.psc \"Hello, world!\" 42");
                    Console.WriteLine();
                    Console.WriteLine("For help with the syntax of PSC, do 'PS /S'.");
                    return Misc.ExitState.HelpCommand;
                }
                if (args[0] == "/S")
                {
                    Console.WriteLine("PSC Syntax:");
                    Console.WriteLine();
                    Console.WriteLine("BEEP                 Makes a sound.");
                    Console.WriteLine("CLEARSCREEN          Clears the console screen.");
                    Console.WriteLine("COMMENT [text]       Comments out text.");
                    Console.WriteLine("CLEARSCREEN          Clears the console screen.");
                    Console.WriteLine("DIR [directory]      Changes PS's working directory, or sets it to the root if");
                    Console.WriteLine("                       no directory is supplied.");
                    Console.WriteLine("EXIT [number]        Exits the program and returns a specified integer (-128 to");
                    Console.WriteLine("                       127) to the OS.  If no number is supplied or the number");
                    Console.WriteLine("                       is invalid, -128 is returned.");
                    Console.WriteLine("LOG [text]           Logs some text to a logfile in the working directory.");
                    Console.WriteLine("MAKEFOLDER [name]    Creates a folder in the current working directory with the");
                    Console.WriteLine("                       specified name.");
                    Console.WriteLine("OUTPUT [text]        Outputs some text to the console.");
                    Console.WriteLine("STARTPROCESS [name]  Opens the specified file in the working directoy on the");
                    Console.WriteLine("                       user's machine.");
                    return Misc.ExitState.HelpCommand;
                }
            }
            if (args[0].StartsWith("/F:", true, null))
            {
                string[] q = new string[args.Length - 1];
                for (uint w = 0; w < q.Length; w++) q[w] = args[w + 1];
                return PSCProcessor.GetReady(args[0].Substring(3, args[0].Length - 3), q);
            }
            else
            {
                Console.WriteLine("Invalid syntax.  Type 'PS /?' for help and examples.");
                return Misc.ExitState.BadSyntax;
            }
        }
    }
}
