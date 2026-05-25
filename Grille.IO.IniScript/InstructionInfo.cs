using Grille.IO.IniScript.Utils;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grille.IO.IniScript;

public sealed class InstructionInfo
{
    private readonly Token[] _tokens;
    private readonly string[] _comments;

    internal ReadOnlySpan<Token> Tokens => _tokens;

    public ReadOnlySpan<string> Comments => _comments;

    public InstructionLocation Location { get; }

    public bool IsEmpty => _tokens.Length == 0;

    internal InstructionInfo(Token[] tokens, InstructionLocation location, string[]? comments = null)
    {
        _tokens = tokens;
        _comments = comments ?? Array.Empty<string>();
        Location = location;
    }
}
