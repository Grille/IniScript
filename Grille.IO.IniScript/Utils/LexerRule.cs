using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Utils;

internal readonly ref struct LexerRuleContext
{
    public readonly ReadOnlySpan<char> Text;
    public readonly int Index;
    public readonly int TokenLength;

    public LexerRuleContext(ReadOnlySpan<char> text, int index, int tokenLength = 0)
    {
        Text = text;
        Index = index;
        TokenLength = tokenLength;
    }

    public int TokenStart => Index - TokenLength;

    public ReadOnlySpan<char> TokenText => Text.Slice(TokenStart, TokenLength);

    public bool Is(ReadOnlySpan<char> set, int offset = 0) => set.Contains(GetChar(offset));

    public bool Is(char c, int offset = 0) => GetChar(offset) == c;

    public bool IsEndOfLine()
    {                          
        if (TokenLength == 0) 
        {
            return Is("\r\n");
        }
        else if (TokenLength == 1)
        {
            char char0 = GetTokenChar(0);
            char char1 = GetTokenChar(1);
            return (char0 == '\r' && char1 == '\n') || (char0 == '\n' && char1 == '\r');
        }
        return false;
    }

    private char GetAbsChar(int index)
    {
        return index >= 0 && index < Text.Length ? Text[index] : '\0';
    }

    public char GetChar(int offset = 0) => GetAbsChar(Index + offset);

    public char GetTokenChar(int offset = 0) => GetAbsChar(TokenStart + offset);

    public char this[int offset] => GetChar(offset);
}

internal class LexerRule
{
    public delegate bool Predicate(LexerRuleContext ctx);

    public TokenType Type { get; }

    public Predicate Begin { get; }

    public Predicate Continue { get; }

    public LexerRule(TokenType type, Predicate beginRule, Predicate continueRule)
    {
        Type = type;
        Begin = beginRule;
        Continue = continueRule;
    }

    public LexerRule(TokenType type, Predicate beginRule) : this(type, beginRule, beginRule) { }

    public static readonly LexerRule EndOfLine = new LexerRule(TokenType.EndOfLine, (ctx) => ctx.IsEndOfLine());
}