using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Evaluation.Compilation;

public sealed class CompiledScript
{
    public CompiledFunction EntryPoint { get; }

    internal Dictionary<string, CompiledFunction> Functions { get; }

    internal CompiledScript(CompiledFunction[] functions, string entryPoint = Script.DefaultSectionName)
    {
        Functions = new Dictionary<string, CompiledFunction>();
        foreach (var function in functions)
        {
            Functions[function.Name] = function;
        }
        EntryPoint = Functions[entryPoint];
    }

    public void Invoke()
    {
        EntryPoint.Invoke(new Runtime());
    }

    public void Invoke(string name)
    {
        Functions[name].Invoke(new Runtime());
    }

    public void Invoke(Runtime runtime)
    {
        EntryPoint.Invoke(runtime);
    }

    public void Invoke(Runtime runtime, string name)
    {
        Functions[name].Invoke(runtime);
    }

}
