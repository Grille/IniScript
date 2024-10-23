using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Evaluation;

public class Scope
{
    public string Name { get; }

    public Scope? Parent { get; }

    public int InstructionPointer { get; set; }

    Dictionary<string, Argument> _dict;

    public Scope(Scope? parent, string name)
    {
        Parent = parent;
        Name = name;

        _dict = new();
    }

    public Argument GetVariable(string key)
    {
        if (_dict.TryGetValue(key, out var value))
        {
            return value;
        }
        else if (Parent != null)
        {
            return Parent.GetVariable(key);
        }
        throw new KeyNotFoundException();
    }

    public void SetVariable(string key, Argument obj)
    {
        _dict[key] = obj;
    }
}
