using Grille.IO.IniScript.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Tokenization;

internal readonly struct Token
{
    private readonly string _text;
    public readonly int Start;
    public readonly int Length;
    public readonly TokenType Type;
    public readonly TokenLocation Location;

    public Range Range => new Range(Start, Start + Length + 1);

    public Token(string text, int start, int length, TokenType type, TokenLocation location)
    {
        _text = text;
        Start = start;
        Length = length;
        Type = type;
        Location = location;
    }

    public Token(string text, TokenType type) : this(text, 0, text.Length, type, TokenLocation.Empty) { }

    public Token(TokenType type, TokenLocation location) : this(string.Empty, 0, 0, type, location) { }

    public string Substring() => _text.Substring(Start, Length);

    public ReadOnlySpan<char> AsSpan() => _text.AsSpan(Start, Length);

    public ReadOnlyMemory<char> AsMemory() => _text.AsMemory(Start, Length);

    public override string ToString() => $"{StringSerializer.Serialize(AsSpan())} {Type}";

    public bool CompareSpans(ReadOnlySpan<char> span, StringComparison comparisonType = StringComparison.Ordinal) => AsSpan().Equals(span, comparisonType);

    public bool CompareSpans(in Token token, StringComparison comparisonType = StringComparison.Ordinal) => AsSpan().Equals(token.AsSpan(), comparisonType);

    public static bool operator ==(Token left, Token right) => left == right.Type && left.CompareSpans(right) && left.Location == right.Location;

    public static bool operator !=(Token left, Token right) => !(left == right);

    public static bool operator ==(Token left, ReadOnlySpan<char> rightSpan) => left.CompareSpans(rightSpan);

    public static bool operator !=(Token left, ReadOnlySpan<char> rightSpan) => !(left == rightSpan);

    public static bool operator ==(Token left, TokenType type) => left.Type == type;

    public static bool operator !=(Token left, TokenType type) => left.Type != type;

    public static bool operator ==(Token left, (TokenType Type, string Value) right) => left == right.Type && left == right.Value;

    public static bool operator !=(Token left, (TokenType Type, string Value) right) => !(left == right);

    public static bool operator ==(Token left, (string Value, TokenType Type) right) => left == right.Type && left == right.Value;

    public static bool operator !=(Token left, (string Value, TokenType Type) right) => !(left == right);

    public static implicit operator ReadOnlySpan<char>(Token token) => token.AsSpan();

    public static implicit operator ReadOnlyMemory<char>(Token token) => token.AsMemory();

    public static implicit operator string(Token token) => token.Substring();

    public override bool Equals(object? obj)
    {
        if (obj is Token token)
        {
            return this == token;
        }
        return false;
    }

    public override int GetHashCode() => HashCode.Combine(Type, Start, Length, Location);
}
