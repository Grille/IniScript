using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Tokenization;

internal class RangedArrayBuilder<T>
{
    private readonly List<T> _items;

    private readonly List<Range> _ranges;

    private int _rangeStart;

    public RangedArrayBuilder(int capacity = 0)
    {
        _items = new List<T>(capacity);
        _ranges = new List<Range>(capacity);
    }

    public void Add(T value) => _items.Add(value);

    public int YieldRange()
    {
        if (_rangeStart == _items.Count) return -1;
        var range = new Range(_rangeStart, _items.Count);
        _rangeStart = _items.Count;
        _ranges.Add(range);
        return _ranges.Count - 1;
    }

    public RangedArray<T> ToArray() => new RangedArray<T>(_items.ToArray(), _ranges.ToArray());
}
