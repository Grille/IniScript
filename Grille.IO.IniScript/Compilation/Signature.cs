using Grille.IO.IniScript.Tokenization;
using Grille.IO.IniScript.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Compilation;

using static ParameterParser;

public sealed class Signature 
{
    internal enum MatchType
    {
        None,
        Keyword,
        Symbol,
        Parameter,
        ParameterList,
    }

    internal class TokenMatcher
    {
        public readonly MatchType Type;
        public readonly string? Text;
        public readonly int? ParameterIndex;

        private TokenMatcher() => Type = MatchType.None;

        internal TokenMatcher(MatchType type, string text)
        {
            Type = type;
            Text = text;
        }

        internal TokenMatcher(MatchType type, int parameterIndex)
        {
            Type = type;
            ParameterIndex = parameterIndex;
        }

        public static readonly TokenMatcher Empty = new();

        private static bool MatchTypeCollision(MatchType type0, MatchType type1)
        {
            int GetCode(MatchType type) => type switch
            {
                MatchType.Keyword => 0,
                MatchType.Parameter => 0,
                MatchType.ParameterList => 0,
                MatchType.Symbol => 1,
                _ => 0,
            };
            return GetCode(type0) == GetCode(type1);
        }

        private static bool TextCollision(string? text0, string? text1)
        {
            if (text0 == null || text1 == null) return true;
            return text0 == text1;
        }

        public bool Matches(TokenMatcher match)
        {
            return MatchTypeCollision(Type, match.Type) && TextCollision(Text, match.Text);
        }

        public bool Matches(ref TokenReader tokens)
        {
            if (Type == MatchType.Parameter)
            {
                return Skip(ref tokens) == SkipResult.Parsed;
            }
            else if (Type == MatchType.ParameterList)
            {
                return SkipInlineArray(ref tokens) != SkipResult.Error;
            }
            var tokenType = Type switch
            {
                MatchType.Keyword => TokenType.Word,
                MatchType.Symbol => TokenType.Symbol,
                _ => TokenType.None,
            };
            var token = tokens.Next();
            if (token.Type != tokenType) return false;
            if (Text != null && Text != token) return false;
            return true;
        }

        public void ToString(StringBuilder sb)
        {
            if (ParameterIndex.HasValue)
            {
                sb.Append(Type switch
                {
                    MatchType.Parameter => "#",
                    MatchType.ParameterList => "#LIST",
                    _ => "#ERROR"
                });
                sb.Append(ParameterIndex.Value);
            }
            else
            {
                sb.Append(Text!);
            }
        }

        public override string ToString() => StringBuilder.ToString(ToString);
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

    public SignatureBuilder EditCopy()
    {
        var sb = new SignatureBuilder();
        foreach (var token in _matchTokens) sb.Add(token);
        sb.ParameterIndex = ParameterCount;
        return sb;
    }

    public static Signature CreateFunc(string key, int argumentCount = 0, bool requireParentesis = false)
    {
        var sb = new SignatureBuilder();

        sb.Keyword(key);

        if (requireParentesis)
        {
            sb.OpenBracket('(');
        }

        for (int i = 0; i < argumentCount; i++)
        {
            sb.Parameter();
        }

        if (requireParentesis)
        {
            sb.CloseBracket();
        }

        return sb.CreateSignature();
    }

    public int ParameterCount { get; }

    public int Length => _matchTokens.Length;

    public bool Matches(Signature signature)
    {
        if (Length != signature.Length) return false;
        for (int i = 0; i < _matchTokens.Length; i++)
        {
            if (!_matchTokens[i].Matches(signature._matchTokens[i])) return false;
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

    internal Parameter[] ExtractArguments(TokenReader tokens)
    {
        var args = new Parameter[ParameterCount];
        ExtractArguments(tokens, args);
        return args;
    }

    internal void ExtractArguments(TokenReader tokens, Span<Parameter> args)
    {
        if (args.Length != ParameterCount) throw new ArgumentException();
        foreach (var match in _matchTokens)
        {
            if (match.ParameterIndex.HasValue)
            {
                if (match.Type == MatchType.Parameter)
                {
                    args[match.ParameterIndex.Value] = Parse(ref tokens);
                }
                else if (match.Type == MatchType.ParameterList)
                {
                    args[match.ParameterIndex.Value] = new(ParseInlineArray(ref tokens));
                }
                else throw new InvalidOperationException($"{match.Type}");
            }
            else
            {
                tokens.Position += 1;
            }
        }
    }

    public void ToString(StringBuilder sb)
    {
        foreach (var match in _matchTokens)
        {
            match.ToString(sb);
        }
    }

    public override string ToString() => StringBuilder.ToString(ToString);
}
