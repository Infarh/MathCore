using System.Diagnostics;
// ReSharper disable InconsistentNaming

namespace MathCore.Tests;

[TestClass]
public class IntervalTests
{
    /* ------------------------------------------------------------------------------------------ */

    private Random _RandomGenerator;

    /* ------------------------------------------------------------------------------------------ */

    public TestContext TestContext { get; set; }

    #region Дополнительные аттрибуты теста

    //[ClassInitialize]
    //public static void MyClassInitialize(TestContext testContext)  { }

    ////Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup]
    //public static void MyClassCleanup()  {}

    //Use TestInitialize to run code before running each test
    [TestInitialize]
    public void MyTestInitialize() => _RandomGenerator = new();

    //Use TestCleanup to run code after each test has run
    [TestCleanup]
    public void MyTestCleanup() => _RandomGenerator = null;

    #endregion

    /* ------------------------------------------------------------------------------------------ */

    #region Свойства

    [TestMethod, Priority(1), Description("Тестирование свойства нижней границы интервала")]
    public void PropertyMinTest()
    {
        var I = new Interval<double>(-3, 5);
        Assert.AreEqual(-3, I.Min, "Нижняя граница интервала не соответствует установленной в конструкторе");
        Assert.IsTrue(I.Check(-2), $"Величина {-2} чуть выше нижней границы интервала не вошла в интервал {I}");
        I = I.SetMin(0);
        Assert.AreEqual(0, I.Min, "Нижняя граница интервала не соответствует установленной через метод записи свойства");
        Assert.IsFalse(I.Check(-2), $"Величина {-2} чуть ниже нижней границы интервала вошла в интервал {I}");
    }

    [TestMethod, Priority(1), Description("Тестирование свойства верхней границы интервала")]
    public void PropertyMaxTest()
    {
        var I = new Interval<double>(-3, 5);
        Assert.AreEqual(5, I.Max, "Верхняя граница интервала не соответствует установленной в конструкторе");
        Assert.IsFalse(I.Check(6), $"Величина {6} чуть выше верхней границы интервала вошла в интервал {I}");
        I = I.SetMax(7);
        Assert.AreEqual(7, I.Max, "Верхняя граница интервала не соответствует установленной через метод записи свойства");
        Assert.IsTrue(I.Check(6), $"Величина {6} чуть выше верхней границы интервала не вошла в интервал {I}");
    }

    [TestMethod, Priority(1), Description("Тестирование свойства верхней границы интервала")]
    public void PropertyMinIncludeTest()
    {
        var I = new Interval<double>(-3, false, 5, true);
        Assert.IsFalse(I.MinInclude, "Режим обработки нижней границы интервала не соответствует объявленному в конструкторе");
        Assert.IsFalse(I.Check(-3), $"Величина {-3} на нижней границе интервала вошла в интервал {I}");
        I = I.IncludeMin(true);
        Assert.IsTrue(I.MinInclude, "Режим обработки нижней границы интервала не соответствует установленному через свойство");
        Assert.IsTrue(I.Check(-3), $"Величина {-3} на нижней границе интервала не вошла в интервал {I}");
    }

    [TestMethod, Priority(1), Description("Тестирование свойства верхней границы интервала")]
    public void PropertyMaxIncludeTest()
    {
        var I = new Interval<double>(-3, true, 5, false);
        Assert.IsFalse(I.MaxInclude, "Режим обработки верхней границы интервала не соответствует объявленному в конструкторе");
        Assert.IsFalse(I.Check(5), $"Величина {5} на верхней границе интервала вошла в интервал {I}");
        I = I.IncludeMax(true);
        Assert.IsTrue(I.MaxInclude, "Режим обработки верхней границы интервала не соответствует установленному через свойство");
        Assert.IsTrue(I.Check(5), $"Величина {5} на верхней границе интервала не вошла в интервал {I}");
    }

    #endregion

    /* ------------------------------------------------------------------------------------------ */

    #region Конструкторы

    [TestMethod, Priority(1), Description("Тестирование конструктора с двумя параметрами (Min, Max)")]
    public void IntervalConstructor_Min_Max_Test()
    {
        Debug.WriteLine("Тестирование конструктора класса Интервал с двумя параметрами");

        const int min = -5;
        const int max = 5;

        var interval = new Interval<double>(min, max);
        Assert.AreEqual(min, interval.Min, "Минимальное значение установлено некорректно");
        Assert.AreEqual(max, interval.Max, "Максимальное значение установлено некорректно");
        Assert.IsTrue(interval.MinInclude, "Флаг проверки вхождения нижнего предела установлен некорректно");
        Assert.IsTrue(interval.MaxInclude, "Флаг проверки вхождения верхнего предела установлен некорректно");
    }

