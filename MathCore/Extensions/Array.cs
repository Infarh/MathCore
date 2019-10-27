using MathCore.Annotations;

namespace System
{
    public static class Array<T>
    {
        [NotNull] public static Creator Length(int Length) => new Creator(Length);

        [NotNull] public static Creator Length(int Length, [CanBeNull] Func<int, T> Initializer) => new Creator(Length, Initializer);

        public class Creator
        {
            private readonly int _Length;
            [CanBeNull] private Func<int, T> _Initializer;
            [CanBeNull] private Func<int, T, T> _Updater;

            private bool _SetDefaultValue;
            private T _DefaultValue;

            public Creator(int Length)
            {
                if (Length < 0) throw new ArgumentOutOfRangeException(nameof(Length), Length, "Длина массива должна быть неотрицательной");
                _Length = Length;
            }

            public Creator(int Length, Func<int, T> Initializer)
            {
                if (Length < 0) throw new ArgumentOutOfRangeException(nameof(Length), Length, "Длина массива должна быть неотрицательной");
                _Length = Length;
                _Initializer = Initializer;
            }

            public Creator Default(T DefaultValue)
            {
                _SetDefaultValue = true;
                _DefaultValue = DefaultValue;
                return this;
            }

            public Creator ResetDefault()
            {
                _DefaultValue = default;
                _SetDefaultValue = false;
                return this;
            }

            public Creator Init([CanBeNull] Func<int, T> Initializer)
            {
                _Initializer = Initializer;
                return this;
            }

            public Creator Update([CanBeNull] Func<int, T, T> Updater)
            {
                _Updater = Updater;
                return this;
            }

            [NotNull]
            public T[] Create()
            {
                var result = new T[_Length];

                if (_Initializer is null)
                    if (_Updater is null)
                    {
                        if (!_SetDefaultValue) return result;
                        for (var i = 0; i < _Length; i++)
                            result[i] = _DefaultValue;
                    }
                    else if (_SetDefaultValue)
                        for (var i = 0; i < _Length; i++)
                            result[i] = _Updater(i, _DefaultValue);
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

            [CanBeNull] public static implicit operator T[]([CanBeNull] Creator creator) => creator?.Create();
        }
    }
}