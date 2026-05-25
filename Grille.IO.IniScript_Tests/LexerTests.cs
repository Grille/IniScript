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
        Token Token(TokenType type, string value) => new Token(value, type);

        var equals = Token(Symbol, "=");
        var open = Token(Bracket, "(");
        var close = Token(Bracket, ")");
        var secopen = Token(Bracket, "[");
        var secclose = Token(Bracket, "]");

        var key = Token(Word, "Key");
        var arg0 = Token(Word, "Arg0");

        var x = Token(Word, "X");
        var y = Token(Word, "Y");

        var s = Token(Whitespace, " ");
        var c = Token(Symbol, ",");
        var comment0 = Token(Comment, ";Comment");
        var comment1 = Token(Comment, "#Comment");
        var comment2 = Token(Comment, "//Comment");
        var comment3 = Token(Comment, "/*Comment*/");

        Section("Lexer Single");

        TestToken("Key", Word);
        TestToken("_0K", Word);
        TestToken("0x0", Number);
        TestToken("0XF", Number);
        TestToken("1E-1", Number);
        TestToken("1E+1", Number);
        TestToken("0.0_0", Number);
        TestToken(".0", Number);
        TestToken("\n", EndOfLine);
        TestToken("\r", EndOfLine);
        TestToken("\n\r", EndOfLine);
        TestToken("\r\n", EndOfLine);
        TestToken("\"str\"", LiteralString);
        TestToken("\"str", LiteralString);
        TestToken("\"s\n", LiteralString);
        TestToken("$\"str\"", InterpolatedString);
        TestToken("$\"str{}str\"", InterpolatedString);
        TestToken("$\"str{\"str\"}str\"", InterpolatedString);

        Section("Lexer Groups");

        TestTokens("[Key]", [secopen, key, secclose]);
        TestTokens("[Key] Arg0", [secopen, key, secclose, s, arg0]);

        TestTokens(" ", [s]);
        TestTokens("X", [x]);
        TestTokens("X ", [x, s]);
        TestTokens(" X", [s, x]);
        TestTokens(" X ", [s, x, s]);
        TestTokens(" X Y ", [s, x, s, y, s]);

        TestTokens("Key", [key]);
        TestTokens("Key()", [key, open, close]);
        TestTokens("Key;Comment", [key, comment0]);
        TestTokens("Key#Comment", [key, comment1]);
        TestTokens("Key//Comment", [key, comment2]);
        TestTokens("Key/*Comment*/Key", [key, comment3, key]);
        TestTokens("Key(aa, bb)", [key, open, Token(Word, "aa"), c, s, Token(Word, "bb"), close]);
        TestTokens("Key(\"aa, bb\")", [key, open, Token(LiteralString, "\"aa, bb\""), close]);
        TestTokens("Key(\"aa\\\"bb\")", [key, open, Token(LiteralString, "\"aa\\\"bb\""), close]);
        TestTokens("Key(\"aa{bb}\")", [key, open, Token(LiteralString, "\"aa{bb}\""), close]);
        TestTokens("Key($\"aa{bb}\")", [key, open, Token(InterpolatedString, "$\"aa{bb}\""), close]);
        TestTokens("Key($\"aa\")", [key, open, Token(InterpolatedString, "$\"aa\""), close]);

        TestTokens("Key = Arg0", [key, s, equals, s, arg0]);
        TestTokens("Key=$\"Text{Var}\"", [key, equals, Token(InterpolatedString,"$\"Text{Var}\"")]);
    }

    static string FormatTitle(string text)
    {
        var sb = new StringBuilder();
        StringSerializer.SerializeEscape(text, sb);
        return sb.ToString();
    }

    static string TokensToDebugString(ReadOnlySpan<Token> tokens)
    {
        var types = new TokenType[tokens.Length];
        for (int i = 0; i < types.Length; i++) types[i] = tokens[i].Type;
        return string.Join(", ", types);
    }

    static TokenList Tokenize(string text, out string debug)
    {
        var result = ParserLexer.Lexer.Tokenize(text);
        debug = TokensToDebugString(result);
        return result;
    }

    static void TestToken(string text, TokenType type) => Test(FormatTitle(text), () => _TestToken(text, type));

    static void _TestToken(string text, TokenType type)
    {
        var result = Tokenize(text, out var message);
        Assert.IsEqual(2, result.TokenCount, message);
        Assert.IsEqual(type, result[0].Type);
        if (result[1].Type != EndOfFile) Warn(message);
        Succes(result[0].Type.ToString());
    }

    static void TestTokens(string text, Token[] tokens) => Test(FormatTitle(text), () => _TestTokens(text, tokens));

    static void _TestTokens(string text, Token[] tokens)
    {
        var result = Tokenize(text, out var message);
        Assert.IsEqual(tokens.Length + 1, result.TokenCount, message);
        for (int i = 0; i < tokens.Length; i++)
        {
            Assert.IsEqual(tokens[i].Type, result[i].Type);
            Assert.IsEqual((string)tokens[i], (string)result[i]);
        }
        Assert.IsEqual(result[^1].Type, EndOfFile);
        Succes(message);
    }
}
