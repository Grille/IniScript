using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Evaluation;

public class TreeDict<TValue> : Dictionary<string, TValue>
{
    public Dictionary<string, TreeDict<TValue>> Children { get; }

    public TreeDict()
    {
        Children = new();
    }
}
