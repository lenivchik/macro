using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MacroProcessor
{
    static class Program
    {

        #region Поля для скрытия/открытия консоли

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        #endregion

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            var handle = GetConsoleWindow();

            if (args.Length == 0)
            {
                // Hide console window
                ShowWindow(handle, SW_HIDE);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            else
            {
                // Show console window
                ShowWindow(handle, SW_SHOW);

                try
                {
                    ConsoleProgram program = new ConsoleProgram(args);
                    Console.WriteLine(program.GetProgGuide());
                    string ch = "";
                    while ((ch = Console.ReadLine().ToUpper().Trim()) != "0")
                    {
                        switch (ch)
                        {
                            case "1":
                                if (!program.isEnd)
                                {
                                    if (program.firstEnd == true || program.secondEnd == true)
                                    {
                                        Console.WriteLine("\nПродолжайте тестирование в пошаговом режиме или перезапустите программу.");
                                        break;
                                    }
                                    program.step = true;
                                    Console.WriteLine("\nОчередной шаг выполнен!\n");
                                    program.NextStep();
                                }
                                else
                                {
                                    Console.WriteLine("\nПрограмма завершила свои действия. Запустите ее заново.\n");
                                }
                                break;
                            case "2":
                                if (!program.isEnd)
                                {
                                    program.FirstRun();
                                    Console.WriteLine();
                                }
                                else
                                {
                                    Console.WriteLine("\nПрограмма завершила свои действия. Запустите ее заново.\n");
                                }
                                break;
                            case "3":
                                if (!program.isEnd)
                                {
                                    program.SecondRun();
                                    Console.WriteLine();
                                }
                                else
                                {
                                    Console.WriteLine("\nПрограмма завершила свои действия. Запустите ее заново.\n");
                                }
                                break;
                            case "4":
                                Console.WriteLine("\nИсходный код\n");
                                foreach (string str in program.sourceStrings)
                                {
                                    Console.WriteLine(str);
                                }
                                Console.WriteLine();
                                break;
                            case "5":
                                Console.WriteLine("\nАссемблерный код\n");
                                program.sourceCode.PrintAsm();
                                Console.WriteLine();
                                break;
                            case "6":
                                Console.WriteLine("\nТаблица глобальных переменных\n");
                                Global.PrintGlobal();
                                Console.WriteLine();
                                break;
                            case "7":
                                Console.WriteLine("\nТМО\n");
                                TMO.PrintTMO();
                                Console.WriteLine();
                                break;
                            case "9":
                                Console.WriteLine("\nОбновлено все\n");
                                TMO.refresh();
                                Global.Refresh();
                                program = new ConsoleProgram(args);
                                program.sourceCode.Result = new List<SourceEntity>();
                                Console.WriteLine();
                                break;
                            case "8":
                                try
                                {
                                    StreamWriter sw = new StreamWriter(program.output_file);
                                    foreach (SourceEntity se in program.sourceCode.Result)
                                    {
                                        sw.WriteLine(se.ToString());
                                    }
                                    sw.Close();
                                    Console.WriteLine("\nЗапись успешна\n");
                                    Process.Start("notepad.exe", program.output_file);
                                }
                                catch
                                {
                                    Console.WriteLine("\nЗапись не успешна, возможно не задан или не найден файл\n");
                                }
                                break;
                            default:
                                Console.WriteLine("\nОшибка! Введен неверный ключ!\n");
                                break;
                        }
                        Console.WriteLine(program.GetProgGuide());
                    }
                }
                catch (ConsoleException ex)
                {
                    Console.WriteLine("\n\nОшибка " + ex.Message + "\n\n");
                    Console.WriteLine(ConsoleProgram.GetUserGuide());
                    Console.WriteLine("\nПрограмма завершила свои действия. Запустите ее заново.\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n\nОшибка" + ex.Message + "\n\n");
                    Console.WriteLine(ConsoleProgram.GetUserGuide());
                    Console.WriteLine("\nПрограмма завершила свои действия. Запустите ее заново.\n");
                }
            }
        }
    }
}
