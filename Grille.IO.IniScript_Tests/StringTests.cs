using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grille.ConsoleTestLib.Asserts;

namespace Grille.IO.IniScript_Tests;

static class StringTests
{
    public static void Run()
    {
        Section("Strings");

        TestS("A", "\"A\"");
        TestD("\"A\"", "A");

        TestS("\\", "\"\\\\\"");
        TestD("\"\\\\\"", "\\");

        TestS("\"", "\"\\\"\"");
        TestD("\"\\\"\"", "\"");

        TestS("\t", "\"\\t\"");
        TestD("\"\\t\"", "\t");
    }

    static void TestS(string src, string dst)
    {
        Test($"S {src} -> {dst}", () =>
        {
            var result = StringSerializer.Serialize(src);
            Assert.IsEqual(dst, result);
        });
    }

    static void TestD(string src, string dst)
    {
        Test($"D {src} -> {dst}", () =>
        {
            var result = StringSerializer.Deserialize(src);
            Assert.IsEqual(dst, result);
        });
    }
}
