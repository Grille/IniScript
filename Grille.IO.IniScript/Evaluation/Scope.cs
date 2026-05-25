using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Evaluation;

public sealed class Scope
{
    private readonly Dictionary<string, object> _dict;

    public string Name { get; }

    public Scope? Parent { get; }

    public int InstructionPointer { get; set; }

    [MemberNotNullWhen(false, nameof(Parent))]
    public bool IsRoot => Parent == null;

    private string? _fullName;

    public string FullName { 
        get
        {
            if (_fullName != null) return _fullName;
            if (IsRoot || Parent.IsRoot) return Name;
            return $"{Parent.FullName}.{Name}";
        }
    }

    internal Scope(Scope? parent, string name)
    {
        Parent = parent;
        Name = name;

        _dict = new();
    }

    public bool TryGetValue(string key, out object value)
    {
        if (_dict.TryGetValue(key, out value!)) return true;
        if (Parent != null) return Parent.TryGetValue(key, out value);
        return false;
    }

    public bool ContainsKey(string key) => TryGetValue(key, out var _);

    private object Get(string key)
    {
        if (TryGetValue(key, out var value)) return value;
        throw new KeyNotFoundException();
    }
    private void Set(string key, object value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _dict[key] = value;
    }

    public object this[string key]
    {
        get => Get(key);
        set => Set(key, value);
    }
}
