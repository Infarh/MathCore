using System;

namespace MathCore.Vectors
{
    public class VectorND<T>
    {
        private readonly VectorND<T>[] _Demention;

        public VectorND<T> this[int i] => _Demention[i];

        public T[] Values { get; }

        public bool IsFinite => _Demention is null;

        public VectorND(int[] Dementions) : this(Dementions, 0) { }

        private VectorND(int[] Dementions, int i)
        {
            if(Dementions is null) throw new ArgumentNullException(nameof(Dementions));
            if(i + 1 > Dementions.Length) 
                throw new ArgumentOutOfRangeException(nameof(i), "«апрашиваемый индекс оси превышает указанную размерность");

            if(i + 1 == Dementions.Length)
            {
                _Demention = null;
                Values = new T[Dementions[i]];
                return;
            }

            var lenght = Dementions[i];
            if(lenght < 1)
                throw new ArgumentOutOfRangeException(nameof(Dementions), $"–азмер по оси {i} не может быть меньше 1"); 

            _Demention = new VectorND<T>[lenght];
            for(var j = 0; j < lenght; j++)
                _Demention[j] = new VectorND<T>(Dementions, i);
        }
    }
}