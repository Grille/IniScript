using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Compilation;

public sealed class Compiler
{
    readonly Parser _parser;
    readonly List<CompiledFunction> _functions;
    readonly List<CompiledInstruction> _instructions;
    readonly CommandRegistry _commands;

    public CommandRegistry Commands => _commands;

    public Compiler(CommandRegistry commands)
    {
        _commands = commands;
        _parser = new();
        _functions = new();
        _instructions = new();
    }

    internal Compiler(CommandRegistry commands, Parser parser)
    {
        _commands = commands;
        _parser = parser;
        _functions = new();
        _instructions = new();
    }

    public CompiledScript Compile(string script) => Compile(_parser.Parse(script));

    public CompiledScript Compile(Stream script) => Compile(_parser.Parse(script));
    
    public CompiledScript Compile(TextReader script) => Compile(_parser.Parse(script));

    internal CompiledScript Compile(ScriptCreationObject script)
    {
        _functions.Clear();

        foreach (var func in script.Values)
        {
            _functions.Add(Compile(func));
        }

        var functions = _functions.ToArray();

        return new CompiledScript(functions);
    }

    internal CompiledFunction Compile(ScriptCreationObject.Section func)
    {
        _instructions.Clear();

        int[] blockSize = GetBlockSizes(func);

        for (int i = 0; i < func.Count; i++)
        {
            var inst = func[i];
            if (inst != null && !inst.IsEmpty)
            {
                var pair = _commands.GetPair(inst.Tokens);
                var args = pair.Signature.ExtractArguments(inst.Tokens);
                var instruction = new CompiledInstruction(pair.Command, args, blockSize[i]);
                _instructions.Add(instruction);
            }
        }

        var instructionsArray = _instructions.ToArray();

        return new CompiledFunction(func.Key, instructionsArray);
    }

    private int GetBlockSize(ScriptCreationObject.Section func, int thisIndex)
    {
        var thisInstruction = func[thisIndex];
        var thisIndentation = thisInstruction.Location.Indentation;

        int nextIndex = thisIndex + 1;

        while (nextIndex < func.Count)
        {
            var nextInstruction = func[nextIndex];
            var nextIndentation = thisInstruction.Location.Indentation;

            if (nextIndentation <= thisIndentation && !nextInstruction.IsEmpty)
            {
                break;
            }

            nextIndex += 1;
        }

        return nextIndex - thisIndex - 1;
    }

    private int[] GetBlockSizes(ScriptCreationObject.Section func)
    {
        int[] blockSize = new int[func.Count];

        for (int i = 0; i < func.Count; i++)
        {
            blockSize[i] = GetBlockSize(func, i);
        }

        return blockSize;
    }
}
