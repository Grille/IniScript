using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grille.IO.IniScript.Utils;
using Grille.IO.IniScript.Evaluation;
using System.Diagnostics.CodeAnalysis;
using Grille.IO.IniScript.Tokenization;

using Grille;
using Grille.IO;
using Grille.IO.IniScript;
using Grille.IO.IniScript.Compilation;

namespace Grille.IO.IniScript.Compilation;

public sealed class Parameter
{
    public object Value { get; }

    public string? Modifier { get; }

    public Parameter? Indexer { get; }

    public Parameter(object value, string? modifier = null, Parameter? indexer = null)
    {
        (Value, Modifier) = TryApplyModifier(value, modifier);
        Indexer = indexer;
    }

    (object Value, string? Modifier) TryApplyModifier(object value, string? modifier)
    {
        if (modifier == "-")
        {
            if (value is double f64) value = -f64;
            else if (value is long i64) value = -i64;
            else throw new ArgumentException();
            modifier = null;
        }
        else if (modifier == "+")
        {
            if (!(value is double || value is long)) throw new ArgumentException();
            modifier = null;
        }
        return (value, modifier);
    }

    public void ToString(StringBuilder sb)
    {
        if (Modifier != null) sb.Append(Modifier);
        sb.Append(Value);
        if (Indexer != null)
        {
            sb.Append('[');
            Indexer.ToString(sb);
            sb.Append(']');
        }
    }

    public override string ToString() => StringBuilder.ToString(ToString);
}
