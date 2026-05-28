using Grille.IO.IniScript.Tokenization;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Compilation.Internal;

internal readonly struct InstructionInfo
{
    private readonly RangedArray<Token> _tokens;
    private readonly int _index;

    internal ReadOnlySpan<Token> Tokens => _tokens[_index];

    public readonly InstructionLocation Location;

    public bool IsEmpty => _tokens.Length == 0;

    internal InstructionInfo(RangedArray<Token> tokens, int index, InstructionLocation location)
    {
        _tokens = tokens;
        _index = index;
        Location = location;
    }
}
