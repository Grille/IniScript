using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Grille.IO.IniScript.Evaluation;

internal static class InternalCommands
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

        public void Push( object arg)
        {
            runtime.ValueStack.Push(arg);
        }

        public void Push(Argument[] args)
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

        public void Return(Argument[] args)
        {

        }

        public void GetReturnValue(Argument args)
        {

        }
    }
}
