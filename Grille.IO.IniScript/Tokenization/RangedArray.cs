using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Tokenization;

internal class RangedArray<T>
{
    private readonly T[] _items;
    private readonly Range[] _ranges;

    public RangedArray(T[] items, Range[] ranges)
    {
        _items = items;
        _ranges = ranges;
    }

    public ReadOnlySpan<T> Items => _items;

    public ReadOnlySpan<Range> Ranges => _ranges;

    public ReadOnlySpan<T> this[int index] => RangeAsSpan(index);

    public ReadOnlySpan<T> this[Index index] => RangeAsSpan(index);

    public int Length => _ranges.Length;

    private ReadOnlySpan<T> RangeAsSpan(Index index)
    {
        var range = _ranges[index];
        return _items.AsSpan(range);
    }
}
