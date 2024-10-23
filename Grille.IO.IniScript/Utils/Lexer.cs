using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Grille.IO.IniScript.Utils.Lexer;

namespace Grille.IO.IniScript.Utils;

internal class Lexer
{
    readonly List<Token[]> _lineBuffer;
    readonly List<Token> _tokenBuffer;
    readonly Rule[] _rules;

    public Lexer(Rule[] rules)
    {
        _tokenBuffer = new List<Token>();
        _lineBuffer = new List<Token[]>();
        _rules = rules;
    }

    public Token[][] Tokenize(string text)
    {
        using var reader = new StringReader(text);
        return Tokenize(reader);
    }

    public Token[][] Tokenize(TextReader reader)
    {
        _lineBuffer.Clear();

        int row = 0;

        while (true)
        {
            var line = reader.ReadLine();
            if (line == null) break;

            var tokens = TokenizeLine(line, row);
            _lineBuffer.Add(tokens);

            row += 1;
        }

        return _lineBuffer.ToArray();
    }

    public Token[] TokenizeLine(string text, int row)
    {
        _tokenBuffer.Clear();

        Rule? activeRule = null;

        int begin = 0;

        for (int i = 0; i <= text.Length; i++)
        {
            bool eol = i == text.Length;

            int length = i - begin;
            var location = new StringLocation(text, i, length);

            if (activeRule != null)
            {
                if (!activeRule.Continue(location) || eol)
                {
                    var subtext = text.Substring(begin, length);
                    var token = new Token() { Type = activeRule.Type, Value = subtext, Row = row, Column = begin };
                    _tokenBuffer.Add(token);
                    activeRule = null;
                }
            }

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

            if (!eol && activeRule == null)
            {
                throw new InvalidDataException($"Unexpected character '{text[i]}'");
            }
        }

        return _tokenBuffer.ToArray();
    }



    public record struct StringLocation(string Text, int Index, int Length)
    {
        public bool IsLetter() => CharSets.IsLetter(GetChar(0));

        public bool IsDigit() => CharSets.IsDigit(GetChar(0));

        public bool IsWord() => CharSets.IsWord(GetChar(0));

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

        public bool IsIStringBegin()
        {
            return GetChar(0) == '$' && GetChar(1) == '"';
        }

        public bool IsIStringEnd()
        {
            return GetChar(-2) != '\\' && GetChar(-1) == '"' && Length > 2;
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

    public record Rule(TokenType Type, Predicate<StringLocation> Begin, Predicate<StringLocation> Continue)
    {
        public Rule(TokenType Type, Predicate<StringLocation> Begin) : this(Type, Begin, Begin) { }
    }
}
