using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript;

public class Instruction
{
    public string? Key { get; }
    public Argument[]? Args { get; }
    public string? Comment { get; }
    public int Indentation { get; }

    [MemberNotNullWhen(false, nameof(Key))]
    public bool IsEmpty => Key == null && ArgsLength == 0;

    public int ArgsLength => Args == null ? 0 : Args.Length;

    public Instruction(string? key, Argument[]? args, int indentation = 0, string? comment = null)
    {
        Key = key;
        Args = args;
        Comment = comment;
        Indentation = indentation;
    }

    public static Instruction Empty { get; } = new Instruction(null, null);
}
