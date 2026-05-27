using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Utils;

internal ref struct TokenReader
{
    private readonly Token None = new Token(TokenType.None, TokenLocation.Empty);

    public readonly ReadOnlySpan<Token> Tokens;

    public int Position;

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

    public bool NextIf(string value)
    {
        if (Current == value)
        {
            Position += 1;
            return true;
        }
        return false;
    }

    public bool NextIf(TokenType type)
    {
        if (Current == type)
        {
            Position += 1;
            return true;
        }
        return false;
    }

    public Token Peek(int offset = 1) => this[Position + offset];

    public static implicit operator TokenReader(ReadOnlySpan<Token> tokens) => new TokenReader(tokens);
}
