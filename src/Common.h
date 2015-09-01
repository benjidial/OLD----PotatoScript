// PotatoScript Alpha 0.6.0 Alpha Common.h
// The PotatoScript programming language is made by Benji Dial and Warren Galloway.  It is licensed under the MIT license.
// The official PotatoScript GitHub repository is available at <https://github.com/benjidial/PotatoScript>.

#ifndef PS_COMMON_H
#define PS_COMMON_H

#include <stdio.h>

enum asmBits { SIXTEENBIT, THIRTYTWOBIT, SIXTYFOURBIT };
enum os { LINUX, MACOSX, WINDOWS };
enum asmSyntax { GAS, NASM };

enum errorCode { NOTSUPPORTED = -1, PSCFNF = -2 };

int comingSoon(char* message)
{
  return anError(message, NOTSUPPORTED);
}

int anError(char* message, int exitCode)
{
  printf("An error has occured: %s\nError code: 0x%X\n", message, exitCode);
  return exitCode;
}

#endif
