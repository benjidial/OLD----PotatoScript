using System;
using System.Collections.Generic;
using System.IO;

namespace NegativeFourPotatoes.PS
{
    internal static class Misc
    {
        /// <summary>
        /// These are the returnable states of the functions.
        /// </summary>
        public enum ExitState
        {
            /// <summary>
            /// The PSC code returned a negative value.
            /// </summary>
            NegativeExit = -6,
            /// <summary>
            /// A file error occurred, the source of which is not known.
            /// </summary>
            UnknownFileError = -5,
            /// <summary>
            /// A file or folder could not be found or does not exist.
            /// </summary>
            FileNotFound = -4,
            /// <summary>
            /// There was an error in the syntax of a .psc file.
            /// </summary>
            PSCSyntaxError = -3,
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
    }

    public static class Wrapper
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
                    Console.WriteLine("CONCAT [1] [2] [3]   Sets the variable 3 equal to variable 1 + variable 2.");
                    Console.WriteLine("DIR [directory]      Changes the script's working directory, or sets it to the");
                    Console.WriteLine("                       root if no directory is supplied.");
                    Console.WriteLine("EXIT [number]        Exits the program and returns a specified integer (-128 to");
                    Console.WriteLine("                       127) to the OS.  If no number is supplied or the number");
                    Console.WriteLine("                       is invalid, -128 is returned.");
                    Console.WriteLine("LOG [text]           Logs some text to a log file in the working directory.");
                    Console.WriteLine("LOGVAR [variable]    Logs a variable to a log file in the working directory.");
                    Console.WriteLine("MAKEFOLDER [folder]  Creates a folder in the current working directory with the");
                    Console.WriteLine("                       specified name.");
                    Console.WriteLine("OUTPUT [text]        Outputs some text to the console.");
                    Console.WriteLine("OUTPUTVAR [variable] Outputs a variable to the console.");
                    Console.WriteLine("READ [variable]      Reads from the console into a variable.");
                    Console.WriteLine("SETVAR [var] [text]  Sets a variable.");
                    Console.WriteLine("STARTPROCESS [name]  Opens the specified file in the working directory on the");
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

