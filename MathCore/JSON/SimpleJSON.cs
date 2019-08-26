using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathCore.Annotations;

namespace MathCore.JSON
{
    /// <summary>Базовый класс генераторов JSON объектов</summary>
    public abstract class JSONObjectCreatorBase
    {
        /// <summary>Создать объект JSON</summary>
        /// <param name="obj">Объект-прототип, на основе которого генерируется JSON-объекта</param>
        /// <returns>Объект JSON</returns>
        internal abstract JSONObject Create(object obj);
    }

    internal static class JSONTest
    {
        public static JSONObjectCreator<T> GetJSON<T>(this T obj) => new JSONObjectCreator<T>(obj);
    }

    /// <summary>Генератор JSON-объекта по объекту-прототипу</summary>
    /// <typeparam name="T">ТИп объкета-прототипа</typeparam>
    public sealed class JSONObjectCreator<T> : JSONObjectCreatorBase
    {
        /// <summary>Объект-прототип</summary>
        private readonly T _Obj;
        /// <summary>Список именованных методов получения JSON-объектов - полей из объекта-прототипа</summary>
        private readonly List<KeyValuePair<string, Func<T, object>>> _FieldsDescriptions = new List<KeyValuePair<string, Func<T, object>>>();

        /// <summary>Инициализация нового генератора JSON-объектов</summary>
        /// <param name="obj"></param>
        public JSONObjectCreator(T obj = default) => _Obj = obj;

        /// <summary>Добавить новый метод определения поля JSON-объекта из объекта-прототипа</summary>
        /// <param name="Name">Имя поля</param>
        /// <param name="Field">Метод получения JSON-объекта поля</param>
        /// <returns>Генератор JSON-объектов с добавленным методом</returns>
        public JSONObjectCreator<T> AddField(string Name, Func<T, object> Field)
        {
            _FieldsDescriptions.Add(new KeyValuePair<string, Func<T, object>>(Name, Field));
            return this;
        }

        /// <summary>Создать JSON-объект из исходного объекта-прототипа</summary>
        /// <returns></returns>
        public JSONObject Create() => Create(_Obj);

        /// <summary>Создать JSON-объект из указанного объекта-прототипа</summary>
        /// <param name="obj">Объект-прототип</param>
        /// <returns>JSON-объект</returns>
        internal override JSONObject Create(object obj) => Create((T)obj);

        /// <summary>Создать JSON-объект из указанного объекта-прототипа</summary>
        /// <param name="obj">Объект-прототип</param>
        /// <returns>JSON-объект</returns>
        public JSONObject Create(T obj)
        {
            var fields = new List<JSONObject>(_FieldsDescriptions.Count);
            foreach(var (key, func) in _FieldsDescriptions)
            {
                var name = key;
                var value = func?.Invoke(obj);
                if(value is JSONObjectCreatorBase creator_base)
                    fields.Add(new JSONObject(name, creator_base.Create(obj)));
                else
                    fields.Add(new JSONObject(name, value?.ToString()));
            }
            return new JSONObject(fields);
        }

        /// <summary>Оператор неявного приведения типа объекта-генератора к типу JSON-объекта</summary>
        /// <param name="creator">Объект-генератор</param>
        public static implicit operator JSONObject(JSONObjectCreator<T> creator) => creator.Create();
    }

    /// <summary>Парсер структуры JSON</summary>
    public sealed class JSONObject : IEnumerable<JSONObject>
    {
        internal class TextOBJ
        {
            public string NAME { get; set; }
            public bool VALUE { get; set; }
        }

        private class TestObj
        {
            public string Name { get; set; }
            public int Value { get; set; }
            public TextOBJ Field { get; set; }
        }

        internal static void Test()
        {
            var test = new TestObj
            {
                Name = "Name",
                Value = 123,
                Field = new TextOBJ
                {
                    NAME = "ASD",
                    VALUE = true
                }
            };



            var json = test.GetJSON()
                .AddField("Name", o => o.Name)
                .AddField("Value", o => o.Value)
                .AddField("Field", o => o.Field.GetJSON()
                    .AddField("NAME", f => f.NAME)
                    .AddField("VALUE", f => f.VALUE))
                .Create();

            Console.WriteLine(json);
        }

        /// <summary>Имя структуры</summary>
        private readonly string _Name;
        /// <summary>Значение структуры</summary>
        private readonly string _Data;
        /// <summary>Словарь полей структуры по ключу-имени поля</summary>
        private readonly JSONObject[] _Fields;

        /// <summary>Имя структуры</summary>
        public string Name => _Name;
        /// <summary>Значение структуры</summary>
        public string Data => _Data;
        /// <summary>Признак того, что структура является сложной - со вложенными полями</summary>
        public bool IsComplex => _Fields != null && _Fields.Length > 0;

        /// <summary>Перечисление имён полей структуры</summary>
        public IEnumerable<string> Fields => _Fields?.Select(f => f.Name) ?? Enumerable.Empty<string>();

        /// <summary>Перечисление полей структуры по указанному имени</summary>
        /// <param name="Field">Имя поля структуры</param>
        /// <returns>Вложенные поля с указаным именем</returns>
        public IEnumerable<JSONObject> this[string Field] => _Fields?.Where(f => string.Equals(Field, f._Name)) ?? Enumerable.Empty<JSONObject>();

