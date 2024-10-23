using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Evaluation.Compilation;

public sealed class CompiledFunction
{
    public string Name { get; }

    internal CompiledInstruction[] Instructions { get; }

    internal CompiledFunction(string name, CompiledInstruction[] compiledNode)
    {
        Name = name;
        Instructions = compiledNode;
    }

    public void Invoke(Runtime runtime)
    {
        for (int i = 0; i < Instructions.Length; i++)
        {
            Instructions[i].Invoke(runtime);
        }
    }
}
