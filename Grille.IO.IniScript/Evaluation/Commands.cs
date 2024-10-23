using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Evaluation;

public sealed class Commands
{
    public Action<Runtime, string, Argument[]>? GenericAction { get; set; }

    Dictionary<string, Action<Runtime>> _dict0;
    Dictionary<string, Action<Runtime, Argument>> _dict1;
    Dictionary<string, Action<Runtime, Argument, Argument>> _dict2;
    Dictionary<string, Action<Runtime, Argument, Argument, Argument>> _dict3;
    Dictionary<string, Action<Runtime, Argument, Argument, Argument, Argument>> _dict4;
    Dictionary<string, Action<Runtime, Argument[]>> _dictx;

    public Commands()
    {
        _dict0 = new();
        _dict1 = new();
        _dict2 = new();
        _dict3 = new();
        _dict4 = new();
        _dictx = new();

        Register("Call", (Action<Runtime, Argument>)InternalCommands.Call);
        Register("Call", (Action<Runtime, Argument[]>)InternalCommands.Call);

        Register("Push", (Action<Runtime, Argument>)InternalCommands.Push);
        Register("Pop", (Action<Runtime, Argument>)InternalCommands.Pop);

        Register("Var", InternalCommands.Var);

    }

    public void Register(string key, Action<Runtime> action) => _dict0[key] = action;
                                            
    public void Register(string key, Action<Runtime, Argument> action) => _dict1[key] = action;
                                            
    public void Register(string key, Action<Runtime, Argument, Argument> action) => _dict2[key] = action;
                                            
    public void Register(string key, Action<Runtime, Argument, Argument, Argument> action) => _dict3[key] = action;
                                            
    public void Register(string key, Action<Runtime, Argument, Argument, Argument, Argument> action) => _dict4[key] = action;
                                            
    public void Register(string key, Action<Runtime, Argument[]> action) => _dictx[key] = action;

    public Action<Runtime> GetAction(Instruction entry)
    {
        var key = entry.Key;
        var args = entry.Args!;

        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        bool TryGetValue<TAction>(Dictionary<string, TAction> dict, int i, out TAction action)
        {
            var result = dict.TryGetValue(key, out action!);
            return entry.ArgsLength == i && result;
        }

        if (TryGetValue(_dict0, 0, out var action0))
        {
            return action0;
        }
        if (TryGetValue(_dict1, 1, out var action1))
        {
            return (ctx) => action1(ctx, args[0]);
        }
        if (TryGetValue(_dict2, 2, out var action2))
        {
            return (ctx) => action2(ctx, args[0], args[1]);
        }
        if (TryGetValue(_dict3, 3, out var action3))
        {
            return (ctx) => action3(ctx, args[0], args[1], args[2]);
        }
        if (TryGetValue(_dict4, 4, out var action4))
        {
            return (ctx) => action4(ctx, args[0], args[1], args[2], args[3]);
        }


        if (_dictx.TryGetValue(key, out var actionx))
        {
            return (ctx) => actionx(ctx, args);
        }

        if (GenericAction != null)
        {
            var actiong = GenericAction;
            return (ctx) => actiong(ctx, key, args);
        }

        throw new KeyNotFoundException(key);
    }

}
