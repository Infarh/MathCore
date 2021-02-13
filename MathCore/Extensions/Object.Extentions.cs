#nullable enable
using MathCore;
using MathCore.Evaluations;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using MathCore.Annotations;

using cEx = System.Linq.Expressions.ConstantExpression;
using Ex = System.Linq.Expressions.Expression;
using mcEx = System.Linq.Expressions.MethodCallExpression;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMember.Global
namespace System
{
    /// <summary>Класс методов-расширений для объекта</summary>
    public static class ObjectExtensions
    {
        /// <summary>Вызов к объекту с обработкой ошибок</summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="obj">Объект, вызов к которому надо выполнить</param>
        /// <param name="action">Выполняемое над объектом действие, ошибки в котором требуется перехватить</param>
        /// <param name="OnError">Метод обработки исключения</param>
        public static void Try<T>(this T obj, Action<T> action, Action<T, Exception> OnError)
        {
            if (OnError is null) throw new ArgumentNullException(nameof(OnError));
            try
            {
                action(obj);
            }
            catch (Exception e)
            {
                OnError(obj, e);
            }
        }

        /// <summary>Вычислить значение для указанного объекта с обработкой ошибок</summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <typeparam name="TResult">Тип результата</typeparam>
        /// <param name="obj">Объект, для которого вычисляется значение</param>
        /// <param name="func">Метод вычисления значения</param>
        /// <param name="OnError">Функция обработки исключения, возвращающий значение в случае его возникновения</param>
        /// <returns>Результат вызова</returns>
        public static TResult? Try<T, TResult>(
            this T obj,
             Func<T, TResult> func,
             Func<T, Exception, TResult>? OnError = null)
        {
            if (func is null) throw new ArgumentNullException(nameof(func));
            try
            {
                return func(obj);
            }
            catch (Exception e)
            {
                return OnError is null ? default : OnError(obj, e);
            }
        }

        /// <summary>Вычислить значение для указанного объекта с обработкой ошибок</summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <typeparam name="TResult">Тип результата</typeparam>
        /// <param name="obj">Объект, для которого вычисляется значение</param>
        /// <param name="func">Метод вычисления значения</param>
        /// <param name="OnError">Действие обработки исключения</param>
        /// <param name="DefaultResult">Значение по умолчанию, возвращаемое в случае возникновения исключения</param>
        /// <returns>Результат вызова</returns>
        public static TResult? Try<T, TResult>(
            this T obj, Func<T, TResult> func,
            Action<T, Exception>? OnError = null,
            TResult DefaultResult = default)
        {
            if (func is null) throw new ArgumentNullException(nameof(func));
            try
            {
                return func(obj);
            }
            catch (Exception e)
            {
                OnError?.Invoke(obj, e);
                return DefaultResult;
            }
        }

        /// <summary>Преобразование объекта в бесконечное перечисление</summary>
        /// <typeparam name="T">Тип элементов генерируемого перечисления</typeparam>
        /// <param name="obj">Объект, на основе которого создаётся перечисление</param>
        /// <param name="NextObject">Метод, генерирующий новый объект последовательности</param>
        /// <param name="TakeFirst">Выдать в последовательность исходный элемент</param>
        /// <returns>Бесконечная последовательность элементов, генерируемая указанным методом</returns>

