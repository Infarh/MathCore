using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using MathCore.WPF.ViewModels;

namespace MathCore.Tests.WPF.ViewModels;

internal class Test1WindowViewModel() : TitledViewModel("Тестовое окно")
{
    public SelectableCollection<Test1StudentViewModel> Students1 { get; } =
    [
        new() { Id = 1, Name = "Иванов1", Description = "123", Rating = 5 },
        new() { Id = 2, Name = "Петров1", Description = "321", Rating = 4 },
        new() { Id = 3, Name = "Сидоров1", Description = "000", Rating = 3 },
    ];

    public ObservableCollection<Test1StudentViewModel> Students2 { get; } =
    [
        new() { Id = 1, Name = "Иванов2", Description = "123", Rating = 5 },
        new() { Id = 2, Name = "Петров2", Description = "321", Rating = 4 },
        new() { Id = 3, Name = "Сидоров2", Description = "000", Rating = 3 },
    ];
}

internal class Test1StudentViewModel : ViewModel
{
    [DisplayName("№")]
    public int Id { get; set => Set(ref field, value); }

    [DisplayName("Имя")]
    [field: AllowNull]
    public string Name { get; set => Set(ref field, value); }

    [DisplayName("Описание")]
    [field: AllowNull]
    public string Description { get; set => Set(ref field, value); }

    [DisplayName("Оценка")]
    public double Rating { get; set => Set(ref field, value); }
}
