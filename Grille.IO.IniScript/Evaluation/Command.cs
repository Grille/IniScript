using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Evaluation;

public class Command
{
    private readonly Func<Runtime, object?[], object?> _func;
    private readonly Type[] _parameterTypes;

    public Command(MethodInfo method)
    {
        (_func, _parameterTypes, ReturnType) = CreateFunc(method);
    }

    public ReadOnlySpan<Type> ParameterTypes => _parameterTypes;

    public int ParameterCount => _parameterTypes.Length; 

    public Type ReturnType { get; }

    public bool Returns => ReturnType != typeof(void);

    public object? Invoke(Runtime runtime, object?[] args)
    {
        return _func(runtime, args);
    }

    static (Func<Runtime, object?[], object?>, Type[], Type) CreateFunc(MethodInfo method)
    {
        var runtimeParam = Expression.Parameter(typeof(Runtime), "runtime");
        var argsParam = Expression.Parameter(typeof(object?[]), "args");

        var parameters = method.GetParameters();
        var arguments = new Expression[parameters.Length];
        var argsTypes = new Type[parameters.Length - 1];

        arguments[0] = runtimeParam;

        for (int i = 1; i < parameters.Length; i++)
        {
            var type = parameters[i].ParameterType;
            argsTypes[i - 1] = type;
            var index = Expression.Constant(i);
            var access = Expression.ArrayIndex(argsParam, index);
            arguments[i] = Expression.Convert(access, type);
        }

        var call = Expression.Call(method, arguments);
        Expression body;

        if (method.ReturnType == typeof(void))
        {
            body = Expression.Block(
                call,
                Expression.Constant(null, typeof(object))
            );
        }
        else
        {
            body = Expression.Convert(call, typeof(object));
        }

        var lambda = Expression.Lambda<Func<Runtime, object?[], object?>>(body, runtimeParam, argsParam);

        return (lambda.Compile(), argsTypes, method.ReturnType);
    }
}
