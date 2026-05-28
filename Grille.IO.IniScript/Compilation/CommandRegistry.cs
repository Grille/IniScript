using Grille.IO.IniScript.Compilation.Internal;
using Grille.IO.IniScript.Evaluation;

using Grille.IO.IniScript.Tokenization;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Grille.IO.IniScript.Compilation;

public sealed class CommandRegistry
{
    internal class CommandSignatureRegistry
    {
        public readonly record struct Pair(Command Command, Signature Signature);

        private readonly List<Pair> _list = new();

        private int FindOverlapIndex(Signature signature)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                var pair = _list[i];
                if (signature.Overlaps(pair.Signature))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Register(Command command, Signature signature, bool allowOveride = false)
        {
            if (command.ParameterCount != signature.ParameterCount)
            {
                throw new ArgumentException();
            }
            var pair = new Pair(command, signature);
            int index = FindOverlapIndex(signature);
            if (index != -1)
            {
                if (!allowOveride)
                {
                    throw new InvalidOperationException($"A command with an overlapping signature is already registered.");
                }
                else
                {
                    _list[index] = pair;
                }
            }
            else
            {
                _list.Add(pair);
            }
        }

        public bool TryGetPair(Signature signature, [MaybeNullWhen(false)] out Pair result)
        {
            int index = FindOverlapIndex(signature);
            if (index != -1)
            {
                result = _list[index];
                return true;
            }
            result = default; 
            return false;
        }

        internal bool TryGetPair(ReadOnlySpan<Token> tokens, [MaybeNullWhen(false)] out Pair result)
        {
            foreach (var pair in _list)
            {
                if (pair.Signature.Matches(tokens))
                {
                    result = pair;
                    return true;
                }
            }
            result = default;
            return false;
        }
    }

    private readonly CommandSignatureRegistry _level0Default = new();

    private readonly CommandSignatureRegistry _level1Fallback = new();

    public Func<MethodInfo, Signature[]>? SignatureFactory { get; set; }

    public bool AllowOveride { get; set; } = false;

    public CommandRegistry()
    {
        _level0Default = new();
        _level1Fallback = new();
    }

    public void RegisterDefault()
    {
        InternalCommands.Register(this);
    }

    private static Signature[] DefaultSignatureFactory(MethodInfo method) =>
    [
        Signature.CreateFunc(method.Name, method.GetParameters().Length - 1, true),
        Signature.CreateFunc(method.Name, method.GetParameters().Length - 1, false),
    ];

    private Signature[] CreateSignatures(MethodInfo method) => (SignatureFactory ?? DefaultSignatureFactory)(method);

    public void RegisterFallback(Command command, Signature signature)
    {
        _level1Fallback.Register(command, signature, true);
    }

    public void Register(Command command, Signature signature)
    {
        _level0Default.Register(command, signature, AllowOveride);
    }

    public void Register(MethodInfo method)
    {
        var signatures = CreateSignatures(method);
        var command = new Command(method);
        foreach (var signature in signatures)
        {
            Register(command, signature);
        }
    }

    public void Register(Delegate del) => Register(del.Method);

    public int Register(Type type)
    {
        int count = 0;
        var methods = type.GetMethods();
        foreach (var method in methods)
        {
            if (Command.Validate(method) == Command.ValidationResult.Valid)
            {
                count++;
            }
        }
        return count;
    }

    public struct Result
    {
        public readonly int Level;
        public readonly Signature Signature;
        public readonly Command Command;

        public Result(Command command, Signature signature, int level)
        {
            Command = command;
            Signature = signature;
            Level = level;
        }
    }

    public bool TryGetPair(Signature signature, [MaybeNullWhen(false)] out Result result)
    {
        if (_level0Default.TryGetPair(signature, out var pair0))
        {
            result = new Result(pair0.Command, pair0.Signature, 0);
            return true;
        }
        if (_level1Fallback.TryGetPair(signature, out var pair1))
        {
            result = new Result(pair1.Command, pair1.Signature, 1);
            return true;
        }
        result = default;
        return false;
    }

    public Result GetPair(Signature signature)
    {
        return TryGetPair(signature, out var command) ? command : throw new KeyNotFoundException();
    }

    internal bool TryGetPair(ReadOnlySpan<Token> tokens, [MaybeNullWhen(false)] out Result result)
    {
        if (_level0Default.TryGetPair(tokens, out var pair0))
        {
            result = new Result(pair0.Command, pair0.Signature, 0);
            return true;
        }
        if (_level1Fallback.TryGetPair(tokens, out var pair1))
        {
            result = new Result(pair1.Command, pair1.Signature, 1);
            return true;
        }
        result = default;
        return false;
    }

    internal Result GetPair(ReadOnlySpan<Token> tokens)
    {
        return TryGetPair(tokens, out var command) ? command : throw new KeyNotFoundException();
    }
}
