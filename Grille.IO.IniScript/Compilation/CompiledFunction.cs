using Grille.IO.IniScript.Evaluation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Compilation;

public sealed class CompiledFunction
{
    public string Name { get; }

    public string? Parent { get; }

    internal CompiledInstruction[] Instructions { get; }

    internal CompiledFunction(string name, string? parent, CompiledInstruction[] compiledNode)
    {
        Name = name;
        Parent = parent;
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
