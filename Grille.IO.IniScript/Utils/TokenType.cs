using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Utils;

internal enum TokenType
{
    None,
    Word,
    Number,
    String,
    InterpolatedString,
    Section,
    Symbol,
    Comment,
}
