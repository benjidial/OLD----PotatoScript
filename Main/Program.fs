namespace NegativeFourPotatoes.PotatoScript

exception Error of string * int * bool

module ProcessCommandLine =

  let CommandLineSyntax () =
    printfn "Command Line Syntax:\n"
    printfn "PotatoScript /?"
    printfn "PotatoScript /S"
    printfn "PotatoScript /I"
    printfn "PotatoScript [/Y | /N] filename [arg1 [arg2 ...]]\n"
    printfn "/?        This will output this screen."
    printfn "/S        This will output the syntax of .pcs files."
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
    printfn "Invalid command line syntax!"
    CommandLineSyntax()
    raise  (Error("Invalid command line syntax!", -2, false))

  let PotatoScriptSyntax () = 
    printfn "Syntax of PotatoScript:\n"
    printfn "TODO"

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
        |"/S" -> PotatoScriptSyntax(); 2   // PotatoScript /S
        |"/I" -> PotatoScriptInteractive() // PotatoScript /I
        |"/Y" -> InvalidCommandLine()      // PotatoScript /Y
        |"/N" -> InvalidCommandLine()      // PotatoScript /N
        |a -> ProcessFile(a, [||], false, false) // PotatoScript _
      elif argv.Length = 2
      then
        match argv.[0] with
        |"/?" -> InvalidCommandLine() // PotatoScript /? _
        |"/S" -> InvalidCommandLine() // PotatoScript /S _
        |"/I" -> InvalidCommandLine() // PotatoScript /I _
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
        |a -> ProcessFile(a, (Array.sub argv 1 (argv.Length - 1)), false, false)  // PotatoScript *
    with
    |Error(errorReason, exitCode, tellUser) ->
      if tellUser
      then printfn "%s\nCode:%x" errorReason exitCode
      exitCode
    |ex -> printfn "An unexpected error occured:\n%s" ex.Message; -1