    [TestMethod, Priority(1), Description("Тестирование конструктора с тремя параметрами (Min, Max, IncludeLimits)")]
    public void IntervalConstructor_Min_Max_IncludeLimits_Test()
    {
        Debug.WriteLine("Тестирование конструктора класса Интервал с тремя параметрами");

        const int min = -5;
        const int max = 5;

        var interval = new Interval<double>(min, max, false);
        Assert.AreEqual(min, interval.Min, "Минимальное значение установлено некорректно");
        Assert.AreEqual(max, interval.Max, "Максимальное значение установлено некорректно");
        Assert.IsFalse(interval.MinInclude, "Флаг проверки вхождения нижнего предела установлен некорректно");
        Assert.IsFalse(interval.MaxInclude, "Флаг проверки вхождения верхнего предела установлен некорректно");

        interval = new(min, max, true);

        Assert.AreEqual(min, interval.Min, "Минимальное значение установлено некорректно");
        Assert.AreEqual(max, interval.Max, "Максимальное значение установлено некорректно");
        Assert.IsTrue(interval.MinInclude, "Флаг проверки вхождения нижнего предела установлен некорректно");
        Assert.IsTrue(interval.MaxInclude, "Флаг проверки вхождения верхнего предела установлен некорректно");
    }

    [TestMethod, Priority(1), Description("Тестирование конструктора с тремя параметрами (Min, IncludeMin, Max)")]
    public void IntervalConstructor_Min_IncludeMin_Max_Test()
    {
        Debug.WriteLine("Тестирование конструктора класса Интервал с тремя параметрами");

        const int min = -5;
        const int max = 5;

        var I = new Interval<double>(min, false, max, true);
        Assert.AreEqual(min, I.Min, $"Минимальное значение установлено некорректно для интервала {I}");
        Assert.AreEqual(max, I.Max, "Максимальное значение установлено некорректно");
        Assert.IsFalse(I.MinInclude, $"Флаг проверки вхождения нижнего предела установлен некорректно для интервала {I}");
        Assert.IsTrue(I.MaxInclude, $"Флаг проверки вхождения верхнего предела установлен некорректно для интервала {I}");

        I = new(min, true, max, true);

        Assert.AreEqual(min, I.Min, $"Минимальное значение установлено некорректно для интервала {I}");
        Assert.AreEqual(max, I.Max, $"Максимальное значение установлено некорректно для интервала {I}");
        Assert.IsTrue(I.MinInclude, $"Флаг проверки вхождения нижнего предела установлен некорректно для интервала {I}");
        Assert.IsTrue(I.MaxInclude, $"Флаг проверки вхождения верхнего предела установлен некорректно для интервала {I}");
    }

    [TestMethod, Priority(1), Description("Тестирование конструктора с четырьмя параметрами (Min, IncludeMin, Max, IncludeMax)")]
    public void IntervalConstructor_Min_IncludeMin_Max_IncludeMax_Test()
    {
        const int min = -5;
        const int max = 5;

        var I = new Interval<double>(min, false, max, false);
        Assert.AreEqual(min, I.Min, $"Минимальное значение установлено некорректно для интервала {I}");
        Assert.AreEqual(max, I.Max, $"Максимальное значение установлено некорректно для интервала {I}");
        Assert.IsFalse(I.MinInclude, $"Флаг проверки вхождения нижнего предела установлен некорректно для интервала {I}");
        Assert.IsFalse(I.MaxInclude, $"Флаг проверки вхождения верхнего предела установлен некорректно для интервала {I}");

        I = new(min, true, max, false);

        Assert.AreEqual(min, I.Min, $"Минимальное значение установлено некорректно для интервала {I}");
        Assert.AreEqual(max, I.Max, $"Максимальное значение установлено некорректно для интервала {I}");
        Assert.IsTrue(I.MinInclude, $"Флаг проверки вхождения нижнего предела установлен некорректно для интервала {I}");
        Assert.IsFalse(I.MaxInclude, $"Флаг проверки вхождения верхнего предела установлен некорректно для интервала {I}");

        I = new(min, true, max, true);

        Assert.AreEqual(min, I.Min, $"Минимальное значение установлено некорректно для интервала {I}");
        Assert.AreEqual(max, I.Max, $"Максимальное значение установлено некорректно для интервала {I}");
        Assert.IsTrue(I.MinInclude, $"Флаг проверки вхождения нижнего предела установлен некорректно для интервала {I}");
        Assert.IsTrue(I.MaxInclude, $"Флаг проверки вхождения верхнего предела установлен некорректно для интервала {I}");

        I = new(min, false, max, true);

        Assert.AreEqual(min, I.Min, $"Минимальное значение установлено некорректно для интервала {I}");
        Assert.AreEqual(max, I.Max, $"Максимальное значение установлено некорректно для интервала {I}");
        Assert.IsFalse(I.MinInclude, $"Флаг проверки вхождения нижнего предела установлен некорректно для интервала {I}");
        Assert.IsTrue(I.MaxInclude, $"Флаг проверки вхождения верхнего предела установлен некорректно для интервала {I}");
    }

