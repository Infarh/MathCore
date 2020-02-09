// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    public class DictionaryReadOnly<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _Dictionary;

        public DictionaryReadOnly(IDictionary<TKey, TValue> Dictionary) => _Dictionary = Dictionary;

        #region Implementation of IEnumerable

        /// <summary>
        /// Возвращает перечислитель, выполняющий перебор элементов в коллекции.
        /// </summary>
        /// <returns>
        /// Интерфейс <see cref="T:System.Collections.Generic.IEnumerator`1"/>, который может использоваться для перебора элементов коллекции.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _Dictionary.GetEnumerator();

        /// <summary>
        /// Возвращает перечислитель, который осуществляет перебор элементов коллекции.
        /// </summary>
        /// <returns>
        /// Объект <see cref="T:System.Collections.IEnumerator"/>, который может использоваться для перебора элементов коллекции.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Dictionary).GetEnumerator();

        #endregion

        #region Implementation of ICollection<KeyValuePair<TKey,TValue>>

        /// <summary>
        /// Добавляет элемент в интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">Объект, добавляемый в интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">Интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения.</exception>
        public void Add(KeyValuePair<TKey, TValue> item) => throw new NotSupportedException();

        /// <summary>
        /// Удаляет все элементы из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">Интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения. </exception>
        public void Clear() => throw new NotSupportedException();

        /// <summary>
        /// Определяет, содержит ли интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> указанное значение.
        /// </summary>
        /// <returns>
        /// Значение true, если объект <paramref name="item"/> найден в <see cref="T:System.Collections.Generic.ICollection`1"/>; в противном случае — значение false.
        /// </returns>
        /// <param name="item">Объект, который требуется найти в <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(KeyValuePair<TKey, TValue> item) => _Dictionary.Contains(item);

        /// <summary>
        /// Копирует элементы <see cref="T:System.Collections.Generic.ICollection`1"/> в массив <see cref="T:System.Array"/>, начиная с указанного индекса <see cref="T:System.Array"/>.
        /// </summary>
        /// <param name="array">Одномерный массив <see cref="T:System.Array"/>, в который копируются элементы из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>. Индексация в массиве <see cref="T:System.Array"/> должна начинаться с нуля.</param><param name="arrayIndex">Значение индекса (с нуля) в массиве <paramref name="array"/>, с которого начинается копирование.</param><exception cref="T:System.ArgumentNullException">Параметр <paramref name="array"/> имеет значение null.</exception><exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="arrayIndex"/> меньше 0.</exception><exception cref="T:System.ArgumentException">Массив <paramref name="array"/> является многомерным.-или-
        ///                 Значение индекса массива <paramref name="arrayIndex"/> больше или равно длине массива <paramref name="array"/>.-или-Количество элементов в исходном интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/> превышает размер доступного места, начиная с индекса <paramref name="arrayIndex"/> и до конца массива назначения <paramref name="array"/>.-или-Тип <paramref name="T"/> не может быть автоматически приведен к типу массива назначения <paramref name="array"/>.</exception>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _Dictionary.CopyTo(array, arrayIndex);

        /// <summary>
        /// Удаляет первое вхождение указанного объекта из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// Значение true, если объект <paramref name="item"/> успешно удален из <see cref="T:System.Collections.Generic.ICollection`1"/>, в противном случае — значение false. Этот метод также возвращает значение false, если параметр <paramref name="item"/> не найден в исходном интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">Объект, который необходимо удалить из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">Интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения.</exception>
        public bool Remove(KeyValuePair<TKey, TValue> item) => throw new NotSupportedException();

        /// <summary>
        /// Получает число элементов, содержащихся в интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// Число элементов, содержащихся в интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count => _Dictionary.Count;

        /// <summary>
        /// Получает значение, указывающее, доступен ли интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> только для чтения.
        /// </summary>
        /// <returns>
        /// Значение true, если интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения, в противном случае — значение false.
        /// </returns>
        public bool IsReadOnly => true;

        #endregion

        #region Implementation of IDictionary<TKey,TValue>

        /// <summary>
        /// Определяет, содержится ли элемент с указанным ключом в <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// Значение true, если в <see cref="T:System.Collections.Generic.IDictionary`2"/> содержится элемент с данным ключом; в противном случае — значение false.
        /// </returns>
        /// <param name="key">Ключ, который требуется найти в <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param><exception cref="T:System.ArgumentNullException">Параметр <paramref name="key"/> имеет значение null.</exception>
        public bool ContainsKey(TKey key) => _Dictionary.ContainsKey(key);

        /// <summary>
        /// Добавляет элемент с указанными ключом и значением в <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">Объект, используемый в качестве ключа добавляемого элемента.</param><param name="value">Объект, используемый в качестве значения добавляемого элемента.</param><exception cref="T:System.ArgumentNullException">Параметр <paramref name="key"/> имеет значение null.</exception><exception cref="T:System.ArgumentException">Элемент с таким ключом уже существует в <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception><exception cref="T:System.NotSupportedException">Объект <see cref="T:System.Collections.Generic.IDictionary`2"/> доступен только для чтения.</exception>
        public void Add(TKey key, TValue value) => throw new NotSupportedException();

        /// <summary>
        /// Удаляет элемент с указанным ключом из <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// Значение true, если элемент успешно удален, в противном случае — значение false.  Этот метод также возвращает значение false, если параметр <paramref name="key"/> не найден в исходном объекте <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <param name="key">Ключ элемента, который необходимо удалить.</param><exception cref="T:System.ArgumentNullException">Параметр <paramref name="key"/> имеет значение null.</exception><exception cref="T:System.NotSupportedException">Объект <see cref="T:System.Collections.Generic.IDictionary`2"/> доступен только для чтения.</exception>
        public bool Remove(TKey key) => throw new NotSupportedException();

        /// <summary>
        /// Получает значение, связанное с указанным ключом.
        /// </summary>
        /// <returns>
        /// Значение true, если объект, реализующий <see cref="T:System.Collections.Generic.IDictionary`2"/>, содержит элемент с указанным ключом, в противном случае — значение false.
        /// </returns>
        /// <param name="key">Ключ, значение которого необходимо получить.</param><param name="value">Этот метод возвращает значение, связанное с указанным ключом, если он найден; в противном случае — значение по умолчанию для данного типа параметра <paramref name="value"/>. Этот параметр передается не инициализированным.</param><exception cref="T:System.ArgumentNullException">Параметр <paramref name="key"/> имеет значение null.</exception>
        public bool TryGetValue(TKey key, out TValue value) => _Dictionary.TryGetValue(key, out value);

        /// <summary>
        /// Получает или задает элемент с указанным ключом.
        /// </summary>
        /// <returns>
        /// Элемент с указанным ключом.
        /// </returns>
        /// <param name="key">Ключ элемента, который требуется получить или задать.</param><exception cref="T:System.ArgumentNullException">Параметр <paramref name="key"/> имеет значение null.</exception><exception cref="T:System.Collections.Generic.KeyNotFoundException">Свойство получено и параметр <paramref name="key"/> не найден.</exception><exception cref="T:System.NotSupportedException">Свойство задано, и объект <see cref="T:System.Collections.Generic.IDictionary`2"/> доступен только для чтения.</exception>
        public TValue this[TKey key] { get => _Dictionary[key]; set => throw new NotSupportedException(); }

        /// <summary>
        /// Получает интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/>, содержащий ключи <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// Интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/>, содержащий ключи объекта, который реализует <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<TKey> Keys => _Dictionary.Keys;

        /// <summary>
        /// Получает интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/>, содержащий значения <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// Интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/>, содержащий значения объекта, который реализует <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<TValue> Values => _Dictionary.Values;

        #endregion
    }
}