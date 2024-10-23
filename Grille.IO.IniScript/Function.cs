using System;
using System.Collections;
using System.Collections.Generic;

namespace Grille.IO.IniScript;

public class Function : IReadOnlyCollection<Instruction>, IReadOnlyList<Instruction>
{
    private readonly List<Instruction> _entries;

    public string Name { get; }

    public Instruction Instruction { get; }

    internal bool WriteName { get; set; }

    public int Count => _entries.Count;

    public Instruction this[int index]
    {
        get => _entries[index];
        set => _entries[index] = value;
    }

    public Function(string name) : this(new Instruction(name, null, 0, null)) { }

    public Function(Instruction instruction)
    {
        ArgumentNullException.ThrowIfNull(instruction.Key);
        Instruction = instruction;
        Name = instruction.Key;
        WriteName = true;
        _entries = new List<Instruction>();
    }

    public bool ContainsKey(string key)
    {
        foreach (var entry in _entries)
        {
            if (entry.Key == key) return true;
        }
        return false;
    }

    public void Add(Instruction entry)
    {
        _entries.Add(entry);
    }

    public void Add(string key, Argument[]? args = null, int indentation = 0, string? comment = null)
    {
        var entry = new Instruction(key, args, indentation, comment);
        _entries.Add(entry);
    }

    public void Add(string key, string[] sargs, int indentation = 0, string? comment = null)
    {
        var args = new Argument[sargs.Length];
        for (int i = 0; i < args.Length; i++)
        {
            args[i] = sargs[i];
        }
        Add(key, args, indentation, comment);
    }

    public IEnumerable<Instruction> Enumerate(bool includeEmpty = false)
    {
        foreach (var entry in _entries)
        {
            if (!includeEmpty && entry.IsEmpty) continue;
            yield return entry;
        }
    }

    public IEnumerator<Instruction> GetEnumerator() => _entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _entries.GetEnumerator();
}
