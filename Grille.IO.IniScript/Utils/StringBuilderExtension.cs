using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Utils;

internal static class StringBuilderExtension
{
    extension(StringBuilder)
    {
        public static string ToString(Action<StringBuilder> action)
        {
            var sb = new StringBuilder();
            action(sb);
            return sb.ToString();
        }
    }
}
