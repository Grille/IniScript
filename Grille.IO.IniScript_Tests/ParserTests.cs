using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grille.ConsoleTestLib.Asserts;
using Grille.IO.IniScript;

namespace Grille.IO.CfgScript_Tests;

static class ParserTests
{
    public static void Run()
    {
        Section("Parser");

        Test("Lines", Lines);
        Test("Sections", Sections);
        /*
        Test("2", Test2);
        Test("Ini", TestIni);
        */
    }

    static void Lines()
    {
        var script = Parse("\n\n\n  K\n");

        var section = script.CurrentSection;
        Assert.IsEqual(ScriptCreationObject.DefaultSectionName, section.Key);

        Assert.IsEqual(1, section.Count, "Sections.Count");

        var entry = section[0];
        Assert.IsEqual("K", entry.Tokens[0], "Key");
        Assert.IsEqual(3, entry.Location.Line, "Line");
        Assert.IsEqual(2, entry.Location.Indentation, "Indentation");
    }

    static void Sections()
    {
        var script = Parse("[A]\n[B]\n");
        Assert.IsEqual(script.Count, 3);
        Assert.IsTrue(script.ContainsKey(ScriptCreationObject.DefaultSectionName));
    }

    /*
    static void Test1()
    {
        var script = Parse("[S]\nKey A0, 0x42");

        var section = script.ActiveSection;
        Assert.IsEqual("S", section.Name);

        Assert.IsEqual(1, section.Count);

        var entry = section[0];

        Assert.IsEqual("Key", entry.Key);
        Assert.IsEqual("A0", entry.Args[0].Text);
        Assert.IsEqual("0x42", entry.Args[1].Hex32);
    }

    static void Test2()
    {
        var script = Parse("#TabSize 4\n[S]\n\nKey A0, \"A\\t1\"\n  ;text\n  JMP 0x56\nX");

        var sb = new StringBuilder();
        var tw = new StringWriter(sb);

        var s = new Serializer();
        s.Serialize(tw, script);

        //var text = sb.ToString();

        Succes();
    }

    static void TestIni()
    {
        var script = Parse("[S]\nKey = A0, 0x42");

        var section = script.ActiveSection;
        Assert.IsEqual("S", section.Name);

        Assert.IsEqual(1, section.Count);

        var entry = section[0];

        Assert.IsEqual("Set", entry.Key);
        Assert.IsEqual("Key", entry.Args[0].Text);
        Assert.IsEqual("A0", entry.Args[1].Text);
    }
    */

    static ScriptCreationObject Parse(string text)
    {
        var parser = new Parser();
        return parser.Parse(text);
    }
    
   
}