        #region Конструкторы

        public JSONObject(params JSONObject[] fields) : this((IEnumerable<JSONObject>)fields) { }

        public JSONObject(IEnumerable<JSONObject> Fields)
        {
            var data = new StringBuilder();
            var fields = Fields.ForeachLazy(f => data.AppendLine($"\"{f._Name}\" : {f._Data}")).ToArray();
            if(fields.Length > 0) _Fields = fields;
        }

        /// <summary>Инициализация новой структуры JSON из строкового представления</summary>
        /// <param name="str">Строковое представление структуры JSON</param>
        public JSONObject([NotNull] string str) : this(null, str) { }

        /// <summary>Инициализация новой структуры JSON из строкового представления с указанием имени</summary>
        /// <param name="Name">Имя структуры</param>
        /// <param name="str">Строковое представление структуры JSON</param>
        public JSONObject([NotNull] string Name, [NotNull] string str)
        {
            _Name = Name;
            _Data = str.Trim();
            if(_Data.Length <= 1 || _Data[0] != '{') return;
            var fields = new List<JSONObject>();
            str = str.Trim('{', '}', ' ');
            var len = str.Length;
            var pos = 0;
            while(pos < len)
            {
                var name = GetText(str, ref pos, "\"", "\"");
                while(pos < len && (char.IsSeparator(str, pos) || str[pos] == ':')) pos++;
                var body = str[pos] == '{'
                    ? $"{{{GetText(str, ref pos, "{", "}")}}}"
                    : GetText(str, ref pos, "\"", "\"");
                fields.Add(new JSONObject(name, body));
                while(pos < len && (char.IsSeparator(str, pos) || str[pos] == ',')) pos++;
            }
            if(fields.Count > 0) _Fields = fields.ToArray();
        }

        #endregion


        /// <summary>Преобразование значения структуры к целому числу</summary>
        /// <param name="Default">Значение по умолчанию</param>
        /// <returns>Целочисленное значение структуры, либо значение по умолчанию, если преобразование невозможно</returns>
        public int? ToInt(int? Default = null) => int.TryParse(_Data, out var v) ? v : Default;
        /// <summary>Преобразование значения структуры к вещественному числу</summary>
        /// <param name="Default">Значение по умолчанию</param>
        /// <returns>Вещественное значение структуры, либо значение по умолчанию, если преобразование невозможно</returns>
        public double? ToDouble(double? Default = null) => double.TryParse(_Data, out var v) ? v : Default;
        /// <summary>Преобразование значения структуры к логическому значению</summary>
        /// <param name="Default">Значение по умолчанию</param>
        /// <returns>Логическое значение структуры, либо значение по умолчанию, если преобразование невозможно</returns>
        public bool? ToBool(bool? Default = null) => bool.TryParse(_Data, out var v) ? v : Default;

        /// <summary>Строковое представление структуры</summary>
        /// <returns>Строковое представление структуры</returns>
        public override string ToString() => _Name.IsNullOrWhiteSpace() ? _Data : $"{_Name} : {_Data}";

        /// <summary>Оператор неявного приведения структуры JSON к строковому значению</summary>
        /// <param name="json">Структура JSON</param>
        public static implicit operator string(JSONObject json) => json._Data;

        /// <summary>
        /// Выделение подстроки, ограниченной шаблоном начала и шаблоном окончания строки начиная с указанного смещения
        /// </summary>
        /// <param name="Str">Входная строка</param>
        /// <param name="Offset">
        /// Смещение во входной строке начала поиска - в конце работы метода соответствует месту окончания поиска
        /// </param>
        /// <param name="Open">Шаблон начала подстроки</param>
        /// <param name="Close">Шаблон окончания подстроки</param>
        /// <returns>Подстрока, заключённая между указанными шаблонами начала и окончания</returns>
        /// <exception cref="FormatException">
        /// Если шаблон завершения строки на нейден, либо если количество шаблонов начала строки превышает 
        /// количество шаблонов окончания во входной строке
        /// </exception>
        public static string GetText(string Str, ref int Offset, string Open, string Close)
        {
            var Start = Str.IndexOf(Open, Offset, StringComparison.Ordinal);
            if(Start == -1) return null;
            var Stop = Str.IndexOf(Close, Start + 1, StringComparison.Ordinal);
            if(Stop == -1) throw new FormatException();
            var start = Start;
            do
            {
                start = Str.IndexOf(Open, start + 1, StringComparison.Ordinal);
                if(start != -1 && start < Stop)
                    Stop = Str.IndexOf(Close, Stop + 1, StringComparison.Ordinal);
            } while(start != -1 && start < Stop);
            if(Stop == -1 || Stop < Start) throw new FormatException();
            Offset = Stop + Close.Length;
            Start += Open.Length;
            return Str.Substring(Start, Stop - Start);
        }

        IEnumerator<JSONObject> IEnumerable<JSONObject>.GetEnumerator() => (_Fields ?? Enumerable.Empty<JSONObject>()).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this).GetEnumerator();
    }

}
