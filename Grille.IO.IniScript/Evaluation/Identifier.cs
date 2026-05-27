using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Evaluation;

public class Identifier
{
    public string Literal { get; }

    public string[] Path { get; }

    public string Name => Path[^1];

    public Identifier(string literal)
    {
        Literal = literal;
        Path = Literal.Split('.');
    }

    public static implicit operator Identifier(string name) => new Identifier(name);
}
