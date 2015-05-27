using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Benji;
using Benji.PS;
using Benji.PS.Code;

namespace Benji.PS.Code
{
    /// <summary>
    /// This has the main wrapper's code.
    /// </summary>
    public static class Hub
    {
        /// <summary>
        /// These are the returnable states of the functions.
        /// </summary>
        public enum ExitState
        {
            /// <summary>
            /// The PSC code returned a negative value.
            /// </summary>
            NegativeExit = -5,
            /// <summary>
            /// A file error occurred, the source of which is not known.
            /// </summary>
            UnknownFileError = -4,
            /// <summary>
            /// A file or folder could not be found or does not exist.
            /// </summary>
            FileNotFound = -3,
            /// <summary>
            /// An unexpected error occurred.
            /// </summary>
            UnknownError = -2,
            /// <summary>
            /// The command passed to the command line did not have the correct syntax for the program.
            /// </summary>
            BadSyntax = -1,
            /// <summary>
            /// The PSC code returned a zero.
            /// </summary>
            Success = 0,
            /// <summary>
            /// The command 'PS /?' or 'PS /S' was passed to the command line.
            /// </summary>
            HelpCommand = 1,
            /// <summary>
            /// The PSC code returned a positive value.
            /// </summary>
            PositiveExit = 2,
        }

