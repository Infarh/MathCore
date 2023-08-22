#nullable enable
namespace MathCore.JSON;

/// <summary>Генератор JSON-объекта по объекту-прототипу</summary>
/// <typeparam name="T">Тип объекта-прототипа</typeparam>
public sealed class JSONObjectCreator<T> : JSONObjectCreatorBase
{
    /// <summary>Объект-прототип</summary>
    private readonly T? _Obj;
    /// <summary>Список именованных методов получения JSON-объектов - полей из объекта-прототипа</summary>
    private readonly List<KeyValuePair<string, Func<T?, object?>>> _FieldsDescriptions = new();

    /// <summary>Инициализация нового генератора JSON-объектов</summary>
    /// <param name="obj"></param>
    public JSONObjectCreator(T? obj = default) => _Obj = obj;

    /// <summary>Добавить новый метод определения поля JSON-объекта из объекта-прототипа</summary>
    /// <param name="Name">Имя поля</param>
    /// <param name="Field">Метод получения JSON-объекта поля</param>
    /// <returns>Генератор JSON-объектов с добавленным методом</returns>
    public JSONObjectCreator<T> AddField(string Name, Func<T?, object> Field)
    {
        _FieldsDescriptions.Add(new(Name, Field));
        return this;
    }

    /// <summary>Создать JSON-объект из исходного объекта-прототипа</summary>
    /// <returns></returns>
    public JSONObject Create() => Create(_Obj);

    /// <summary>Создать JSON-объект из указанного объекта-прототипа</summary>
    /// <param name="obj">Объект-прототип</param>
    /// <returns>JSON-объект</returns>
    internal override JSONObject Create(object? obj) => Create((T?)obj);

    /// <summary>Создать JSON-объект из указанного объекта-прототипа</summary>
    /// <param name="obj">Объект-прототип</param>
    /// <returns>JSON-объект</returns>
    public JSONObject Create(T? obj)
    {
        var fields = new List<JSONObject>(_FieldsDescriptions.Count);
        foreach (var (key, func) in _FieldsDescriptions)
            switch (func.Invoke(obj))
            {
                case JSONObjectCreatorBase creator:
                    fields.Add(new JSONObject(key, creator.Create(obj)));
                    break;
                case string str:
                    fields.Add(new JSONObject(key, str));
                    break;
                case { } o:
                    fields.Add(new JSONObject(key, o.ToString() ?? ""));
                    break;
                default:
                    fields.Add(new JSONObject(key, ""));
                    break;
            }
        
        return new JSONObject(fields);
    }

    /// <summary>Оператор неявного приведения типа объекта-генератора к типу JSON-объекта</summary>
    /// <param name="creator">Объект-генератор</param>
    public static implicit operator JSONObject(JSONObjectCreator<T> creator) => creator.Create();
}