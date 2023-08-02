using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroProcessor
{
    public class SPException : Exception
    {
        /// <summary>
        /// Создание исключения для этого курсача
        /// </summary>
        public String errorString;
        public SPException(string message)
            : base(message)
        {
            this.errorString = null;
        }

        /// <summary>
        /// Перегруженный конструктор для макрогенерации
        /// Пока не реализовано :)
        /// </summary>
        /// <param name="errorString">Строка с ошибкой</param>
        public SPException(string message, string errorString)
            : base(message)
        {
            this.errorString = errorString;
        }
    }
}
