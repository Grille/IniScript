using Grille.IO.IniScript.Evaluation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Compilation;

public sealed class CompiledScript
{
    internal Dictionary<string, CompiledFunction> Functions { get; }

    internal CompiledScript(CompiledFunction[] functions)
    {
        Functions = new Dictionary<string, CompiledFunction>();
        foreach (var function in functions)
        {
            Functions[function.Name] = function;
        }
    }

    public void Invoke(Runtime runtime)
    {
        foreach (var pair in Functions)
        {
            pair.Value.Invoke(runtime);
        }
    }

    public void Invoke(Runtime runtime, string name)
    {
        Functions[name].Invoke(runtime);
    }

}
