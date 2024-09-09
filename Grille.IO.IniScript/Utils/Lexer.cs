using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.Utils;

internal class Lexer
{
    List<Token> tokenBuffer;

    LexerRule[] rules;

    public Lexer()
    {
        tokenBuffer = new List<Token>();

        rules = [
            new LexerRule(TokenType.String, (a) => a.IsStringBegin(), (a) => !a.IsStringEnd()),
            new LexerRule(TokenType.Section, (a) => a.GetChar() == '[', (a) => a.GetChar(-1) != ']'),
            new LexerRule(TokenType.Comment, (a) => a.BeginComment(), (a) => true),
            new LexerRule(TokenType.Value, (a) => !a.IsWhitespace(), (a) => !a.IsWhitespace()),
        ];
    }

    public Token[][] Tokenize(string text)
    {
        using var reader = new StringReader(text);
        return Tokenize(reader);
    }

    public Token[][] Tokenize(TextReader reader)
    {
        var lines = new List<Token[]>();

        int row = 0;

        while (true)
        {
            var line = reader.ReadLine();
            if (line == null) break;

            var tokens = TokenizeLine(line, row);
            lines.Add(tokens);

            row += 1;
        }

        return lines.ToArray();
    }

    public Token[] TokenizeLine(string text, int row)
    {
        tokenBuffer.Clear();
        var list = tokenBuffer;

        LexerRule? activeRule = null;

        int begin = 0;

        void YieldToken(int length)
        {
            var subtext = text.Substring(begin, length);
            var token = new Token() { Type = activeRule.Type, Value = subtext, Row = row, Column = begin };
            list.Add(token);
            activeRule = null;
        }

        for (int i = 0; i < text.Length; i++)
        {
            var location = new StringLocation(text, i, 0);

            if (activeRule == null)
            {
                foreach (var rule in rules)
                {
                    if (rule.Begin(location))
                    {
                        activeRule = rule;
                        begin = i;
                        break;
                    }
                }
            }

            int length = i - begin;
            location = new StringLocation(text, i, length);

            if (activeRule != null)
            {
                if (!activeRule.Continue(location))
                {
                    YieldToken(length);
                }
            }
        }

        if (activeRule != null)
        {
            YieldToken(text.Length - begin);
        }

        return list.ToArray();
    }



    record struct StringLocation(string Text, int Index, int Length)
    {
        public bool IsWhitespace()
        {
            char c = GetChar();
            return c == ' ' || c == '\t' || c == '(' || c == ')' || c == ',' || c == ';' || c == ':' || c == '=';
        }

        public bool IsStringBegin()
        {
            return GetChar() == '"';
        }

        public bool IsStringEnd()
        {
            return GetChar(-2) != '\\' && GetChar(-1) == '"' && Length > 1;
        }

        public bool BeginComment()
        {
            var char0 = GetChar(0);
            var char1 = GetChar(1);

            return char0 == ';' || char0 == '#' || char0 == '/' && char1 == '/';
        }

        public char GetChar(int offset = 0)
        {
            int index = Index + offset;
            return index >= 0 && index < Text.Length ? Text[index] : '\0';
        }
    }

    record LexerRule(TokenType Type, Predicate<StringLocation> Begin, Predicate<StringLocation> Continue);
}
