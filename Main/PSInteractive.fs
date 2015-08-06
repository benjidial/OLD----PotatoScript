namespace NegativeFourPotatoes.PotatoScript

module PSInteractive =

  let PSInteractive () : int =
    printfn "Welcome to the PotatoScript interactive prompt!"
    while PSState.isRunning do
      try
        ProcessPSStatement.ProcessPSStatement(System.Console.ReadLine())
      with
      |ParseError(message, prompt, continueKey, cancelKey, errorCode) ->
        printfn "%s" message
        if PSState.autoError
        then PSState.isRunning <- PSState.errorResponse
        else
          let mutable valid = false
          while not valid do
            System.Console.Write(prompt);
            let key = System.Console.ReadKey().KeyChar
            if key = continueKey
            then valid <- true
            elif key = cancelKey
            then
              valid <- true
              PSState.isRunning <- false
              PSState.returnValue <- errorCode
            else printfn "Invalid choice!"
    PSState.returnValue