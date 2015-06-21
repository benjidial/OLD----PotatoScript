# PotatoScript
This is the PotatoScript programming language.  It is developed by Benji Dial and Warren Galloway.  
Please read the license file.  It is also available online at <http://opensource.org/licenses/MIT>.  
Feel free to create issues and pull requests to the repository as long as they are relevant.

## Version History
### Alpha 0.4.1
Changed some internal workings.  
Made some code easier to keep up with.
### Alpha 0.4.0
**First Alpha, variables!**  
Reorganized program, including moving Dll into exe.  
Terminal's title will be changed to PotatoScript whilst code is running.  
Added `CONCAT`, `LOGVAR`, `OUTPUTVAR`, `READ`, and `SETVAR` commands.  
Slightly modified the visual style of `PS /S`.  
The program will now prompt a user on whether he or she wants to attempt to execute code with syntax errors in it.
### Pre-Alpha 0.3.1
Fixed a *large* issue which prefixes the `filename` argument with 'F:'.
### Pre-Alpha 0.3.0
Added `LOG` command.  
Added `OUTPUT` command.  
Fixed bug with `EXIT` that made it unusable.  
Added error handling for `MAKEFOLDER`.  
Renamed `PSWindows` to `PSWrapper`.  
Renamed `LanguageIno.txt` to `Syntax.txt`.  
Changed old PSW icon to new PS icon.  
### Pre-Alpha 0.2.2
Fixed a **major** issue which makes the program think it has no arguments.
### Pre-Alpha 0.2.1
Fixed some bugs.
### Pre-Alpha 0.2.0
Renamed PSW to PS, and `PSW /?` to `PS /?`.  
Added the ability to process .psc files with `PS /F:<filename>`.  
Added `PS /S` for the syntax of PSC.  
Added `BEEP`, `CLEARSCREEN`, `COMMENT`, `DIR`, `EXIT`, `MAKEFOLDER`, and `STARTPROCESS` commands to PSC.  
### Pre-Alpha 0.1.0
All it has is `PSW /?`, but it's a start.
