namespace NegativeFourPotatoes.PotatoScript

exception CommandLineError of string * int

module ProcessCommandLine =

  let CommandLineSyntax () =
    printfn "Command Line Syntax:\n"
    printfn "PotatoScript /?"
    printfn "PotatoScript /S [command]"
    printfn "PotatoScript /I"
    printfn "PotatoScript [/Y | /N] filename [arg1 [arg2 ...]]\n"
    printfn "/?        This will output this screen."
    printfn "/S        This will output the syntax of .pcs files."
    printfn "command   This is the command the syntax of which to"
    printfn "            output.  If it is not specified, a list of"
    printfn "            commands will be output."
    printfn "/I        This starts PotatoScript's interactive feature,"
    printfn "            which allows direct typing of PotatoScript"
    printfn "            commands.  (Useful for debugging)"
    printfn "/Y        This automatically says yes or continue to any"
    printfn "            warnings that may arrise. (Not recommended)"
    printfn "/N        This automatically says no or cancel to any"
    printfn "            warnings that may arrise."
    printfn "filename  The name and optionally directory of the file to"
    printfn "            run.  If no directory is supplied, it will use"
    printfn "            the directory in which PotatoScript resides. If"
    printfn "            it cannot find the file it will try again with"
    printfn "            filename.psc before failing."
    printfn "arg1 ...  These are arguments to supply to the file."

  let InvalidCommandLine () =
    raise  (CommandLineError("Invalid command line syntax!", -2))

  let PotatoScriptSyntax (command : string) = 
    match command with
    |"" ->
      printfn "List of PotatoScript commands:"
      printfn "start, exit"
      printfn "To see information about a specific command, type"
      printfn "  PotatoScript /S command"
    |"start" ->
      printfn "start command:"
      printfn "Put this in a file to specify where the processing starts."
      printfn "If more than one of these are found, it will start at the"
      printfn "first one found.  If none are found, it will start at the"
      printfn "beginning of the file."
      printfn "Usage:  start"
    |"exit" ->
      printfn "exit command:"
      printfn "This will stop the processing of the file and return the"
      printfn "specified integer to the OS.  If no integer is specified, 0"
      printfn "will be returned."
      printfn "Usage:    exit [expression]"
      printfn "Example:  exit 0"
    |a -> raise (CommandLineError("No help available for " + a + "!", -3))

  let PotatoScriptInteractive () : int =
    printfn "TODO"
    0

  let ProcessFile(str : string, parameters : string[], autoError : bool, errorResponse : bool) =
    printfn "TODO"
    0

  [<EntryPoint>]
  let main argv = 
    try
      if argv.Length = 0
      then InvalidCommandLine() // PotatoScript
      elif argv.Length = 1
      then
        match argv.[0] with
        |"/?" -> CommandLineSyntax(); 1    // PotatoScript /?
        |"/S" -> PotatoScriptSyntax(""); 2 // PotatoScript /S
        |"/I" -> PotatoScriptInteractive() // PotatoScript /I
        |"/Y" -> InvalidCommandLine()      // PotatoScript /Y
        |"/N" -> InvalidCommandLine()      // PotatoScript /N
        |a -> ProcessFile(a, [||], false, false) // PotatoScript _
      elif argv.Length = 2
      then
        match argv.[0] with
        |"/?" -> InvalidCommandLine()            // PotatoScript /? _
        |"/S" -> PotatoScriptSyntax(argv.[1]); 2 // PotatoScript /S _
        |"/I" -> InvalidCommandLine()            // PotatoScript /I _
        |"/Y" -> ProcessFile(argv.[1], [||], true, true)  // PotatoScript /Y _
        |"/N" -> ProcessFile(argv.[1], [||], true, false) // PotatoScript /N _
        |a -> ProcessFile(a, [|argv.[1]|], false, false)  // PotatoScript _ _
      else
        match argv.[0] with
        |"/?" -> InvalidCommandLine() // PotatoScript /? *
        |"/S" -> InvalidCommandLine() // PotatoScript /S *
        |"/I" -> InvalidCommandLine() // PotatoScript /I *
        |"/Y" -> ProcessFile(argv.[1], (Array.sub argv 2 (argv.Length - 2)), true, true)  // PotatoScript /Y *
        |"/N" -> ProcessFile(argv.[1], (Array.sub argv 2 (argv.Length - 2)), true, false) // PotatoScript /N *
        |a -> ProcessFile(a, (Array.sub argv 1 (argv.Length - 1)), false, false)          // PotatoScript *
    with
    |CommandLineError(errorReason, exitCode) ->
      printfn "%s\nCode: 0x%x" errorReason exitCode
      exitCode
    |ex -> printfn "An unexpected error occured:\n%s" ex.Message; -1