    #endregion

    /* ------------------------------------------------------------------------------------------ */

    #region Методы

    [TestMethod, Priority(1), Description("Тестирование метода проверки на вхождение величины в интервал")]
    public void CheckTest()
    {
        var I = new Interval<double>(-5, 5);
        Assert.IsFalse(I.Check(-7), $"Величина, лежащая ниже интервала, вошла в интервал {I}");
        Assert.IsTrue(I.Check(-5), $"Величина, лежащая на нижней границе интервала, не вошла в интервал {I}");
        Assert.IsTrue(I.Check(3), $"Величина не вошла в интервал {I}");
        Assert.IsTrue(I.Check(5), $"Величина, лежащая на верхней границе интервала, не вошла в интервал {I}");
        Assert.IsFalse(I.Check(7), $"Величина, лежащая выше интервала, вошла в интервал {I}");

        I = new(-5, 5, true);
        Assert.IsFalse(I.Check(-100), $"Величина, лежащая ниже интервала, вошла в интервал {I}");
        Assert.IsTrue(I.Check(-5), $"Величина, лежащая на нижней границе интервала, не вошла в интервал {I}");
        Assert.IsTrue(I.Check(4), $"Величина не вошла в интервал {I}");
        Assert.IsTrue(I.Check(5), $"Величина, лежащая на верхней границе интервала, не вошла в интервал {I}");
        Assert.IsFalse(I.Check(8), $"Величина, лежащая выше интервала, вошла в интервал {I}");

        I = new(-5, false, 5, true);
        Assert.IsFalse(I.Check(-6), $"Величина, лежащая ниже интервала, вошла в интервал {I}");
        Assert.IsFalse(I.Check(-5), $"Величина, лежащая на нижней границе интервала, вошла в интервал {I}");
        Assert.IsTrue(I.Check(0), $"Величина не вошла в интервал {I}");
        Assert.IsTrue(I.Check(5), $"Величина, лежащая на верхней границе интервала, не вошла в интервал {I}");
        Assert.IsFalse(I.Check(12), $"Величина, лежащая выше интервала, вошла в интервал {I}");
    }

    [TestMethod, Priority(1), Description("Тестирование метода определения эквивалентности объектов типа интервал")]
    public void Equals_Object_Test()
    {
        var I = new Interval<double>(-5, 5);
        Assert.IsTrue(I.Equals((object)I));
        Assert.IsTrue(I.Equals((object)new Interval<double>(-5, 5)));
        Assert.IsFalse(I.Equals(null));
        Assert.IsFalse(I.Equals(new object()));
        Assert.IsFalse(I.Equals((object)new Interval<double>(-7, 5)));
        Assert.IsFalse(I.Equals((object)new Interval<double>(-5, 8)));
        Assert.IsFalse(I.Equals((object)new Interval<double>(-5, 5, false)));
        Assert.IsFalse(I.Equals((object)new Interval<double>(-5, false, 5, true)));
        Assert.IsFalse(I.Equals((object)new Interval<double>(-5, true, 5, false)));
    }

    [TestMethod, Priority(1), Description("Тестирование метода определения эквивалентности интервалов")]
    public void Equals_T_Test()
    {
        var I = new Interval<double>(-5, 5);
        Assert.IsTrue(I.Equals(I));
        Assert.IsTrue(I.Equals(new(-5, 5)));
        Assert.IsFalse(I.Equals(new(-7, 5)));
        Assert.IsFalse(I.Equals(new(-5, 8)));
        Assert.IsFalse(I.Equals(new(-5, 5, false)));
        Assert.IsFalse(I.Equals(new(-5, false, 5, true)));
        Assert.IsFalse(I.Equals(new(-5, true, 5, false)));
    }