        /// <summary>
        /// This is the wrapper for most of the code.
        /// </summary>
        /// <param name="args">This is an array of arguments, not including the executable.</param>
        /// <returns>This returns a value representing the final state of the program.
        /// This is expressed as a <see cref="Benji.PS.Code.Hub.ExitState"/>.</returns>
        public static ExitState Wrapper(string[ ] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Invalid syntax.  Type 'PS /?' for help and examples.");
                    return ExitState.BadSyntax;
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
                        return ExitState.HelpCommand;
                    }
                    if (args[0] == "/S")
                    {
                        Console.WriteLine("PSC Syntax:");
                        Console.WriteLine();
                        Console.WriteLine("BEEP                 Makes a sound.");
                        Console.WriteLine("CLEARSCREEN          Clears the console screen.");
                        Console.WriteLine("COMMENT [text]       Comments out text.");
                        Console.WriteLine("CLEARSCREEN          Clears the console screen.");
                        Console.WriteLine("DIR [directory]      Changes PSW's working directory, or sets it to the root if no directory is supplied.");
                        Console.WriteLine("EXIT [number]        Exits the program and returns a specified integer (-128 to 127) to the OS.  If no number is supplied or the number is invalid, -128 is returned.");
                        Console.WriteLine("MAKEFOLDER [name]    Creates a folder in the current working directory with the specified name.");
                        Console.WriteLine("STARTPROCESS [name]  Starts the specified process on the user's machine.");
                        return ExitState.HelpCommand;
                    }
                }
                if (args[0].StartsWith("/F:", true, null))
                {
                    string[] q = new string[args.Length - 1];
                    for (uint w = 0; w < q.Length; w++) q[w] = args[w + 1];
                    return PSCProcessor.GetReady(args[0].TrimStart("/F:".ToCharArray()), q);
                }
                else
                {
                    Console.WriteLine("Invalid syntax.  Type 'PS /?' for help and examples.");
                    return ExitState.BadSyntax;
                }
            }
            catch (Exception q)
            {
                Console.WriteLine("An error occured!  Here's some info:");
                Console.WriteLine(q.ToString());
                return ExitState.UnknownError;
            }
        }
    }

    /// <summary>
    /// This contains the code to process and run PSC.
    /// </summary>
    public static class PSCProcessor
    {
        /// <summary>
        /// This reads the PSC file.
        /// </summary>
        private static StreamReader psc;
        /// <summary>
        /// This contains the variables.
        /// </summary>
        private static Dictionary<string, string> vars = new Dictionary<string,string>();

        /// <summary>
        /// This reads the PSC, converts it to PSMC, and prepares PS to process the PSMC.
        /// </summary>
        /// <param name="filename">This is the name of the .psc file.</param>
        /// <param name="args">These are the arguments to be passed to PSM</param>
        /// <returns>This returns a value in the form of <see cref="Benji.PS.Code.Hub.ExitState"/>
        /// representing the state at the end of the processing of the PSMC or a value representing
        /// an error occuring when converting the PSC to PSMC.</returns>
        internal static Hub.ExitState GetReady(string filename, string[ ] args)
        {
            /*****************/
             /**  Open File  **/
              /*****************/
            Console.Write("Opening file... ");
            try { psc = new StreamReader(filename); }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error! We could find the folder, but not the file.");
                return Hub.ExitState.FileNotFound;
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Error! We could not find the folder with that file.");
                return Hub.ExitState.FileNotFound;
            }
            catch (IOException q)
            {
                Console.WriteLine("Error!");
                Console.WriteLine(q.ToString());
                return Hub.ExitState.UnknownFileError;
            }
            catch (Exception q)
            {
                Console.WriteLine("Error!");
                Console.WriteLine(q.ToString());
                return Hub.ExitState.UnknownError;
            }
            Console.WriteLine("Success!");
            /*****************/
             /**  Read File  **/
              /*****************/
            Console.Write("Reading file... ");
            string strFullCode = "";
            do
            {
                string strLine = psc.ReadLine().Trim();
                if      (strLine.StartsWith(  "COMMENT", true, null)) continue;
                if      (strLine.ToUpper() == "BEEP")                       strFullCode +=  "BEEP\n";
                else if (strLine.ToUpper() == "CLEARSCREEN")                strFullCode +=  "CLEAR\n";
                else if (strLine.StartsWith(  "DIR ", true, null))          strFullCode += ("FOLDER\n" + strLine.Substring(4, strLine.Length - 4) + "\n");
                else if (strLine.StartsWith(  "EXIT ", true, null))         strFullCode += ("EXIT\n" + strLine.Substring(5, strLine.Length - 4) + "\n");
                else if (strLine.StartsWith(  "MAKEFOLDER ", true, null))   strFullCode += ("CREATEFOLDER\n" + strLine.Substring(11, strLine.Length - 11) + "\n");
                else if (strLine.StartsWith(  "STARTPROCESS ", true, null)) strFullCode += ("START\n" + strLine.Substring(13, strLine.Length - 13) + "\n");
                else Console.Write("Err: " + strLine + " ");
            } while (!psc.EndOfStream);
            Console.WriteLine("Success!");
            /********************************************************/
             /**  Close File, Set Argument Variables, and Run Code  **/
              /********************************************************/
            psc.Close();
            for (int q = 0; q < args.Length; q++) vars.Add("ARG" + q, args[q]);
            sbyte result = RunCode(strFullCode);
            if (result < 0) return Hub.ExitState.NegativeExit;
            else if (result > 0) return Hub.ExitState.PositiveExit;
            else if (result == 0) return Hub.ExitState.Success;
            else throw new Exception("Impossible state: sbyte result >= 0, <= 0, and != 0");
        }

        /// <summary>
        /// This runs PSMC.
        /// </summary>
        /// <param name="psmc">This is the PSMC to be run.</param>
        /// <returns>This returns a value representing the end state of the program.</returns>
        internal static sbyte RunCode(string psmc)
        {
            char[] q = new char[1];
            q[0] = '\n';
            while (psmc != String.Empty)
            {
                string strCurrentLine = psmc.Split(q)[0];
                psmc = psmc.Substring(strCurrentLine.Length + 1);
                switch (strCurrentLine)
                {
                    case "BEEP":
                        Console.Beep();
                        continue;
                    case "CLEAR":
                        Console.Clear();
                        continue;
                    case "FOLDER":
                        Environment.SetEnvironmentVariable("PS_DIR", psmc.Split(q)[0]);
                        psmc = psmc.Substring(psmc.Split(q)[0].Length + 1);
                        continue;
                    case "EXIT":
                        sbyte result;
                        if (sbyte.TryParse(psmc.Split(q)[0], out result)) return result; else return sbyte.MinValue;
                    case "CREATEFOLER":
                        Directory.CreateDirectory(Environment.GetEnvironmentVariable("PS_DIR") + psmc.Split(q)[0]);
                        psmc = psmc.Substring(psmc.Split(q)[0].Length + 1);
                        continue;
                }
            }
            return 1;
        }
    }
}