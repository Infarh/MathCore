#nullable enable
namespace MathCore.IoC.ServiceRegistrations;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ViewModelAttribute : Attribute
{
    public Type ViewModelType { get; }

    public ViewModelAttribute(Type ViewModelType)
    {
        if (!ViewModelType.IsClass) throw new ArgumentException("Модель-представление должна быть классом", nameof(ViewModelType));
        if (ViewModelType.IsAbstract) throw new ArgumentException("Модель-представление не может быть абстрактным типом", nameof(ViewModelType));

        this.ViewModelType = ViewModelType;
    }
}