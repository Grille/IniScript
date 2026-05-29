using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grille.ConsoleTestLib.Asserts;

namespace Grille.IO.IniScript_Tests;

public static class CompilerTests
{
    
    public static void Run()
    {
        Section("Compiler");

        Test("0", Test0);
    }

    static void Test0()
    {
        var compiler = GetCompiler();
        var asm = compiler.Compile("A");
        var runtime = new Runtime(asm, compiler);
    }

    static void A(Runtime runtime) => runtime.ValueStack.Push("A");

    static Compiler GetCompiler()
    {
        var commands = new CommandRegistry();
        commands.Register(A);
        return new Compiler(commands);
    }
    
}