    [TestMethod, Priority(1), Description("Тестирования метода определения вхождения интервала в интервал")]
    public void IsIncludeTest()
    {
        var I1 = new Interval<double>(-5, 5);
        var I2 = new Interval<double>(-3, 4);
        Assert.IsTrue(I1.IsInclude(I2), $"Интервал {I1} не входит в интервал {I2}");
        Assert.IsFalse(I2.IsInclude(I1), $"Интервал {I2} входит в интервал {I1}");

        var I3 = new Interval<double>(-7, 2);
        Assert.IsFalse(I1.IsInclude(I3), $"Интервал {I1} входит в интервал {I3}");
        Assert.IsFalse(I3.IsInclude(I1), $"Интервал {I3} входит в интервал {I1}");

        var I4 = new Interval<double>(-5, 5, false);
        Assert.IsFalse(I4.IsInclude(I1), $"Интервал {I4} не входит в интервал {I1}");
        Assert.IsTrue(I1.IsInclude(I4), $"Интервал {I1} входит в интервал {I4}");
    }

    [TestMethod, Priority(1), Description("Тестирования метода определения пересечения интервала с интервалом")]
    public void IsIntersectTest()
    {
        var I1 = new Interval<double>(-3, -1);
        var I2 = new Interval<double>(-1, true, 5, false);
        var I3 = new Interval<double>(3, 7);

        Assert.IsFalse(I1.IsIntersect(I3), $"Интервал {I1} пересекает интервал {I3}");
        Assert.IsFalse(I3.IsIntersect(I1), $"Интервал {I3} пересекает интервал {I1}");

        Assert.IsTrue(I2.IsIntersect(I3), $"Интервал {I2} не пересекает интервал {I3}");
        Assert.IsTrue(I3.IsIntersect(I2), $"Интервал {I3} не пересекает интервал {I2}");

        Assert.IsTrue(I1.IsIntersect(I2), $"Интервал {I1} не пересекает интервал {I2}");
        Assert.IsTrue(I2.IsIntersect(I1), $"Интервал {I2} не пересекает интервал {I1}");
    }

    [TestMethod, Priority(1), Description("Тестирования метода пороговой обработки значения интервалом")]
    public void NormalizeTest()
    {
        var I = new Interval<double>(-5, 5);
        Assert.AreEqual(-5, I.Normalize(-10), $"Число {-10} не прошло пороговую обработку интервалом {I}");
        Assert.AreEqual(-5, I.Normalize(-5), $"Число {-5} не прошло пороговую обработку интервалом {I}");
        Assert.AreEqual(3, I.Normalize(3), $"Число {3} не прошло пороговую обработку интервалом {I}");
        Assert.AreEqual(5, I.Normalize(5), $"Число {5} не прошло пороговую обработку интервалом {I}");
        Assert.AreEqual(5, I.Normalize(15), $"Число {15} не прошло пороговую обработку интервалом {I}");
    }

    #endregion

    /* ------------------------------------------------------------------------------------------ */

    #region Операторы

    [TestMethod, Priority(1), Description("Тестирования метода преобразования интервала в строку")]
    public void ToStringTest()
    {
        var I = new Interval<double>(-5, 5);
        Assert.AreEqual("[-5;5]", I.ToString(), $"Некорректное преобразование интервала {I} в строку");

        I = new(-3, false, 4, true);
        Assert.AreEqual("(-3;4]", I.ToString(), $"Некорректное преобразование интервала {I} в строку");

        I = new(7, true, 10, false);
        Assert.AreEqual("[7;10)", I.ToString(), $"Некорректное преобразование интервала {I} в строку");

        I = new(0, 15, false);
        Assert.AreEqual("(0;15)", I.ToString(), $"Некорректное преобразование интервала {I} в строку");
    }

    [TestMethod, Priority(1), Description("Тестирования оператора равенства одного интервала другому")]
    public void op_EqualityTest()
    {
        var I1 = new Interval<double>(3, 5);
        var I2 = I1.Clone();
        var I3 = new Interval<double>(5, 7);
        var I4 = I3.IncludeMin(false);

        Assert.IsTrue(I1 == I2, $"Интервал {I1} не равен интервалу {I2}");
        Assert.IsTrue(I2 == I1, $"Интервал {I2} не равен интервалу {I1}");

        Assert.IsFalse(I1 == I3, $"Интервал {I1} не не равен интервалу {I3}");
        Assert.IsFalse(I3 == I1, $"Интервал {I3} не не равен интервалу {I1}");

        Assert.IsFalse(I3 == I4, $"Интервал {I3} не не равен интервалу {I4}");
        Assert.IsFalse(I4 == I3, $"Интервал {I4} не не равен интервалу {I3}");
    }

