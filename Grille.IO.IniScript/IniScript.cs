using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO;

public class IniScript : IReadOnlyCollection<IniScriptSection>
{
    private readonly Dictionary<string, IniScriptSection> _sections;

    public IniScriptSection DefaultSection { get; }

    public IniScriptSection ActiveSection { get; private set; }

    public int Count => _sections.Count + 1;

    public IniScriptSection this[string key]
    {
        get => _sections[key];
        set => _sections[key] = value;
    }

    public IniScript()
    {
        DefaultSection = new IniScriptSection("Default") { WriteName = false };
        ActiveSection = DefaultSection;
        _sections = new Dictionary<string, IniScriptSection>();
    }

    public IniScriptSection Section(string name)
    {
        if (_sections.TryGetValue(name, out var section))
        {
            ActiveSection = section;
        }
        else
        {
            ActiveSection = _sections[name] = new IniScriptSection(name);
        }
        return ActiveSection;
    }

    public IEnumerable<IniScriptSection> EnumerateSections(bool includeEmpty = false)
    {
        yield return DefaultSection;
        foreach (var pair in _sections)
        {
            yield return pair.Value;
        }
    }

    public IEnumerator<IniScriptSection> GetEnumerator()
    {
        return EnumerateSections().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
