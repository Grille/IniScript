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

    public string? AutoSeperator { get; set; } = ",";

    public int ParameterIndex { get; set; } = 0;

    public bool RequireSeperator { get; private set; } = false;

    public int Count => _tokens.Count;

    private TokenMatcher LastItem => _tokens.Count > 0 ? _tokens[^1] : TokenMatcher.Empty;

    internal void Add(TokenMatcher matcher)
    {
        _tokens.Add(matcher);
    }

    private void Add(MatchType type, string text) => Add(new(type, text));

    private void Add(MatchType type, int parameterIndex) => Add(new(type, parameterIndex));

    private void AddRequiredSeperator()
    {
        if (!RequireSeperator) return;
        if (AutoSeperator != null) Symbol(AutoSeperator);
        else RequireSeperator = false;
    }

    public SignatureBuilder Keyword(string key)
    {
        Add(MatchType.Keyword, key);
        return this;
    }

    public SignatureBuilder Parameter()
    {
        AddRequiredSeperator();
        Add(MatchType.Parameter, ParameterIndex++);
        RequireSeperator = true;
        return this;
    }

    public SignatureBuilder ParameterList()
    {
        AddRequiredSeperator();
        Add(MatchType.ParameterList, ParameterIndex++);
        RequireSeperator = true;
        return this;
    }

    private void AssertIsNotCompoundSymbol(char c1)
    {
        if (LastItem.Type != MatchType.Symbol || LastItem.Text!.Length != 1) return;
        char c0 = LastItem.Text[0];
        if (CharSets.IsCompoundSymbol(c0, c1)) throw new ArgumentException($"'{c0}{c1}' form CompoundSymbol");
    }

    private static int CharSetIndex(char c, string set)
    {
        int index = set.IndexOf(c);
        if (index == -1) throw new ArgumentException($"Char {c} not in '{set}'");
        return index;
    }

    public SignatureBuilder Symbol(string symbol)
    {
        if (symbol.Length == 1)
        {
            if (LastItem.Type == MatchType.ParameterList && symbol == ",")
            {
                throw new ArgumentException("Symbol ',' not valid after ParameterList");
            }
            CharSetIndex(symbol[0], CharSets.Symbols);
            AssertIsNotCompoundSymbol(symbol[0]);
        }
        else if (symbol.Length == 2)
        {
            if (!CharSets.IsCompoundSymbol(symbol[0], symbol[1]))
            {
                throw new ArgumentException($"String {symbol} not in '{CharSets.CompoundSymbols}'");
            }
        }
        else
        {
            throw new ArgumentException("String.Length must be 1 or 2");
        }
        Add(MatchType.Symbol, symbol);
        RequireSeperator = false;
        return this;
    }

    public SignatureBuilder Symbol(char symbol) => Symbol(symbol.ToString());

    public SignatureBuilder OpeningBracket(string bracket)
    {
        if (bracket.Length != 1) throw new ArgumentException("String.Length must be 1");
        int index = CharSetIndex(bracket[0], CharSets.OpeningBrackets);
        AssertIsNotCompoundSymbol(bracket[0]);
        _bracketStack.Push(CharSets.ClosingBrackets[index]);
        Add(MatchType.Symbol, bracket);
        RequireSeperator = false;
        return this;
    }

    public SignatureBuilder OpenBracket(char bracket) => OpeningBracket(bracket.ToString());

    public SignatureBuilder CloseBracket()
    {
        var bracket = _bracketStack.Pop();
        AssertIsNotCompoundSymbol(bracket);
        Add(MatchType.Symbol, bracket.ToString());
        RequireSeperator = false;
        return this;
    }

    public Signature CreateSignature()
    {
        while (_bracketStack.Count > 0)
        {
            CloseBracket();
        }
        return new Signature(_tokens.ToArray());
    }

    public static implicit operator Signature(SignatureBuilder builder) => builder.CreateSignature();
}