        public static IEnumerable<T?> AsEnumerable<T>(this T? obj, Func<T?, T?> NextObject, bool TakeFirst = false)
        {
            if (TakeFirst) yield return obj;
            while (true)
            {
                obj = NextObject(obj);
                yield return obj;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>Селектор элементов</summary>
        /// <typeparam name="TSource">Тип объекта-источника</typeparam>
        /// <typeparam name="TResult">Тип объекта-значения</typeparam>
        public class ObjectSelector<TSource, TResult>
        {
            /// <summary>Продолжать выборку?</summary>
            public bool Continue { get; set; }

            /// <summary>Объект-источник</summary>
            public TSource? Object { get; set; }

            /// <summary>Объект-значение</summary>
            public TResult? Result { get; set; }

            /// <summary>Номер итерации</summary>
            public int Iteration { get; set; }

            /// <summary>Инициализация нового экземпляра <see cref="ObjectSelector{TSource,TResult}"/></summary>
            public ObjectSelector() => Continue = true;

            /// <summary>Переход к следующей итерации</summary>
            /// <param name="result">Результат итерации</param>
            /// <param name="CanContinue">Продолжать выборку</param>
            public void Next(TResult? result, bool CanContinue = true)
            {
                Result = result;
                Continue = CanContinue;
            }
        }

        /// <summary>Генерация последовательности значений на основе алгоритма выборки</summary>
        /// <typeparam name="TSource">Тип объекта-источника</typeparam>
        /// <typeparam name="TResult">Тип объекта-значения последовательности</typeparam>
        /// <param name="source">Источник последовательности</param>
        /// <param name="Selector">Метод выборки элементов из источника</param>
        /// <returns>Последовательность объектов, генерируемых на основе объекта-источника</returns>
        public static IEnumerable<TResult?> SelectObj<TSource, TResult>(this TSource? source, Action<ObjectSelector<TSource, TResult?>> Selector)
        {
            var selector = new ObjectSelector<TSource, TResult?>
            {
                Object = source,
                Result = default
            };
            do
            {
                Selector(selector);
                yield return selector.Result;
                selector.Result = default;
                selector.Iteration++;
            } while (selector.Continue);
        }

        /// <summary>Метод преобразования объекта</summary>
        /// <typeparam name="TSource">Тип источника объекта</typeparam>
        /// <typeparam name="TResult">Тип результата</typeparam>
        /// <param name="source">Объект-источник</param>
        /// <param name="Selector">Метод генерации значения</param>
        /// <returns>Значение, определяемое на основе объекта-источника указанным методом</returns>
        public static TResult SelectObject<TSource, TResult>(this TSource source, Func<TSource, TResult> Selector) => Selector(source);

        /// <summary>Преобразование объекта в форматированную строку</summary>
        /// <param name="obj">Преобразуемый объект</param>
        /// <param name="Format">Строка форматирования</param>
        /// <returns>Форматированная строка текстового представления объекта</returns>
        public static string ToFormattedString(this object obj, string Format = "{0}") => string.Format(Format, obj);

        /// <summary>Преобразование объекта в форматированную строку</summary>
        /// <param name="obj">Преобразуемый объект (идущий нулевым аргументом)</param>
        /// <param name="Format">Строка форматирования</param>
        /// <param name="args">Массив аргументов, добавляемых к объекту для создание форматированной строки</param>
        /// <returns>Форматированная строка текстового представления объекта</returns>
        [StringFormatMethod("Format")]
        public static string ToFormattedString(this object obj, string Format, params object[]? args)
        {
            if (args is null || args.Length == 0) return string.Format(Format, obj);
            var str_args = new object[args.ParamNotNull(nameof(args)).Length + 1];
            Array.Copy(args, 0, str_args, 1, args.Length);
            str_args[0] = obj;
            return string.Format(Format, str_args);
        }

        /// <summary>Метод преобразования объекта в строку</summary>
        /// <typeparam name="T">Тип исходного объекта</typeparam>
        /// <param name="t">Преобразуемый объект</param>
        /// <param name="converter">Метод преобразования объекта в строку</param>
        /// <returns>Сгенерированная строка указанным методом на основе указанного объекта</returns>
        public static string ToString<T>(this T t, Func<T, string> converter) => converter(t);

        /// <summary>Расчёт хеш-кода массива объектов</summary>
        /// <param name="Objects">Массив объектов, хеш-код которых надо рассчитать</param>
        /// <returns>Хеш-код массива объектов</returns>
        public static int GetHashCode(params object[] Objects) => Objects.GetComplexHashCode();

        /// <summary>Расчёт хеш-кода перечисления объектов</summary>
        /// <param name="Objects">Перечисление объектов, хеш-код которых надо рассчитать</param>
        /// <returns>Хеш-код перечисления объектов</returns>
        public static int GetComplexHashCode(this IEnumerable<object> Objects)
        {
            var hash = Consts.BigPrime_int;
            foreach (var obj in Objects)
                unchecked
                {
                    hash = (hash * 397) ^ obj.GetHashCode();
                }
            return hash == Consts.BigPrime_int ? 0 : hash;
        }

        /// <summary>Извлечение атрибута метаданных объекта</summary>
        /// <typeparam name="TAttribute">Тип извлекаемого атрибута</typeparam>
        /// <param name="o">Объект, атрибут которого требуется получить</param>
        /// <param name="Inherited">Искать в цепочке наследования</param>
        /// <returns>Искомый атрибут в случае его наличия, либо null, если атрибут не определён</returns>
        public static TAttribute? GetObjectAttribute<TAttribute>(this object o, bool Inherited = false)
            where TAttribute : Attribute
        {
            var object_attributes = o.GetObjectAttributes<TAttribute>(Inherited);
            return object_attributes is null ? null : object_attributes.Length == 0 ? default : object_attributes[0];
        }

        /// <summary>Извлечение всех атрибутов указанного типа для объекта</summary>
        /// <typeparam name="TAttribute">Тип извлекаемого атрибута</typeparam>
        /// <param name="o">Объект, атрибуты которого требуется получить</param>
        /// <param name="Inherited">Искать в цепочке наследования</param>
        /// <returns>Массив атрибутов указанного типа, определённых для объекта</returns>
        public static TAttribute[]? GetObjectAttributes<TAttribute>(this object? o, bool Inherited = false)
            where TAttribute : Attribute =>
            o?.GetType().GetCustomAttributes<TAttribute>(Inherited);

        /// <summary>Ссылка на объект не равна null</summary>
        /// <param name="o">Проверяемый объект</param>
        /// <returns>Истина, если проверяемый объект не null</returns>
        public static bool IsNotNull(this object? o) => o != null;

        /// <summary>Ссылка на объект равна null</summary>
        /// <param name="o">Проверяемый объект</param>
        /// <returns>Истина, если проверяемый объект null</returns>
        public static bool IsNull(this object? o) => o is null;

        /// <summary>Проверка на пустую ссылку</summary>
        /// <typeparam name="T">Тип проверяемого объекта</typeparam>
        /// <param name="obj">Проверяемое значение</param>
        /// <param name="Message">Сообщение ошибки</param>
        /// <returns>Значение, точно не являющееся пустой ссылкой</returns>
        /// <exception cref="InvalidOperationException">В случае если переданное значение <paramref name="obj"/> == null</exception>

        public static T NotNull<T>(this T? obj, string? Message = null) where T : class => obj ?? throw new InvalidOperationException(Message ?? "Пустая ссылка на объект");

        /// <summary>Проверка параметра на <see langword="null"/></summary>
        /// <typeparam name="T">Тип параметра <paramref name="obj"/></typeparam>
        /// <param name="obj">Проверяемый на <see langword="null"/> объект</param>
        /// <param name="ParameterName">Имя параметра для указания его в исключении</param>
        /// <param name="Message">Сообщение ошибки</param>
        /// <returns>Объект, гарантированно не <see langword="null"/></returns>
        /// <exception cref="T:System.ArgumentException">Если параметр <paramref name="obj"/> == <see langword="null"/>.</exception>

        public static T ParamNotNull<T>(this T? obj, string ParameterName, string? Message = null) where T : class =>
            obj ?? throw new ArgumentException(Message ?? $"Отсутствует ссылка для параметра {ParameterName}", ParameterName);

        /// <summary>Получение списка атрибутов указанного типа для типа переданного объекта</summary>
        /// <typeparam name="TAttribute">Тип извлекаемого атрибута</typeparam>
        /// <typeparam name="TObject">Тип исходного объекта</typeparam>
        /// <param name="_">Объект, атрибуты которого требуется получить</param>
        /// <param name="Inherited">Искать в цепочке наследования</param>
        /// <returns>Массив атрибутов указанного типа, определённых для типа объекта</returns>
        public static TAttribute[] GetAttributes<TAttribute, TObject>(this TObject _, bool Inherited = false)
            where TAttribute : Attribute => typeof(TObject).GetCustomAttributes<TAttribute>(Inherited);

        /// <summary>Инициализировать объект ссылочного типа</summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="obj">Инициализируемый объект</param>
        /// <param name="Initializer">Действие инициализации</param>
        /// <returns>Инициализированный объект</returns>
        public static T? InitializeObject<T>(this T? obj, Action<T?>? Initializer) where T : class
        {
            if (obj != null)
                Initializer?.Invoke(obj);
            return obj;
        }

        /// <summary>Инициализировать объект ссылочного типа</summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <typeparam name="TP">Тип параметра инициализации</typeparam>
        /// <param name="obj">Инициализируемый объект</param>
        /// <param name="parameter">Параметр инициализации</param>
        /// <param name="Initializer">Действие инициализации</param>
        /// <returns>Инициализированный объект</returns>
        public static T? InitializeObject<T, TP>(this T? obj, TP? parameter, Action<T?, TP?>? Initializer) where T : class
        {
            if (obj != null)
                Initializer?.Invoke(obj, parameter);
            return obj;
        }

        /// <summary>Инициализировать объект ссылочного типа</summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <typeparam name="TP1">Тип параметра 1 инициализации</typeparam>
        /// <typeparam name="TP2">Тип параметра 2 инициализации</typeparam>
        /// <param name="obj">Инициализируемый объект</param>
        /// <param name="parameter1">Параметр 1 инициализации</param>
        /// <param name="parameter2">Параметр 2 инициализации</param>
        /// <param name="Initializer">Действие инициализации</param>
        /// <returns>Инициализированный объект</returns>
        public static T? InitializeObject<T, TP1, TP2>(this T? obj, TP1? parameter1, TP2? parameter2, Action<T?, TP1?, TP2?>? Initializer
        ) where T : class
        {
            if (obj != null)
                Initializer?.Invoke(obj, parameter1, parameter2);
            return obj;
        }

        /// <summary>Инициализировать объект ссылочного типа</summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <typeparam name="TP1">Тип параметра 1 инициализации</typeparam>
        /// <typeparam name="TP2">Тип параметра 2 инициализации</typeparam>
        /// <typeparam name="TP3">Тип параметра 3 инициализации</typeparam>
        /// <param name="obj">Инициализируемый объект</param>
        /// <param name="parameter1">Параметр инициализации</param>
        /// <param name="parameter2">Параметр инициализации</param>
        /// <param name="parameter3">Параметр инициализации</param>
        /// <param name="Initializer">Действие инициализации</param>
        /// <returns>Инициализированный объект</returns>
        public static T? InitializeObject<T, TP1, TP2, TP3>(
            this T? obj,
            TP1? parameter1,
            TP2? parameter2,
            TP3? parameter3,
            Action<T?, TP1?, TP2?, TP3?>? Initializer)
            where T : class
        {
            if (obj != null)
                Initializer?.Invoke(obj, parameter1, parameter2, parameter3);
            return obj;
        }

        /// <summary>Инициализировать объект ссылочного типа</summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="obj">Инициализируемый объект</param>
        /// <param name="Initializer">Функция инициализации, определяющая значение конечного объекта</param>
        /// <returns>Объект, возвращённый функцией инициализации</returns>
        public static T? InitializeObject<T>(this T? obj, Func<T?, T?>? Initializer) where T : class =>
            Initializer != null && obj != null ? Initializer(obj) : obj;

        /// <summary>Инициализировать объект ссылочного типа</summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <typeparam name="TP">Тип параметра инициализации</typeparam>
        /// <param name="obj">Инициализируемый объект</param>
        /// <param name="parameter">Параметр инициализации</param>
        /// <param name="Initializer">Функция инициализации, определяющая значение конечного объекта</param>
        /// <returns>Объект, возвращённый функцией инициализации</returns>
        public static T? InitializeObject<T, TP>(this T? obj, TP? parameter, Func<T?, TP?, T?>? Initializer) where T : class =>
            Initializer != null && obj != null ? Initializer(obj, parameter) : obj;

        /// <summary>Инициализировать объект ссылочного типа</summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <typeparam name="TP1">Тип параметра 1 инициализации</typeparam>
        /// <typeparam name="TP2">Тип параметра 2 инициализации</typeparam>
        /// <typeparam name="TP3">Тип параметра 3 инициализации</typeparam>
        /// <param name="obj">Инициализируемый объект</param>
        /// <param name="parameter1">Параметр 1 инициализации</param>
        /// <param name="parameter2">Параметр 2 инициализации</param>
        /// <param name="parameter3">Параметр 3 инициализации</param>
        /// <param name="Initializer">Функция инициализации, определяющая значение конечного объекта</param>
        /// <returns>Объект, возвращённый функцией инициализации</returns>
        public static T? InitializeObject<T, TP1, TP2, TP3>(
            this T? obj,
            TP1? parameter1,
            TP2? parameter2,
            TP3? parameter3,
            Func<T?, TP1?, TP2?, TP3?, T?>? Initializer)
            where T : class =>
            Initializer != null && obj != null ? Initializer(obj, parameter1, parameter2, parameter3) : obj;

        /// <summary>Инициализировать объект ссылочного типа</summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <typeparam name="TP1">Тип параметра 1 инициализации</typeparam>
        /// <typeparam name="TP2">Тип параметра 2 инициализации</typeparam>
        /// <param name="obj">Инициализируемый объект</param>
        /// <param name="parameter1">Параметр 1 инициализации</param>
        /// <param name="parameter2">Параметр 2 инициализации</param>
        /// <param name="Initializer">Функция инициализации, определяющая значение конечного объекта</param>
        /// <returns>Объект, возвращённый функцией инициализации</returns>
        public static T? InitializeObject<T, TP1, TP2>(
            this T? obj,
            TP1? parameter1,
            TP2? parameter2,
            Func<T?, TP1?, TP2?, T?>? Initializer)
            where T : class =>
            Initializer != null && obj != null ? Initializer(obj, parameter1, parameter2) : obj;

        /// <summary>Печать объекта на консоли без переноса строки в конце</summary>
        /// <typeparam name="T">Тип печатаемого объекта</typeparam>
        /// <param name="Obj">Печатаемый объект</param>
        public static void ToConsole<T>(this T? Obj)
        {
            if (Obj is null) return;
            Console.Write(Obj);
        }

        /// <summary>Печать объекта на консоли в указанному формате без переноса строки в конце</summary>
        /// <typeparam name="TObject">Тип печатаемого объекта</typeparam>
        /// <param name="Obj">Печатаемый объект</param>
        /// <param name="Format">Строка форматирования результата</param>
        /// <param name="args">Дополнительные аргументы строки форматирования</param>
        [StringFormatMethod("Format")]
        public static void ToConsole<TObject>(this TObject? Obj, string Format, params object[] args)
        {
            if (Obj is null) return;
            if (args.Length == 0)
                Console.Write(Format, Obj);
            else
                Console.Write(Format, args.AppendFirst(Obj).ToArray());
        }

        /// <summary>Печать объекта на консоли с переносом строки в конце</summary>
        /// <typeparam name="T">Тип печатаемого объекта</typeparam>
        /// <param name="Obj">Печатаемый объект</param>
        public static void ToConsoleLN<T>(this T? Obj)
        {
            if (Obj is null) return;
            Console.WriteLine(Obj);
        }

        /// <summary>Печать объекта на консоли в указанному формате с переносом строки в конце</summary>
        /// <typeparam name="TObject">Тип печатаемого объекта</typeparam>
        /// <param name="Obj">Печатаемый объект</param>
        /// <param name="Format">Строка форматирования результата</param>
        /// <param name="args">Дополнительные аргументы строки форматирования</param>
        [StringFormatMethod("Format")]
        public static void ToConsoleLN<TObject>(this TObject? Obj, string Format, params object[] args)
        {
            if (Obj is null) return;
            if (args.Length != 0)
                Console.WriteLine(Format, args.AppendFirst(Obj).ToArray());
            else
                Console.WriteLine(Format, Obj);
        }

        /// <summary>Печать объекта в отладочной информации без переноса строки в конце</summary>
        /// <typeparam name="T">Тип печатаемого объекта</typeparam>
        /// <param name="Obj">Печатаемый объект</param>
        public static void ToDubugOut<T>(this T? Obj)
        {
            if (Obj is null) return;
            Debug.Write(Obj);
        }

        /// <summary>Печать объекта в отладочной информации без переноса строки в конце</summary>
        /// <typeparam name="T">Тип печатаемого объекта</typeparam>
        /// <param name="Obj">Печатаемый объект</param>
        /// <param name="Condition">Условие (если истина, то объект печатается в отладочный вывод)</param>
        public static void ToDubugOut<T>(this T? Obj, bool Condition)
        {
            if (Obj is null) return;
            Debug.WriteIf(Condition, Obj);
        }

        /// <summary>Печать объекта в отладочной информации с переносом строки в конце</summary>
        /// <typeparam name="T">Тип печатаемого объекта</typeparam>
        /// <param name="Obj">Печатаемый объект</param>
        public static void ToDebugOutLN<T>(this T? Obj)
        {
            if (Obj is null) return;
            Debug.WriteLine(Obj);
        }

        /// <summary>Печать объекта в отладочной информации с переносом строки в конце</summary>
        /// <typeparam name="T">Тип печатаемого объекта</typeparam>
        /// <param name="Obj">Печатаемый объект</param>
        /// <param name="Condition">Условие (если истина, то объект печатается в отладочный вывод)</param>
        public static void ToDebugOutLN<T>(this T? Obj, bool Condition)
        {
            if (Obj is null) return;
            Debug.WriteLineIf(Condition, Obj);
        }

        /// <summary>Преобразование структуры в массив байт</summary>
        /// <typeparam name="T">Тип преобразуемой структуры</typeparam>
        /// <param name="value">Значение преобразуемой структуры</param>
        /// <returns>Массив байт, представляющий указанную структуру</returns>

        public static byte[] ToByteArray<T>(this T value) where T : struct
        {
            var buffer = new byte[Marshal.SizeOf(value)]; // создать массив
            var g_lock = GCHandle.Alloc(buffer, GCHandleType.Pinned); // зафиксировать в памяти
            try
            {
                var p = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0); // и взять его адрес
                Marshal.StructureToPtr(value, p, true); // копировать в массив
                return buffer;
            }
            finally
            {
                g_lock.Free(); // снять фиксацию
            }
        }

        /// <summary>Преобразование массива байт в структуру</summary>
        /// <typeparam name="T">Тип структуры</typeparam>
        /// <param name="data">Массив байт</param>
        /// <param name="offset">Смещение в массиве байт</param>
        public static T ToStructure<T>(this byte[] data, int offset = 0)
            where T : struct
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                var ptr = gch.AddrOfPinnedObject();
                ptr += offset;
                return (T)Marshal.PtrToStructure(ptr, typeof(T));
            }
            finally
            {
                gch.Free();
            }
        }

