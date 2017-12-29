#ifndef PS_ASM_H
#define PS_ASM_H

#include <stdio.h>
#include <string.h>
#include "Common.h"

// PotatoScript Alpha 0.6.0 Alpha Asm.h
// The PotatoScript programming language is made by Benji Dial and Warren Galloway.  It is licensed under the Apache license.
// The official PotatoScript GitHub repository is available at <https://github.com/benjidial/PotatoScript>.

int convertToAsm(int bits, int os, int syntax, char *pscFilename, char *asmFilename) {
  FILE *pscFilePtr = fopen(pscFilename, "r");
  if (!pscFilePtr) {
    return anError("Could not find the psc source file specified.  (Is it in another folder, or not yet saved?)", PSCFNF);
  }
  FILE *asmFilePtr = fopen(asmFilename, "w");
  if (!asmFilePtr) {
     return anError("Could not access the asm ouput file specified.  (Is it on an unmapped drive?  If it exists, is it readonly, or open in another program?)", ASMFNF);
  }
  fprintf(asmFilePtr, "; Auto-generated from %s by PotatoScript Alpha 0.6.0 Alpha.\n", pscFilename);
  fprintf(asmFilePtr, "; Designed for ");
  switch (bits) {
   case SIXTEENBIT:
    fprintf(asmFilePtr, "16-bit ");
    break;
   case THIRTYTWOBIT:
    fprintf(asmFilePtr, "32-bit ");
    break;
   case SIXTYFOURBIT:
    fprintf(asmFilePtr, "64-bit ");
  }
  switch (os) {
   case LINUX:
    fprintf(asmFilePtr, "Linux with ");
    break;
   case MACOSX:
    fprintf(asmFilePtr, "Mac OS X with ");
    break;
   case WINDOWS:
    fprintf(asmFilePtr, "MS Windows with ");
  }
  switch (syntax) {
   case GAS:
    fprintf(asmFilePtr, "GAS syntax.\n");
    break;
   case NASM:
    fprintf(asmFilePtr, "NASM syntax.\n");
  }
  fprintf(asmFilePtr, "; The PotatoScript programming language is made by Benji Dial and Warren Galloway.  It is licensed under the MIT license.\n");
  fprintf(asmFilePtr, "; The official PotatoScript GitHub repository is available at <https://github.com/benjidial/PotatoScript>.\n");
  fprintf(asmFilePtr, "\n\n");
  switch (syntax) {
   case GAS:
    fprintf(asmFilePtr, ".global __MAIN\n");
    fprintf(asmFilePtr, ".text\n");
    fprintf(asmFilePtr, "__MAIN:\n");
    fprintf(asmFilePtr, "    jmp __START\n");
    fprintf(asmFilePtr, "\n");
    break;
   case NASM:
    fprintf(asmFilePtr, "global __MAIN\n");
    fprintf(asmFilePtr, "section .text\n");
    fprintf(asmFilePtr, "__MAIN:\n");
    fprintf(asmFilePtr, "    jmp __START\n");
    fprintf(asmFilePtr, "\n");
  }
  
}

#endif
