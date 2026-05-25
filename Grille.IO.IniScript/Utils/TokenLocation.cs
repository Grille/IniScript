using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Utils;

internal readonly struct TokenLocation
{
    public static readonly TokenLocation None = new(-1, -1);
    public static readonly TokenLocation Empty = new(0, 0);

    public readonly int Row;
    public readonly int Column;

    public TokenLocation(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public override string ToString()
    {
        return $"({Row}, {Column})";
    }

    public static bool operator ==(TokenLocation left, TokenLocation right) => left.Row == right.Row && left.Column == right.Column;

    public static bool operator !=(TokenLocation left, TokenLocation right) => !(left == right);

    public override bool Equals(object? obj)
    {
        if (obj is TokenLocation location)
        {
            return this == location;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }
}