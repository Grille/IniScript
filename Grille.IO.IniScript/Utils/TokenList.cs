using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Utils;

internal sealed class TokenList
{
    private readonly Token[] _tokens;
    private readonly Range[] _lines;

    public TokenList(Token[] tokens, Range[] lines)
    {
        _tokens = tokens;
        _lines = lines;
    }

    public Token this[int index] => _tokens[index];

    public Token this[Index index] => _tokens[index];

    public int LineCount => _lines.Length;

    public int TokenCount => _tokens.Length;

    public Range GetLineRange(int index) => _lines[index];

    public ReadOnlySpan<Token> GetLine(int index)
    {
        if (index < 0 || index >= _lines.Length) throw new ArgumentOutOfRangeException(nameof(index));
        var range = _lines[index];
        return _tokens.AsSpan(range);
    }

    public static implicit operator ReadOnlySpan<Token>(TokenList tokens) => tokens._tokens;
}