        /// <summary>Выбор действия для объекта</summary>
        /// <param name="obj">Объект, на котором выполняется выбор действия</param>
        /// <param name="actions">Словарь возможных действий над объектом</param>
        /// <param name="Default">Действие по умолчанию</param>
        public static void Switch(this object obj, Actions actions, Action<object>? Default = null)
            => actions.GetValue(obj, Default)?.Invoke(obj);

        //public static unsafe void ReadPointer<T>(byte[] data, int offset, out T value)
        //    where T : struct
        //{
        //    fixed (byte* pData = &data[offset])
        //    {
        //        value = *(T*)pData;
        //    }
        //}

        // ReSharper disable CommentTypo
        ///// <summary>Метод чтения структуры данных из массива байт</summary>
        ///// <typeparam name="T">Тип структуры</typeparam>
        ///// <param name="data">Массив байт</param>
        ///// <param name="offset">Смещение в массиве байт</param>
        ///// <param name="value">Прочитанная структура</param>
        //public delegate void StructureReader<T>( byte[] data, int offset, [CanBeNull] out T value);

        //[Copyright("Генерация кода", url = "http://professorweb.ru/my/csharp/optimization/level7/7_9.php")]
        //private static class StructureReadersPool<T>
        //{
        //    /// <summary>Делегат чтения структуры данных</summary>
        //    [CanBeNull] private static volatile StructureReader<T> __Reader;
        //    /// <summary>Делегат чтения структуры данных</summary>
        //    
        //    public static StructureReader<T> Reader
        //    {
        //        get
        //        {
        //            if (__Reader != null) return __Reader;
        //            lock (typeof(StructureReadersPool<T>))
        //            {
        //                if (__Reader != null) return __Reader;
        //                return __Reader = CreateDelegate();
        //            }
        //        }
        //    }

