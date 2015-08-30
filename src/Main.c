// PotatoScript Alpha 0.6.0 Alpha
// The PotatoScript programming language is made by Benji Dial and Warren Galloway.  It is licensed under the MIT license.
// The official PotatoScript GitHub repository is available at <https://github.com/benjidial/PotatoScript>.

#include <stdio.h>

enum asmSyntaxType { INTEL, ATT };

enum errorCode { NOTSUPPORTED = -1 };

int invalidArgs(char* message)
{
  printf("Invalid command line syntax: %s\nTry PotatoScript -help\n", message);
  return 0;
}

int helpCommand()
{
  printf("Usage: PotatoScript {-help|-asm {-intel|-att} file}\n");
  printf("-help  Prints this screen.\n");
  printf("-asm   Generates an x86 assembly file from a specifed PotatoScript source file.\n");
  printf("  -intel  Specifies to use Intel syntax in an output assembly file.\n");
  printf("  -att    Specifies to use AT&T syntax in an output assembly file.\n");
  printf("  file    Specifies a PotatoScript source file to convert into assembly.\n");
  return 0;
}

int comingSoon(char* message)
{
  return anError(message, NOTSUPPORTED);
}

int convertToAsm(int syntaxType, char* filename)
{
  return comingSoon("Converting to assembly is not yet supported.");
}

int anError(char* message, int exitCode)
{
  printf("An error has occured: %s\nError code: %x\n", message, exitCode);
  return exitCode;
}

int main(int argc, char* args[])
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
    if (argc == 2)
    {
      return invalidArgs("If -asm is specifed, -intel or -att should be specified immediately afterward.");
    }
    if (!strcmp(args[2], "-intel"))
    {
      if (argc == 3)
        return invalidArgs("If -asm -intel is specifed, a filename should be specified immediately afterward.");
      if (argc == 4)
        return convertToAsm(INTEL, args[3]);
      return invalidArgs("If -asm -intel file is specified, no other arguments should be specified.");
    }
    if (!strcmp(args[2], "-att"))
    {
      if (argc == 3)
        return invalidArgs("If -asm -att is specifed, a filename should be specified immediately afterward.");
      if (argc == 4)
        return convertToAsm(ATT, args[3]);
      return invalidArgs("If -asm -att file is specified, no other arguments should be specified.");
    }
    return invalidArgs("If -asm is specifed, -intel or -att should be specified immediately afterward.");
  }
  return invalidArgs("The first argument should be -help or -asm.");
}
