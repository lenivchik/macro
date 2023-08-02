using System;
using System.Linq;

namespace MacroProcessor
{
    public static class ExtensionMethos
    {
        public static bool In<T>(this T entity, params T[] list)
        {
            return list.Contains(entity);
        }
    }

    /// <summary>
    /// Методы расширения класса string.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Проверка на равенство строк без учета регистра.
        /// </summary>
        /// <param name="str1">Первая строка.</param>
        /// <param name="str2">Вторая строка.</param>
        /// <returns>Равны ли строки без учета регистра.</returns>
        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            return str1.Equals(str2, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
