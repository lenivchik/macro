using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MacroProcessor
{
    public class GUIProgram
    {
        public SourceCode SourceCode { get; set; }
        public List<string> SourceStrings { get; set; }
        // номер строки
        public int Index { get; set; }
        // false - 1 проход
        public bool Mode { get; set; }

        /// <summary>
        /// Конструктор. Считывает исходники с файла
        /// </summary>
        public GUIProgram(RichTextBox tb)
        {
            TMO.refresh();
            Global.Refresh();
            string[] temp = tb.Text.Split('\n');
            Refresh(temp);
        }

        /// <summary>
        /// Шаг выполнения программы 1 просмотра
        /// </summary>
        public void NextFirstStep()
        {
            try
            {
                this.SourceCode.FirstRunStep(this.SourceCode.Entities[Index++], TMO.Root);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                TMO.Root.LocalTMO = TMO.Root.Children;
                this.Mode = true;
                this.Index = 0;
            }
            catch (SPException ex)
            {
                throw new SPException("Строка \"" + this.SourceCode.Entities[Index - 1].ToString() + "\": " + ex.Message + "\n");
            }
            catch (Exception)
            {
                throw new SPException("Ошибка в строке \"" + this.SourceCode.Entities[Index - 1].ToString() + "\n");
            }
        }

        /// <summary>
        /// Шаг выполнения программы 2 просмотра
        /// </summary>
        public void NextSecondStep(RichTextBox tb)
        {
            try
            {
                this.SourceCode.SecondRunStep(this.SourceCode.Entities[Index++], TMO.Root);
                this.SourceCode.PrintAsm(tb);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Mode = false;
                this.Index = 0;
                this.Refresh(this.SourceStrings.ToArray());
                TMO.refresh();
                Global.Refresh();
            }
            catch (SPException e)
            {
                throw new SPException("Строка \"" + this.SourceCode.Entities[Index - 1].ToString() + "\": " + e.Message + "\n");
            }
            catch (Exception e)
            {
                throw new SPException("Ошибка в строке \"" + this.SourceCode.Entities[Index - 1].ToString() + "\"\n");
            }
        }



        /// <summary>
        /// Обновляет результаты предыдущего прохода
        /// </summary>
        private void Refresh(string[] temp)
        {
            this.SourceCode = new SourceCode(temp);
            this.SourceStrings = new List<string>(temp);
        }

    }
}
