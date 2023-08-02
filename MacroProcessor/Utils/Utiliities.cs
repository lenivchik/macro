using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MacroProcessor
{
	public static class Utiliities
	{
		public static string[] Symbols = { "#", "$", "%", "!", "@", "^", "&", "*", "-", "[", "\"", "*", "(", ")", "\\",
			"/", "?", "№", ";", ":", "_", "+", "=", "[", "]", "{", "}", "|", "<", ">", "`", "~", ".", ",", "'", " " };
		public static string[] Directives = { "BYTE", "RESB", "RESW", "WORD" };
		public static string[] AssemblerKeywords = { "START", "END", "MACRO", "MEND", "ADD", "SAVER1", "SAVER2", "LOADR1", "JMP" };
		public static string[] MacroGenerationDirectives = { "WHILE", "ENDW", "IF", "ELSE", "ENDIF", "AIF", "AGO", "GLOBAL", "SET", "INC" };
		public static string[] Keywords = AssemblerKeywords.Concat(MacroGenerationDirectives).ToArray();
		public static string RussianLetters = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
		public static string[] ComparisonSigns = { "==", ">=", "<=", "!=", ">", "<" };
		public static string CurrentDirectory = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

		/// <summary> 
		/// Проверка на директивы препроцессора
		/// </summary>
		public static bool IsDirective(string operation)
		{
			return Utiliities.Directives.Contains(operation);
		}

		/// <summary> 
		/// Проверка на метку
		/// </summary>
		public static bool IsLabel(string label)
		{
			// Should be operation
			if (!Utiliities.IsOperation(label)) return false;

			// Should not be keyword
			if (Utiliities.Keywords.Contains(label)) return false;

			// Should not contains russian symbols
			if (!IsNotRussian(label)) return false;

			// Should not be TMO entity
			if (TMO.IsInTMO(label)) return false;

			// Should not be global variable
			if (Global.IsInGlobal(label)) return false;

			return true;
		}

		public static bool IsMacroLabel(string label)
		{
			// Should be operation
			if (!Utiliities.IsOperation(label)) return false;

			// Should not be keyword
			if (Utiliities.Keywords.Contains(label)) return false;

			// Should not contains russian symbols
			if (!IsNotRussian(label)) return false;

			return true;
		}

		/// <summary> 
		/// Проверка на операцию
		/// </summary>
		public static bool IsOperation(string label)
		{
			// Should be non empty
			if (String.IsNullOrEmpty(label)) return false;

			// Should not begin with digit
			if (char.IsDigit(label[0])) return false;

			// Should not contain incorrect symbols like #, $, ...
			if (label.Any((Func<char, bool>)(x => Utiliities.Symbols.Contains(x.ToString())))) return false;

			// Should not be a register
			if (IsRegistr(label)) return false;

			// Should not be a directive
			if (IsDirective(label)) return false;

			return true;
		}

		/// <summary> 
		/// Проверка на регистр
		/// </summary>
		public static bool IsRegistr(string reg)
		{
			for (int i = 0; i < 16; i++)
			{
				if ("R" + i.ToString() == reg.Trim().ToUpper())
					return true;
			}
			return false;
		}

		/// <summary> 
		/// Проверка на присутствие русских символов
		/// </summary>
		public static bool IsNotRussian(string word)
		{
			return !word.Any((Func<char, bool>)(x => Utiliities.RussianLetters.Contains(x.ToString())));
		}

		#region Comparison coditions for IF-AIF-WHILE

		/// <summary> 
		/// Get condition parts from string like "a > b"
		/// </summary>
		public static void ParseCondition(string str, out int first, out int second, out string sign)
		{
			string[] arr;
			first = 0;
			second = 0;
			sign = "";
			int temp;
			foreach (string sgn in Utiliities.ComparisonSigns)
			{
				if ((arr = str.Split(new string[] { sgn }, StringSplitOptions.None)).Length > 1)
				{
					if (Global.IsInGlobal(arr[0]))
					{
						if (Global.SearchInGlobal(arr[0]).Value == null)
						{
							throw new SPException("Не инициализированная глобальная переменная '" + arr[0] + "' является частью условия");
						}
						else
						{
							first = (int)Global.SearchInGlobal(arr[0]).Value;
						}
					}
					else if (Int32.TryParse(arr[0], out temp) == false)
					{
						throw new SPException("Часть условия '" + arr[0] + "' не глобальная переменная и не число");
					}
					else
					{
						first = Int32.Parse(arr[0]);
					}

					if (Global.IsInGlobal(arr[1]))
					{
						if (Global.SearchInGlobal(arr[1]).Value == null)
						{
							throw new SPException("Не инициализированная глобальная переменная '" + arr[1] + "' является частью условия");
						}
						else
						{
							second = (int)Global.SearchInGlobal(arr[1]).Value;
						}
					}
					else if (Int32.TryParse(arr[1], out temp) == false)
					{
						throw new SPException("Часть условия '" + arr[1] + "' не глобальная переменная и не число");
					}
					else
					{
						second = Int32.Parse(arr[1]);
					}

					sign = sgn;
					return;
				}
			}
			throw new SPException("Неопознан знак сравнения");
		}

		/// <summary> Сравнение
		/// </summary>
		public static bool Compare(string str)
		{
			int first;
			int second;
			string sign;
			ParseCondition(str, out first, out second, out sign);
			switch (sign)
			{
				case ">":
					return first > second;
				case "<":
					return first < second;
				case ">=":
					return first >= second;
				case "<=":
					return first <= second;
				case "==":
					return first == second;
				case "!=":
					return first != second;
				default:
					return false;
			}
		}

		public static bool IsAssemblerDirective(string operation)
		{
			return AssemblerKeywords.Contains(operation);
		}

		#endregion

		public static void PushConditionArgs(string str, TMOEntity te)
		{
			int first, second;
			string sign;
			Utiliities.ParseCondition(str, out first, out second, out sign);
			string[] arr;
			List<string> list = new List<string>();
			if ((arr = str.Split(new string[] { sign }, StringSplitOptions.None)).Length == 2)
			{
				if (Global.IsInGlobal(arr[0]))
				{
					list.Add(arr[0]);
				}
				if (Global.IsInGlobal(arr[1]))
				{
					list.Add(arr[1]);
				}
			}
			Dictionary<List<string>, TMOEntity> dict = new Dictionary<List<string>, TMOEntity>();
			dict.Add(list, te);
			Global.WhileVar.Push(dict);
		}

		/// <summary> 
		/// Проверка на совпадение имен
		/// </summary>
		public static void CheckNames(string name)
		{
			List<string> list = new List<string>();
			foreach (GlobalEntity glob in Global.Entities)
			{
				list.Add(glob.Name);
			}
			foreach (TMOEntity te in TMO.Entities)
			{
				list.Add(te.Name);
				list.AddRange(te.Parameters.Select(e => e.Name));
			}
			if (list.Contains(name))
			{
				throw new SPException("Имя " + name + " уже используется в качестве глобальной переменной, имени или параметра макроса");
			}
		}

		public static SourceEntity Print(SourceEntity str)
		{
			SourceEntity newStr = str.Clone();
			for (int j = 0; j < newStr.Operands.Count; j++)
			{
				if (Global.IsInGlobal(newStr.Operands[j]))
				{
					if (Global.SearchInGlobal(newStr.Operands[j]).Value.HasValue)
						newStr.Operands[j] = Global.SearchInGlobal(newStr.Operands[j]).Value.Value.ToString();
					else
						throw new SPException("Глобальная переменная " + newStr.Operands[j] + " не инициализирована.");
				}
			}
			return newStr;
		}

	}
}
