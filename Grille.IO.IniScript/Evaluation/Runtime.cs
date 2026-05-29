using Grille.IO.IniScript.Compilation;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Grille.IO.IniScript.Evaluation;

public sealed class Runtime : IDisposable
{
    public IniAssembly Assembly { get; }

    public ConverterRegistry Converters { get; }

    public Compiler? Compiler { get; }

    public Stack<Scope> ScopeStack { get; }

    public Stack<object> ValueStack { get; }

    public Scope RootScope { get; }

    public Scope PeakScope => ScopeStack.Peek();

    public Identifier Namespace { get; set; }

    public Runtime(IniAssembly assembly, Compiler? compiler = null)
    {
        Namespace = Identifier.Empty;
        Assembly = assembly;
        Compiler = compiler;

        Converters = new ConverterRegistry();
        ScopeStack = new Stack<Scope>();
        ValueStack = new Stack<object>();

        RootScope = new Scope(null, "<Root>");
        ScopeStack.Push(RootScope);
    }

    public void Clear()
    {
        ScopeStack.Clear();
        ValueStack.Clear();
    }

    public void Return()
    {

    }

    public void Call(Identifier function)
    {
        IncrementScope(function.Name);
        //function.Invoke(this);
        DecrementScope();
    }

    internal object CastArgument(Parameter arg, Type type)
    {
        if (arg.Value != null)
        {
            return Converters.Cast(arg.Value, type);
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
