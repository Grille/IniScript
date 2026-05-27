using Grille.IO.IniScript.Evaluation.Compilation;
using Grille.IO.IniScript.Utils;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Grille.IO.IniScript.Evaluation;

public sealed class CommandRegistry
{
    public readonly record struct Pair(Command Command, Signature Signature);

    private readonly List<Pair> _commands = new();

    public Func<MethodInfo, Signature[]>? SignatureFactory { get; set; }

    public bool ThrowOnOverlap { get; set; } = true;

    public CommandRegistry()
    {
        _commands = new();
    }

    public void RegisterDefault()
    {
        InternalCommands.Register(this);
    }

    public enum MethodValidationResult
    {
        Valid,
        NotStatic,
        InvalidSignature,
    }

    public void Register(Command command, Signature signature)
    {
        if (command.ParameterCount != signature.ParameterCount)
        {
            throw new ArgumentException();
        }
        var pair = new Pair(command, signature);
        int index = FindOverlapIndex(signature);
        if (index != -1)
        {
            if (ThrowOnOverlap)
            {
                throw new InvalidOperationException($"A command with an overlapping signature is already registered.");
            }
            else
            {
                _commands[index] = pair;
            }
        }
        else
        {
            _commands.Add(pair);
        }
    }

    private static Signature[] DefaultSignatureFactory(MethodInfo method) =>
    [
        Signature.CreateFunc(method.Name, method.GetParameters().Length - 1, true),
        Signature.CreateFunc(method.Name, method.GetParameters().Length - 1, false),
    ];

    public MethodValidationResult TryRegister(MethodInfo method)
    {
        if (!method.IsStatic)
        {
            return MethodValidationResult.NotStatic;
        }

        var parameters = method.GetParameters();
        if (parameters.Length == 0 || parameters[0].ParameterType != typeof(Runtime))
        {
            return MethodValidationResult.InvalidSignature;
        }

        var factory = SignatureFactory != null ? SignatureFactory : DefaultSignatureFactory;
        var signatures = factory(method);
        var command = new Command(method);
        foreach (var signature in signatures)
        {
            Register(command, signature);
        }

        return MethodValidationResult.Valid;
    }

    public MethodValidationResult TryRegister(Delegate del) => TryRegister(del.Method);

    public void Register(MethodInfo method)
    {
        var result = TryRegister(method);
        if (result != MethodValidationResult.Valid)
        {
            throw new InvalidOperationException($"The method '{method.Name}' is not a valid command method. Reason: {result}");
        }
    }

    public void Register(Delegate del) => Register(del.Method);

    public int Register(Type type)
    {
        int count = 0;
        var methods = type.GetMethods();
        foreach (var method in methods)
        {
            if (TryRegister(method) == MethodValidationResult.Valid)
            {
                count++;
            }
        }
        return count;
    }

    private int FindOverlapIndex(Signature signature)
    {
        for (int i = 0; i < _commands.Count; i++)
        {
            var pair = _commands[i];
            if (signature.Overlaps(pair.Signature))
            {
                return i;
            }
        }
        return -1;
    }

    public Pair? FindOverlap(Signature signature)
    {
        int index = FindOverlapIndex(signature);
        return index != -1 ? _commands[index] : null;
    }

    public bool ContainsOverlap(Signature signature) => FindOverlap(signature).HasValue;

    internal bool TryGetPair(ReadOnlySpan<Token> tokens, [MaybeNullWhen(false)] out Pair result)
    {
        foreach (var pair in _commands)
        {
            if (pair.Signature.Matches(tokens))
            {
                result = pair;
                return true;
            }
        }
        result = new Pair(null!, null!);
        return false;
    }

    internal Pair GetPair(ReadOnlySpan<Token> tokens)
    {
        if (TryGetPair(tokens, out var command))
        {
            return command;
        }
        throw new KeyNotFoundException();
    }
}
