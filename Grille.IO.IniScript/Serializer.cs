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

    /*
    public void Serialize(Stream stream, ScriptCreationObject script)
    {
        using var writer = new StreamWriter(stream, leaveOpen: true);
        Serialize(writer, script);
    }

    public void Serialize(TextWriter writer, ScriptCreationObject script)
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

    public void Serialize(TextWriter writer, InstructionInfo entry)
    {
    */
        /*
        for (int i = 0; i < entry.Location.Indentation; i++)
        {
            writer.Write(' ');
        }

        if (!entry.IsEmpty)
        {
            writer.Write(entry.Key);

            for (var i = 0; i < entry.Arguments.Length; i++)
            {
                var arg = entry.Arguments[i];
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

        if (entry.Comments != null)
        {
            writer.Write(entry.Comments);
        }

        writer.WriteLine();
        */
    //}
}
