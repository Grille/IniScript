using Grille.IO.IniScript.Tokenization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Compilation;

public sealed class Signature 
{
    internal enum MatchType
    {
        Keyword,
        Parameter,
        Symbol,
        Bracket,
    }

    internal class TokenMatcher
    {
        public readonly MatchType MatchType;
        public readonly string? Text;
        public readonly int? ParameterIndex;

        internal TokenMatcher(MatchType types, string ? text = null, int paraIdx = -1)
        {
            MatchType = types;
            Text = text;
            if (paraIdx >= 0)
            {
                ParameterIndex = paraIdx;
            }
        }

        private static bool MatchTypeCollision(MatchType type0, MatchType type1)
        {
            int GetCode(MatchType type) => type switch
            {
                MatchType.Keyword => 0,
                MatchType.Parameter => 0,
                MatchType.Symbol => 1,
                MatchType.Bracket => 2,
                _ => 0,
            };
            return GetCode(type0) == GetCode(type1);
        }

        private static bool TextCollision(string? text0, string? text1)
        {
            if (text0 == null || text1 == null) return true;
            return text0 == text1;
        }

        public bool Overlaps(TokenMatcher match)
        {
            return MatchTypeCollision(MatchType, match.MatchType) && TextCollision(Text, match.Text);
        }

        public bool Matches(ref TokenReader tokens)
        {
            if (MatchType == MatchType.Parameter)
            {
                return Argument.Skip(ref tokens);
            }
            var tokenType = MatchType switch
            {
                MatchType.Keyword => TokenType.Word,
                MatchType.Symbol => TokenType.Symbol,
                MatchType.Bracket => TokenType.Bracket,
                _ => TokenType.None,
            };
            var token = tokens.Next();
            if (token.Type != tokenType) return false;
            if (Text != null && Text != token) return false;
            return true;
        }
    }

    private readonly TokenMatcher[] _matchTokens;

    internal Signature(TokenMatcher[] tokens)
    {
        _matchTokens = tokens;
        foreach (var token in _matchTokens)
        {
            var paraIdx = token.ParameterIndex;
            if (paraIdx != null)
            {
                ParameterCount = Math.Max(ParameterCount, paraIdx.Value + 1);
            }
        }
    }

    public static SignatureBuilder New() => new SignatureBuilder();

    public static Signature CreateFunc(string key, int argumentCount = 0, bool requireParentesis = false)
    {
        var sb = new SignatureBuilder();

        sb.Keyword(key);

        if (requireParentesis)
        {
            sb.OpeningBracket('(');
        }

        for (int i = 0; i < argumentCount; i++)
        {
            sb.Parameter();
        }

        if (requireParentesis)
        {
            sb.ClosingBracket();
        }

        return sb.CreateSignature();
    }


    public int ParameterCount { get; }

    public int Length => _matchTokens.Length;

    public bool Overlaps(Signature signature)
    {
        if (Length != signature.Length) return false;
        for (int i = 0; i < _matchTokens.Length; i++)
        {
            if (!_matchTokens[i].Overlaps(signature._matchTokens[i])) return false;
        }
        return true;
    }

    internal bool Matches(TokenReader tokens) => Matches(ref tokens);

    internal bool Matches(ref TokenReader tokens)
    {
        foreach (var match in _matchTokens)
        {
            if (!match.Matches(ref tokens))
            {
                return false;
            }
        }
        return !tokens.CanRead;
    }

    internal Argument[] ExtractArguments(TokenReader tokens)
    {
        var args = new Argument[ParameterCount];
        ExtractArguments(tokens, args);
        return args;
    }

    internal void ExtractArguments(TokenReader tokens, Span<Argument> args)
    {
        foreach (var match in _matchTokens)
        {
            if (match.ParameterIndex.HasValue)
            {
                args[match.ParameterIndex.Value] = Argument.Parse(ref tokens);
            }
            else
            {
                tokens.Position += 1;
            }
        }
    }
}
