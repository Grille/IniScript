using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Evaluation;

public sealed class ConverterRegistry
{
    private readonly Dictionary<(Type Out, Type In), Converter<object, object>> _level0Specific;

    private readonly Dictionary<Type, Converter<object, object>> _level1Generic;

    public ConverterRegistry()
    {
        _level0Specific = new();
        _level1Generic = new();
        RegsiterDefault();
    }

    private void RegsiterDefault()
    {
        Register(Convert.ToBoolean);
        Register(Convert.ToChar);
        Register(ConvertToString);

        Register(Convert.ToSByte);
        Register(Convert.ToInt16);
        Register(Convert.ToInt32);
        Register(Convert.ToInt64);

        Register(Convert.ToByte);
        Register(Convert.ToUInt16);
        Register(Convert.ToUInt32);
        Register(Convert.ToUInt64);

        Register(Convert.ToSingle);
        Register(Convert.ToDouble);
        Register(Convert.ToDecimal);

        Register(Convert.ToChar);
    }

    private static string ConvertToString(object value) => value.ToString() ?? string.Empty;

    public void Register<TInput, TOutput>(Converter<TInput, TOutput> converter) where TInput : notnull where TOutput : notnull
    {
        if (typeof(TInput) == typeof(object))
        {
            _level1Generic[typeof(TOutput)] = (input) => converter((TInput)input);
        }
        else
        {
            _level0Specific[(typeof(TOutput), typeof(TInput))] = (input) => converter((TInput)input);
        }
    }

    public void Register<TOutput>(Converter<object, TOutput> converter) where TOutput : notnull
    {
        _level1Generic[typeof(TOutput)] = (input) => converter(input);
    }

    public TOutput Cast<TOutput>(object obj)
    {
        return (TOutput)Cast(obj, typeof(TOutput));
    }

    public object Cast(object obj, Type outputType)
    {
        var inputType = obj.GetType();
        if (inputType == outputType) return obj;
        if (_level0Specific.TryGetValue((outputType, inputType), out var cast0))
        {
            return cast0(obj);
        }
        if (_level1Generic.TryGetValue(outputType, out var cast1))
        {
            return cast1(obj);
        }
        if (inputType.IsAssignableFrom(outputType))
        {
            return obj;
        }
        throw new InvalidCastException($"No converter registered from {inputType} to {outputType}.");
    }
}
