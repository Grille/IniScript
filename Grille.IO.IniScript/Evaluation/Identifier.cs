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

    public override string ToString() => Literal;


    public override bool Equals(object? obj)
    {
        if (obj is Identifier identifier) return Literal == identifier.Literal;
        if (obj is string literal) return Literal == literal;
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Literal);
    }

    public static implicit operator Identifier(string name) => new Identifier(name);
}
