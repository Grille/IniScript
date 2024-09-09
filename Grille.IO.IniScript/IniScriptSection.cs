using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO;

public class IniScriptSection : IReadOnlyCollection<IniScriptEntry>
{
    public string Name { get; }

    public List<IniScriptEntry> Entries { get; }

    public int Count => Entries.Count;

    public IniScriptSection(string name)
    {
        Name = name;

        Entries = new List<IniScriptEntry>();
    }

    public IEnumerable<IniScriptEntry> Enumerate(bool includeEmpty = false)
    {
        foreach (var entry in Entries)
        {
            if (!includeEmpty && entry.IsEmpty) continue;
            yield return entry;
        }
    }

    public IEnumerator<IniScriptEntry> GetEnumerator() => Entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Entries.GetEnumerator();
}
