using Grille.IO.IniScript.Tokenization;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Compilation.Internal;

internal struct InstructionCreateInfo
{
    private readonly RangedArray<Token> _tokens;
    private readonly int _index;

    public InstructionLocation Location;
    public int BlockSize;

    public ReadOnlySpan<Token> Tokens => _tokens[_index];

    public bool IsEmpty => _tokens.Length == 0;

    internal InstructionCreateInfo(RangedArray<Token> tokens, int index, InstructionLocation location)
    {
        _tokens = tokens;
        _index = index;
        Location = location;
    }
}
