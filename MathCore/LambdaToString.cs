using MathCore.Annotations;
// ReSharper disable UnusedType.Global

namespace MathCore
{
    /// <summary>Форматтер строки с помощью лямбда-выражения</summary>
    // ReSharper disable once UnusedMember.Global
    public class LambdaToString : Factory<string>
    {
        /// <summary>Новый лямбда-форматтер</summary>
        /// <param name="CreateMethod">Метод генерации строки</param>
        public LambdaToString(System.Func<string> CreateMethod) : base(CreateMethod) =>_RaiseLastChangedEvents = false; 

        /// <inheritdoc />
        [CanBeNull]
        public override string ToString() => Create();
    }
}