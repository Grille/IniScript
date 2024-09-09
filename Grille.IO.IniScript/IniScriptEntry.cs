using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO;

public class IniScriptEntry
{
    public string? Key { get; set; }
    public string[] Args { get; set; }
    public string? Comment { get; set; }
    public int Indentation { get; set; }

    public bool IsEmpty => Key == null && Args.Length == 0;

    public IniScriptEntry(string? key, string[]? args, int indentation = 0, string? comment = null)
    {
        if (args == null) args = Array.Empty<string>();

        Key = key;
        Args = args;
        Comment = comment;
        Indentation = indentation;
    }
}
