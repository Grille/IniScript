using System;
using System.Collections.Generic;
using System.IO;

namespace Grille.IO.IniScript.Utils;

internal class Lexer
{
    readonly LexerRule[] _rules;

    public Lexer(LexerRule[] rules)
    {
        _rules = rules;
    }

    public TokenList Tokenize(Stream stream, bool leaveOpen = false)
    {
        using var reader = new StreamReader(stream, leaveOpen: leaveOpen);
        return Tokenize(reader);
    }

    public TokenList Tokenize(TextReader reader)
    {
        var text = reader.ReadToEnd();
        return Tokenize(text);
    }

    public TokenList Tokenize(string text) 
    {
        var tokens = new List<Token>();
        var lines = new List<Range>();
        int textIndex = 0;
        int lineIndex = 0;

        while (true) 
        {
            lines.Add(TokenizeLine(tokens, text, ref lineIndex, ref textIndex));
            if (tokens[^1].Type == TokenType.EndOfFile) break;
        }

        return new TokenList(tokens.ToArray(), lines.ToArray());
    }

    private struct LineCounter
    {
        private readonly string _text;
        private int _tokenLength;

        public int Row;
        public int Column;

        public TokenLocation Location => new(Row, Column);

        public LineCounter(string text, int row)
        {
            _text = text;
            Row = row ;
            Column = 0;
        }

        public void Update(int index)
        {
            var ctx = new LexerRuleContext(_text, index, _tokenLength);
            if (ctx.IsEndOfLine())
            {
                _tokenLength += 1;
            }
            else if (_tokenLength > 0)
            {
                Row += 1;
                Column = -1;
                _tokenLength = 0;
            }
            Column += 1;
        }
    }

    private LexerRule? BeginNew(LexerRuleContext ctx)
    {
        foreach (var rule in _rules)
        {
            if (rule.Begin(ctx))
            {
                return rule;
            }
        }
        return null;
    }

    private Range TokenizeLine(List<Token> tokens, string text, ref int lineIndex, ref int textIndex)
    {
        var lineCounter = new LineCounter(text, lineIndex);
        var location = TokenLocation.Empty;

        LexerRule? activeRule = null;

        int lineStart = tokens.Count;

        int index = textIndex;
        int begin = textIndex;
        bool isEndOfFile = false;

        while (true)
        {
            isEndOfFile = index == text.Length;
            lineCounter.Update(index);

            if (activeRule != null)
            {
                int length = index - begin;
                var ctx = new LexerRuleContext(text, index, length);
                if (!activeRule.Continue(ctx) || isEndOfFile)
                {
                    var token = new Token(text, begin, length, activeRule.Type, location);
                    tokens.Add(token);
                    activeRule = null;
                    if (token == TokenType.EndOfLine) break;
                }
            }

            if (isEndOfFile)
            {
                tokens.Add(new Token(TokenType.EndOfFile, lineCounter.Location));
                break;
            }
            if (activeRule == null)
            {
                location = lineCounter.Location;
                var ctx = new LexerRuleContext(text, index, 0);
                if ((activeRule = BeginNew(ctx)) != null) begin = index;
                else throw new UnexpectedTokenException(text[index], location);
            }

            index += 1;
        }

        textIndex = index;
        lineIndex = lineCounter.Row;

        int lineEnd = tokens.Count;
        return new(lineStart, lineEnd);
    }
}
