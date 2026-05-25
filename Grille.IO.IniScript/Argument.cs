using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grille.IO.IniScript.Utils;
using Grille.IO.IniScript.Evaluation;

namespace Grille.IO.IniScript;

public sealed class Argument
{ 
    public char Modifier { get; }

    public string Literal { get; }

    public Argument? Indexer { get; }

    public object? ConstValue { get; }

    private Argument(string literal, TokenType type, char modifier, Argument? indexer = null)
    {
        Literal = literal;
        Modifier = modifier;
        Indexer = indexer;
        ConstValue = ParseConstValue(type);
    }

    private object? ParseConstValue(TokenType type)
    {
        if (type == TokenType.Number) return NumberSerializer.Deserialize(Literal).ToObject();
        if (type == TokenType.LiteralString) return StringSerializer.Deserialize(Literal);
        if (type == TokenType.InterpolatedString) throw new NotImplementedException();
        return null;
    }

    internal static bool Skip(ref TokenReader tokens)
    {
        var token = tokens.Next();
        if (token.Type == TokenType.Symbol)
        {
            token = tokens.Next();
        }
        if (!token.Type.IsParameter) return false;
        if (token.Type == TokenType.Word)
        {
            var peek = tokens.Peek();
            if (peek != (TokenType.Bracket, "[")) return true;
            tokens.Position += 1;
            if (!Skip(ref tokens)) return false;
            token = tokens.Next();
            if (peek != (TokenType.Bracket, "]")) return false;
        }
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
        if (!token.Type.IsParameter) throw new UnexpectedTokenException(token);
        var literal = token.Substring();
        var type = token.Type;

        if (token == TokenType.Word)
        {
            var peek = tokens.Peek();
            if (peek != (TokenType.Bracket, "["))
            {
                return new Argument(literal, type, modifier);
            }
            tokens.Position += 1;
            var indexer = Parse(ref tokens);
            token = tokens.Next();
            if (token != (TokenType.Bracket, "]"))
            {
                throw new UnexpectedTokenException(token);
            }
            return new Argument(literal, type, modifier, indexer);
        }
        return new Argument(literal, type, modifier);
    }
}
