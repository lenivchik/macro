using System;
using System.Collections.Generic;
using System.Linq;

namespace MacroProcessor
{
    public static class CodeParser
    {
        /// <summary>
        /// Парсит массив строк в масссив SourceEntity, но только до появления первого END в качестве операции
        /// </summary>
        public static List<SourceEntity> Parse(string[] strs)
        {
            List<SourceEntity> result = new List<SourceEntity>();
            foreach (string s in strs)
            {
                // пропускаем пустую строку
                if (String.IsNullOrEmpty(s.Trim()))
                    continue;
                string currentString = s.ToUpper().Trim();
                SourceEntity se = new SourceEntity() { SourceString = currentString };

                //разборка метки
                if (currentString.Contains(':') && (!currentString.Contains("BYTE") || currentString.IndexOf(':') < currentString.IndexOf("C'")))
                {
                    se.Label = currentString.Split(':')[0].Trim();
                    currentString = currentString.Remove(0, currentString.Split(':')[0].Length + 1).Trim();
                }

                if (currentString.Split(null as char[], StringSplitOptions.RemoveEmptyEntries).Length > 0)
                {
                    se.Operation = currentString.Split(null as char[], StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                    currentString = currentString.Remove(0, currentString.Split(null as char[], StringSplitOptions.RemoveEmptyEntries)[0].Length).Trim();
                }

                if (se.Operation == "BYTE")
                {
                    se.Operands.Add(currentString.Trim());
                }
                else
                {
                    for (int i = 0; i < currentString.Split(null as char[], StringSplitOptions.RemoveEmptyEntries).Length; i++)
                    {
                        se.Operands.Add(currentString.Split(null as char[], StringSplitOptions.RemoveEmptyEntries)[i].Trim());
                    }
                }

                //название проги или макроса - в поле метки
                if (se.Operands.Count > 0 && se.Operands[0] == "MACRO")
                {
                    se.Label = se.Operation;
                    se.Operation = se.Operands[0];
                    for (int i = 1; i < se.Operands.Count; i++)
                    {
                        se.Operands[i - 1] = se.Operands[i];
                    }
                    se.Operands.RemoveAt(se.Operands.Count - 1);
                }
                result.Add(se);

                //Читаем только до энда
                if (se.Operation == "END")
                {
                    return result;
                }
            }

            return result;
        }
    }
}