    [TestMethod, Priority(1), Description("Тестирования оператора неравенства одного интервала другому")]
    public void op_InequalityTest()
    {
        var I1 = new Interval<double>(3, 5);
        var I2 = I1.Clone();
        var I3 = new Interval<double>(5, 7);
        var I4 = I3.IncludeMin(false);

        Assert.IsFalse(I1 != I2, $"Интервал {I1} не не равен интервалу {I2}");
        Assert.IsFalse(I2 != I1, $"Интервал {I2} не не равен интервалу {I1}");

        Assert.IsTrue(I1 != I3, $"Интервал {I1} не не равен интервалу {I3}");
        Assert.IsTrue(I3 != I1, $"Интервал {I3} не не равен интервалу {I1}");

        Assert.IsTrue(I3 != I4, $"Интервал {I3} не не равен интервалу {I4}");
        Assert.IsTrue(I4 != I3, $"Интервал {I4} не не равен интервалу {I3}");
    }

    [TestMethod, Priority(1), Description("Тестирование оператора <(Interval<T>, T)")]
    public void op_LessIntervalValueTest()
    {
        const int min = -5;
        const int max = 5;
        var       I   = new Interval<double>(min, max);
        var       d   = _RandomGenerator.NextDouble() + 0.1;

        Assert.IsTrue(I < max + d, $"Значение {max + d}, больше верхней границы должно быть > интервала {I}");
        Assert.IsFalse(I < max, $"Значение {max}, равное верхней границе не должно быть > интервала {I}");
        I = I.Include(false);
        Assert.IsTrue(I < max, $"Значение {max}, равное нижней границе должно быть > интервала {I}");
        Assert.IsFalse(I < 0, $"Значение {0}, внутри не должно быть > интервала {I}");
    }

    [TestMethod, Priority(1), Description("Тестирование оператора <(T, Interval<T>)")]
    public void op_LessValueIntervalTest()
    {
        const int min = -5;
        const int max = 5;
        var       I   = new Interval<double>(min, max);
        var       d   = _RandomGenerator.NextDouble() + 0.1;

        Assert.IsTrue(min - d < I, $"Значение {min - d}, меньшее нижней границы должно быть < интервала {I}");
        Assert.IsFalse(min < I, $"Значение {min}, равное нижней границе не должно быть < интервала {I}");
        I = I.Include(false);
        Assert.IsTrue(min < I, $"Значение {min}, равное нижней границе должно быть < интервала {I}");
        Assert.IsFalse(0 < I, $"Значение {0}, внутри не должно быть < интервала {I}");
    }

    [TestMethod, Priority(1), Description("Тестирование оператора >(Interval<T>, T)")]
    public void op_MoreIntervalValueTest()
    {
        const int min = -5;
        const int max = 5;
        var       I   = new Interval<double>(min, max);
        var       d   = _RandomGenerator.NextDouble() + 0.1;

        Assert.IsTrue(I > min - d, $"Значение {min - d}, меньшее нижней границы должно быть < интервала {I}");
        Assert.IsFalse(I > min, $"Значение {min}, равное нижней границе не должно быть < интервала {I}");
        I = I.Include(false);
        Assert.IsTrue(I > min, $"Значение {min}, равное нижней границе должно быть < интервала {I}");
        Assert.IsFalse(I > 0, $"Значение {0}, внутри не должно быть < интервала {I}");
    }

    [TestMethod, Priority(1), Description("Тестирование оператора >(T, Interval<T>)")]
    public void op_MoreValueIntervalTest()
    {
        const int min = -5;
        const int max = 5;
        var       I   = new Interval<double>(min, max);
        var       d   = _RandomGenerator.NextDouble() + 0.1;

        Assert.IsTrue(max + d > I, $"Значение {max + d}, больше верхней границы должно быть > интервала {I}");
        Assert.IsFalse(max > I, $"Значение {max}, равное верхней границе не должно быть > интервала {I}");
        I = I.Include(false);
        Assert.IsTrue(max > I, $"Значение {max}, равное нижней границе должно быть > интервала {I}");
        Assert.IsFalse(0 > I, $"Значение {0}, внутри не должно быть > интервала {I}");
    }

    #endregion

    /* ------------------------------------------------------------------------------------------ */
}