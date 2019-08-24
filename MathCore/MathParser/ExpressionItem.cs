using System.ComponentModel;
using System.Runtime.CompilerServices;
using MathCore.Annotations;

namespace MathCore.MathParser
{
    /// <summary>Элемент математического выражения</summary>
    public abstract class ExpressionItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string PeoprtyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PeoprtyName));

        [NotifyPropertyChangedInvocator]
        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }

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
}