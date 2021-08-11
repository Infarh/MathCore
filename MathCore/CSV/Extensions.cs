using System.Collections.Generic;
using System.IO;
using MathCore.Annotations;

namespace MathCore.CSV
{
    public static class Extensions
    {
        /// <summary>Открыть файл для чтения данных в формате CSV</summary>
        /// <param name="file">Информация о файле данных</param>
        /// <param name="Separator">Символ-разделитель значений</param>
        /// <returns>Объект, осуществляющий извлечение данных из файла</returns>
        public static CSVQuery OpenCSV([NotNull] this FileInfo file, char Separator = ',') => new(file.OpenText, Separator);

        /// <summary>Представить перечисление в виде объекта записи данных в формате CSV</summary>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <param name="items">Элементы, упаковываемые в формат CSV</param>
        /// <param name="Separator">Символ-разделитель значений</param>
        /// <returns>Объект, осуществляющий запись данных в формате CSV</returns>
        public static CSVWriter<T> AsCSV<T>([NotNull] this IEnumerable<T> items, char Separator = ',') => new(items, Separator);
    }
}
