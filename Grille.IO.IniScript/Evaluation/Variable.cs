using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Evaluation;

public class Variable
{
    private readonly Runtime _runtime;

    public string Key { get; }

    public object Value { 
        get => _runtime.PeakScope[Key];
        set => _runtime.PeakScope[Key] = value;
    }

    internal Variable(Runtime runtime, string key)
    {
        _runtime = runtime;
        Key = key;
    }

    public T As<T>()
    {
        var type = typeof(T);
        return default!;
    }
}