        //    /// <summary>Создать делегат чтения структуры данных</summary>
        //    /// <returns>Делегат, читающий структуру из массива данных</returns>
        //    
        //    private static StructureReader<T> CreateDelegate()
        //    {
        //        var dm = new DynamicMethod
        //        (
        //            name: "Read",
        //            returnType: null,
        //            parameterTypes: new[]
        //            {
        //                typeof(byte[]),
        //                typeof(int),
        //                typeof(T).MakeByRefType()
        //            },
        //            m: Assembly.GetExecutingAssembly().ManifestModule
        //        );

        //        dm.DefineParameter(1, ParameterAttributes.None, "data");
        //        dm.DefineParameter(2, ParameterAttributes.None, "offset");
        //        dm.DefineParameter(3, ParameterAttributes.Out, "value");

        //        var generator = dm.GetILGenerator();

        //        generator.DeclareLocal(typeof(byte).MakePointerType(), pinned: true);

        //        generator.Emit(OpCodes.Ldarg_0);
        //        generator.Emit(OpCodes.Ldarg_1);
        //        generator.Emit(OpCodes.Ldelema, typeof(byte));
        //        generator.Emit(OpCodes.Stloc_0);
        //        generator.Emit(OpCodes.Ldarg_2);
        //        generator.Emit(OpCodes.Ldloc_0);
        //        generator.Emit(OpCodes.Conv_I);
        //        generator.Emit(OpCodes.Ldobj, typeof(T));
        //        generator.Emit(OpCodes.Stobj, typeof(T));
        //        generator.Emit(OpCodes.Ldc_I4_0);
        //        generator.Emit(OpCodes.Conv_U);
        //        generator.Emit(OpCodes.Stloc_0);
        //        generator.Emit(OpCodes.Ret);

