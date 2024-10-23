using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Grille.IO.IniScript.Evaluation;

#pragma warning disable CS8500

internal unsafe static class InternalCommands
{
    public static void Call(Runtime runtime, Argument name)
    {
        runtime.Call(name);
    }

    public static void Call(Runtime runtime, Argument[] args)
    {
        runtime.Call(args[0]);
    }

    public static void Var(Runtime runtime, Argument key, Argument value)
    {
        runtime.SetValue(key, value);
    }

    public static void Push(Runtime runtime, Argument arg)
    {
        runtime.ValueStack.Push(arg);
    }

    public static void Push(Runtime runtime, Argument[] args)
    {
        for (int i = 1; i < args.Length; i++)
        {
            Push(runtime, args[i]);
        }
    }

    public static void Pop(Runtime runtime, Argument arg)
    {
        runtime.SetValue(arg, new Argument(runtime.ValueStack.Pop()));
    }

    public static void Pop(Runtime runtime, Argument[] args)
    {
        for (int i = 1; i < args.Length; i++)
        {
            Pop(runtime, args[i]);
        }
    }

    public static void Return(Runtime runtime, Argument[] args)
    {

    }
}
