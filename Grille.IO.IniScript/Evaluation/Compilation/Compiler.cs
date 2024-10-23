using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Evaluation.Compilation;

public class Compiler
{
    readonly Parser _parser;
    readonly List<CompiledFunction> _functions;
    readonly List<CompiledInstruction> _instructions;
    readonly Commands _commands;

    public Compiler(Commands commands)
    {
        _commands = commands;
        _parser = new();
        _functions = new();
        _instructions = new();
    }

    public Compiler(Commands commands, Parser parser)
    {
        _commands = commands;
        _parser = parser;
        _functions = new();
        _instructions = new();
    }

    protected virtual Instruction? InstructionPreview(Instruction instruction)
    {
        return instruction;
    }

    public CompiledScript Compile(string script) => Compile(_parser.Parse(script));

    public CompiledScript Compile(Stream script) => Compile(_parser.Parse(script));
    
    public CompiledScript Compile(TextReader script) => Compile(_parser.Parse(script));

    public CompiledScript Compile(Script script)
    {
        _functions.Clear();

        foreach (var func in script)
        {
            _functions.Add(Compile(func));
        }

        var functions = _functions.ToArray();

        return new CompiledScript(functions);
    }

    public CompiledFunction Compile(Function func)
    {
        _instructions.Clear();

        int[] blockSize = GetBlockSizes(func);

        for (int i = 0; i < func.Count; i++)
        {
            var inst = InstructionPreview(func[i]);
            if (inst != null && !inst.IsEmpty)
            {
                var action = _commands.GetAction(inst);
                var instruction = new CompiledInstruction(action, blockSize[i]);
                _instructions.Add(instruction);
            }
        }

        var instructions = _instructions.ToArray();

        return new CompiledFunction(func.Name, instructions);
    }

    private int GetBlockSize(Function func, int thisIndex)
    {
        var thisInstruction = func[thisIndex];
        var thisIndentation = thisInstruction.Indentation;

        int nextIndex = thisIndex + 1;

        while (nextIndex < func.Count)
        {
            var nextInstruction = func[nextIndex];
            var nextIndentation = thisInstruction.Indentation;

            if (nextIndentation <= thisIndentation && !nextInstruction.IsEmpty)
            {
                break;
            }

            nextIndex += 1;
        }

        return nextIndex - thisIndex - 1;
    }

    private int[] GetBlockSizes(Function func)
    {
        int[] blockSize = new int[func.Count];

        for (int i = 0; i < func.Count; i++)
        {
            blockSize[i] = GetBlockSize(func, i);
        }

        return blockSize;
    }
}