        //        return (StructureReader<T>)dm.CreateDelegate(typeof(StructureReader<T>));
        //    }
        //}

        ///// <summary>Получить метод чтения структур указанного типа из массива байт</summary>
        ///// <typeparam name="T">Тип структуры данных</typeparam>
        ///// <returns>Делегат чтения структуры данных из массива байт</returns>
        // public static StructureReader<T> GetStructReader<T>() => StructureReadersPool<T>.Reader;

        ///// <summary>Чтение структуры данных из массива байт</summary>
        ///// <typeparam name="T">Тип структуры данных</typeparam>
        ///// <param name="data">Массив байт</param>
        ///// <param name="offset">Смещение в массиве байт</param>
        ///// <param name="value">Значение, прочитанное из структуры данных</param>
        //[Copyright("Генерация кода", url = "http://professorweb.ru/my/csharp/optimization/level7/7_9.php")]
        //public static void ReadPointerLCG<T>( byte[] data, int offset, [CanBeNull] out T value) => GetStructReader<T>()(data, offset, out value);

        //public static unsafe void ReadPointerTypedRef<T>(byte[] data, int offset, ref T value)
        //{
        //    // В действительности мы не изменяем 'value' - нам просто 
        //    // требуется левостороннее значение
        //    TypedReference tr = __makeref(value);

