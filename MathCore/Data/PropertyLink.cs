using System;
using System.ComponentModel;
using System.Diagnostics;
using MathCore.Annotations;

namespace MathCore.Data
{
    /// <summary>Связь между свойствами</summary>
    /// <typeparam name="TSource">Тип объекта-источника данных</typeparam>
    /// <typeparam name="TDestination">Тип объекта-приёмника данных</typeparam>
    [Copyright("Сергей Тепляков", url = "http://www.rsdn.ru/article/dotnet/Data_Binding_Basics.xml")]
    public class PropertyLink<TSource, TDestination>
    {
        /* -------------------------------------------------------------------------------- */

        public event EventHandler EnableChanged;

        protected virtual void OnEnableChanged(EventArgs Args) => EnableChanged?.Invoke(this, Args);

        /* -------------------------------------------------------------------------------- */

        /// <summary>Источник данных</summary>
        private readonly TSource _Source;

        /// <summary>Приёмник данных</summary>
        private readonly TDestination _Destination;

        /// <summary>Дескриптор свойства источника данных</summary>
        private readonly PropertyDescriptor _SourcePropertyDescriptor;

        /// <summary>Дескриптор свойства приёмника данных</summary>
        private readonly PropertyDescriptor _DestinationPropertyDescriptor;

        private bool _Enable;

        /* -------------------------------------------------------------------------------- */

        /// <summary>Источник данных</summary>
        public TSource Source => _Source;

        /// <summary>Приёмник данных</summary>
        public TDestination Destination => _Destination;

        /// <summary>Дескриптор свойства источника данных</summary>
        public PropertyDescriptor SourceProperty => _SourcePropertyDescriptor;

        /// <summary>Дескриптор свйоства примника данных</summary>
        public PropertyDescriptor DestinationProperty => _DestinationPropertyDescriptor;

        /// <summary>Источник данных</summary>
        [NotNull]
        public Type SourceType => typeof(TSource);

        /// <summary>Приёмник данных</summary>
        [NotNull]
        public Type DestinationType => typeof(TDestination);

        /// <summary>Тип свйоства источника данных</summary>
        public Type SourcePropertyType => _SourcePropertyDescriptor.PropertyType;

        /// <summary>Тип свойства приёмника данных</summary>
        public Type DestinationPropertyType => _DestinationPropertyDescriptor.PropertyType;

        /// <summary>Активатор связи</summary>
        public bool Enable
        {
            get => _Enable;
            set
            {
                if(_Enable == value) return;
                _Enable = value;
                OnEnableChanged(EventArgs.Empty);
            }
        }

        /* -------------------------------------------------------------------------------- */

        /// <summary>Новая связь между свойством источника и приёмника данных</summary>
        /// <param name="Source">Источник данных</param>
        /// <param name="SourcePropertyName">Имя свойства источника данных</param>
        /// <param name="Destination">Приёмник данных</param>
        /// <param name="DestinationPropertyName">Имя свойства приёмника данных</param>
        /// <param name="Enable">Признак активности связи (по умолчанию = true)</param>
        public PropertyLink(TSource Source, [NotNull] string SourcePropertyName, TDestination Destination, [NotNull] string DestinationPropertyName, bool Enable = true)
        {
            _Enable = Enable;

            _Source = Source;
            _Destination = Destination;

            // Получаем экземпляр класса PropertyDescriptor для управления свойством элемента управления
            _DestinationPropertyDescriptor = TypeDescriptor
                .GetProperties(Destination)
                .Find(DestinationPropertyName, true);

            if(_DestinationPropertyDescriptor is null)
                throw new ArgumentException(
                            $"Не удалось найти свойство элемента управления с именем {DestinationPropertyName}", nameof(DestinationPropertyName));

            //if (_DestinationPropertyDescriptor.SupportsChangeEvents)
            //    _DestinationPropertyDescriptor.AddValueChanged(Destination, DestinationPropertyChanged);

            // Получаем экземпляр класса PropertyDescriptor для управления свойством источника данных
            _SourcePropertyDescriptor = TypeDescriptor
                .GetProperties(Source)
                .Find(SourcePropertyName, true);

            if(_SourcePropertyDescriptor is null)
                throw new ArgumentException(
                            $"Не удалось найти свойство {SourcePropertyName} источника данных {Source}", nameof(SourcePropertyName));

            if(_SourcePropertyDescriptor.SupportsChangeEvents)
                _SourcePropertyDescriptor.AddValueChanged(Source, SourcePropertyChanged);

            // Генерация события приведет к установке значения свойства элемента управления
            if(_Enable) SourcePropertyChanged(this, EventArgs.Empty);
        }

