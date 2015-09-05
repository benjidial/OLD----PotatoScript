#ifndef PS_COMMON_H
#define PS_COMMON_H

#include <errno.h>
#include <stdio.h>
extern int errno;

// PotatoScript Alpha 0.6.0 Alpha Common.h
// The PotatoScript programming language is made by Benji Dial and Warren Galloway.  It is licensed under the MIT license.
// The official PotatoScript GitHub repository is available at <https://github.com/benjidial/PotatoScript>.

enum asmBits { SIXTEENBIT, THIRTYTWOBIT, SIXTYFOURBIT };
enum os { LINUX, MACOSX, WINDOWS };
enum asmSyntax { GAS, NASM };

enum errorCode { NOTSUPPORTED = -1, PSCFNF = -2, ASMFNF = -3 };

int comingSoon(char* message)
{
  return anError(message, NOTSUPPORTED);
}

int anError(char* message, int exitCode)
{
  perror(message);
  fprintf(stderr, "C Error code: 0x%X  PS Error code: 0x%X\n", errno, exitCode);
  return exitCode;
}

#endif