        //    fixed (byte* ptr = &data[offset])
        //    {
        //        // Первое поле - указатель в структуре TypedReference - это 
        //        // адрес объекта, поэтому мы записываем в него 
        //        // указатель на нужный элемент в массиве с данными
        //        *(IntPtr*)&tr = (IntPtr)ptr;

        //        // __refvalue копирует указатель из TypedReference в 'value'
        //        value = __refvalue(tr, T);
        //    }
        //}
        // ReSharper restore CommentTypo

        /// <summary>Преобразование объекта в вычисление</summary>
        /// <typeparam name="T">Тип исходного элемента</typeparam>
        /// <param name="obj">Оборачиваемый объект</param>
        /// <returns>Вычисление, возвращающее указанный объект</returns>
        public static ValueEvaluation<T> ToEvaluation<T>(this T? obj) => new(obj!);

        /// <summary>Преобразование объекта в именованное вычисление</summary>
        /// <typeparam name="T">Тип исходного элемента</typeparam>
        /// <param name="obj">Оборачиваемый объект</param>
        /// <param name="Name">Имя вычисления</param>
        /// <returns>Вычисление, возвращающее указанный объект</returns>
        public static ValueEvaluation<T> ToEvaluation<T>(this T? obj, string? Name) => new NamedValueEvaluation<T>(obj!, Name);

