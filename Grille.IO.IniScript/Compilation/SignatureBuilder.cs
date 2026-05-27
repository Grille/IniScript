using Grille.IO.IniScript.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Compilation;

using static Signature;

public sealed class SignatureBuilder
{
    private readonly List<TokenMatcher> _tokens = new();

    private readonly Stack<char> _bracketStack = new();

    public int ParameterIndex { get; set; } = 0;

    public bool RequireSeperator { get; private set; } = false;

    public int Count => _tokens.Count;

    private void Add(MatchType type, string? text = null, int argIndex = -1)
    {
        _tokens.Add(new TokenMatcher(type, text, argIndex));
    }

    private void AddRequiredSeperator()
    {
        if (RequireSeperator)
        {
            Add(MatchType.Symbol, ",");
            RequireSeperator = false;
        }
    }

    public SignatureBuilder Keyword(string key)
    {
        AddRequiredSeperator();
        Add(MatchType.Keyword, key);
        RequireSeperator = true;
        return this;
    }

    public SignatureBuilder Parameter(int index)
    {
        ParameterIndex = index;
        return Parameter();
    }

    public SignatureBuilder Parameter()
    {
        AddRequiredSeperator();
        Add(MatchType.Parameter, null, ParameterIndex++);
        RequireSeperator = true;
        return this;
    }

    private static int CharSetIndex(char c, string set)
    {
        int index = set.IndexOf(c);
        if (index == -1) throw new ArgumentException($"Char {c} not in '{set}'");
        return index;
    }

    public SignatureBuilder Symbol(char symbol)
    {
        CharSetIndex(symbol, CharSets.Symbols);
        Add(MatchType.Symbol, symbol.ToString());
        RequireSeperator = false;
        return this;
    }

    public SignatureBuilder OpeningBracket(char bracket)
    {
        int index = CharSetIndex(bracket, CharSets.OpeningBrackets);
        _bracketStack.Push(CharSets.ClosingBrackets[index]);
        Add(MatchType.Bracket, bracket.ToString());
        RequireSeperator = false;
        return this;
    }

    public SignatureBuilder ClosingBracket()
    {
        var bracket = _bracketStack.Pop();
        Add(MatchType.Bracket, bracket.ToString());
        RequireSeperator = false;
        return this;
    }

    public Signature CreateSignature()
    {
        while (_bracketStack.Count > 0)
        {
            ClosingBracket();
        }
        return new Signature(_tokens.ToArray());
    }

    public static implicit operator Signature(SignatureBuilder builder) => builder.CreateSignature();
}
