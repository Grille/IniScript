using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO;

public class IniScriptSection : IReadOnlyCollection<IniScriptEntry>, IReadOnlyList<IniScriptEntry>
{
    private readonly List<IniScriptEntry> _entries;

    public string Name { get; }

    internal bool WriteName { get; set; }

    public int Count => _entries.Count;

    public IniScriptEntry this[int index]
    {
        get => _entries[index];
        set => _entries[index] = value;
    }

    public IniScriptSection(string name)
    {
        Name = name;
        WriteName = true;
        _entries = new List<IniScriptEntry>();
    }

    public void Add(IniScriptEntry entry)
    {
        _entries.Add(entry);
    }

    public void Add(string key, IniScriptArg[]? args = null, int indentation = 0, string? comment = null)
    {
        var entry = new IniScriptEntry(key, args, indentation, comment);
        _entries.Add(entry);
    }

    public void Add(string key, string[] sargs, int indentation = 0, string? comment = null)
    {
        var args = new IniScriptArg[sargs.Length];
        for (int i = 0; i< args.Length; i++)
        {
            args[i] = sargs[i];
        }
        Add(key, args, indentation, comment);
    }

    public IEnumerable<IniScriptEntry> Enumerate(bool includeEmpty = false)
    {
        foreach (var entry in _entries)
        {
            if (!includeEmpty && entry.IsEmpty) continue;
            yield return entry;
        }
    }

    public IEnumerator<IniScriptEntry> GetEnumerator() => _entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _entries.GetEnumerator();
}
