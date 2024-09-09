using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grille.ConsoleTestLib.Asserts;

namespace Grille.IO.CfgScript_Tests;

using static TokenType;

public static class LexerTests
{
    public static void Run()
    {
        Token T(TokenType type, string value) => new Token() { Type = type, Value = value };

        Section("Lexer");

        TestTokens("[Key]", [T(Section, "[Key]")]);
        TestTokens("[Key] Args", [T(Section, "[Key]"), T(Value, "Args")]);

        TestTokens("X", [T(Value, "X")]);
        TestTokens("X ", [T(Value, "X")]);
        TestTokens(" X", [T(Value, "X")]);
        TestTokens(" X ", [T(Value, "X")]);
        TestTokens(" X Y ", [T(Value, "X"), T(Value, "Y")]);
        
        TestTokens("Key", [T(Value, "Key")]);
        TestTokens("Key()", [T(Value, "Key")]);
        TestTokens("Key() ;C", [T(Value, "Key"), T(Comment, ";C")]);
        TestTokens("Key(aa, bb)", [T(Value, "Key"), T(Value, "aa"), T(Value, "bb")]);
        TestTokens("Key(\"aa, bb\")", [T(Value, "Key"), T(String, "\"aa, bb\"")]);
        TestTokens("Key(\"aa\\\"bb\")", [T(Value, "Key"), T(String, "\"aa\\\"bb\"")]);
    }

    static void TestTokens(string text, Token[] tokens)
    {
        Test(text, () =>
        {
            var lexer = new Lexer();

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
