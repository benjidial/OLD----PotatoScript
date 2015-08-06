namespace NegativeFourPotatoes.PotatoScript

exception ParseError of string * string * char * char * int

module ProcessPSStatement =
  
  let IgnoreAbort (message : string, exitCode : int) =
    raise (ParseError(message, "Ignore this and continue or abort processing? (I/A)", 'i', 'a', exitCode))

  let IncorrectNumberOfArguments (command : string, numberOfArguments : string, builtIn : bool) =
    if builtIn
    then IgnoreAbort (command + " should have " + numberOfArguments + " after it. (Try PotatoScript /S " + command + ")", -17)
    else IgnoreAbort (command + " should have " + numberOfArguments + " after it.", -17)

  let UnrecognizedCommand (command : string, builtIn : bool) =
    if builtIn
    then IgnoreAbort ("The command " + command + " was not recognized! (Try PotatoScript /S)", -18)
    else IgnoreAbort ("The command " + command + " was not recognized!", -18)

  let ProcessPSStatement(statement : string) : unit =
    
    if statement <> "" && statement.[0] = '#'
    then
      match statement.Split([|' '|]).[0] with
      |"#auto" ->
        if statement.Split([|' '|]).Length <> 2
        then IncorrectNumberOfArguments ("#auto", "one argument", true)
        else PSState.autoLocations.Add(statement.Split([|' '|]).[1])
      |"#exit" ->
        if statement.Split([|' '|]).Length > 2
        then IncorrectNumberOfArguments ("#exit", "no or one argument(s)", true)
        else
          PSState.isRunning <- false
          if statement.Split([|' '|]).Length = 2
          then
            try
              PSState.returnValue <- System.Int32.Parse(statement.Split([|' '|]).[1])
            with
            | :? System.FormatException -> PSState.returnValue <- 1
            | :? System.OverflowException -> PSState.returnValue <- 1
      |a ->
        if a <> "#start"
        then UnrecognizedCommand (a, true)