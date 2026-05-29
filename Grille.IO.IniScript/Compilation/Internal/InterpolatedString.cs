using Grille.IO.IniScript.Tokenization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Compilation.Internal;

internal class InterpolatedString
{
    public string[] Strings { get; }

    public Parameter[] Arguments { get; }

    public InterpolatedString(string[] strings, Parameter[] arguments)
    {
        Strings = strings;
        Arguments = arguments;
    }

    public static InterpolatedString Parse(Token token)
    {
        return null!;
    }
}
