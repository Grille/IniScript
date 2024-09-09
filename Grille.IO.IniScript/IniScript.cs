using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO;

public class IniScript
{
    public Dictionary<string, IniScriptSection> Sections { get; }

    public IniScriptSection DefaultSection { get; }

    public IniScriptSection ActiveSection { get; private set; }

    public IniScript()
    {
        DefaultSection = new IniScriptSection("Default");
        ActiveSection = DefaultSection;
        Sections = new Dictionary<string, IniScriptSection>();
    }

    public void Section(string name)
    {
        if (Sections.TryGetValue(name, out var section))
        {
            ActiveSection = section;
        }
        else
        {
            ActiveSection = Sections[name] = new IniScriptSection(name);
        }
    }

    public IEnumerable<IniScriptEntry> EnumerateAllSections(bool includeEmpty = false)
    {
        foreach (var pair in Sections)
        {
            foreach (var entry in pair.Value.Enumerate(includeEmpty))
            {
                yield return entry;
            }
        }
    }

    public IEnumerable<IniScriptEntry> EnumerateActiveSection(bool includeEmpty = false)
    {
        foreach (var entry in ActiveSection.Enumerate(includeEmpty))
        {
            yield return entry;
        }
    }
}
