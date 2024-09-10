using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grille.ConsoleTestLib.Asserts;

namespace Grille.IO.CfgScript_Tests;

static class ParserTests
{
    public static void Run()
    {
        Section("Parser");

        Test("0", Test0);
        Test("1", Test1);
        Test("2", Test2);
    }

    static void Test0()
    {
        var script = Parse("\n\n\n  K\n");

        var section = script.ActiveSection;
        Assert.IsEqual("Default", section.Name);

        Assert.IsEqual(4, section.Count);

        var entry = section[3];
        Assert.IsEqual("K", entry.Key);
        Assert.IsEqual(2, entry.Indentation);
    }

    static void Test1()
    {
        var script = Parse("[S]\nKey A0, 0x42");

        var section = script.ActiveSection;
        Assert.IsEqual("S", section.Name);

        Assert.IsEqual(1, section.Count);

        var entry = section[0];

        Assert.IsEqual("Key", entry.Key);
        Assert.IsEqual("A0", entry.Args[0].Value);
        Assert.IsEqual(0x42, entry.Args[1].Hex32);
    }

    static void Test2()
    {
        var script = Parse("#TabSize 4\n[S]\n\nKey A0, \"A\\t1\"\n  ;text\n  JMP 0x56\nX");

        var sb = new StringBuilder();
        var tw = new StringWriter(sb);

        var s = new IniScriptSerializer();
        s.Serialize(tw, script);

        var text = sb.ToString();

        Succes(text);
    }


    static IniScript Parse(string text)
    {
        var parser = new IniScriptParser();
        return parser.Parse(text);
    }
}
