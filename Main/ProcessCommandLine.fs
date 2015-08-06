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
    raise (CommandLineError("Invalid command line syntax!", -2))

  let PSSyntax (command : string) = 
    match command with
    |"" ->
      printfn "List of PotatoScript commands:"
      printfn "Top-level:\n#auto, #start, #exit"
      printfn "To see information about a specific command, type"
      printfn "  PotatoScript /S command"
    |"#auto" ->
      printfn "#auto command:"
      printfn "Location: Top-level"
      printfn "Result:   none"
      printfn "This tells the PotatoScript processor to automatically look"
      printfn "for commands in this location as though they were actually"
      printfn "top-level commands."
      printfn "Usage:    #auto location"
      printfn "Example:  #auto BillyBob.MyLibrary"
    |"exit" ->
      printfn "#exit command:"
      printfn "Location: Top-level"
      printfn "Result:   none"
      printfn "This will stop the processing of the file and return the"
      printfn "specified integer to the OS.  If no integer is specified, 0"
      printfn "will be returned.  If the value specified cannot be"
      printfn "converted to an integer or is outside the range of a 32-bit"
      printfn "signed integer, 1 will be returned."
      printfn "Usage:    #exit integer"
      printfn "Example:  #exit 0"
    |"start" ->
      printfn "#start command:"
      printfn "Location: Top-level"
      printfn "Result:   none"
      printfn "Put this in a file to specify where the processing starts."
      printfn "If more than one of these are found, it will start at the"
      printfn "first one found.  If none are found, it will start at the"
      printfn "beginning of the file."
      printfn "Usage:  #start"
    |a -> raise (CommandLineError("No help available for " + a + "!", -3))

  [<EntryPoint>]
  let ProcessCommandLine argv = 
    try
      if argv.Length = 0
      then InvalidCommandLine()
      elif argv.Length = 1
      then
        match argv.[0] with
        |"/?" -> CommandLineSyntax(); 1
        |"/S" -> PSSyntax(""); 2
        |"/I" -> PSInteractive.PSInteractive()
        |"/Y" -> InvalidCommandLine()
        |"/N" -> InvalidCommandLine()
        |a -> ProcessFile.ProcessFile(a, [||])
      elif argv.Length = 2
      then
        match argv.[0] with
        |"/?" -> InvalidCommandLine()
        |"/S" -> PSSyntax(argv.[1]); 2
        |"/I" -> InvalidCommandLine()
        |"/Y" ->
          PSState.autoError <- true
          PSState.errorResponse <- true
          ProcessFile.ProcessFile(argv.[1], [||])
        |"/N" ->
          PSState.autoError <- true
          ProcessFile.ProcessFile(argv.[1], [||])
        |a -> ProcessFile.ProcessFile(a, [|argv.[1]|])
      else
        match argv.[0] with
        |"/?" -> InvalidCommandLine()
        |"/S" -> InvalidCommandLine()
        |"/I" -> InvalidCommandLine()
        |"/Y" ->
          PSState.autoError <- true
          PSState.errorResponse <- true
          ProcessFile.ProcessFile(argv.[1], (Array.sub argv 2 (argv.Length - 2)))
        |"/N" ->
          PSState.autoError <- true
          ProcessFile.ProcessFile(argv.[1], (Array.sub argv 2 (argv.Length - 2)))
        |a -> ProcessFile.ProcessFile(a, (Array.sub argv 1 (argv.Length - 1)))
    with
    |CommandLineError(errorReason, exitCode) ->
      printfn "%s\nCode: 0x%x" errorReason exitCode
      exitCode
    |ex -> printfn "An unexpected error occured:\n%s" ex.Message; -1