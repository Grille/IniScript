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
        (var compiler, var sb) = GetCompiler();
        var script = compiler.Compile("A;\nP(cc)");

        script.Invoke();

        var text = sb.ToString();

        Assert.IsEqual("Acc", text);
    }

    static (Compiler, StringBuilder) GetCompiler()
    {
        var sb = new StringBuilder();

        var commands = new Commands();
        commands.Register("A", (r) => sb.Append("A"));
        commands.Register("P", (r, a) => sb.Append(a.Text));
        var compiler = new Compiler(commands);
        return (compiler, sb);
    }
}
