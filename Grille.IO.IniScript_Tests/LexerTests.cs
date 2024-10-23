using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grille.ConsoleTestLib.Asserts;
using Grille.IO.IniScript.Utils;

namespace Grille.IO.CfgScript_Tests;

using static TokenType;

public static class LexerTests
{
    public static void Run()
    {
        Token Token(TokenType type, string value) => new Token() { Type = type, Value = value };

        var equals = Token(Symbol, "=");
        var open = Token(Symbol, "(");
        var close = Token(Symbol, ")");

        var section = Token(Section, "[Key]");
        var mnemonic = Token(Word, "Key");
        var arg0 = Token(Word, "Arg0");

        var x = Token(Word, "X");
        var y = Token(Word, "Y");

        var s = Token(Whitespace, " ");
        var c = Token(Whitespace, ",");
        var cs = Token(Whitespace, ", ");
        var comment = Token(Comment, ";Comment");

        Section("Lexer");

        TestTokens("[Key]", [section]);
        TestTokens("[Key] Arg0", [section, s, arg0]);

        TestTokens(" ", [s]);
        TestTokens("X", [x]);
        TestTokens("X ", [x, s]);
        TestTokens(" X", [s, x]);
        TestTokens(" X ", [s, x, s]);
        TestTokens(" X Y ", [s, x, s, y, s]);

        TestTokens("Key", [mnemonic]);
        TestTokens("Key()", [mnemonic, open, close]);
        TestTokens("Key() ;Comment", [mnemonic, open, close, s, comment]);
        TestTokens("Key(aa, bb)", [mnemonic, open, Token(Word, "aa"), cs, Token(Word, "bb"), close]);
        TestTokens("Key(\"aa, bb\")", [mnemonic, open, Token(String, "\"aa, bb\""), close]);
        TestTokens("Key(\"aa\\\"bb\")", [mnemonic, open, Token(String, "\"aa\\\"bb\""), close]);
        TestTokens("Key(\"aa{bb}\")", [mnemonic, open, Token(String, "\"aa{bb}\""), close]);
        TestTokens("Key($\"aa{bb}\")", [mnemonic, open, Token(InterpolatedString, "$\"aa{bb}\""), close]);
        TestTokens("Key($\"aa\")", [mnemonic, open, Token(InterpolatedString, "$\"aa\""), close]);

        TestTokens("Key = Arg0", [mnemonic, s, equals, s, arg0]);
        TestTokens("Key=$\"Text{Var}\"", [mnemonic, equals, Token(InterpolatedString,"$\"Text{Var}\"")]);
    }

    static void TestTokens(string text, Token[] tokens)
    {
        Test(text, () =>
        {
            if (text == "Key()")
            {

            }

            var parser = new Parser();
            var lexer = parser._lexer;

            var result = lexer.Tokenize(text)[0];

            Assert.IsEqual(tokens.Length, result.Length);

            for (int i = 0; i < tokens.Length; i++)
            {
                Assert.IsEqual(tokens[i].Type, result[i].Type);
                Assert.IsEqual(tokens[i].Value, result[i].Value);
            }

            var types = new List<TokenType>();
            foreach (var token in result)
            {
                types.Add(token.Type);
            }
            Succes(string.Join(", ", types));
        });
    }
}
