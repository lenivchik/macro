using System;
using System.Collections.Generic;

namespace MacroProcessor
{
	public class ConsoleProgram
	{
		public string input_file = Utiliities.CurrentDirectory + "\\source.txt";
		public string output_file = Utiliities.CurrentDirectory + "\\result.txt";

		public SourceCode sourceCode = null;
		public List<string> sourceStrings = null;
		// номер строки
		public int index = 0;
		// false - 1 проход
		public bool mode = false;

		public bool firstEnd = false;
		public bool secondEnd = false;
		public bool step = false;
		public bool isEnd = false;

		/// <summary>
		/// Конструктор. Считывает исходники с файла
		/// </summary>
		public ConsoleProgram(string[] args)
		{
			#region Разбор аргyментов командной строки

			switch (args.Length)
			{
				case 1:
					if (args[0].ToUpper() == "HELP")
					{
						Console.WriteLine(ConsoleProgram.GetUserGuide());
					}
					else
					{
						throw new ConsoleException("Некорректное использование аргументов командной строки");
					}
					break;
				case 2:
					if (args[0].ToUpper() == "INPUT_FILE")
					{
						this.input_file = args[1];
					}
					else if (args[0].ToUpper() == "OUTPUT_FILE")
					{
						this.output_file = args[1];
					}
					else
					{
						throw new ConsoleException("Некорректное использование аргументов командной строки");
					}
					break;
				case 4:
					if (args[0].ToUpper() == "INPUT_FILE")
					{
						this.input_file = args[1];
						if (args[2].ToUpper() == "OUTPUT_FILE")
						{
							this.output_file = args[3];
						}
						else
						{
							throw new ConsoleException("Недопустимый ключ! Должен быть OUTPUT_FILE");
						}
					}
					else if (args[0].ToUpper() == "OUTPUT_FILE")
					{
						this.output_file = args[1];
						if (args[2].ToUpper() == "INPUT_FILE")
						{
							this.input_file = args[3];
						}
						else
						{
							throw new ConsoleException("Недопустимый ключ! Должен быть INPUT_FILE");
						}
					}
					else
					{
						throw new ConsoleException("Некорректное использование аргументов командной строки");
					}
					break;
				default:
					throw new ConsoleException("Неверное количество аргументов");
			}

			#endregion
			string[] temp = null;
			try
			{
				temp = System.IO.File.ReadAllLines(this.input_file);
			}
			catch (Exception e)
			{
				throw new ConsoleException("Не удалось загрузить данные с файла. Возможно путь к файлу указан неверно");
			}
			this.sourceCode = new SourceCode(temp);
			this.sourceStrings = new List<string>(temp);
		}

		/// <summary>
		/// Следующий шаг выполнения проги
		/// </summary>
		public void NextStep()
		{
			if (this.mode == false)
			{
				try
				{
					this.sourceCode.FirstRunStep(this.sourceCode.Entities[index++], TMO.Root);
				}
				catch (ArgumentOutOfRangeException ex)
				{
					TMO.Root.LocalTMO = TMO.Root.Children;
					this.mode = true;
					this.index = 0;
				}
				catch (SPException ex)
				{
					isEnd = true;
					Console.WriteLine("\nСтрока \"" + this.sourceCode.Entities[index - 1].ToString() + "\": " + ex.Message + "\n");
				}
				catch (Exception)
				{
					isEnd = true;
					Console.WriteLine("\nСтрока \"" + this.sourceCode.Entities[index - 1].ToString() + "\n");
				}
			}
			else
			{
				try
				{
					this.sourceCode.SecondRunStep(this.sourceCode.Entities[index++], TMO.Root);
				}
				catch (SPException e)
				{
					isEnd = true;
					Console.WriteLine("\nСтрока \"" + this.sourceCode.Entities[index - 1].ToString() + "\": " + e.Message + "\n");
				}
				catch (Exception)
				{
					isEnd = true;
					Console.WriteLine("\nСтрока \"" + this.sourceCode.Entities[index - 1].ToString() + "\n");
				}
			}
			if (this.index == this.sourceCode.Entities.Count && this.mode == true)
			{
				Console.WriteLine("\nПрограмма успешно завершила работу\n");
				this.isEnd = true;
				return;
			}
		}

		/// <summary>
		/// Первый проход
		/// </summary>
		public void FirstRun()
		{
			if (step == true)
			{
				Console.WriteLine("\nПродолжайте тестирование в пошаговом режиме или перезапустите программу.");
				return;
			}
			if (firstEnd == true)
			{
				Console.WriteLine("\n1 проход уже завершен.");
				return;
			}
			if (secondEnd == true)
			{
				Console.WriteLine("\n2 проход уже завершен. Перезапустите программу");
				return;
			}
			try
			{
				while (true)
				{
					this.NextStep();
					if (this.index == this.sourceCode.Entities.Count && this.mode == false)
					{
						Console.WriteLine("\n1 проход успешно завершен");
						TMO.Root.LocalTMO = TMO.Root.Children;
						this.firstEnd = true;
						return;
					}
				}
			}
			catch (SPException ex)
			{
				isEnd = true;
				Console.WriteLine("\nПроизошла ошибка :" + ex.Message);
			}
		}

		/// <summary>
		/// Второй проход
		/// </summary>
		public void SecondRun()
		{
			if (step == true)
			{
				Console.WriteLine("\nПродолжайте тестирование в пошаговом режиме или перезапустите программу.");
				return;
			}
			if (firstEnd == false)
			{
				Console.WriteLine("\nСначала нужно произвести 1 проход");
				return;
			}
			if (secondEnd == true)
			{
				Console.WriteLine("\n2 проход уже завершен. Перезапустите программу");
				return;
			}
			this.mode = true;
			this.index = 0;
			try
			{
				while (true)
				{
					this.NextStep();
					if (this.index == this.sourceCode.Entities.Count && this.mode == true)
					{
						this.secondEnd = true;
						isEnd = true;
						return;
					}
				}
			}
			catch (SPException ex)
			{
				isEnd = true;
				Console.WriteLine("\nПроизошла ошибка :" + ex.Message);
			}
		}

		/// <summary>
		/// Возвращает строку со справкой по программе
		/// </summary>
		/// <returns></returns>
		public static string GetUserGuide()
		{
			return "Справка по данной программе.\r\n\n " +
						"Доступные ключи: [input_file] [output_file] [help]\r\n\n" +
						"input_file\tКлюч для указания пути к файлу с исходнм текстом\r\n" +
						"output_file\tКлюч для указания пути сохранения результирующего ассемблерного кода\r\n" +
						"help\t\tВызов данной справки.\r\n";
		}

		/// <summary>
		/// Менюшка
		/// </summary>
		public string GetProgGuide()
		{
			return "1 - шаг вперед\n" +
					"2 - выполнить 1 проход\n" +
					"3 - выполнить 2 проход\n" +
					"4 - распечатать Исходный код\n" +
					"5 - распечатать Ассемблерный код\n" +
					"6 - распечатать таблицу глобальных переменных\n" +
					"7 - распечатать ТМО\n" +
					"8 - распечатать Ассемблерный код в файл\n" +
					"9 - начать заново\n" +
					"0 - выход\n";
		}

	}


	public class ConsoleException : Exception
	{
		public ConsoleException(string message)
			: base(message)
		{
		}
	}
}
