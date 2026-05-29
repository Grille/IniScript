using Grille.IO.IniScript.Evaluation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Compilation;

public class Command
{
    public enum ValidationResult
    {
        Valid,
        NotStatic,
        MissingArg0Runtime,
    }

    private readonly Func<Runtime, object[], object?> _func;
    private readonly Type[] _parameterTypes;

    public Command(MethodInfo method)
    {
        AssertValid(method);
        (_func, _parameterTypes, ReturnType) = CreateFunc(method);
    }

    public ReadOnlySpan<Type> ParameterTypes => _parameterTypes;

    public int ParameterCount => _parameterTypes.Length; 

    public Type ReturnType { get; }

    public bool Returns => ReturnType != typeof(void);

    public object? Invoke(Runtime runtime, object[] args)
    {
        return _func(runtime, args);
    }

    static (Func<Runtime, object[], object?>, Type[], Type) CreateFunc(MethodInfo method)
    {
        var runtimeParam = Expression.Parameter(typeof(Runtime), "runtime");
        var argsParam = Expression.Parameter(typeof(object[]), "args");

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

    private static void AssertValid(MethodInfo method)
    {
        var code = Validate(method);
        if (code != ValidationResult.Valid)
        {
            throw new InvalidOperationException($"{code}");
        }
    }

    public static ValidationResult Validate(MethodInfo method)
    {
        if (!method.IsStatic)
        {
            return ValidationResult.NotStatic;
        }
        var parameters = method.GetParameters();
        if (parameters.Length == 0 || parameters[0].ParameterType != typeof(Runtime))
        {
            return ValidationResult.MissingArg0Runtime;
        }
        return ValidationResult.Valid;
    }

    public static ValidationResult Validate(Delegate del) => Validate(del.Method);

    public static implicit operator Command(MethodInfo method) => new Command(method);

    public static implicit operator Command(Delegate del) => new Command(del.Method);

    public static implicit operator Func<Runtime, object[], object?>(Command command) => command._func;
}
