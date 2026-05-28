using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grille.IO.IniScript.Utils;
using Grille.IO.IniScript.Evaluation;
using System.Diagnostics.CodeAnalysis;
using Grille.IO.IniScript.Tokenization;

using Grille;
using Grille.IO;
using Grille.IO.IniScript;
using Grille.IO.IniScript.Compilation;

namespace Grille.IO.IniScript.Compilation;

using static Argument.SkipResult;

public sealed class Argument
{ 
    public char Modifier { get; }

    public Argument? Indexer { get; }

    public object Value { get; }

    public Argument(object value, char modifier = '\0', Argument? indexer = null)
    {
        (Value, Modifier) = TryApplyModifier(value, modifier);
        Indexer = indexer;
    }

    (object Value, char Modifier) TryApplyModifier(object value, char modifier)
    {
        if (modifier == '-')
        {
            if (value is double f64) value = -f64;
            else if (value is long i64) value = -i64;
            else throw new ArgumentException();
            modifier = '\0';
        }
        else if (modifier == '+')
        {
            if (!(value is double || value is long)) throw new ArgumentException();
            modifier = '\0';
        }
        return (value, modifier);
    }

    internal enum SkipResult
    {
        None,
        Parsed,
        Failure,
    }

    internal static bool Skip(ref TokenReader tokens)
    {
        tokens.NextIf(TokenType.Symbol);
        var array = SkipArray(ref tokens);
        if (array == Failure) return false;
        if (array == None && !tokens.Next().Type.IsParameter) return false;
        if (SkipIndexer(ref tokens) == Failure) return false;
        return true;
    }

    private static SkipResult SkipArray(ref TokenReader tokens)
    {
        if (!tokens.NextIf("[")) return None;
        while (true)
        {
            if (!Skip(ref tokens)) return Failure;
            if (tokens.NextIf("]")) break;
            else if (tokens.NextIf(",")) continue;
            else return Failure;
        }
        return Parsed;
    }

    private static SkipResult SkipIndexer(ref TokenReader tokens)
    {
        if (!tokens.NextIf("[")) return None;
        if (!Skip(ref tokens)) return Failure;
        if (!tokens.NextIf("]")) return Failure;
        return Parsed;
    }

    internal static Argument Parse(ref TokenReader tokens)
    {
        char modifier = '\0';
        if (tokens.NextIf(TokenType.Symbol))
        {
            modifier = tokens.Current.AsSpan()[0];
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

    public void ToString(StringBuilder sb)
    {
        if (Modifier != '\0') sb.Append(Modifier);
        sb.Append(Value);
        if (Indexer != null)
        {
            sb.Append('[');
            Indexer.ToString(sb);
            sb.Append(']');
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        ToString(sb);
        return sb.ToString();
    }
}
