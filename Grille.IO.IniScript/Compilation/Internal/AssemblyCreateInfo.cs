using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Compilation.Internal;

internal class AssemblyCreateInfo : Dictionary<string, AssemblyCreateInfo.Section>
{
    public class Section
    {
        public readonly string Key;

        public List<InstructionCreateInfo> List = new();

        public InstructionCreateInfo[] Array = System.Array.Empty<InstructionCreateInfo>();

        public string? ParentKey;

        public Parameter[]? Parameters;

        public Section(string key) => Key = key;
    }

    public static readonly string DefaultKey = "[#]";

    public AssemblyCreateInfo()
    {
        GetSection(DefaultKey);
    }

    public Section CurrentSection { get; private set; }

    [MemberNotNull(nameof(CurrentSection))]
    public Section GetSection(string name)
    {
        if (TryGetValue(name, out var section))
        {
            return CurrentSection = section;
        }
        return CurrentSection = this[name] = new(name);
    }
}
