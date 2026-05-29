using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Tokenization;

internal ref struct TokenReader
{
    private readonly Token None = new Token(TokenType.None, TokenLocation.Empty);

    public readonly ReadOnlySpan<Token> Tokens;

    public int Position;

    public int Remaining => Tokens.Length - Position;

    public TokenReader(ReadOnlySpan<Token> tokens)
    {
        Tokens = tokens;
        Position = 0;
    }

    private bool PositioninRange(int position) => position >= 0 && position < Tokens.Length;

    public bool CanRead => PositioninRange(Position);

    public Token this[int index] => PositioninRange(index) ? Tokens[index] : None;

    public Token Current => this[Position];

    public Token Next() => this[Position++];

    private bool IncIfTrue(bool value)
    {
        if (value) Position += 1;
        return value;
    }

    public bool NextIf(string value) => IncIfTrue(Current == value);

    public bool NextIf(TokenType type) => IncIfTrue(Current == type);

    public bool NextIf(Predicate<Token> predicate) => IncIfTrue(predicate(Current));

    public Token Peek(int offset = 1) => this[Position + offset];

    [DoesNotReturn]
    public void ThrowUnexpected() => throw new UnexpectedTokenException(Current);

    public static implicit operator TokenReader(ReadOnlySpan<Token> tokens) => new TokenReader(tokens);
}
