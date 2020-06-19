namespace MathCore.Values
{
    /// <summary>Сумматор вещественных чисел повышенной точности на основе алгоритма Кохена</summary>
    /// <remarks>
    /// https://ru.wikipedia.org/wiki/Алгоритм_Кэхэна
    /// http://www.machinelearning.ru/wiki/index.php?title=Сложение_большого_множества_чисел%2C_существенно_отличающихся_по_величине
    /// </remarks>
    public class KahanSummator : IAddValue<double>, IResettable
    {
        private double _Value;
        private double _Tail;

        public double Value { get => _Value; set => _Value = value; }

        public double AddValue(double value)
        {
            var delta = value - _Tail;
            var sum = _Value + delta;

            _Tail = sum - _Value - delta;
            return _Value = sum;
        }

        public void Reset()
        {
            _Value = 0;
            _Tail = 0;
        }
    }
}
