using Grille.IO.IniScript.Evaluation;
using Grille.IO.IniScript.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Compilation;

public sealed class IniAssembly
{
    private readonly Dictionary<string, IniAssembly> Assemblies = new();

    private readonly Dictionary<string, object> Objects = new();

    public IniAssembly? Parent { get; }

    public Identifier Name { get; }

    public Identifier FullName { get; }

    public IniAssembly()
    {
        Name = Identifier.Empty;
        FullName = Name;
    }

    private IniAssembly(Identifier name, IniAssembly parent)
    {
        Name = name;
        Parent = parent;
        FullName = parent.FullName.Join(Name);
    }

    public object this[string? fullName]
    {
        get => this[(Identifier)fullName];
        set => this[(Identifier)fullName] = value;
    }

    public object this[Identifier fullName]
    {
        get => GetParentAssembly(fullName).Objects[fullName.Name];
        set => GetParentAssembly(fullName).Objects[fullName.Name] = value;
    }

    public void Merge(IniAssembly assembly, string? fullName = null) => Merge(assembly, (Identifier)fullName);

    public void Merge(IniAssembly assembly, Identifier fullName) => GetAssembly(fullName).Merge(assembly);

    public void Merge(IniAssembly assembly)
    {
        foreach (var funcInfo in assembly.Objects)
        {
            Objects.Add(funcInfo.Key, funcInfo.Value);
        }
        foreach (var childInfo in assembly.Assemblies)
        {
            GetAssembly(childInfo.Key).Merge(childInfo.Value);
        }
    }

    private IniAssembly GetParentAssembly(Identifier fullName)
    {
        if (fullName.Path.Length == 0) throw new ArgumentException();
        return GetAssembly(fullName.Path.Slice(0, fullName.Path.Length - 1));
    }

    private IniAssembly GetAssembly(string? fullName = null) => GetAssembly((Identifier)fullName);

    private IniAssembly GetAssembly(Identifier fullName) => GetAssembly(fullName.Path);

    private IniAssembly GetAssembly(ReadOnlySpan<string> fullName)
    {
        if (fullName.Length == 0) return this;
        var dict = GetLocalAssembly(fullName[0]);
        if (fullName.Length == 1) return dict;
        return dict.GetAssembly(fullName.Slice(1));
    }

    private IniAssembly GetLocalAssembly(string key)
    {
        if (!Assemblies.TryGetValue(key, out var dict))
        {
            Assemblies[key] = dict = new IniAssembly(key, this);
        }
        return dict;
    }
}