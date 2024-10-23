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
using Grille.IO.IniScript.Evaluation.Expressions;

using static System.Collections.Specialized.BitVector32;

namespace Grille.IO.IniScript.Evaluation;

public class Runtime : IDisposable
{
    readonly StringBuilder _sb;

    ExpressionParser ExpressionParser { get; set; }

    readonly Dictionary<string, CompiledFunction> _functions;

    public Compiler? Compiler { get; }

    public Stack<Scope> CallStack { get; }

    public Stack<Argument> ValueStack { get; }

    public Scope RootScope { get; }

    public Scope PeakScope => CallStack.Peek();

    public Runtime() : this(null) { }

    public Runtime(Compiler? compiler)
    {
        _functions = new Dictionary<string, CompiledFunction>();

        _sb = new StringBuilder();

        ExpressionParser = new ExpressionParser();

        Compiler = compiler;

        CallStack = new Stack<Scope>();
        ValueStack = new Stack<Argument>();

        RootScope = new Scope(null, "Root");
        CallStack.Push(RootScope);
    }

    public void Clear()
    {
        _functions.Clear();
        CallStack.Clear();
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

    void IncrementScope(string name)
    {
        var scope = new Scope(PeakScope, name);
        CallStack.Push(scope);

    }

    void DecrementScope()
    {
        if (CallStack.Count == 1)
        {
            throw new InvalidOperationException();
        }
        CallStack.Pop();
    }

    public Argument GetValue(string key)
    {
        /*
        if (key[0] == '$')
        {
            return PeakScope.GetVariable(key);
        }
        else if (key[0] == '%')
        {
            _sb.Clear();
            var tokens = ExpressionParser.Tokenize(key.AsSpan(1), false);
            foreach (var token in tokens)
            {
                if (token.Type == ExpressionParser.TokenType.Literal)
                {
                    _sb.Append(token.Text);
                }
                else if (token.Type == ExpressionParser.TokenType.UntokenizedExpression)
                {
                    _sb.Append(GetValue(token.Text));
                }
            }
        }
        */
        return key;
    }

    public void SetValue(string key, Argument value)
    {
        if (key[0] != '$')
        {
            throw new InvalidOperationException();
        }

        PeakScope.SetVariable(key, value);
    }

    public virtual Argument EvalArgument(string arg)
    {
        if (arg.StartsWith("$"))
        {
            //return EvalArgument(PeakScope.GetVariable(value));
        }
        return new Argument();
    }

    public void Dispose()
    {
        Clear();
    }
}
