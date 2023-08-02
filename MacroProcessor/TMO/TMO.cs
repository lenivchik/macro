using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MacroProcessor
{
    public static class TMO
    {
        public static List<TMOEntity> Entities { get; private set; } = new List<TMOEntity>();
        public static TMOEntity Root { get; private set; } = new TMOEntity() { ParentMacro = null, Children = new List<TMOEntity>(), Name = "root" };

        /// <summary>
        /// Обновить ТМО (сделать список ТМО пустым)
        /// </summary>
        public static void refresh()
        {
            TMO.Entities = new List<TMOEntity>();
            TMO.Root = new TMOEntity() { ParentMacro = null, Children = new List<TMOEntity>(), Name = "root" };
        }

        /// <summary>
        /// Поиск макроса в ТМО по имени
        /// </summary>
        public static TMOEntity SearchInTMO(string name)
        {
            TMOEntity result = (from TMOEntity te in TMO.Entities
                                where te.Name == name
                                select te).SingleOrDefault<TMOEntity>();
            return result;
        }

        /// <summary>
        /// Есть ли макрос в ТМО
        /// </summary>
        public static bool IsInTMO(string name)
        {
            return !(SearchInTMO(name) == null);
        }

        /// <summary>
        /// Распечатать ТМО в таблицу
        /// </summary>
        public static void PrintTMO(DataGridView dgv)
        {
            dgv.Rows.Clear();
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                dgv.Rows.Remove(dgv.Rows[i]);
            }
            foreach (TMOEntity e in TMO.Entities)
            {
                dgv.Rows.Add(e.Name, e.Body.Count > 0 ? e.Body[0].ToString() : "");
                for (int i = 1; i < e.Body.Count; i++)
                {
                    dgv.Rows.Add(null, e.Body[i].ToString());
                }
            }
        }

        /// <summary>
        /// Распечатать ТМО в консоль
        /// </summary>
        public static void PrintTMO()
        {
            foreach (TMOEntity e in TMO.Entities)
            {
                Console.WriteLine("Макрос    " + e.Name + ":");
                for (int i = 0; i < e.Body.Count; i++)
                {
                    Console.WriteLine(e.Body[i].ToString());
                }
            }
        }
    }
}
