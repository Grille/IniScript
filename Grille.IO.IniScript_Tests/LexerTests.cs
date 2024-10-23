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
        Token T(TokenType type, string value) => new Token() { Type = type, Value = value };

        Section("Lexer");

        TestTokens("[Key]", [T(Section, "[Key]")]);
        TestTokens("[Key] Args", [T(Section, "[Key]"), T(Word, "Args")]);

        TestTokens("X", [T(Word, "X")]);
        TestTokens("X ", [T(Word, "X")]);
        TestTokens(" X", [T(Word, "X")]);
        TestTokens(" X ", [T(Word, "X")]);
        TestTokens(" X Y ", [T(Word, "X"), T(Word, "Y")]);
        
        TestTokens("Key", [T(Word, "Key")]);
        TestTokens("Key()", [T(Word, "Key")]);
        TestTokens("Key() ;C", [T(Word, "Key"), T(Comment, ";C")]);
        TestTokens("Key(aa, bb)", [T(Word, "Key"), T(Word, "aa"), T(Word, "bb")]);
        TestTokens("Key(\"aa, bb\")", [T(Word, "Key"), T(String, "\"aa, bb\"")]);
        TestTokens("Key(\"aa\\\"bb\")", [T(Word, "Key"), T(String, "\"aa\\\"bb\"")]);
    }

    static void TestTokens(string text, Token[] tokens)
    {
        Test(text, () =>
        {
            var parser = new Parser();
            var lexer = parser._lexer;

            var result = lexer.Tokenize(text)[0];

            Assert.IsEqual(tokens.Length, result.Length);

            for (int i = 0; i < tokens.Length; i++)
            {
                Assert.IsEqual(tokens[i].Type, result[i].Type);
                Assert.IsEqual(tokens[i].Value, result[i].Value);
            }
        });
    }
}
