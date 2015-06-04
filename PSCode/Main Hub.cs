using System;
using System.Collections.Generic;
using System.IO;
using NegativeFourPotatoes;
using NegativeFourPotatoes.PS;

namespace NegativeFourPotatoes.PS
{
    /// <summary>
    /// This has the <see cref="NegativeFourPotatoes.PS.Misc.ExitState"/> enum.
    /// </summary>
    public static class Misc
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
        /// <returns>This returns a value in the form of <see cref="NegativeFourPotatoes.PS.Misc.ExitState"/>
        /// representing the state at the end of the processing of the PSMC or a value representing
        /// an error occuring when converting the PSC to PSMC.</returns>
        public static Misc.ExitState GetReady(string filename, string[ ] args)
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
            do
            {
                string strLine = psc.ReadLine().Trim();
                if      (strLine.StartsWith(  "COMMENT", true, null)) continue;
                if      (strLine.ToUpper() == "BEEP")                       strFullCode +=  "BEEP\n";
                else if (strLine.ToUpper() == "CLEARSCREEN")                strFullCode +=  "CLEAR\n";
                else if (strLine.StartsWith(  "DIR ", true, null))          strFullCode += ("FOLDER\n" + strLine.Substring(4, strLine.Length - 4) + "\n");
                else if (strLine.StartsWith(  "EXIT ", true, null))         strFullCode += ("EXIT\n" + strLine.Substring(5, strLine.Length - 5) + "\n");
                else if (strLine.StartsWith(  "LOG ", true, null))          strFullCode += ("LOG\n" + strLine.Substring(4, strLine.Length - 4) + "\n");
                else if (strLine.StartsWith(  "MAKEFOLDER ", true, null))   strFullCode += ("CREATEFOLDER\n" + strLine.Substring(11, strLine.Length - 11) + "\n");
                else if (strLine.StartsWith(  "OUTPUT ", true, null))       strFullCode += ("CONSOLE\n" + strLine.Substring(7, strLine.Length - 7) + "\n");
                else if (strLine.StartsWith(  "STARTPROCESS ", true, null)) strFullCode += ("START\n" + strLine.Substring(13, strLine.Length - 13) + "\n");
                else Console.Write("Err: " + strLine + " ");
            } while (!psc.EndOfStream);
            Console.WriteLine("Success!");
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

        /// <summary>
        /// This runs PSMC.
        /// </summary>
        /// <param name="psmc">This is the PSMC to be run.</param>
        /// <returns>This returns a value representing the end state of the program.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        private static sbyte RunCode(string psmc)
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
                    case "LOG":
                        StreamWriter logfile;
                        try
                        {
                            logfile = new StreamWriter(Environment.GetEnvironmentVariable("PS_DIR") + "logfile.log", true);
                            logfile.WriteLine(psmc.Split(q)[0]);
                            logfile.Flush();
                            logfile.Close();
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
                        psmc = psmc.Substring(psmc.Split(q)[0].Length + 1);
                        continue;
                    case "CREATEFOLER":
                        try { Directory.CreateDirectory(Environment.GetEnvironmentVariable("PS_DIR") + psmc.Split(q)[0]); }
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
                    case "START":
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        process.StartInfo.FileName = Environment.GetEnvironmentVariable("PS_DIR") + psmc.Split(q)[0];
                        if (!process.Start()) Console.WriteLine("Warning!  Could not start \'" + process.StartInfo.FileName + "\'!");
                        process.Close();
                        continue;
                }
            }
            return 1;
        }
    }
}