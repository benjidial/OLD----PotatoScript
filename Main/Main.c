#include <stdio.h>

int InvalidArgs();
int HelpCommand();
int ProcessFile(char *file);
int ProcessStatement(char *statement);

int main(int argc, char *argv[])
{
    if (argc == 1)
        return InvalidArgs();
    if (argc == 2)
    {
        char *helpSwitch = "/?";
        if (*argv[1] == *helpSwitch)
            return HelpCommand();
        else
            return ProcessFile(argv[1]);
    }
    else
        return InvalidArgs();
}

int InvalidArgs()
{
    printf("Invalid syntax!  Try typing 'PotatoScript /?'.\n");
    return -1;
}

int HelpCommand()
{
    printf("PotatoScript /?\nPotatoScript file\n\n/?    Outputs this help screen.\nfile  This is the file to process.\n");
    return 1;
}

int ProcessFile(char *file)
{
    FILE *ptrFile = fopen(file, "r");
    if (ptrFile == NULL)
    {
        printf("Could not open that file!");
        return -2;
    }
    while (!feof(ptrFile))
    {
        char currentChar = NULL;
        char *statement = "";
        while (currentChar != '\n')
        {
            if (currentChar != NULL)
                statement += currentChar;
            currentChar = fgetc(ptrFile);
        }
        ProcessStatement(statement);
        // TODO: Handle that result
    }
    fclose(ptrFile);
    return 0;
}

int ProcessStatement(char *statement)
{
    // TODO: Process the statement
    return 0;
}