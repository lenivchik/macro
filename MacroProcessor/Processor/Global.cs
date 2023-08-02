using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MacroProcessor
{
    public static class Global
    {
        public static List<GlobalEntity> Entities { get; set; } = new List<GlobalEntity>();
        public static Stack<Dictionary<List<string>, TMOEntity>> WhileVar { get; set; } = new Stack<Dictionary<List<string>, TMOEntity>>();

        /// <summary>
        /// Обновить Global (сделать список Global пустым)
        /// </summary>
        public static void Refresh()
        {
            Global.Entities = new List<GlobalEntity>();
            Global.WhileVar = new Stack<Dictionary<List<string>, TMOEntity>>();
        }

        /// <summary>
        /// Поиск макроса в Global по имени
        /// </summary>
        public static GlobalEntity SearchInGlobal(string name)
        {
            GlobalEntity result = (from GlobalEntity ge in Global.Entities
                                   where ge.Name == name
                                   select ge).SingleOrDefault<GlobalEntity>();
            return result;
        }

        /// <summary>
        /// Есть ли глобальная переменная в Global
        /// </summary>
        public static bool IsInGlobal(string name)
        {
            return !(SearchInGlobal(name) == null);
        }

        /// <summary>
        /// Распечатать Global в таблицу
        /// </summary>
        public static void PrintGlobal(DataGridView dgv)
        {
            dgv.Rows.Clear();
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                dgv.Rows.Remove(dgv.Rows[i]);
            }
            foreach (GlobalEntity e in Global.Entities)
            {
                dgv.Rows.Add(e.Name, e.Value != null ? e.Value.ToString() : "");
            }
        }

        /// <summary>
        /// Распечатать Global в консоль
        /// </summary>
        public static void PrintGlobal()
        {
            foreach (GlobalEntity e in Global.Entities)
            {
                Console.WriteLine(e.Name + " = " + (e.Value != null ? e.Value.ToString() : ""));
            }
        }

    }


    /// <summary>
    /// Элемент - глобальная переменная
    /// </summary>
    public class GlobalEntity
    {
        public string Name { get; set; }
        public int? Value { get; set; }

        public GlobalEntity(string name, int? value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