        /// <summary>Преобразование объекта в выражение-константу</summary>
        /// <param name="obj">Преобразуемый объект</param>
        /// <returns>Выражение-константа</returns>
        public static cEx ToExpression(this object? obj) => Ex.Constant(obj);

        /// <summary>Получить выражение вызова метода объекта</summary>
        /// <param name="obj">Объект, метод которого надо вызвать</param>
        /// <param name="d">Делегат метода</param>
        /// <param name="p">Параметры метода</param>
        /// <returns>Выражение вызова метода</returns>
        public static mcEx GetCallExpression(this object? obj, Delegate d, params Ex[] p) => obj.GetCallExpression(d.Method, p);

        /// <summary>Получить выражение вызова метода объекта</summary>
        /// <param name="obj">Объект, метод которого надо вызвать</param>
        /// <param name="d">Описание метода</param>
        /// <param name="p">Параметры метода</param>
        /// <returns>Выражение вызова метода</returns>
        public static mcEx GetCallExpression(this object? obj, MethodInfo d, params Ex[] p) => Ex.Call(obj.ToExpression(), d, p);

        /// <summary>Получить выражение вызова метода объекта</summary>
        /// <param name="obj">Объект, метод которого надо вызвать</param>
        /// <param name="MethodName">Имя метода</param>
        /// <param name="p">Параметры метода</param>
        /// <returns>Выражение вызова метода</returns>

