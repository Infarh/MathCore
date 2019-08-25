using MathCore;
using MathCore.Annotations;
using MathCore.Evulations;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using cEx = System.Linq.Expressions.ConstantExpression;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using Ex = System.Linq.Expressions.Expression;
using mcEx = System.Linq.Expressions.MethodCallExpression;

namespace System
{
    /// <summary>Класс методов-расширений для объекта</summary>
    public static class ObjectExtentions
    {
        /// <summary>Преобразование объекта в бесконечное перечисление</summary>
        /// <typeparam name="T">Тип элементов генерируемого перечисления</typeparam>
        /// <param name="obj">Объект, на основе которого создаётся перечисление</param>
        /// <param name="NextObject">Метод, генерирующий новый объект последовательности</param>
        /// <param name="TakeFirst">Выдать в последовательность исходный элемент</param>
        /// <returns>Бесконечная последовательность элементов, генерируемая указанным методом</returns>
        public static IEnumerable<T> AsEnumerable<T>(this T obj, Func<T, T> NextObject, bool TakeFirst = false)
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
            public TSource Object { get; set; }

            /// <summary>Объект-значение</summary>
            public TResult Result { get; set; }

            /// <summary>Номер итерации</summary>
            public int Iteration { get; set; }

            /// <summary>Инициализация нового экземпляра <see cref="ObjectSelector{TSource,TResult}"/></summary>
            public ObjectSelector() => Continue = true;

            /// <summary>Переход к следующей итерации</summary>
            /// <param name="result">Результат итерации</param>
            /// <param name="continue">Продолжать выборку</param>
            [DST]
            public void Next(TResult result, bool @continue = true)
            {
                Result = result;
                Continue = @continue;
            }
        }