    internal static class PSCProcessor
    {
        private static StreamReader psc;
        private static Dictionary<string, string> vars = new Dictionary<string, string>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "sbyte"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal static Misc.ExitState GetReady(string filename, string[ ] args)
        {
            /*****************/
            /**  Open File  **/
            /*****************/
            Console.Write("Opening file... ");
            try { psc = new StreamReader(filename); }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error! We could find the folder, but not the file.");
                return Misc.ExitState.FileNotFound;
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Error! We could not find the folder with that file.");
                return Misc.ExitState.FileNotFound;
            }
            catch (IOException q)
            {
                Console.WriteLine("Error!");
                Console.WriteLine(q.ToString());
                return Misc.ExitState.UnknownFileError;
            }
            catch (Exception q)
            {
                Console.WriteLine("Error!");
                Console.WriteLine(q.ToString());
                return Misc.ExitState.UnknownError;
            }
            Console.WriteLine("Success!");
            /*****************/
            /**  Read File  **/
            /*****************/
            Console.Write("Reading file... ");
            string strFullCode = "";
            char[] space = new char[1];
            space[0] = ' ';
            bool bCorrect = true;
            do
            {
                string strLine = psc.ReadLine().Trim();
                if      (strLine.StartsWith(  "COMMENT", true, null))       continue;
                if      (strLine.ToUpper() == "BEEP")                       strFullCode +=  "SOUND\n";
                else if (strLine.ToUpper() == "CLEARSCREEN")                strFullCode +=  "CLEAR\n";
                else if (strLine.StartsWith(  "CONCAT ", true, null))       strFullCode += ("CONCATENATE\n"     + strLine.Substring(7, strLine.Length  - 7).Split(space)[0] + "\n" + strLine.Substring(7, strLine.Length - 7).Split(space)[1] + "\n" + strLine.Substring(7, strLine.Length - 7).Split(space)[2] + "\n");
                else if (strLine.StartsWith(  "DIR ", true, null))          strFullCode += ("SETVAR\n__DIR__\n" + strLine.Substring(4,  strLine.Length - 4)  + "\n");
                else if (strLine.StartsWith(  "EXIT ", true, null))         strFullCode += ("EXIT\n"            + strLine.Substring(5,  strLine.Length - 5)  + "\n");
                else if (strLine.StartsWith(  "LOG ", true, null))          strFullCode += ("LOG\n"             + strLine.Substring(4,  strLine.Length - 4)  + "\n");
                else if (strLine.StartsWith(  "LOGVAR ", true, null))       strFullCode += ("LOGVARIABLE\n"     + strLine.Substring(7,  strLine.Length - 7)  + "\n");
                else if (strLine.StartsWith(  "MAKEFOLDER ", true, null))   strFullCode += ("CREATEFOLDER\n"    + strLine.Substring(11, strLine.Length - 11) + "\n");
                else if (strLine.StartsWith(  "OUTPUT ", true, null))       strFullCode += ("CONSOLE\n"         + strLine.Substring(7,  strLine.Length - 7)  + "\n");
                else if (strLine.StartsWith(  "OUTPUTVAR ", true, null))    strFullCode += ("CONSOLEVARIABLE\n" + strLine.Substring(10, strLine.Length - 10) + "\n");
                else if (strLine.StartsWith(  "READ ", true, null))         strFullCode += ("USERVARIABLE\n"    + strLine.Substring(5,  strLine.Length - 5)  + "\n");
                else if (strLine.StartsWith(  "SETVAR ", true, null))       strFullCode += ("VARIABLE\n"        + strLine.Substring(7,  strLine.Length - 7).Split(space)[0] + "\n" + strLine.Substring(7, strLine.Length - 7).Split(space)[1] + "\n");
                else if (strLine.StartsWith(  "STARTPROCESS ", true, null)) strFullCode += ("START\n"           + strLine.Substring(13, strLine.Length - 13) + "\n");
                else { Console.Write("Err: " + strLine + " "); bCorrect = false; }
            } while (!psc.EndOfStream);
            if (bCorrect) Console.WriteLine("Success!");
            else
            {
                Console.WriteLine("\nOne or more error(s) occured!");
                bool bValid;
                do
                {
                    Console.Write("Would you like to attempt to run this code anyway? (Y/N)");
                    char q = (char)Console.Read();
                    if ((q == 'y') || (q == 'Y')) bValid = true;
                    else if ((q == 'n') || (q == 'N')) return Misc.ExitState.PSCSyntaxError;
                    else bValid = false;
                } while (!bValid);
            }
            /****************************/
            /**  Et Cetera, Et Cetera  **/
            /****************************/
            psc.Close();
            for (int q = 0; q < args.Length; q++) vars.Add("ARG" + q, args[q]);
            string strTitle = Console.Title;
            Console.Title = "PotatoScript";
            sbyte result = RunCode(strFullCode);
            Console.Title = strTitle;
            if (result < 0) return Misc.ExitState.NegativeExit;
            else if (result > 0) return Misc.ExitState.PositiveExit;
            else if (result == 0) return Misc.ExitState.Success;
            else throw new Exception("Impossible state: sbyte result >= 0, <= 0, and != 0");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static sbyte RunCode(string psmc)
        {
            char[] q = new char[1];
            q[0] = '\n';
            while (!String.IsNullOrEmpty(psmc))
            {
                string strCurrentLine = psmc.Split(q)[0];
                psmc = psmc.Substring(strCurrentLine.Length + 1);
                switch (strCurrentLine)
                {
                    case "SOUND":
                        Console.Beep();
                        continue;
                    case "CLEAR":
                        Console.Clear();
                        continue;
                    case "CONCATENATE":
                        if (vars.ContainsKey(psmc.Split(q)[2])) vars.Remove(psmc.Split(q)[2]);
                        string string1 = String.Empty;
                        string string2 = String.Empty;
                        vars.TryGetValue(psmc.Split(q)[0], out string1);
                        vars.TryGetValue(psmc.Split(q)[1], out string2);
                        vars.Add(psmc.Split(q)[2], string1 + string2);
                        psmc = psmc.Substring(psmc.Split(q)[0].Length + 1).Substring(psmc.Split(q)[0].Length + 1).Substring(psmc.Split(q)[0].Length + 1);
                        continue;
                    case "EXIT":
                        sbyte result;
                        if (sbyte.TryParse(psmc.Split(q)[0], out result)) return result; else return sbyte.MinValue;
                    case "LOG":
                        StreamWriter logfile = null;
                        try
                        {
                            logfile = new StreamWriter(vars("__DIR__") + "logfile.log", true);
                            logfile.WriteLine(psmc.Split(q)[0]);
                            logfile.Flush();
                        }
                        catch (UnauthorizedAccessException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Try running this from an administrator command prompt.");
                        }
                        catch (DirectoryNotFoundException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Is it on a disconnected network drive?");
                        }
                        catch (PathTooLongException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("You're too organized!");
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Where is it?");
                        }
                        catch (IOException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("In what language is this folder?");
                        }
                        finally
                        {
                            if (logfile != null) logfile.Close();
                        }
                        psmc = psmc.Substring(psmc.Split(q)[0].Length + 1);
                        continue;
                    case "LOGVARIABLE":
                        string variable = String.Empty;
                        logfile = null;
                        try
                        {
                            logfile = new StreamWriter(vars("__DIR__") + "logfile.log", true);
                            vars.TryGetValue(psmc.Split(q)[0], out variable);
                            logfile.WriteLine(variable);
                            logfile.Flush();
                        }
                        catch (UnauthorizedAccessException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Try running this from an administrator command prompt.");
                        }
                        catch (DirectoryNotFoundException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Is it on a disconnected network drive?");
                        }
                        catch (PathTooLongException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("You're too organized!");
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Where is it?");
                        }
                        catch (IOException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("In what language is this folder?");
                        }
                        finally
                        {
                            if (logfile != null) logfile.Close();
                        }
                        psmc = psmc.Substring(psmc.Split(q)[0].Length + 1);
                        continue;
                    case "CREATEFOLER":
                        try { Directory.CreateDirectory(vars("__DIR__") + psmc.Split(q)[0]); }
                        catch (PathTooLongException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("I don't think you need any more folders.");
                        }
                        catch (DirectoryNotFoundException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Plug it in!");
                        }
                        catch (IOException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Fix it!");
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("I can't read this!");
                        }
                        catch (NotSupportedException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("What did I tell you about using colons in your folder names?");
                        }
                        psmc = psmc.Substring(psmc.Split(q)[0].Length + 1);
                        continue;
                    case "CONSOLE":
                        Console.WriteLine(psmc.Split(q)[0]);
                        psmc = psmc.Substring(psmc.Split(q)[0].Length + 1);
                        continue;
                    case "CONSOLEVARIABLE":
                        variable = String.Empty;
                        vars.TryGetValue(psmc.Split(q)[0], out variable);
                        Console.WriteLine(variable);
                        psmc = psmc.Substring(psmc.Split(q)[0].Length + 1);
                        continue;
                    case "USERVARIABLE":
                        if (vars.ContainsKey(psmc.Split(q)[0])) vars.Remove(psmc.Split(q)[0]);
                        vars.Add(psmc.Split(q)[0], Console.ReadLine());
                        psmc = psmc.Substring(psmc.Split(q)[0].Length + 1);
                        continue;
                    case "VARIABLE":
                        if (vars.ContainsKey(psmc.Split(q)[0])) vars.Remove(psmc.Split(q)[0]);
                        vars.Add(psmc.Split(q)[0], psmc.Split(q)[1]);
                        psmc = psmc.Substring(psmc.Split(q)[0].Length + 1).Substring(psmc.Split(q)[0].Length + 1);
                        continue;
                    case "START":
                        System.Diagnostics.Process process = null;
                        try
                        {
                            process = new System.Diagnostics.Process();
                            process.StartInfo.FileName = vars("__DIR__") + psmc.Split(q)[0];
                            if (!process.Start()) Console.WriteLine("Warning!  Could not start \'" + process.StartInfo.FileName + "\'!");
                        }
                        catch (InvalidOperationException) { }
                        finally
                        {
                            if (process != null) process.Close();
                        }
                        continue;
                }
            }
            return 1;
        }
    }
}
