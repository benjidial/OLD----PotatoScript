#ifndef PS_CMDLINE_H
#define PS_CMDLINE_H

#include <stdio.h>
#include "Asm.h"
#include "Common.h"

// PotatoScript Alpha 0.6.0 Alpha CmdLine.h
// The PotatoScript programming language is made by Benji Dial and Warren Galloway.  It is licensed under the MIT license.
// The official PotatoScript GitHub repository is available at <https://github.com/benjidial/PotatoScript>.

int invalidArgs(char* message)
{
  printf("Invalid command line syntax: %s\nTry PotatoScript -help\n", message);
  return 0;
}

int helpCommand()
{
  printf("Usage: PotatoScript -help\n");
  printf("Usage: PotatoScript -asm bits os syntax pscfile asmfile\n");
  printf("\n");
  printf("-help  Prints this screen.\n");
  printf("-asm   Generates an x86 assembly file from a specifed PotatoScript source file.\n");
  printf("  bits     One of: -16bit, -32bit, or -64bit\n");
  printf("    -16bit    Specifies to use 16-bit code in an output assembly file.\n");
  printf("    -32bit    Specifies to use 32-bit code in an output assembly file.\n");
  printf("    -64bit    Specifies to use 64-bit code in an output assembly file.\n");
  printf("  os       One of: -linux, -macosx, or -windows\n");
  printf("    -linux    Specifies to use Linux interrupts in an output assembly file.\n");
  printf("    -macosx   Specifies to use Mac OS X interrupts in an output assembly file.\n");
  printf("    -windows  Specifies to use MS Windows interrupts in an output assembly file.\n");
  printf("  syntax   One of: -gas or -nasm\n");
  printf("    -gas      Specifies to use GAS syntax in an output assembly file.\n");
  printf("    -nasm     Specifies to use NASM syntax in an output assembly file.\n");
  printf("  pscfile  Specifies a PotatoScript source file to convert into assembly.\n");
  printf("  asmfile  Specifies the name of the output assembly file.\n");
  return 0;
}

int cmdLine(int argc, char* args[])
{
  if (argc == 1)
    return invalidArgs("Please specify at least one argument.");
  if (!strcmp(args[1], "-help"))
  {
    if (argc == 2)
      return helpCommand();
    return invalidArgs("If -help is specified, no other arguments should be specified.");
  }
  if (!strcmp(args[1], "-asm"))
  {
    int bitsArg;
    int osArg;
    int syntaxArg;
    if (argc == 2)
      return invalidArgs("If -asm is specified, -16bit, -32bit, or -64bit should be specified immediately afterward.");
    if (!strcmp(args[2], "-16bit"))
      bitsArg = SIXTEENBIT;
    else if (!strcmp(args[2], "-32bit"))
      bitsArg = THIRTYTWOBIT;
    else if (!strcmp(args[2], "-64bit"))
      bitsArg = SIXTYFOURBIT;
    else
      return invalidArgs("If -asm is specified, -16bit, -32bit, or -64bit should be specified immediately afterward.");
    if (argc == 3)
      return invalidArgs("If -asm bits is specified, -linux, -macosx, or -windows should be specified immediately afterward.");
    if (!strcmp(args[3], "-linux"))
      osArg = LINUX;
    else if (!strcmp(args[3], "-macosx"))
      osArg = MACOSX;
    else if (!strcmp(args[3], "-windows"))
      osArg = WINDOWS;
    else
      return invalidArgs("If -asm bits is specified, -linux, -macosx, or -windows should be specified immediately afterward.");
    if (argc == 4)
      return invalidArgs("If -asm bits os is specified, -gas or -nasm should be specified immediately afterward.");
    if (!strcmp(args[4], "-gas"))
      syntaxArg = GAS;
    else if (!strcmp(args[4], "-nasm"))
      syntaxArg = NASM;
    else
      return invalidArgs("If -asm bits os is specified, -gas or -nasm should be specified immediately afterward.");
    if (argc == 5)
      return invalidArgs("If -asm bits os syntax is specified, a psc source file should be specified immediately afterward.");
    if (argc == 6)
      return invalidArgs("If -asm bits os syntax pscfile is specified, a filename for the asm output should be specified immediately afterward.");
    if (argc == 7)
      return convertToAsm(bitsArg, osArg, syntaxArg, args[5], args[6]);
    return invalidArgs("If -asm bits os syntax pscfile asmfile is specified, no other arguments should be specified.");
  }
  return invalidArgs("The first argument should be -help or -asm.");
}

#endif
