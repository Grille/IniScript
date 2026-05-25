using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Utils;

using static TokenType;

internal enum TokenType
{
    None,
    Whitespace,
    Comment,

    Word,
    Number,
    LiteralString,
    InterpolatedString,

    Symbol,
    Bracket,

    EndOfLine,
    EndOfFile,

    System,
}

internal static class TokenTypeExtension
{
    extension (TokenType type)
    {
        public bool IsString => type == LiteralString || type == InterpolatedString;
        public bool IsParameter => type == Word || type == Number || type.IsString;
    }
}