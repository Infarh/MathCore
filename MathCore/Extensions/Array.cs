#nullable enable

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Статический класс для работы с типизированными массивами</summary>
/// <typeparam name="T">Тип элементов массива</typeparam>
public static class Array<T>
{
    /// <summary>Создать генератор массивов для указанной длины</summary>
    /// <param name="Length">Требуемая длина массива</param>
    /// <returns>Генератор массивов, создающий массивы указанной длины</returns>
    public static Creator Length(int Length) => new(Length);

    /// <summary>Создать генератор массивов для указанной длины</summary>
    /// <param name="Length">Требуемая длина массива</param>
    /// <param name="Initializer">Функция инициализации элементов массива</param>
    /// <returns>Генератор массивов, создающий массивы указанной длины</returns>
    public static Creator Length(int Length, Func<int, T>? Initializer) => new(Length, Initializer);

    /// <summary>Объект, позволяющий создавать массивы и инициализировать их элементами</summary>
    public class Creator
    {
        /// <summary>Длина создаваемого массива</summary>
        private readonly int _Length;

        /// <summary>Функция инициализации элементов массива</summary>
        private Func<int, T>? _Initializer;

        /// <summary>Функция обновления значений элементов массива</summary>
        private Func<int, T, T>? _Updater;

        /// <summary>Устанавливать значения ячеек массива в значение по умолчанию</summary>
        private bool _SetDefaultValue;

        /// <summary>Значение по умолчанию для элементов массива</summary>
        private T? _DefaultValue;

        /// <summary>Инициализация нового экземпляра <see cref="Creator"/></summary>
        /// <param name="Length">Длина массива, создаваемого объектом</param>
        public Creator(int Length)
        {
            if (Length < 0) throw new ArgumentOutOfRangeException(nameof(Length), Length, "Длина массива должна быть неотрицательной");
            _Length = Length;
        }

        /// <summary>Инициализация нового экземпляра <see cref="Creator"/></summary>
        /// <param name="Length">Длина массива, создаваемого объектом</param>
        /// <param name="Initializer">Функция инициализации ячеек массива</param>
        public Creator(int Length, Func<int, T>? Initializer)
        {
            if (Length < 0) throw new ArgumentOutOfRangeException(nameof(Length), Length, "Длина массива должна быть неотрицательной");
            _Length      = Length;
            _Initializer = Initializer;
        }

        /// <summary>Установить значение по умолчанию для ячеек массива</summary>
        /// <param name="DefaultValue">Значение по умолчанию для ячеек массива</param>
        /// <returns>Исходный экземпляр <see cref="Creator"/></returns>
        public Creator Default(T DefaultValue)
        {
            _SetDefaultValue = true;
            _DefaultValue    = DefaultValue;
            return this;
        }

        /// <summary>Сбросить значение по умолчанию</summary>
        /// <returns>Исходный экземпляр <see cref="Creator"/></returns>
        public Creator ResetDefault()
        {
            _DefaultValue    = default;
            _SetDefaultValue = false;
            return this;
        }

        /// <summary>Задать функцию инициализации ячеек массива</summary>
        /// <param name="Initializer">Функция, генерирующая значение ячейки массива по её индексу</param>
        /// <returns>Исходный экземпляр <see cref="Creator"/></returns>
        public Creator Init(Func<int, T>? Initializer)
        {
            _Initializer = Initializer;
            return this;
        }

        /// <summary>Задать функцию обновления значений ячеек массива</summary>
        /// <param name="Updater">
        /// Функция, принимающая в качестве параметром индекс ячейки и её значение,
        /// результат вызова этой функции будет размещён в ячейке массива с указанным индексом
        /// </param>
        /// <returns>Исходный экземпляр <see cref="Creator"/></returns>
        public Creator Update(Func<int, T, T>? Updater)
        {
            _Updater = Updater;
            return this;
        }

        /// <summary>Создать массив с заданными в объекте параметрами</summary>
        /// <returns>Новый массив, длина и ячейки которого определяются объектом инициализации</returns>
        public T[] Create()
        {
            var result = new T[_Length];

            if (_Initializer is null)
                if (_Updater is null)
                {
                    if (!_SetDefaultValue) return result;
                    for (var i = 0; i < _Length; i++)
                        result[i] = _DefaultValue!;
                }
                else if (_SetDefaultValue)
                    for (var i = 0; i < _Length; i++)
                        result[i] = _Updater(i, _DefaultValue!);
                else
                    for (var i = 0; i < _Length; i++)
                        result[i] = _Updater(i, result[i]);
            else if (_Updater is null)
                for (var i = 0; i < _Length; i++)
                    result[i] = _Initializer(i);
            else
                for (var i = 0; i < _Length; i++)
                    result[i] = _Updater(i, _Initializer(i));

            return result;
        }

        /// <summary>Оператор неявного преобразования <see cref="Creator"/> к типу массива</summary>
        /// <param name="creator">Объект инициализации массива</param>
        public static implicit operator T[]?(Creator? creator) => creator?.Create();
    }
}