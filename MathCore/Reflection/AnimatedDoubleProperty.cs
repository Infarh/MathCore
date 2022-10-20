using System;

using MathCore.Annotations;
// ReSharper disable UnusedType.Global

namespace MathCore.Reflection;

/// <summary>Анимация вещественного свойства</summary>
/// <typeparam name="TObject">Тип объекта, анимацию свойства которого требуется выполнить</typeparam>
public class AnimatedDoubleProperty<TObject> : AnimatedProperty<TObject, double>
{
    /// <summary>Инициализация нового экземпляра <see cref="AnimatedDoubleProperty{TObject}"/></summary>
    /// <param name="o">Объект, свойство которого анимируется</param>
    /// <param name="Name">Имя анимируемого свойства</param>
    /// <param name="Samples">Число шагов анимации</param>
    /// <param name="Timeout">Временной интервал анимации</param>
    /// <param name="Translator">Функция, вычисляющая очередное значение свойства на основе номера шага анимации и числа шагов</param>
    /// <param name="Private">Искать приватное свойство?</param>
    public AnimatedDoubleProperty(
        TObject o,
        [NotNull] string Name,
        int Samples,
        int Timeout,
        Func<int, int, double> Translator,
        bool Private = false)
        : base(o, Name, Samples, Timeout, Translator, Private) { }
}