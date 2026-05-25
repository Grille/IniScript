using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Grille.IO.IniScript.Evaluation.Compilation;

using static System.Collections.Specialized.BitVector32;

namespace Grille.IO.IniScript.Evaluation;

public sealed class Runtime : IDisposable
{
    readonly StringBuilder _sb;

    readonly Dictionary<string, CompiledFunction> _functions;

    public ConverterRegistry Converters { get; }

    public Compiler? Compiler { get; }

    public Stack<Scope> ScopeStack { get; }

    public Stack<object> ValueStack { get; }

    public Scope RootScope { get; }

    public Scope PeakScope => ScopeStack.Peek();

    public Runtime() : this(null) { }

    public Runtime(Compiler? compiler)
    {
        _functions = new Dictionary<string, CompiledFunction>();

        _sb = new StringBuilder();

        Compiler = compiler;

        Converters = new ConverterRegistry();
        ScopeStack = new Stack<Scope>();
        ValueStack = new Stack<object>();

        RootScope = new Scope(null, "<Root>");
        ScopeStack.Push(RootScope);
    }

    public void Clear()
    {
        _functions.Clear();
        ScopeStack.Clear();
        ValueStack.Clear();
    }

    public void Return()
    {

    }

    public void Import(string script)
    {
        
    }

    public void Import(CompiledScript script)
    {
        foreach (var func in script.Functions)
        {
            _functions[func.Key] = func.Value;
        }
    }

    public void Call(string name)
    {
        var func = _functions[name];
        Call(func);
    }

    public void Call(CompiledFunction function)
    {
        IncrementScope(function.Name);
        function.Invoke(this);
        DecrementScope();
    }

    internal object CastArgument(Argument arg, Type type)
    {
        if (arg.ConstValue != null)
        {
            return Converters.Cast(arg.ConstValue, type);
        }

        return null;
    }

    void IncrementScope(string name)
    {
        var scope = new Scope(PeakScope, name);
        ScopeStack.Push(scope);

    }

    void DecrementScope()
    {
        if (ScopeStack.Count == 1)
        {
            throw new InvalidOperationException();
        }
        ScopeStack.Pop();
    }

    public void Dispose()
    {
        Clear();
    }
}
