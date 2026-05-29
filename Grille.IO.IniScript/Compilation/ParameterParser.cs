using Grille.IO.IniScript.Compilation.Internal;
using Grille.IO.IniScript.Evaluation;
using Grille.IO.IniScript.Tokenization;
using Grille.IO.IniScript.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Compilation;

using static ParameterParser.SkipResult;

internal static class ParameterParser
{
    internal enum SkipResult
    {
        None,
        Parsed,
        Error,
    }

    private static bool IsModifier(Token token)
    {
        if (token.Type != TokenType.Symbol) return false;
        return token != "[" && token != "]";
    }

    private static bool IsParameter(Token token) => token.Type.IsParameter;

    internal static SkipResult Skip(ref TokenReader tokens)
    {
        tokens.NextIf(IsModifier);
        var array = SkipArray(ref tokens);
        if (array == Error) return Error;
        if (array == None && !tokens.NextIf(IsParameter)) return None;
        if (SkipIndexer(ref tokens) == Error) return Error;
        return Parsed;
    }

    internal static SkipResult SkipInlineArray(ref TokenReader tokens)
    {
        var first = Skip(ref tokens);
        if (first != Parsed) return first;
        while (tokens.NextIf(","))
        {
            if (Skip(ref tokens) != Parsed) return Error;
        }
        return Parsed;
    }

    private static SkipResult SkipArray(ref TokenReader tokens)
    {
        if (!tokens.NextIf("[")) return None;
        if (SkipInlineArray(ref tokens) == Error) return Error;
        if (!tokens.NextIf("]")) return Error;
        return Parsed;
    }

    private static SkipResult SkipIndexer(ref TokenReader tokens)
    {
        if (!tokens.NextIf("[")) return None;
        if (Skip(ref tokens) != Parsed) return Error;
        if (!tokens.NextIf("]")) return Error;
        return Parsed;
    }

    internal static Parameter Parse(ref TokenReader tokens)
    {
        string? modifier = null;
        if (tokens.NextIf(IsModifier))
        {
            modifier = tokens.Current;
        }
        var value = ParseValue(ref tokens);
        var indexer = ParseIndexer(ref tokens);
        return new Parameter(value, modifier, indexer);
    }

    private static object ParseValue(ref TokenReader tokens)
    {
        var array = ParseArray(ref tokens);
        if (array != null) return array;
        var value = tokens.Current;
        if (!tokens.NextIf(IsParameter)) tokens.ThrowUnexpected();
        return ParseTokenValue(value);
    }

    private static SkipResult VirtualSkip(TokenReader tokens) => Skip(ref tokens);

    internal static Parameter[] ParseInlineArray(ref TokenReader tokens)
    {
        if (VirtualSkip(tokens) == None) return Array.Empty<Parameter>();
        var list = new List<Parameter>();
        do list.Add(Parse(ref tokens)); while (tokens.NextIf(","));
        return list.ToArray();
    }

    private static Parameter[]? ParseArray(ref TokenReader tokens)
    {
        if (!tokens.NextIf("[")) return null;
        var array = ParseInlineArray(ref tokens);
        if (!tokens.NextIf("]")) tokens.ThrowUnexpected();
        return array;
    }

    private static Parameter? ParseIndexer(ref TokenReader tokens)
    {
        if (!tokens.NextIf("[")) return null;
        var indexer = Parse(ref tokens);
        if (!tokens.NextIf("]")) tokens.ThrowUnexpected();
        return indexer;
    }

    private static object ParseTokenValue(Token token) => token.Type switch
    {
        TokenType.Word => Identifier.Cache.Get(token),
        TokenType.Number => NumberSerializer.Deserialize(token).ToObject(),
        TokenType.LiteralString => StringSerializer.Deserialize(token),
        TokenType.InterpolatedString => throw new NotImplementedException(),
        _ => throw new NotImplementedException(),
    };
}
