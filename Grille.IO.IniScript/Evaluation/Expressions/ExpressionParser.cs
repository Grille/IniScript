using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Grille.IO.IniScript.Utils;

namespace Grille.IO.IniScript.Evaluation.Expressions;

public class ExpressionParser
{
    readonly internal Lexer _lexer;

    public ExpressionParser()
    {
        var rules = new Lexer.Rule[]
        {

        };
        _lexer = new Lexer(rules);
    }

    public void Parse(string text)
    {
        _lexer.Tokenize(text);
    }
}
