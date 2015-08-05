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

  let ProcessFile(str : string, parameters : string[]) =
    printfn "TODO"
    0

  [<EntryPoint>]
  let main argv = 
    let mutable errorAuto = false
    let mutable errorResponse = false
    try
      if argv.Length = 0
      then InvalidCommandLine()
      elif argv.Length = 1
      then
        match argv.[0] with
        |"/?" -> CommandLineSyntax(); 1
        |"/S" -> PotatoScriptSyntax(); 2
        |"/I" -> PotatoScriptInteractive()
        |"/Y" -> InvalidCommandLine()
        |"/N" -> InvalidCommandLine()
        |a -> ProcessFile(a, null)
      else printfn "TODO"; 0
    with
    |Error(errorReason, exitCode, tellUser) ->
      if tellUser
      then printfn "%s\nCode:%x" errorReason exitCode
      exitCode
    |ex -> printfn "An unexpected error occured:\n%s" ex.Message; -1