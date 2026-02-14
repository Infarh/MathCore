using MathCore.ViewModels;

namespace MathCore.MathParser;

/// <summary>Элемент математического выражения</summary>
public abstract class ExpressionItem : ViewModel
{
    /// <summary>Имя</summary>
    public string Name { get; set => Set(ref field!, value); } = null!;

    /// <summary>Инициализация нового элемента математического выражения</summary>
    protected ExpressionItem() { }

    /// <summary>Инициализация нового элемента математического выражения</summary><param name="Name">Имя элемента</param>
    protected ExpressionItem(string Name) => this.Name = Name;

    /// <summary>Метод определения значения</summary><returns>Численное значение элемента выражения</returns>
    public abstract double GetValue();
}