using System;
using System.Collections.Generic;
using System.IO;

namespace NFP.PS
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
                    Console.WriteLine("PS filename [argument [argument [argument...]]]");
                    Console.WriteLine();
                    Console.WriteLine("  filename  Specifies a file to execute.");
                    Console.WriteLine("  argument  Specifies the argument or arguments to be passed to the file.");
                    Console.WriteLine();
                    Console.WriteLine("Arguments which contain spaces must be put inside double quotes.");
                    Console.WriteLine();
                    Console.WriteLine("For example:");
                    Console.WriteLine();
                    Console.WriteLine("PS Adder.psc 1 2");
                    Console.WriteLine("PS Printer.psc \"Hello, world!\"");
                    Console.WriteLine("PS SpaceInvaders.psc");
                    Console.WriteLine("PS RepeatText.psc \"Hello, world!\" 42");
                    Console.WriteLine();
                    Console.WriteLine("For help with the syntax of PSC, do 'PS /S'.");
                    return Misc.ExitState.HelpCommand;
                }
                if (args[0] == "/S")
                {
                    Console.WriteLine("PSC Syntax:");
                    Console.WriteLine("Commands are not case-sensitive, but arguments are.");
                    Console.WriteLine();
                    Console.WriteLine("ADD n1 n2 n3         Sets 3 equal to 1 + 2.");
                    Console.WriteLine("BEEP                 Makes a sound.");
                    Console.WriteLine("CLEARSCREEN          Clears the console screen.");
                    Console.WriteLine("COMMENT text         Comments out text.");
                    Console.WriteLine("CONCAT s1 s2 s3      Sets the variable 3 equal to variable 1 + variable 2.");
                    Console.WriteLine("COPY n1 n2           Sets n2 equal to n1.");
                    Console.WriteLine("DIR folder           Changes the script's working directory, or sets it to the");
                    Console.WriteLine("                       root if no directory is supplied.");
                    Console.WriteLine("DIVIDE n1 n2 n3      Sets 3 equal to 1 / 2.");
                    Console.WriteLine("EXIT number          Exits the program and returns a specified integer (-128 to");
                    Console.WriteLine("                       127) to the OS.  If no number is supplied or the number");
                    Console.WriteLine("                       is invalid, -128 is returned.");
                    Console.WriteLine("EXPONENT n1 n2 n3    Sets 3 equal to 1 ^ 2.");
                    Console.WriteLine("LOG text             Logs some text to a log file in the working directory.");
                    Console.WriteLine("LOGVAR variable      Logs a variable to a log file in the working directory.");
                    Console.WriteLine("MAKEFOLDER folder    Creates a folder in the current working directory with the");
                    Console.WriteLine("                       specified name.");
                    Console.WriteLine("MULTIPLY n1 n2 n3    Sets 3 equal to 1 * 2.");
                    Console.WriteLine("OUTPUT text          Outputs some text to the console.");
                    Console.WriteLine("OUTPUTVAR variable   Outputs a variable to the console.");
                    Console.WriteLine("READ variable        Reads from the console into a variable.");
                    Console.WriteLine("SETVAR var text      Sets a variable.");
                    Console.WriteLine("STARTPROCESS file    Opens the specified file in the working directory on the");
                    Console.WriteLine("                       user's machine.");
                    Console.WriteLine("SUBTRACT n1 n2 n3    Sets 3 equal to 1 - 2.");
                    Console.WriteLine("TONUMBER svar nvar   Converts a string variable to a number variable.");
                    Console.WriteLine("TOSTRING nvar svar   Converts a number variable to a string variable.");
                    return Misc.ExitState.HelpCommand;
                }
            }
            string[] q = new string[args.Length - 1];
            for (uint w = 0; w < q.Length; w++) q[w] = args[w + 1];
            return PSCProcessor.GetReady(args[0], q);
        }
    }

    internal static class PSCProcessor
    {
        private static string rest(string from, int offset)
        {
            return from.Substring(offset, from.Length - offset);
        }
        private static void nextline(out string newtext, string oldtext, string line)
        {
            newtext = oldtext.Substring(line.Length + 1);
        }

        private static StreamReader psc;
        private static Dictionary<string, string> vars = new Dictionary<string, string>();
        private static Dictionary<string, double> nums = new Dictionary<string, double>();
        private static Dictionary<string, bool>arenums = new Dictionary<string, bool>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "sbyte"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
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
                System.Globalization.CultureInfo cul = System.Globalization.CultureInfo.InvariantCulture;
                string strLine = psc.ReadLine().Trim();
                if (strLine.StartsWith("COMMENT", true, null)) continue;
                if (strLine.StartsWith("ADD ", true, null)) strFullCode += String.Format("VARIABLE\n__ZERO__\n0\nSUBTRACT\n__ZERO__\n{1}\n__TEMP__\nSUBRACT\n{0}\n__TEMP__\n{2}", rest(strLine, 4).Split(space)[0], rest(strLine, 4).Split(space)[1], rest(strLine, 4).Split(space)[2]);
                else if (strLine.ToUpper(cul) == "BEEP") strFullCode += "SOUND\n";
                else if (strLine.ToUpper(cul) == "CLEARSCREEN") strFullCode += "CLEAR\n";
                else if (strLine.StartsWith("CONCAT ", true, null)) strFullCode += String.Format("CONCATENATE\n{0}\n{1}\n{2}\n", rest(strLine, 7).Split(space)[0], rest(strLine, 7).Split(space)[1], rest(strLine, 7).Split(space)[2]);
                else if (strLine.StartsWith("COPY ", true, null)) strFullCode += String.Format("COPYVARIABLE\n{0}\n{1}\n", rest(strLine, 5).Split(space)[0], rest(strLine, 5).Split(space)[1]);
                else if (strLine.StartsWith("DIR ", true, null)) strFullCode += String.Format("VARIABLE\n__DIR__\n{0}\n", rest(strLine, 4));
                else if (strLine.StartsWith("DIVIDE ", true, null)) strFullCode += String.Format("DIVIDE\n{0}\n{1}\n{2}\n", rest(strLine, 7).Split(space)[0], rest(strLine, 7).Split(space)[1], rest(strLine, 7).Split(space)[2]);
                else if (strLine.StartsWith("EXIT ", true, null)) strFullCode += String.Format("EXIT\n{0}\n", rest(strLine, 5));
                else if (strLine.StartsWith("EXPONENT ", true, null)) strFullCode += String.Format("EXPONENT\n{0}\n{1}\n{2}\n", rest(strLine, 9).Split(space)[0], rest(strLine, 9).Split(space)[1], rest(strLine, 9).Split(space)[2]);
                else if (strLine.StartsWith("LOG ", true, null)) strFullCode += String.Format("VARIABLE\n__TEMP__\n{0}\nLOG\n__TEMP__\n", rest(strLine, 4));
                else if (strLine.StartsWith("LOGVAR ", true, null)) strFullCode += String.Format("LOG\n{0}\n", rest(strLine, 7));
                else if (strLine.StartsWith("MAKEFOLDER ", true, null)) strFullCode += String.Format("CREATEFOLDER\n{0}\n", rest(strLine, 11));
                else if (strLine.StartsWith("MULTIPLY ", true, null)) strFullCode += String.Format("VARIABLE\n__ONE__\n1\nDIVIDE\n__ONE__\n{1}\n__TEMP__\nDIVIDE\n{0}\n__TEMP__\n{2}", rest(strLine, 9).Split(space)[0], rest(strLine, 9).Split(space)[1], rest(strLine, 9).Split(space)[2]);
                else if (strLine.StartsWith("OUTPUT ", true, null)) strFullCode += String.Format("VARIABLE\n__TEMP__\n{0}\nCONSOLE\n__TEMP__\n", rest(strLine, 7));
                else if (strLine.StartsWith("OUTPUTVAR ", true, null)) strFullCode += String.Format("CONSOLE\n{0}\n", rest(strLine, 10));
                else if (strLine.StartsWith("READ ", true, null)) strFullCode += String.Format("USERVARIABLE\n{0}\n", rest(strLine, 5));
                else if (strLine.StartsWith("SETVAR ", true, null)) strFullCode += String.Format("VARIABLE\n{0}\n{1}\n", rest(strLine, 7).Split(space)[0], rest(strLine, 7).Split(space)[1]);
                else if (strLine.StartsWith("STARTPROCESS ", true, null)) strFullCode += String.Format("START\n{0}\n", rest(strLine, 13));
                else if (strLine.StartsWith("SUBTRACT ", true, null)) strFullCode += String.Format("SUBTRACT\n{0}\n{1}\n{2}\n", rest(strLine, 9).Split(space)[0], rest(strLine, 9).Split(space)[1], rest(strLine, 9).Split(space)[2]);
                else if (strLine.StartsWith("TONUMBER ", true, null)) strFullCode += String.Format("TONUMBER\n{0}\n{1}", rest(strLine, 9).Split(space)[0], rest(strLine, 9).Split(space)[1]);
                else if (strLine.StartsWith("TOSTRING ", true, null)) strFullCode += String.Format("TOSTRING\n{0}\n{1}", rest(strLine, 9).Split(space)[0], rest(strLine, 9).Split(space)[1]);
                else { Console.Write("Err: " + strLine + " "); bCorrect = false; }
            } while (!psc.EndOfStream);
            if (bCorrect) Console.WriteLine("Success!");
            else
            {
                Console.WriteLine("\nOne or more error(s) occurred!");
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
            for (int q = 0; q < args.Length; q++)
            {
                vars.Add("ARG" + q, args[q]);
                arenums.Add("ARG" + q, false);
            }
            string strTitle = Console.Title;
            Console.Title = "PotatoScript";
            sbyte result = RunCode(strFullCode);
            Console.Title = strTitle;
            if (result < 0) return Misc.ExitState.NegativeExit;
            else if (result > 0) return Misc.ExitState.PositiveExit;
            else return Misc.ExitState.Success;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static sbyte RunCode(string psmc)
        {
            bool isnum;
            char[] q = new char[1];
            q[0] = '\n';
            while (!String.IsNullOrEmpty(psmc))
            {
                string strLine = psmc.Split(q)[0];
                nextline(out psmc, psmc, strLine);
                switch (strLine)
                {
                    case "SOUND":
                        Console.Beep();
                        continue;
                    case "CLEAR":
                        Console.Clear();
                        continue;
                    case "CONCATENATE":
                        arenums.Remove(psmc.Split(q)[2]);
                        arenums.Add(psmc.Split(q)[2], false);
                        vars.Remove(psmc.Split(q)[2]);
                        string string1 = String.Empty;
                        string string2 = String.Empty;
                        vars.TryGetValue(psmc.Split(q)[0], out string1);
                        vars.TryGetValue(psmc.Split(q)[1], out string2);
                        vars.Add(psmc.Split(q)[2], string1 + string2);
                        nextline(out psmc, psmc, strLine);
                        nextline(out psmc, psmc, strLine);
                        nextline(out psmc, psmc, strLine);
                        continue;
                    case "COPY":
                        arenums.Remove(psmc.Split(q)[1]);
                        vars.Remove(psmc.Split(q)[1]);
                        nums.Remove(psmc.Split(q)[1]);
                        bool isfirstnum;
                        arenums.TryGetValue(psmc.Split(q)[0], out isfirstnum);
                        arenums.Add(psmc.Split(q)[1], isfirstnum);
                        if (isfirstnum)
                        {
                            double number;
                            nums.TryGetValue(psmc.Split(q)[0], out number);
                            nums.Add(psmc.Split(q)[1], number);
                        }
                        else
                        {
                            string text;
                            vars.TryGetValue(psmc.Split(q)[0], out text);
                            vars.Add(psmc.Split(q)[1], text);
                        }
                        nextline(out psmc, psmc, strLine);
                        nextline(out psmc, psmc, strLine);
                        continue;
                    case "TOSTRING":
                        arenums.Remove(psmc.Split(q)[1]);
                        arenums.Add(psmc.Split(q)[1], false);
                        vars.Remove(psmc.Split(q)[1]);
                        nums.Remove(psmc.Split(q)[1]);
                        double firstnumber;
                        nums.TryGetValue(psmc.Split(q)[0], out firstnumber);
                        vars.Add(psmc.Split(q)[1], firstnumber.ToString());
                        nextline(out psmc, psmc, strLine);
                        nextline(out psmc, psmc, strLine);
                        continue;
                    case "TONUMBER":
                        arenums.Remove(psmc.Split(q)[1]);
                        arenums.Add(psmc.Split(q)[1], true);
                        vars.Remove(psmc.Split(q)[1]);
                        nums.Remove(psmc.Split(q)[1]);
                        string firststring;
                        vars.TryGetValue(psmc.Split(q)[0], out firststring);
                        try
                        {
                            nums.Add(psmc.Split(q)[1], double.Parse(firststring));
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("I don't think that command will work here.");
                        }
                        catch (OverflowException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Be reasonable!  We can talk this out!");
                        }
                        finally
                        {
                            nextline(out psmc, psmc, strLine);
                            nextline(out psmc, psmc, strLine);
                        }
                        continue;
                    case "DIVIDE":
                        arenums.Remove(psmc.Split(q)[2]);
                        arenums.Add(psmc.Split(q)[2], true);
                        vars.Remove(psmc.Split(q)[2]);
                        nums.Remove(psmc.Split(q)[2]);
                        double left;
                        double right;
                        nums.TryGetValue(psmc.Split(q)[0], out left);
                        nums.TryGetValue(psmc.Split(q)[1], out right);
                        nums.Add(psmc.Split(q)[2], left / right);
                        nextline(out psmc, psmc, strLine);
                        nextline(out psmc, psmc, strLine);
                        nextline(out psmc, psmc, strLine);
                        continue;
                    case "EXPONENT":
                        arenums.Remove(psmc.Split(q)[2]);
                        arenums.Add(psmc.Split(q)[2], true);
                        vars.Remove(psmc.Split(q)[2]);
                        nums.Remove(psmc.Split(q)[2]);
                        nums.TryGetValue(psmc.Split(q)[0], out left);
                        nums.TryGetValue(psmc.Split(q)[1], out right);
                        nums.Add(psmc.Split(q)[2], System.Math.Pow(left, right));
                        nextline(out psmc, psmc, strLine);
                        nextline(out psmc, psmc, strLine);
                        nextline(out psmc, psmc, strLine);
                        continue;
                    case "EXIT":
                        sbyte result;
                        if (sbyte.TryParse(psmc.Split(q)[0], out result)) return result; else return sbyte.MinValue;
                    case "LOG":
                        string variable = String.Empty;
                        StreamWriter logfile = null;
                        try
                        {
                            string dir;
                            if (vars.TryGetValue("__DIR__", out dir))
                            {
                                logfile = new StreamWriter(dir + "logfile.log", true);
                                arenums.TryGetValue(psmc.Split(q)[0], out isnum);
                                if (isnum)
                                {
                                    double number;
                                    nums.TryGetValue(psmc.Split(q)[0], out number);
                                    variable = number.ToString();
                                }
                                else vars.TryGetValue(psmc.Split(q)[0], out variable);
                                logfile.WriteLine(variable);
                                logfile.Flush();
                            }
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
                        try
                        {
                            string dir;
                            if (vars.TryGetValue("__DIR__", out dir)) Directory.CreateDirectory(dir + psmc.Split(q)[0]);
                        }
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
                        variable = String.Empty;
                        arenums.TryGetValue(psmc.Split(q)[0], out isnum);
                        if (isnum)
                        {
                            double number;
                            nums.TryGetValue(psmc.Split(q)[0], out number);
                            variable = number.ToString();
                        }
                        else vars.TryGetValue(psmc.Split(q)[0], out variable);
                        Console.WriteLine(variable);
                        nextline(out psmc, psmc, strLine);
                        continue;
                    case "SUBTRACT":
                        arenums.Remove(psmc.Split(q)[2]);
                        arenums.Add(psmc.Split(q)[2], true);
                        vars.Remove(psmc.Split(q)[2]);
                        nums.Remove(psmc.Split(q)[2]);
                        nums.TryGetValue(psmc.Split(q)[0], out left);
                        nums.TryGetValue(psmc.Split(q)[1], out right);
                        nums.Add(psmc.Split(q)[2], left - right);
                        nextline(out psmc, psmc, strLine);
                        nextline(out psmc, psmc, strLine);
                        nextline(out psmc, psmc, strLine);
                        continue;
                    case "USERVARIABLE":
                        arenums.Remove(psmc.Split(q)[0]);
                        vars.Remove(psmc.Split(q)[0]);
                        nums.Remove(psmc.Split(q)[0]);
                        string input = Console.ReadLine();
                        double inputnum;
                        if (double.TryParse(input, out inputnum))
                        {
                            nums.Add(psmc.Split(q)[0], inputnum);
                            arenums.Add(psmc.Split(q)[0], true);
                        }
                        else
                        {
                            vars.Add(psmc.Split(q)[0], input);
                            arenums.Add(psmc.Split(q)[0], false);
                        }
                        nextline(out psmc, psmc, strLine);
                        continue;
                    case "VARIABLE":
                        arenums.Remove(psmc.Split(q)[0]);
                        vars.Remove(psmc.Split(q)[0]);
                        nums.Remove(psmc.Split(q)[0]);
                        if (double.TryParse(psmc.Split(q)[1], out inputnum))
                        {
                            nums.Add(psmc.Split(q)[0], inputnum);
                            arenums.Add(psmc.Split(q)[0], true);
                        }
                        else
                        {
                            vars.Add(psmc.Split(q)[0], psmc.Split(q)[1]);
                            arenums.Add(psmc.Split(q)[0], false);
                        }
                        nextline(out psmc, psmc, strLine);
                        nextline(out psmc, psmc, strLine);
                        continue;
                    case "START":
                        System.Diagnostics.Process process = null;
                        try
                        {
                            string dir;
                            if (vars.TryGetValue("__DIR__", out dir))
                            {
                                process = new System.Diagnostics.Process();
                                process.StartInfo.FileName = dir + psmc.Split(q)[0];
                                try { if (!process.Start()) Console.WriteLine("Warning!  Could not start \'" + process.StartInfo.FileName + "\'!"); }
                                catch (System.ComponentModel.Win32Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    Console.WriteLine("AHH!  AN ERROR!");
                                }
                            }
                        }
                        catch (InvalidOperationException) { }
                        finally
                        {
                            if (process != null) process.Close();
                            nextline(out psmc, psmc, strLine);
                        }
                        continue;
                }
            }
            return 1;
        }
    }
}
