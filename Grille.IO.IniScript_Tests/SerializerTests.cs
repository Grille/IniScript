using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grille.ConsoleTestLib.Asserts;
using Grille.IO.IniScript.Utils;

namespace Grille.IO.IniScript_Tests;

static class SerializerTests
{
    public static void Run()
    {
        Section("Serializer Strings");

        TestSstr("A", "A");
        TestSstr("\'", "\\\'");
        TestSstr("\"", "\\\"");
        TestSstr("\\", "\\\\");
        TestSstr("\0", "\\0");
        TestSstr("\t", "\\t");
        TestSstr("\n", "\\n");
        TestSstr("\r", "\\r");
        TestSstr("\\A\\\\0", "\\\\A\\\\\\\\0");
        TestSstr("\\A\\\\\0", "\\\\A\\\\\\\\\\0");

        Section("Serializer Numbers");

        TestNr("0", 0, 0);
        TestNr("1", 1, 1);
        TestNr("+1", 1, 1);
        TestNr("-1", -1, -1);
        TestNr("1.0", 1, 1, false);
        TestNr("1.1", 1, 1.1, true);
        TestNr("2E1", 20, 20);
        TestNr("0x1", 1, 1);
        TestNr("0xFF", 255, 255);
        TestNr("0x10", 16, 16);
        TestNr("0b10", 2, 2);
        TestNr("9.9", 9, 9.9, true);
        TestNr("0xF.F", 15, 15.9375, true);
        TestNr("2E+1", 20, 20);
        TestNr("2E-1", 0, 0.2, true);
        TestNr("46.6532", 46, 46.6532, true);
        TestNr("46.0", 46, 46);
    }

    static void TestSstr(string raw, string text)
    {
        string str = $"\"{text}\"";
        Test($"S {text}", () =>
        {
            var result = StringSerializer.Serialize(raw);
            Assert.IsEqual(str, result);
        });
        Test($"D {text}", () =>
        {
            var result = StringSerializer.Deserialize(str);
            Assert.IsEqual(raw, result);
        });
    }

    static void TestNr(string text, long integer, double @decimal, bool isDecimal = false)
    {
        Test($"Nr {text}", () =>
        {
            var result = NumberSerializer.Deserialize(text);
            Assert.IsEqual(integer, result.Integer);
            Assert.IsEqual(@decimal, result.Decimal);
            Assert.IsEqual(isDecimal, result.IsDecimal);
        });
    }
}
