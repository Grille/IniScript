using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grille.IO.IniScript.Utils;

namespace Grille.IO.IniScript;

public class Serializer
{
    public bool FunctionStyleEnabled = true;

    public void Serialize(Stream stream, Script script)
    {
        using var writer = new StreamWriter(stream, leaveOpen: true);
        Serialize(writer, script);
    }

    public void Serialize(TextWriter writer, Script script)
    {
        foreach (var section in script)
        {
            Serialize(writer, section);
        }
    }

    private void Serialize(TextWriter writer, Function section)
    {
        if (section.WriteName)
        {
            writer.Write('[');
            writer.Write(section.Name);
            writer.Write(']');
            writer.WriteLine();
        }

        foreach (var entry in section)
        {
            Serialize(writer, entry);
        }
    }

    public void Serialize(TextWriter writer, Instruction entry)
    {
        for (int i = 0; i < entry.Indentation; i++)
        {
            writer.Write(' ');
        }

        if (!entry.IsEmpty)
        {
            writer.Write(entry.Key);

            for (var i = 0; i < entry.Args.Length; i++)
            {
                var arg = entry.Args[i];
                writer.Write(' ');
                if (StringSerializer.IsStringifyNecessary(arg))
                {
                    writer.Write(StringSerializer.Serialize(arg));
                }
                else
                {
                    writer.Write(arg);
                }
            }
        }

        if (entry.Comment != null)
        {
            writer.Write(entry.Comment);
        }

        writer.WriteLine();
    }
}
