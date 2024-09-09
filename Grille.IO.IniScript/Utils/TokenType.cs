using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.Utils;

internal enum TokenType
{
    None,
    Value,
    String,
    Section,
    Comment,
}