        public static mcEx GetCallExpression(this object obj, string MethodName, params Ex[] p)
        {
            var type = obj.GetType();
            var method = type.GetMethod(MethodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                p.Select(pp => pp.Type).ToArray(),
                null)
               .NotNull();
            return obj.GetCallExpression(method, p);
        }
    }

    /// <summary>Словарь действий</summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Actions : Dictionary<object, Action<object>> { }
}
namespace System.Tags
{
    /// <summary>Класс методов-расширений для реализации функциональности добавления объектов, которые могут быть приложены к другим объектам</summary>
    public static class TagExtensions
    {
        /// <summary>Получить объект-метку указанного типа из целевого объекта</summary>
        /// <typeparam name="TTag">Тип объекта-метки</typeparam>
        /// <param name="o">Целевой объект</param>
        /// <returns>Объект метка, если он существует в указанном объекте</returns>
        public static TTag? GetTag<TTag>(this object o) => TagPool.Tag<TTag>(o);

        /// <summary>Установить объект-метку для указанного объекта</summary>
        /// <typeparam name="TTag">Тип объекта-метки</typeparam>
        /// <param name="o">Целевой объект</param>
        /// <param name="Tag">Объект-метка, прикладываемый к целевому объекту</param>
        public static void SetTag<TTag>(this object o, TTag? Tag) => TagPool.SetTag(o, Tag);

        #region Nested type: TagPool

        /// <summary>Пул меток</summary>
        private static class TagPool
        {
            /// <summary>Объект межпотоковой синхронизации</summary>
            private static readonly object __Lock = new();
            /// <summary>Словарь меток</summary>
            private static Dictionary<WeakReference, Dictionary<Type, object?>>? __Tags;

            /// <summary>Установить метку указанному объекту</summary>
            /// <param name="o">Целевой объект</param>
            /// <param name="Tag">Добавляемая метка</param>
            public static void SetTag(object o, object? Tag)
            {
                lock (__Lock)
                {
                    var tags = __Tags ??= new Dictionary<WeakReference, Dictionary<Type, object?>>();

                    tags.Keys.Where(w => !w.IsAlive).ToArray().Foreach(w => tags.Remove(w));
                    bool Selector(WeakReference w) => o.Equals(w.Target);

                    var reference = tags.Keys.Find(Selector);
                    Dictionary<Type, object?> dictionary;
                    if (reference != null)
                        dictionary = tags[reference];
                    else
                        tags.Add(new WeakReference(o), dictionary = new Dictionary<Type, object?>());

                    var type = Tag?.GetType() ?? typeof(object);

                    if (dictionary.Keys.Contains(type))
                        dictionary[type] = Tag;
                    else
                        dictionary.Add(type, Tag);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static bool IsAlive(WeakReference w) => !w.IsAlive;

            /// <summary>Получить метку указанного типа для указанного объекта</summary>
            /// <typeparam name="TTagType">Тип объекта-метки</typeparam>
            /// <param name="o">Целевой объект</param>
            /// <returns>Объект-метка</returns>
            public static TTagType? Tag<TTagType>(object o)
            {
                bool Selector(WeakReference w) => o.Equals(w.Target);

                lock (__Lock)
                {
                    var tags = __Tags;
                    tags?.Keys.Where(IsAlive).Foreach(tags, (w, t) => t.Remove(w));
                    var reference = tags?.Keys.Find(Selector);
                    return reference is null
                        ? default
                        : !tags![reference].TryGetValue(typeof(TTagType), out var result)
                            ? default
                            : (TTagType)result!;
                }
            }
        }

        #endregion
    }
}