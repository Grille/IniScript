using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Evaluation;

public class Identifier
{
    public static class Cache
    {
        private static readonly Dictionary<string, Identifier> _cache = new();

        public static int Count => _cache.Count;

        public static long Size { get; private set; }

        public static void Clear()
        {
            lock (_cache)
            {
                Size = 0;
                _cache.Clear();
            }
        }

        public static Identifier Get(string? literal)
        {
            if (string.IsNullOrEmpty(literal)) return Empty;
            lock (_cache)
            {
                if (!_cache.TryGetValue(literal, out var result))
                {
                    Size += literal.Length;
                    _cache[literal] = result = Parse(literal);
                }
                return result;
            }
        }
    }

    public static readonly Identifier Empty = new(string.Empty, Array.Empty<string>());

    private readonly string[] _path;

    public ReadOnlySpan<string> Path => _path;

    public string Literal { get; }

    public string Name { get; }

    public bool IsEmpty => _path.Length == 0;

    private Identifier(string literal, string[] path)
    {
        _path = path;
        Literal = literal;
        Name = IsEmpty ? string.Empty : _path[^1];
    }

    public override string ToString() => Literal;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        else if (obj is Identifier identifier) return Literal == identifier.Literal;
        else if (obj is string str) return Literal == str;
        return base.Equals(obj);
    }

    public override int GetHashCode() => Literal.GetHashCode();

    public static Identifier Parse(string? literal)
    {
        if (string.IsNullOrEmpty(literal)) return Empty;
        if (literal.Contains(' ') || literal.Contains('\t')) throw new ArgumentException();
        var split = literal.Split('.');
        return new Identifier(literal, split);
    }

    public static implicit operator Identifier(string? fullName) => Cache.Get(fullName);

    public Identifier Join(Identifier fullName)
    {
        if (fullName.IsEmpty) return this;
        else if (IsEmpty) return fullName;
        return $"{Literal}.{fullName.Literal}";
    }

}
