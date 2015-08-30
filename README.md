# PotatoScript
This is the PotatoScript programming language.  It is developed by [Benji Dial](https://github.com/benjidial) and [Warren Galloway](https://github.com/nightofthecastle).  
Please read the license file.  It is also available online at <http://opensource.org/licenses/MIT>.  
Feel free to create issues and pull requests to the repository as long as they are relevant.

## Project members:
* Management:
  * [Benji Dial](https://github.com/benjidial)
  * [Warren Galloway](https://github.com/nightofthecastle)
* Programming:
  * Lead Programming:
    * [Benji Dial](https://github.com/benjidial)
  * The community
* Design:
  * Lead Design:
    * [Benji Dial](https://github.com/benjidial)
    * [Warren Galloway](https://github.com/nightofthecastle)
  * The community
* Documentation:
  * [Benji Dial](https://github.com/benjidial)

## Version History
### Alpha 0.4.0 to Alpha 0.5.0
#### Alpha 0.5.0
**Numbers and examples!**  
Renamed `NegativeFourPotatoes` namespace to `NFP`.  
Added number variables.  
Added `ADD`, `SUBTRACT`, `MULTIPLY`, `DIVIDE`, `EXPONENT`, `TONUMBER`, and `TOSTRING` commands.  
Added `COPY` command.  
Added error handling for `STARTPROCESS` command.  
Got rid of `/F:` in front of filename on command-line.  Do `PS /?` for more info.  
Modified the way the code deals with errors during `GetReady`.  
Added `Adder.psc` and `Printer.psc` examples.  
No longer crashes upon starting with no arguments.
#### Alpha 0.4.1
Changed some internal workings.  
Made some code easier to keep up with.
#### Alpha 0.4.0
**First Alpha, variables!**  
Reorganized program, including moving Dll into exe.  
Terminal's title will be changed to PotatoScript whilst code is running.  
Added `CONCAT`, `LOGVAR`, `OUTPUTVAR`, `READ`, and `SETVAR` commands.  
Slightly modified the visual style of `PS /S`.  
The program will now prompt a user on whether he or she wants to attempt to execute code with syntax errors in it.
### Pre-Alpha 0.1.0 to Pre-Alpha 0.3.1
#### Pre-Alpha 0.3.1
Fixed a *large* issue which prefixes the `filename` argument with 'F:'.
#### Pre-Alpha 0.3.0
Added `LOG` command.  
Added `OUTPUT` command.  
Fixed bug with `EXIT` that made it unusable.  
Added error handling for `MAKEFOLDER`.  
Renamed `PSWindows` to `PSWrapper`.  
Renamed `LanguageIno.txt` to `Syntax.txt`.  
Changed old PSW icon to new PS icon.  
#### Pre-Alpha 0.2.2
Fixed a **major** issue which makes the program think it has no arguments.
#### Pre-Alpha 0.2.1
Fixed some bugs.
#### Pre-Alpha 0.2.0
Renamed PSW to PS, and `PSW /?` to `PS /?`.  
Added the ability to process .psc files with `PS /F:<filename>`.  
Added `PS /S` for the syntax of PSC.  
Added `BEEP`, `CLEARSCREEN`, `COMMENT`, `DIR`, `EXIT`, `MAKEFOLDER`, and `STARTPROCESS` commands to PSC.  
#### Pre-Alpha 0.1.0
All it has is `PSW /?`, but it's a start.
