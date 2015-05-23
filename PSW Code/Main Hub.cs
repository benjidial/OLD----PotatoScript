using System;
using System.Collections.Generic;
using System.Text;
using Benji;
using Benji.PSW;
using Benji.PSW.Code;

namespace Benji.PSW.Code
{
    public static class Hub
    {
        public enum ExitState
        {
            HelpCommand = 1,
        }

        /// <summary>
        /// This is the wrapper for most of the code.
        /// </summary>
        /// <param name="args">This is an array of arguments, not including the executable.</param>
        /// <returns>This returns a value representing the final state of the program.
        /// This is expressed as a <see cref="Benji.PSW.Code.Hub.ExitState"/>.</returns>
        public static ExitState Wrapper(string[ ] args)
        {
            if ((args.Length == 1) && (args[0] == "/?"))
            {
                Console.WriteLine("Executes PotatoScriptW code.");
                Console.WriteLine();
                Console.WriteLine("PSW /F:filename [argument [argument [argument...]]]");
                Console.WriteLine();
                Console.WriteLine("  /F:filename  Specifies a file to execute.");
                Console.WriteLine("  argument     Specifies the argument or arguments to be passed to the file.");
                Console.WriteLine();
                Console.WriteLine("Arguments which contain spaces must be put inside double quotes.");
                Console.WriteLine();
                Console.WriteLine("For example:");
                Console.WriteLine();
                Console.WriteLine("PSW /F:Adder.pswc 1 2");
                Console.WriteLine("PSW /F:Printer.pswc \"Hello, world!\"");
                Console.WriteLine("PSW /F:SpaceInvaders.pswc");
                Console.WriteLine("PSW /F:RepeatText.pswc \"Hello, world!\" 42");
                Console.WriteLine();
                return ExitState.HelpCommand;
            }
            throw new System.NotImplementedException("This command has not been put into PSW.");
        }
    }
}
