using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Compilation;

internal class ScriptCreationObject : Dictionary<string, ScriptCreationObject.Section>
{
    public class Section : List<InstructionInfo>
    {
        public string Key { get; }

        public string? ParentKey { get; }

        public Section(string key, string? parent = null)
        {
            Key = key;
            ParentKey = parent;
        }
    }

    public const string DefaultSectionName = "<Default>";

    public ScriptCreationObject()
    {
        GetSection(DefaultSectionName);
    }

    public Section CurrentSection { get; private set; }

    [MemberNotNull(nameof(CurrentSection))]
    public Section GetSection(string name, string? parent = null)
    {
        if (TryGetValue(name, out var section))
        {
            if (section.ParentKey != parent)
            {
                throw new InvalidOperationException($"Section '{name}' already exists with a different parent.");
            }
            return CurrentSection = section;
        }
        return CurrentSection = this[name] = new(name, parent);
    }
}
