namespace NegativeFourPotatoes.PotatoScript

module PSState =
  let autoLocations = new System.Collections.ObjectModel.Collection<string>()
  let mutable autoError = false
  let mutable errorResponse = false
  let mutable isRunning = true
  let mutable returnValue = 0