        /// <summary>Генерация последовательности значений на основе алгоритма выборки</summary>
        /// <typeparam name="TSource">Тип объекта-источника</typeparam>
        /// <typeparam name="TResult">Тип объекта-значения последовательности</typeparam>
        /// <param name="source">Источник последовательности</param>
        /// <param name="Selector">Метод выборки элементов из источника</param>
        /// <returns>Последовательность объектов, генерируемых на основе объекта-источника</returns>
        [DST]
        public static IEnumerable<TResult> SelectObj<TSource, TResult>
        (
            this TSource source,
            Action<ObjectSelector<TSource, TResult>> Selector
        )
        {
            Contract.Requires(!ReferenceEquals(Selector, null));
            var selector = new ObjectSelector<TSource, TResult>
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
        [DST]
        public static TResult SelectObject<TSource, TResult>(this TSource source, Func<TSource, TResult> Selector)
        {
            Contract.Requires(!ReferenceEquals(Selector, null));
            return Selector(source);
        }


        /// <summary>Преобразование объекта в форматированную строку</summary>
        /// <param name="obj">Преобразуемый объект</param>
        /// <param name="Format">Строка форматирования</param>
        /// <returns>Форматированная строка текстового представления объекта</returns>
        [DST]
        public static string ToFormattedString(this object obj, string Format = "{0}")
        {
            Contract.Requires(!ReferenceEquals(obj, null)); obj.ToConsole();
            return string.Format(Format, obj);
        }

        /// <summary>Преобразование объекта в форматированную строку</summary>
        /// <param name="obj">Преобразуемый объект (идущий нулевым аргументом)</param>
        /// <param name="Format">Строка форматирования</param>
        /// <param name="args">Массив аргументов, доавбляемых к объекту для создание форматированной строки</param>
        /// <returns>Форматированная строка текстового представления объекта</returns>
        [DST, StringFormatMethod("Format")]
        public static string ToFormattedString(this object obj, string Format, params object[] args)
        {
            Contract.Requires(!ReferenceEquals(obj, null)); obj.ToConsole();
            return args?.Length != 0 ? string.Format(Format, args.AppendFirst(obj).ToArray()) : string.Format(Format, obj);
        }

        /// <summary>Метод преобразования объекта в строку</summary>
        /// <typeparam name="T">Тип исходного объекта</typeparam>
        /// <param name="t">Преобразуемый объект</param>
        /// <param name="converter">Метод преобразвоания объекта в строку</param>
        /// <returns>Сгенерированная строка указанным методом на основе указанного объекта</returns>
        [DST]
        public static string ToString<T>(this T t, Func<T, string> converter)
        {
            Contract.Requires(converter != null);
            return converter(t);
        }

        /// <summary>Расчёт хеш-кода массива объектов</summary>
        /// <param name="Objects">Массив объектов, хеш-код которых надо расчитать</param>
        /// <returns>Хеш-код массива объектов</returns>
        [DST]
        public static int GetHashCode(params object[] Objects)
        {
            Contract.Requires(Objects != null);
            Contract.Requires(Objects.Length > 0);
            return Objects.GetComplexHashCode();
        }

        /// <summary>Расчёт хеш-кода перечисления объектов</summary>
        /// <param name="Objects">Перечисление объектов, хеш-код которых надо расчитать</param>
        /// <returns>Хеш-код перечисления объектов</returns>
        [DST]
        public static int GetComplexHashCode([NotNull] this IEnumerable<object> Objects)
        {
            Contract.Requires(Objects != null);
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
        [CanBeNull, DST, Diagnostics.Contracts.Pure]
        public static TAttribute GetObjectAttribute<TAttribute>(this object o, bool Inherited = false)
            where TAttribute : Attribute
        {
            Contract.Requires(o != null);
            var object_attributes = o.GetObjectAttributes<TAttribute>(Inherited);
            return object_attributes.Length != 0 ? object_attributes[0] : default(TAttribute);
        }

        /// <summary>Извлечение всех атрибутов указанного типа для объекта</summary>
        /// <typeparam name="TAttribute">Тип извлекаемого атрибута</typeparam>
        /// <param name="o">Объект, атрибуты которого требуется получить</param>
        /// <param name="Inherited">Искать в цепочке наследования</param>
        /// <returns>Массив атрибутов указанного типа, определённых для объекта</returns>
        [CanBeNull, DST, Diagnostics.Contracts.Pure]
        public static TAttribute[] GetObjectAttributes<TAttribute>(this object o, bool Inherited = false)
            where TAttribute : Attribute
        {
            Contract.Requires(!ReferenceEquals(o, null));
            return o.GetType().GetCustomAttributes<TAttribute>(Inherited);
        }

        /// <summary>Ссылка на объект не равна null</summary>
        /// <param name="o">Проверяемый объект</param>
        /// <returns>Истина, если проверяемый объект не null</returns>
        [DST, Diagnostics.Contracts.Pure]
        public static bool IsNotNull(this object o)
        {
            Contract.Ensures(Contract.Result<bool>() != ReferenceEquals(o, null));
            return !ReferenceEquals(o, null);
        }

        /// <summary>Ссылка на объект равна null</summary>
        /// <param name="o">Проверяемый объект</param>
        /// <returns>Истина, если проверяемый объект null</returns>
        [DST, Diagnostics.Contracts.Pure]
        public static bool IsNull(this object o)
        {
            Contract.Ensures(Contract.Result<bool>() == ReferenceEquals(o, null));
            return ReferenceEquals(o, null);
        }

        /// <summary>Получение списка атрибутов указанного типа для типа переданного объекта</summary>
        /// <typeparam name="TAttribute">Тип извлекаемого атрибута</typeparam>
        /// <typeparam name="TObject">Тип исходного объекта</typeparam>
        /// <param name="o">Объект, атрибуты которого требуется получить</param>
        /// <param name="Inherited">Искать в цепочке наследования</param>
        /// <returns>ассив атрибутов указанного типа, определённых для типа объекта</returns>
        [CanBeNull, DST, Pure]
        public static TAttribute[] GetAttributes<TAttribute, TObject>(this TObject o, bool Inherited = false)
            where TAttribute : Attribute => typeof(TObject).GetCustomAttributes<TAttribute>(Inherited);

        /// <summary>Инициализировать объект ссылочного типа</summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="obj">Инициализируемый объект</param>
        /// <param name="Initializator">Действие инициализации</param>
        /// <returns>Инициализированный объект</returns>
        [DST, CanBeNull]
        public static T InitializeObject<T>
        (
            [CanBeNull] this T obj,
            [CanBeNull]Action<T> Initializator
        ) where T : class
        {
            Contract.Requires(!ReferenceEquals(obj, null));
            Contract.Ensures(!ReferenceEquals(Contract.Result<T>(), null));
            Contract.Ensures(ReferenceEquals(Contract.Result<T>(), obj));
            Initializator?.Invoke(obj);
            return obj;
        }

        /// <summary>Инициализировать объект ссылочного типа</summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <typeparam name="TParameter">Тип параметра инициализации</typeparam>
        /// <param name="obj">Инициализируемый объект</param>
        /// <param name="parameter">Параметр инициализации</param>
        /// <param name="Initializator">Действие инициализации</param>
        /// <returns>Инициализированный объект</returns>
        [DST, CanBeNull]
        public static T InitializeObject<T, TParameter>
        (
            [CanBeNull] this T obj,
            [CanBeNull] TParameter parameter,
            [CanBeNull]Action<T, TParameter> Initializator
        ) where T : class
        {
            Contract.Requires(!ReferenceEquals(obj, null));
            Contract.Ensures(!ReferenceEquals(Contract.Result<T>(), null));
            Contract.Ensures(ReferenceEquals(Contract.Result<T>(), obj));
            Initializator?.Invoke(obj, parameter);
            return obj;
        }

        /// <summary>Инициализировать объект ссылочного типа</summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="obj">Инициализируемый объект</param>
        /// <param name="Initializator">Функция инициализации, определяющая значение конечного объекта</param>
        /// <returns>Объект, возвращённый функцией инициализации</returns>
        [DST, CanBeNull]
        public static T InitializeObject<T>
        (
            [CanBeNull] this T obj,
            [CanBeNull]Func<T, T> Initializator
        ) where T : class
        {
            Contract.Ensures(Initializator == null && ReferenceEquals(Contract.Result<T>(), obj));
            return Initializator != null ? Initializator(obj) : obj;
        }

        /// <summary>Инициализировать объект ссылочного типа</summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <typeparam name="TParameter">Тип параметра инициализации</typeparam>
        /// <param name="obj">Инициализируемый объект</param>
        /// <param name="parameter">Параметр инициализации</param>
        /// <param name="Initializator">Функция инициализации, определяющая значение конечного объекта</param>
        /// <returns>Объект, возвращённый функцией инициализации</returns>
        [DST, CanBeNull]
        public static T InitializeObject<T, TParameter>
        (
            [CanBeNull] this T obj,
            [CanBeNull] TParameter parameter,
            [CanBeNull]Func<T, TParameter, T> Initializator
        ) where T : class
        {
            Contract.Ensures(Initializator == null && ReferenceEquals(Contract.Result<T>(), obj));
            return Initializator != null ? Initializator(obj, parameter) : obj;
        }

        /// <summary>Печать объекта на консоли без переноса строки в конце</summary>
        /// <typeparam name="T">Тип печатаемого объекта</typeparam>
        /// <param name="Obj">Печатаемый объект</param>
        [DST]
        public static void ToConsole<T>([CanBeNull]this T Obj)
        {
            if (ReferenceEquals(Obj, null)) return;
            Console.Write(Obj);
        }

        /// <summary>Печать объекта на консоли в указанному формате без переноса строки в конце</summary>
        /// <typeparam name="TObject">Тип печатаемого объекта</typeparam>
        /// <param name="Obj">Печатаемый объект</param>
        /// <param name="Format">Строка форматирования результата</param>
        /// <param name="args">Дополнительные аргументы строки форматирования</param>
        [DST, StringFormatMethod("Format")]
        public static void ToConsole<TObject>([CanBeNull]this TObject Obj, [NotNull] string Format, [NotNull, ItemNotNull] params object[] args)
        {
            if (Format is null) throw new ArgumentNullException(nameof(Format));
            if (ReferenceEquals(Obj, null)) return;
            if (args?.Length != 0)
                Console.Write(Format, args.AppendFirst(Obj).ToArray());
            else
                Console.Write(Format, Obj);
        }

        /// <summary>Печать объекта на консоли с переносом строки в конце</summary>
        /// <typeparam name="T">Тип печатаемого объекта</typeparam>
        /// <param name="Obj">Печатаемый объект</param>
        [DST]
        public static void ToConsoleLN<T>([CanBeNull]this T Obj)
        {
            if (ReferenceEquals(Obj, null)) return;
            Console.WriteLine(Obj);
        }

        /// <summary>Печать объекта на консоли в указанному формате с переносом строки в конце</summary>
        /// <typeparam name="TObject">Тип печатаемого объекта</typeparam>
        /// <param name="Obj">Печатаемый объект</param>
        /// <param name="Format">Строка форматирования результата</param>
        /// <param name="args">Дополнительные аргументы строки форматирования</param>
        [DST, StringFormatMethod("Format")]
        public static void ToConsoleLN<TObject>([CanBeNull]this TObject Obj, [NotNull] string Format, [NotNull, ItemNotNull] params object[] args)
        {
            if (Format is null) throw new ArgumentNullException(nameof(Format));
            if (ReferenceEquals(Obj, null)) return;
            if (args?.Length != 0)
                Console.WriteLine(Format, args.AppendFirst(Obj).ToArray());
            else
                Console.WriteLine(Format, Obj);
        }

        /// <summary>Печать объекта в отладочной информации без переноса строки в конце</summary>
        /// <typeparam name="T">Тип печатаемого объекта</typeparam>
        /// <param name="Obj">Печатаемый объект</param>
        [DST, Conditional("DEBUG")]
        public static void ToDubugOut<T>([CanBeNull]this T Obj)
        {
            if (ReferenceEquals(Obj, null)) return;
            Debug.Write(Obj);
        }

        /// <summary>Печать объекта в отладочной информации без переноса строки в конце</summary>
        /// <typeparam name="T">Тип печатаемого объекта</typeparam>
        /// <param name="Obj">Печатаемый объект</param>
        /// <param name="Condition">Условие (если истина, то объект печатается в отладочный вывод)</param>
        [DST, Conditional("DEBUG")]
        public static void ToDubugOut<T>([CanBeNull]this T Obj, bool Condition)
        {
            if (ReferenceEquals(Obj, null)) return;
            Debug.WriteIf(Condition, Obj);
        }

        /// <summary>Печать объекта в отладочной информации с переносом строки в конце</summary>
        /// <typeparam name="T">Тип печатаемого объекта</typeparam>
        /// <param name="Obj">Печатаемый объект</param>
        [DST, Conditional("DEBUG")]
        public static void ToDubugOutLN<T>([CanBeNull]this T Obj)
        {
            if (ReferenceEquals(Obj, null)) return;
            Debug.WriteLine(Obj);
        }

        /// <summary>Печать объекта в отладочной информации с переносом строки в конце</summary>
        /// <typeparam name="T">Тип печатаемого объекта</typeparam>
        /// <param name="Obj">Печатаемый объект</param>
        /// <param name="Condition">Условие (если истина, то объект печатается в отладочный вывод)</param>
        [DST, Conditional("DEBUG")]
        public static void ToDubugOutLN<T>([CanBeNull]this T Obj, bool Condition)
        {
            if (ReferenceEquals(Obj, null)) return;
            Debug.WriteLineIf(Condition, Obj);
        }

        /// <summary>Преобразование структуры в массив байт</summary>
        /// <typeparam name="T">Тип преобразуемой структуры</typeparam>
        /// <param name="value">Значение преобразуемой структуры</param>
        /// <returns>Массив байт, представляющий указанную структуру</returns>
        [NotNull]
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
        public static T ToStructure<T>([NotNull] this byte[] data, int offset = 0)
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
        /// <param name="obj">ОБъект, на котором выполняется выбор действия</param>
        /// <param name="actions">Словарь возможных действий над объектом</param>
        /// <param name="Default">Действие по умолчанию</param>
        public static void Switch([NotNull] this object obj, [NotNull] Actions actions, [CanBeNull] Action<object> Default = null)
            => actions.GetValue(obj, Default)?.Invoke(obj);

        //public static unsafe void ReadPointer<T>(byte[] data, int offset, out T value)
        //    where T : struct
        //{
        //    fixed (byte* pData = &data[offset])
        //    {
        //        value = *(T*)pData;
        //    }
        //}

        /// <summary>Метод чтения структуры данных из массива байт</summary>
        /// <typeparam name="T">Тип структуры</typeparam>
        /// <param name="data">Массив байт</param>
        /// <param name="offset">Смещение в массиве байт</param>
        /// <param name="value">Прочитанная структура</param>
        public delegate void StructureReader<T>([NotNull] byte[] data, int offset, [CanBeNull] out T value);

        //[Copyright("Генерация кода", url = "http://professorweb.ru/my/csharp/optimization/level7/7_9.php")]
        //private static class StructureReadersPool<T>
        //{
        //    /// <summary>Делегат чтения структуры данных</summary>
        //    [CanBeNull] private static volatile StructureReader<T> __Reader;
        //    /// <summary>Делегат чтения структуры данных</summary>
        //    [NotNull]
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
        //    [NotNull]
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
        //[NotNull] public static StructureReader<T> GetStructReader<T>() => StructureReadersPool<T>.Reader;

        ///// <summary>Чтение структуры данных из массива байт</summary>
        ///// <typeparam name="T">Тип структуры данных</typeparam>
        ///// <param name="data">Массив байт</param>
        ///// <param name="offset">Смещение в массиве байт</param>
        ///// <param name="value">Значение, прочитанное из структуры данных</param>
        //[Copyright("Генерация кода", url = "http://professorweb.ru/my/csharp/optimization/level7/7_9.php")]
        //public static void ReadPointerLCG<T>([NotNull] byte[] data, int offset, [CanBeNull] out T value) => GetStructReader<T>()(data, offset, out value);

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

        /// <summary>Преобразование объекта в вычисление</summary>
        /// <typeparam name="T">Тип исходного элемента</typeparam>
        /// <param name="obj">Оборачиваемый объект</param>
        /// <returns>Вычисление, возвращающее указанный объект</returns>
        [NotNull]
        [DST]
        public static ValueEvulation<T> ToEvulation<T>([CanBeNull] this T obj) => new ValueEvulation<T>(obj);

        /// <summary>Преобразование объекта в именованное вычисление</summary>
        /// <typeparam name="T">Тип исходного элемента</typeparam>
        /// <param name="obj">Оборачиваемый объект</param>
        /// <param name="Name">Имя вычисления</param>
        /// <returns>Вычисление, возвращающее указанный объект</returns>
        [NotNull]
        [DST]
        public static ValueEvulation<T> ToEvulation<T>([CanBeNull] this T obj, [CanBeNull] string Name) => new NamedValueEvulation<T>(obj, Name);

        /// <summary>Преобразование объекта в выражение-константу</summary>
        /// <param name="obj">Преобразуемый объект</param>
        /// <returns>Выражение-константа</returns>
        [NotNull]
        [DST]
        public static cEx ToExpression([CanBeNull] this object obj) => Ex.Constant(obj);

        /// <summary>Получить выражение вызова метода объекта</summary>
        /// <param name="obj">Объект, метод которого надо вызвать</param>
        /// <param name="d">Делегат метода</param>
        /// <param name="p">Параметры метода</param>
        /// <returns>Выражение вызова метода</returns>
        [NotNull] public static mcEx GetCallExpression([CanBeNull] this object obj, [NotNull] Delegate d, [NotNull, ItemNotNull] params Ex[] p) => obj.GetCallExpression(d.Method, p);

        /// <summary>Получить выражение вызова метода объекта</summary>
        /// <param name="obj">Объект, метод которого надо вызвать</param>
        /// <param name="d">Описание методаа</param>
        /// <param name="p">Параметры метода</param>
        /// <returns>Выражение вызова метода</returns>
        [NotNull] public static mcEx GetCallExpression([CanBeNull] this object obj, [NotNull] MethodInfo d, [NotNull, ItemNotNull] params Ex[] p) => Ex.Call(obj.ToExpression(), d, p);

        /// <summary>Получить выражение вызова метода объекта</summary>
        /// <param name="obj">Объект, метод которого надо вызвать</param>
        /// <param name="MethodName">Имя метода</param>
        /// <param name="p">Параметры метода</param>
        /// <returns>Выражение вызова метода</returns>
        [NotNull]
        public static mcEx GetCallExpression([CanBeNull] this object obj, [NotNull] string MethodName, [NotNull, ItemNotNull] params Ex[] p)
        {
            var type = obj.GetType();
            var method = type.GetMethod(MethodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                p.Select(pp => pp.Type).ToArray(),
                null);
            return obj.GetCallExpression(method, p);
        }
    }

    /// <summary>Словарь действий</summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Actions : Dictionary<object, Action<object>> { }
}

namespace System.Tags
{
    /// <summary>Класс методов-расширений для реализации функциональности добавления объектов, которые могут быть приложенны к другим объектам</summary>
    public static class TagExtentions
    {
        /// <summary>Получить объект-метку у казанного типа из целевого объекта</summary>
        /// <typeparam name="TTag">Тип объекта-метки</typeparam>
        /// <param name="o">Целевой объект</param>
        /// <returns>Объект метка, если он существует в указанном объекте</returns>
        [CanBeNull]
        [DST]
        public static TTag GetTag<TTag>([NotNull] this object o)
        {
            Contract.Requires(o != null);
            return TagPool.Tag<TTag>(o);
        }

