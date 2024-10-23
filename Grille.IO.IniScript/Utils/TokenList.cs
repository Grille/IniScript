using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Utils;

internal class TokenList
{
    List<Token> tokens;
    List<Range> lines;

    public TokenList()
    {
        tokens = new List<Token>();
        lines = new List<Range>();
    }

    public void Add(Token token)
    {

    }

    public void EndLine()
    {

    }
}
