using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grille.ConsoleTestLib.Asserts;
using Grille.IO.IniScript.Compilation.Internal;
using Grille.IO.IniScript.Tokenization;

namespace Grille.IO.CfgScript_Tests;

static class ParserTests
{
    public static void Run()
    {
        Section("Parser");

        Test("Lines", Lines);
        Test("Sections", Sections);

        Section("Parser Arguments");
        TestArgument<double>("0.123", 0.123);
        TestArgument<long>("3124576", 3124576);
        TestArgument<long>("0xFF", 0xFF);
        TestArgument<string>("\"str\"", "str");
        TestArgument<Identifier>("Key", "Key");
        TestArgument<Argument[]>("[0,1]", [new Argument(0L), new Argument(1L)], 5, ToString);
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
        var script = Parse("[A]\n[B]:A\n");
        Assert.IsEqual(script.Count, 3);
        Assert.IsTrue(script.ContainsKey(ScriptCreationObject.DefaultSectionName));
        Assert.IsTrue(script.ContainsKey("A"));
        Assert.IsTrue(script.ContainsKey("B"));
        Assert.IsEqual("A", script["B"].ParentKey);
    }

    private static string ToString(Argument[] arguments)
    {
        var sb = new StringBuilder();
        sb.Append("{");
        for (int i = 0; i < arguments.Length; i++)
        {
            if (i > 0) sb.Append(",");
            var arg = arguments[i];
            arg.ToString(sb);
        }
        sb.Append("}");
        return sb.ToString();
    }

    public static void TestArgument<T>(string text, T expected, int position = 1, Converter<T, string>? converter = null) where T : notnull
    {
        Test(text, () =>
        {
            var tokens = ParserLexer.Lexer.Tokenize(text)[0];
            var reader = new TokenReader(tokens);
            Assert.IsTrue(Argument.Skip(ref reader), "Skip");
            Assert.IsEqual(position, reader.Position, "Position after Skip");
            reader.Position = 0;
            var arg = Argument.Parse(ref reader);
            Assert.IsEqual(position, reader.Position, "Position after Parse");
            Assert.IsEqual(typeof(T), arg.Value.GetType());
            if (converter == null) Assert.IsEqual(expected, arg.Value);
            else Assert.IsEqual(converter(expected), converter((T)arg.Value));
        });
    }

    static ScriptCreationObject Parse(string text)
    {
        var parser = new Parser();
        return parser.Parse(text);
    }
    
   
}