        /// <summary>Установить объект-метку для указанного объекта</summary>
        /// <typeparam name="TTag">Тип объекта-метки</typeparam>
        /// <param name="o">Целевой объект</param>
        /// <param name="Tag">Объект-метка, прикладываемый к целевому объекту</param>
        [DST]
        public static void SetTag<TTag>([NotNull] this object o, [CanBeNull] TTag Tag)
        {
            Contract.Requires(o != null);
            TagPool.SetTag(o, Tag);
        }

        #region Nested type: TagPool

        /// <summary>Пул меток</summary>
        private static class TagPool
        {
            /// <summary>Объект межпотоковой синхронизации</summary>
            [NotNull] private static readonly object __Lock = new object();
            /// <summary>Словарь меток</summary>
            [CanBeNull] private static Dictionary<WeakReference, Dictionary<Type, object>> __Tags;

            /// <summary>Установить метку указанному объекту</summary>
            /// <param name="o">Целевой объект</param>
            /// <param name="Tag">Добавляемая метка</param>
            public static void SetTag([NotNull] object o, [CanBeNull] object Tag)
            {
                lock (__Lock)
                {
                    var tags = __Tags ?? (__Tags = new Dictionary<WeakReference, Dictionary<Type, object>>());

                    tags.Keys.Where(w => !w.IsAlive).ToArray().Foreach(w => tags.Remove(w));
                    bool Selector(WeakReference w) => o.Equals(w.Target);

                    var reference = tags.Keys.Find(Selector);
                    Dictionary<Type, object> dictionary;
                    if (reference != null)
                        dictionary = tags[reference];
                    else
                        tags.Add(new WeakReference(o), dictionary = new Dictionary<Type, object>());

                    var type = Tag?.GetType() ?? typeof(object);

                    if (dictionary.Keys.Contains(type))
                        dictionary[type] = Tag;
                    else
                        dictionary.Add(type, Tag);
                }
            }

            /// <summary>Получить метку указанного типа для указанного объекта</summary>
            /// <typeparam name="TTagType">Тип объекта-метки</typeparam>
            /// <param name="o">Целевой объект</param>
            /// <returns>Объект-метка</returns>
            [CanBeNull]
            public static TTagType Tag<TTagType>([NotNull] object o)
            {
                bool Selector(WeakReference w) => o.Equals(w.Target);
                bool IsAlive(WeakReference w) => !w.IsAlive;
                lock (__Lock)
                {
                    var tags = __Tags;
                    tags?.Keys.Where(IsAlive).Foreach(w => tags.Remove(w));
                    var reference = tags?.Keys.Find(Selector);
                    if (reference is null) return default;
                    var dictionary = tags[reference];
                    if (!dictionary.TryGetValue(typeof(TTagType), out var result)) return default;
                    return (TTagType)result;
                }
            }
        }

        #endregion
    }
}