using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript;

public readonly struct InstructionLocation
{
    public int Line { get; }

    public int Indentation { get; }

    public string? FilePath { get; }

    public InstructionLocation(int line, int indentation, string? filePath = null)
    {
        Line = line;
        Indentation = indentation;
        FilePath = filePath;
    }

}
