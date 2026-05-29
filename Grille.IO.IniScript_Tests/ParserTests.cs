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
        Test("SectionArgs", SectionArgs);

        Section("Parser Arguments");
        TestArgument<double>("0.123", 0.123);
        TestArgument<long>("3124576", 3124576);
        TestArgument<long>("0xFF", 0xFF);
        TestArgument<string>("\"str\"", "str");
        TestArgument<Identifier>("Key", "Key");
        TestArgument<Parameter[]>("[0,1]", [new Parameter(0L), new Parameter(1L)], 5, ToString);
    }

    static void Lines()
    {
        var script = Parse("\n\n\n  K\n");

        var section = script.CurrentSection;
        Assert.IsEqual(AssemblyCreateInfo.DefaultKey, section.Key);

        Assert.IsEqual(1, section.Array.Length, "Sections.Count");

        var entry = section.Array[0];
        Assert.IsEqual("K", entry.Tokens[0], "Key");
        Assert.IsEqual(3, entry.Location.Line, "Line");
        Assert.IsEqual(2, entry.Location.Indentation, "Indentation");
    }

    static void Sections()
    {
        var script = Parse("[A]\n[B]:A\n");
        Assert.IsEqual(3, script.Count);
        Assert.IsTrue(script.ContainsKey(AssemblyCreateInfo.DefaultKey));
        Assert.IsTrue(script.ContainsKey("A"));
        Assert.IsTrue(script.ContainsKey("B"));
        Assert.IsEqual("A", script["B"].ParentKey);
    }

    static void SectionArgs()
    {
        var script = Parse("[A](arg0, arg1)");
        Assert.IsEqual(2, script.Count);
        Assert.IsTrue(script.ContainsKey("A"));

        var section = script["A"];
        Assert.IsTrue(section.Parameters != null);

        var args = section.Parameters!;
        Assert.IsEqual(2, args.Length);
        Assert.IsEqual("arg0", args[0].Value.ToString());
        Assert.IsEqual("arg1", args[1].Value.ToString());
    }

    private static string ToString(Parameter[] arguments)
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
            Assert.IsTrue(ParameterParser.Skip(ref reader) == ParameterParser.SkipResult.Parsed, "Skip");
            Assert.IsEqual(position, reader.Position, "Position after Skip");
            reader.Position = 0;
            var arg = ParameterParser.Parse(ref reader);
            Assert.IsEqual(position, reader.Position, "Position after Parse");
            Assert.IsEqual(typeof(T), arg.Value.GetType());
            if (converter == null) Assert.IsEqual(expected, arg.Value);
            else Assert.IsEqual(converter(expected), converter((T)arg.Value));
        });
    }

    static AssemblyCreateInfo Parse(string text)
    {
        var parser = new Parser();
        return parser.Parse(text);
    }
    
   
}
