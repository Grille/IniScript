using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Evaluation.Compilation; 

public class CompiledInstruction
{
    private readonly Command _command;
    private readonly Argument[] _arguments;

    public int BlockSize { get; }

    internal CompiledInstruction(Command command, Argument[] arguments, int blockSize = 0)
    {
        _command = command;
        _arguments = arguments;
        BlockSize = blockSize;
    }

    public void Invoke(Runtime runtime)
    {
        var values = new object[_arguments.Length];
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = runtime.CastArgument(_arguments[i], _command.ParameterTypes[i]);
        }
        _command.Invoke(runtime, values);
    }
}
