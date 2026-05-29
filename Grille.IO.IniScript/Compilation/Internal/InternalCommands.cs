using Grille.IO.IniScript.Evaluation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Grille.IO.IniScript.Compilation.Internal;

file static class InternalCommandsMethods
{
    extension(Runtime runtime)
    {
        public void Call(string name)
        {
            runtime.Call(name);
        }

        public void Var( string key, object value)
        {
            runtime.PeakScope[key] = value;
        }

        public void Push(object arg)
        {
            runtime.ValueStack.Push(arg);
        }

        public void Push(Parameter[] args)
        {
            for (int i = 1; i < args.Length; i++)
            {
                Push(runtime, args[i]);
            }
        }

        public void Pop(string arg)
        {
            runtime.PeakScope[arg] = runtime.ValueStack.Pop();
        }

        public void Return(Parameter[] args)
        {

        }

        public void GetReturnValue(Parameter args)
        {

        }
    }
}

internal static class InternalCommands
{
    public static void Register(CommandRegistry registry)
    {

    }
}
