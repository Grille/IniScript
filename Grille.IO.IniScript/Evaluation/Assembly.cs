using Grille.IO.IniScript.Compilation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Evaluation;
 
public class Assembly
{
    Dictionary<string, Assembly> Assemblies;

    Dictionary<string, CompiledFunction> Functions;

    Dictionary<string, object> Constants;

    public Assembly()
    {
        Assemblies = new();
        Functions = new();
        Constants = new();
    }


    //public Dictionary<string, >
}
