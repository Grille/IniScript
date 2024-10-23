using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript;

public class Script : IReadOnlyCollection<Function>
{
    private readonly Dictionary<string, Function> _sections;

    public const string DefaultSectionName = "<DefaultMain>";

    public Function DefaultSection { get; }

    public Function ActiveSection { get; private set; }

    public int Count => _sections.Count + 1;

    public Function this[string key]
    {
        get => _sections[key];
        set => _sections[key] = value;
    }

    public Script()
    {
        DefaultSection = new Function(DefaultSectionName) { WriteName = false };
        ActiveSection = DefaultSection;
        _sections = new Dictionary<string, Function>();
    }

    public Function Section(string name)
    {
        if (_sections.TryGetValue(name, out var section))
        {
            ActiveSection = section;
        }
        else
        {
            ActiveSection = _sections[name] = new Function(name);
        }
        return ActiveSection;
    }

    internal void Add(Function func)
    {
        _sections.Add(func.Name, func);
        ActiveSection = func;
    }

    public bool ContainsKey(string key) => _sections.ContainsKey(key);

    public IEnumerable<Function> EnumerateSections(bool includeEmpty = false)
    {
        yield return DefaultSection;
        foreach (var pair in _sections)
        {
            yield return pair.Value;
        }
    }

    public IEnumerator<Function> GetEnumerator()
    {
        return EnumerateSections().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