        /* -------------------------------------------------------------------------------- */


        ///// <summary>
        ///// Обработчик события изменения свойства элемента управления
        ///// </summary>
        ///// <param name="sender">Источник события</param>
        ///// <param name="e">Аргументы события</param>
        //private void DestinationPropertyChanged(object sender, EventArgs e)
        //{
        //    // получаем новое значение свойства элемента управления
        //    var lv_ControlPropertyValue = _DestinationPropertyDescriptor.GetValue(_Destination);

        //    // Дразу присвоить свойству источника данных новое значение нельзя, т.к. типы свойств могут не совпадать.
        //    // Для этого воспользуемся TypeConverter-ами, которые являются частью класса PropertyDescriptor

        //    Debug.Assert(_SourcePropertyDescriptor.Converter != null,
        //                 "_SourcePropertyDescriptor.Converter != null");
        //    Debug.Assert(_DestinationPropertyDescriptor.Converter != null,
        //        "_DestinationPropertyDescriptor.Converter != null");

        //    if (_DestinationPropertyDescriptor.Converter.CanConvertTo(_SourcePropertyDescriptor.PropertyType))
        //    {
        //        var lv_ConvertedValue = _DestinationPropertyDescriptor.Converter.ConvertTo(
        //            lv_ControlPropertyValue,
        //            _SourcePropertyDescriptor.PropertyType);
        //        изменяем значение свойства источника данных
        //                _SourcePropertyDescriptor.SetValue(_Source, lv_ConvertedValue);
        //    }
        //    else
        //        if (_SourcePropertyDescriptor.Converter.CanConvertFrom(_DestinationPropertyDescriptor.PropertyType))
        //    {
        //        var lv_ConvertedValue = _SourcePropertyDescriptor.Converter.ConvertFrom(lv_ControlPropertyValue);
        //        // изменяем значение свойства источника данных
        //        _SourcePropertyDescriptor.SetValue(_Source, lv_ConvertedValue);
        //    }
        //    else
        //        throw new Exception("Невозможно преобразование типа источника к типу свойства");

        //}


        /// <summary>Обработчик события изменения свойства источника данных</summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргумент события</param>
        private void SourcePropertyChanged(object sender, EventArgs e)
        {
            if(!_Enable) return;
            //получаем новое значение свойства источника данных
            var data_source_value = _SourcePropertyDescriptor.GetValue(_Source);

            // сразу присвоить свойству элемента управление полученное значение 
            // нельзя, т.к. типы свойств могут не совпадать.
            // Воспользуемся TypeConverter-ами, которые являются частью класса PropertyDescriptor

            Debug.Assert(_SourcePropertyDescriptor.Converter != null, "_SourcePropertyDescriptor.Converter != null");
            Debug.Assert(_DestinationPropertyDescriptor.Converter != null, "_DestinationPropertyDescriptor.Converter != null");

            if(_SourcePropertyDescriptor.Converter.CanConvertTo(_DestinationPropertyDescriptor.PropertyType))
            {
                var converted_value = _SourcePropertyDescriptor.Converter.ConvertTo(data_source_value, _DestinationPropertyDescriptor.PropertyType);
                // изменяем значение свойства элемента управления
                _DestinationPropertyDescriptor.SetValue(_Destination, converted_value);
            }
            else
                if(_DestinationPropertyDescriptor.Converter.CanConvertFrom(_SourcePropertyDescriptor.PropertyType))
                {
                    var converted_value = _DestinationPropertyDescriptor.Converter.ConvertFrom(data_source_value);
                    _DestinationPropertyDescriptor.SetValue(_Destination, converted_value);
                }
                else
                    throw new Exception("Невозможно преобразование типа источника к типу свойства");
        }

        /* -------------------------------------------------------------------------------- */

        public override string ToString() =>
            string.Format("{0} {1}:({2}).{3} -{8}-> {4} {5}:({6}).{7}",
                SourcePropertyType.Name, SourceType.Name, Source, _SourcePropertyDescriptor.Name,
                DestinationPropertyType.Name, DestinationType.Name, Destination, _DestinationPropertyDescriptor.Name,
                _Enable ? "" : "x");

        /* -------------------------------------------------------------------------------- */
    }
}