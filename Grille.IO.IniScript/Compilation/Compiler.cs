using Grille.IO.IniScript.Compilation.Internal;
using Grille.IO.IniScript.Evaluation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Grille.IO.IniScript.Compilation;

using static CommandRegistry.RegistryLevel;

public sealed class Compiler
{
    readonly static Parser _parser = new();

    public CommandRegistry Commands { get; }

    public Compiler(CommandRegistry commands)
    {
        Commands = commands;
    }

    public IniAssembly Compile(string script) => Compile(_parser.Parse(script));

    public IniAssembly Compile(Stream script) => Compile(_parser.Parse(script));
    
    public IniAssembly Compile(TextReader script) => Compile(_parser.Parse(script));

    public void Compile(string script, IniAssembly assembly) => Compile(_parser.Parse(script), assembly);

    public void Compile(Stream script, IniAssembly assembly) => Compile(_parser.Parse(script), assembly);

    public void Compile(TextReader script, IniAssembly assembly) => Compile(_parser.Parse(script), assembly);

    private IniAssembly Compile(AssemblyCreateInfo info)
    {
        var assembly = new IniAssembly();
        Compile(info, assembly);
        return assembly;
    }

    private void Compile(AssemblyCreateInfo info, IniAssembly assembly)
    {
        foreach (var func in info.Values)
        {
            assembly[func.Key] = Compile(func);
        }
    }

    private CompiledFunction Compile(AssemblyCreateInfo.Section section)
    {
        var src = section.Array;
        var dst = new List<CompiledInstruction>(src.Length * 2);

        for (int i = 0; i < src.Length; i++)
        {
            Compile(src[i], dst);
        }

        return new CompiledFunction(section.Key, section.ParentKey, dst.ToArray());
    }

    private void Compile(InstructionCreateInfo info, List<CompiledInstruction> dst)
    {
        CompiledInstruction Build(Pair pair)
        {
            var args = pair.Signature.ExtractArguments(info.Tokens);
            return new CompiledInstruction(pair.Command, args, info.BlockSize);
        }

        var tokens = info.Tokens;
        bool isAssignment = InternalSignatures.IsAssignment(tokens);

        for (int i = 0; i < Commands.Levels.Length; i++)
        {
            var level = Commands.Levels[i];

            if (level.TryGetPair(info.Tokens, out var pair))
            {
                dst.Add(Build(pair));
                return;
            }
        }
        throw new KeyNotFoundException();
    }
}
