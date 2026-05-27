using System;
using System.Collections.Generic;
using System.IO;

namespace Grille.IO.IniScript.Utils;

internal class Lexer
{
    private struct Context
    {
        private readonly string Text;
        private readonly LexerRule[] Rules;
        private readonly RangedArrayBuilder<Token> Tokens;
         
        private int Index;
        private int EOLStartIndex = -1;
        private int Row;
        private int Column;

        private Context(LexerRule[] rules, string text)
        {
            Rules = rules;
            Text = text;
            Tokens = new();
            Column = -1;
        }

        private bool IsEndOfFile => Index == Text.Length;

        private bool IsEndOfLine(int length) => new LexerRuleContext(Text, Index, length).IsEndOfLine();

        private TokenLocation Location => new(Row, Column);

        private void UpdateLocation()
        {
            if (EOLStartIndex != -1 && !IsEndOfLine(Index - EOLStartIndex)) //continue
            {
                EOLStartIndex = -1;
                Row += 1;
                Column = -1;
            }
            if (EOLStartIndex == -1 && IsEndOfLine(0)) //begin
            {
                EOLStartIndex = Index;
            }
            Column += 1;
        }

        private LexerRule GetNextRule()
        {
            var ctx = new LexerRuleContext(Text, Index);
            foreach (var rule in Rules)
            {
                if (rule.Begin(ctx))
                {
                    return rule;
                }
            }
            throw new UnexpectedTokenException(Text[Index], Location);
        }

        private void TokenizeLines()
        {
            LexerRule? activeRule = null;
            int beginIndex = Index;
            var beginLocation = TokenLocation.Empty;

            while (true)
            {
                UpdateLocation();

                if (activeRule != null)
                {
                    int length = Index - beginIndex;
                    var ctx = new LexerRuleContext(Text, Index, length);
                    if (!activeRule.Continue(ctx) || IsEndOfFile)
                    {
                        var token = new Token(Text, beginIndex, length, activeRule.Type, beginLocation);
                        Tokens.Add(token);
                        activeRule = null;
                        if (token == TokenType.EndOfLine)
                        {
                            Tokens.YieldRange();
                        }
                    }
                }

                if (IsEndOfFile) break;

                if (activeRule == null)
                {
                    beginIndex = Index;
                    beginLocation = Location;
                    activeRule = GetNextRule();
                }

                Index += 1;
            }
        }

        private RangedArray<Token> Tokenize()
        {
            TokenizeLines();
            Tokens.Add(new Token(TokenType.EndOfFile, Location));
            Tokens.YieldRange();
            return Tokens.ToArray();
        }

        public static RangedArray<Token> Tokenize(LexerRule[] rules, string text)
        {
            return new Context(rules, text).Tokenize();
        }
    }

    private readonly LexerRule[] _rules;

    public Lexer(LexerRule[] rules) => _rules = rules;

    public RangedArray<Token> Tokenize(Stream stream, bool leaveOpen = false)
    {
        using var reader = new StreamReader(stream, leaveOpen: leaveOpen);
        return Tokenize(reader);
    }

    public RangedArray<Token> Tokenize(TextReader reader)
    {
        var text = reader.ReadToEnd();
        return Tokenize(text);
    }

    public RangedArray<Token> Tokenize(string text) 
    {
        return Context.Tokenize(_rules, text);
    }
}
