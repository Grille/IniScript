﻿using Grille.ConsoleTestLib;
using Grille.IO.IniScript_Tests;

namespace Grille.IO.CfgScript_Tests;

internal class Program
{
    static void Main(string[] args)
    {
        RunAsync = true;

        StringTests.Run();
        LexerTests.Run();
        ParserTests.Run();

        RunTests();
    }
}
