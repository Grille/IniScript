using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Utils;

internal class Lexer
{
    List<Token> _tokenBuffer;

    Rule[] _rules;



    public Lexer(Rule[] rules)
    {
        _tokenBuffer = new List<Token>();
        _rules = rules;
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
        _tokenBuffer.Clear();
        var list = _tokenBuffer;

        Rule? activeRule = null;

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
                foreach (var rule in _rules)
                {
                    if (rule.Begin(location))
                    {
                        activeRule = rule;
                        begin = i;
                        break;
                    }
                }
            }

            if (activeRule == null && !location.IsWhitespace())
            {
                throw new InvalidDataException($"Unexpected character '{text[i]}'");
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



    public record struct StringLocation(string Text, int Index, int Length)
    {
        public bool IsWord() => CharSets.IsWord(GetChar(0));

        public bool IsNumber() => CharSets.IsNumber(GetChar(0));

        public bool IsWordOrNumber() => CharSets.IsWordOrNumber(GetChar(0));

        public bool IsWhitespace() => CharSets.IsWhitespace(GetChar(0));

        public bool IsSymbol()=> CharSets.IsSymbol(GetChar(0));


        public bool Is(char[] set)
        {
            char c = GetChar();
            for (int i = 0; i < set.Length; i++)
            {
                if (set[i] == c)
                {
                    return true;
                }
            }
            return false;
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

    public record Rule(TokenType Type, Predicate<StringLocation> Begin, Predicate<StringLocation> Continue);
}
