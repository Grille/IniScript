using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Evaluation.Compilation;

public struct CompiledInstruction
{
    public readonly int IndentedBlockSize;
    public readonly Action<Runtime> Action;

    internal CompiledInstruction(Action<Runtime> action, int blockSize = 0)
    {
        Action = action;
        IndentedBlockSize = blockSize;
    }

    public void Invoke(Runtime runtime)
    {
        Action(runtime);
    }
}
