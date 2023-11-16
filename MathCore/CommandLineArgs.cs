namespace MathCore;

/// <summary>Парсер аргументов командной строки</summary>
public class CommandLineArgs
{
    /// <summary>Массив аргументов командной строки</summary>
    private readonly string[] _ArgsSrc;

    /// <summary>Первые аргументы до ключей</summary>
    private readonly (int Pos, int Count) _StartArgs;

    /// <summary>Свободные аргументы в конце строки</summary>
    private readonly (int Pos, int Count) _EndArgs;

    /// <summary>Список ключей со значениями</summary>
    private readonly List<KeyValuePair<string, (int Pos, int Count)>> _Args = new();

    /// <summary>Получение значений указанного ключа</summary>
    /// <param name="key">Требуемый ключ</param>
    /// <returns>Перечисление значений для указанного ключа</returns>
    public IEnumerable<string> this[string key] => _Args
       .Where(v => v.Key == key)
       .SelectMany(v => EnumValues(v.Value));

    /// <summary>Первые аргументы до ключей</summary>
    public IEnumerable<string> StartArgs => EnumValues(_StartArgs);
    /// <summary>Свободные аргументы в конце строки</summary>
    public IEnumerable<string> EndArgs => EnumValues(_EndArgs);
    /// <summary>Свободные аргументы</summary>
    public IEnumerable<string> FreeArgs => StartArgs.Concat(EndArgs);

    /// <summary>Перечисление значений в указателе памяти в массиве элементов</summary>
    /// <param name="values">Указатель на элемент массива элементов</param>
    /// <returns>Перечисление значений по указателю в массиве элементов</returns>
    private IEnumerable<string> EnumValues((int Pos, int Count) values)
    {
        for (var i = 0; i < values.Count; i++)
            yield return _ArgsSrc[values.Pos + i];
    }
        
    /// <summary>Перечисление ключей со значениями</summary>
    public IEnumerable<KeyValuePair<string, string>> KeyValues =>
        _Args.SelectMany(v => EnumValues(v.Value), (v, s) => new KeyValuePair<string, string>(v.Key, s));

    /// <summary>Список элементов командой строки</summary>
    public IReadOnlyList<string> Args => _ArgsSrc;

    /// <summary>Объединение элементов массива в случае наличия кавычек</summary>
    /// <param name="args">Массив элементов командной строки</param>
    /// <returns>Массив элементов командной строки с объединением значений в кавычках</returns>
    private static string[] CheckArgs(string[] args)
    {
        var corrected = false;
        var count     = args.Length;
        for (var i = 0; i < count; i++)
            //if (args[i] is { Length: > 2 } arg1 && arg1[0] is '"' or '\'')
            if (args[i] is [('"' or '\'') and var a0, _, ..])
            {
                var k = i + 1;
                while (k < count)
                {
                    if (args[k] is [var a2, .., var an])
                        if (a2 == a0)
                            break;
                        else if (an == a0)
                        {
                            corrected = true;
                            args[i]   = string.Join(" ", EnumStrs(args, i, k + 1))[1..^1];
                            static IEnumerable<string> EnumStrs(string[] str, int Min, int Max)
                            {
                                for (var i = Min; i < Max; i++)
                                    yield return str[i];
                            }

                            for (var j = i + 1; j <= k; j++)
                                args[j] = null;
                            i += k;
                            break;
                        }
                    k++;
                }
            }

        return corrected ? args.Where(arg => arg is { Length: > 0 }).ToArray() : args;
    }

    /// <summary>Инициализация нового экземпляра парсера аргументов командной строки</summary>
    /// <param name="args">Массив элементов командной строки</param>
    public CommandLineArgs(string[] args)
    {
        _ArgsSrc = CheckArgs(args);
        var count = _ArgsSrc.Length;

        var i = 0;
        for (; i < count; i++)
            if (_ArgsSrc[i] is ['-' or '/', ..])
            {
                _StartArgs = (0, i);
                break;
            }

        _Args.Add(new("", _StartArgs));

        for (; i < count; i++)
        {
            var arg = _ArgsSrc[i];
            if (arg == "--") break;

            var arg_name   = arg.TrimStart('-');
            var prefix_len = arg.Length - arg_name.Length;

            (int Pos, int Count) param_args = default;
            for (var j = i + 1; j < _ArgsSrc.Length; j++)
                if (_ArgsSrc[j] is ['-' or '/', ..])
                {
                    param_args = (i + 1, j - i - 1); //_ArgsSrc.AsMemory((i + 1)..j);
                    break;
                }

            if (prefix_len == 1 && arg_name.Length > 1)
                foreach (var c in arg_name)
                    _Args.Add(new(new(c, 1), param_args));
            else
                _Args.Add(new(arg_name, param_args));

            i += param_args.Count;
        }

        if (i == count) return;
        _EndArgs = (i + 1, _ArgsSrc.Length - i - 1); //_ArgsSrc.AsMemory((i + 1)..);
        _Args.Add(new("--", _EndArgs));
    }

    /// <summary>Проверка на наличие ключа в строке</summary>
    /// <param name="Key">Проверяемый ключ</param>
    /// <returns>Истина, если проверяемый ключ присутствует в строке</returns>
    public bool ContainsKey(string Key) => _Args.Any(v => v.Key == Key);

    /// <summary>Получение всех значений указанного ключа</summary>
    /// <param name="key">Проверяемый ключ</param>
    /// <returns>Перечисление значений каждого встреченного значения указанного ключа</returns>
    public IEnumerable<IEnumerable<string>> GetKeyValues(string key) =>
        _Args.Where(v => v.Key == key).Select(v => EnumValues(v.Value));

    /// <summary>Число включений указанного ключа в строку</summary>
    /// <param name="key">Проверяемый ключ</param>
    /// <returns>Число раз, сколько ключ присутствует в строке</returns>
    public int KeysCount(string key) => _Args.Count(v => v.Key == key);

    /// <summary>Выполнить действие для каждого значения указанного ключа</summary>
    /// <param name="key">Проверяемый ключ</param>
    /// <param name="action">Выполняемое действие</param>
    /// <returns>Истина, если действие было выполнено хотя бы раз</returns>
    public bool ExecForKey(string key, Action<string> action)
    {
        var any = false;
        foreach (var value in this[key])
        {
            action(value);
            any = true;
        }

        return any;
    }
}