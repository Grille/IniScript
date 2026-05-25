using Grille.ConsoleTestLib;
using Grille.IO.IniScript_Tests;

using System;

namespace Grille.IO.CfgScript_Tests;

internal class Program
{
    static void Main(string[] args)
    {
        RunAsync = false;
        //ExecuteImmediately = true;

        SerializerTests.Run();
        LexerTests.Run();
        ParserTests.Run();
        //CompilerTests.Run();

        RunTests();
    }
}
