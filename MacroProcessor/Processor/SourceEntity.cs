using System;
using System.Collections.Generic;

namespace MacroProcessor
{
    public class SourceEntity
    {
        // строка как string, как в исходниках
        public string SourceString { get; set; }
        public string Label { get; set; }
        public string Operation { get; set; }
        public List<string> Operands { get; set; } = new List<string>();
        // родитель для исходных строк
        public SourceCode Sources { get; set; }

        /// <summary>
        /// Обычное представление строки
        /// </summary>
        public override string ToString()
        {
            string temp = "";
            if (!String.IsNullOrEmpty(this.Label))
            {
                temp += this.Label;
                if (this.Operation == "MACRO")
                {
                    temp += " ";
                }
                else
                {
                    temp += ": ";
                }
            }
            temp += this.Operation;

            foreach (string op in this.Operands)
            {
                temp += " " + op;
            }
            return temp;
        }

        /// <summary>
        /// клонирует строку кода
        /// </summary>
        public SourceEntity Clone()
        {
            return new SourceEntity()
            {
                Label = this.Label,
                Operation = this.Operation,
                Operands = new List<string>(this.Operands),
                Sources = this.Sources,
                SourceString = this.SourceString
            };
        }
    }
}
