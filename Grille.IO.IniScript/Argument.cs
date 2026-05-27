using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grille.IO.IniScript.Utils;
using Grille.IO.IniScript.Evaluation;
using System.Diagnostics.CodeAnalysis;

namespace Grille.IO.IniScript;

public sealed class Argument
{ 
    public char Modifier { get; }

    public Argument? Indexer { get; }

    public object Value { get; }

    private Argument(object value, char modifier = '\0', Argument? indexer = null)
    {
        Value = value;
        Modifier = modifier;
        Indexer = indexer;
    }

    internal static bool Skip(ref TokenReader tokens)
    {
        var token = tokens.Next();
        if (token.Type == TokenType.Symbol)
        {
            token = tokens.Next();
        }
        if (!SkipArray(ref tokens)) return false;
        else if (!token.Type.IsParameter) return false;
        if (!SkipIndexer(ref tokens)) return false;
        return true;
    }

    private static bool SkipArray(ref TokenReader tokens)
    {
        if (!tokens.NextIf("[")) return true;
        while (true)
        {
            if (!Skip(ref tokens)) return false;
            if (tokens.NextIf("]")) break;
            else if (tokens.NextIf(",")) continue;
            else return false;
        }
        return true;
    }

    private static bool SkipIndexer(ref TokenReader tokens)
    {
        if (!tokens.NextIf("[")) return true;
        if (!Skip(ref tokens)) return false;
        if (!tokens.NextIf("]")) return false;
        return true;
    }

    internal static Argument Parse(ref TokenReader tokens)
    {
        var token = tokens.Next();
        char modifier = '\0';
        if (token.Type == TokenType.Symbol)
        {
            modifier = token.AsSpan()[0];
            token = tokens.Next();
        }
        var value = ParseValue(ref  tokens);
        var indexer = ParseIndexer(ref tokens);
        return new Argument(value, modifier, indexer);
    }

    private static object ParseValue(ref TokenReader tokens)
    {
        var array = TryParseArray(ref tokens);
        if (array != null) return array;
        var token = tokens.Next();
        if (!token.Type.IsParameter) throw new UnexpectedTokenException(token);
        return ParseToken(token);
    }

    private static Argument[]? TryParseArray(ref TokenReader tokens)
    {
        if (!tokens.NextIf("[")) return null;

        var list = new List<Argument>();

        while (true)
        {
            list.Add(Parse(ref tokens));
            if (tokens.NextIf("]")) break;
            else if (tokens.NextIf(",")) continue;
            else throw new UnexpectedTokenException(tokens.Current);
        }

        return list.ToArray();
    }

    private static Argument? ParseIndexer(ref TokenReader tokens)
    {
        var token = tokens.Peek();
        if (token != (TokenType.Bracket, "[")) return null;
        tokens.Position += 1;

        var indexer = Parse(ref tokens);

        token = tokens.Next();
        if (token != (TokenType.Bracket, "]")) throw new UnexpectedTokenException(token);

        return indexer;
    }

    private static object ParseToken(Token token) => token.Type switch
    {
        TokenType.Word => new Identifier(token),
        TokenType.Number => NumberSerializer.Deserialize(token).ToObject(),
        TokenType.LiteralString => StringSerializer.Deserialize(token),
        TokenType.InterpolatedString => throw new NotImplementedException(),
        _ => throw new NotImplementedException(),
    };
}
