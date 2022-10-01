#nullable enable
using MathCore.ViewModels;

namespace MathCore.MathParser;

/// <summary>Элемент математического выражения</summary>
public abstract class ExpressionItem : ViewModel
{
    private string _Name;

    /// <summary>Имя</summary>
    public string Name { get => _Name; set => Set(ref _Name, value); }

    /// <summary>Инициализация нового элемента математического выражения</summary>
    protected ExpressionItem() { }

    /// <summary>Инициализация нового элемента математического выражения</summary><param name="Name">Имя элемента</param>
    protected ExpressionItem(string Name) => this.Name = Name;

    /// <summary>Метод определения значения</summary><returns>Численное значение элемента выражения</returns>
    public abstract double GetValue();
}