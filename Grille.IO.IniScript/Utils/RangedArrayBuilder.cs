using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Utils;

internal class RangedArrayBuilder<T>
{
    private readonly List<T> _items = new();

    private readonly List<Range> _ranges = new();

    public int Start { get; private set; }

    public void Add(T value) => _items.Add(value);

    public int YieldRange()
    {
        if (Start == _items.Count) return -1;
        var range = new Range(Start, _items.Count - 1);
        Start = _items.Count;
        _ranges.Add(range);
        return _ranges.Count - 1;
    }

    public RangedArray<T> ToArray() => new RangedArray<T>(_items.ToArray(), _ranges.ToArray());
}
