using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MacroProcessor
{
	public class SourceCode
	{

		#region Поля класса

		// вложенность макроопределений
		public int MacroCount { get; set; }
		public int MacroCountSecond { get; set; }
		public string MacroName { get; set; }
		// Список строк кода
		public List<SourceEntity> Entities { get; set; }
		// результаты первого и второго проходов
		public List<SourceEntity> Result { get; set; }

		/// <summary>
		/// Парсер параметров.
		/// </summary>
		public readonly MacroParametersParser MacroParametersParser;

		private bool _isEnd;

		public SourceCode(string[] strs)
		{
			//парсим строки в объектное представление
			this.Entities = CodeParser.Parse(strs);
			this.Result = new List<SourceEntity>();
			//назначаем родителя для исходных строк
			foreach (SourceEntity se in this.Entities)
			{
				se.Sources = this;
			}
			MacroParametersParser = new MixedMacroParametersParser(this);
		}

		public SourceCode(List<SourceEntity> strs)
		{
			//парсим строки в объектное представление
			this.Entities = strs;
			this.Result = new List<SourceEntity>();
			//назначаем родителя для исходных строк
			foreach (SourceEntity se in this.Entities)
			{
				se.Sources = this;
			}
			MacroParametersParser = new MixedMacroParametersParser(this);
		}

		#endregion

		/// <summary>
		/// Шаг первого прохода
		/// </summary>
		public void FirstRunStep(SourceEntity se, TMOEntity te)
		{
			String operation = se.Operation;
			String label = se.Label;
			List<String> operands = se.Operands;

			CheckSourceEntityFirstRun.CheckLabel(se);
			if (operation == "END")
			{
				CheckSourceEntityFirstRun.CheckEND(se, this.MacroCount);
				this._isEnd = true;
			}
			else if (operation == "MACRO")
			{
				if (this.MacroCount == 0)
				{
					CheckSourceEntityFirstRun.CheckMACRO(se, this.MacroCount);
                    //if (te != TMO.Root)
                    //{
                    //    throw new SPException("Макроопределения внутри макросов запрещены");
                    //}
                    var currentTe = new TMOEntity()
					{
						Name = label,
						Parameters = MacroParametersParser.Parse(operands, label),
					};
					currentTe.ParentMacro = te;
					te.Children.Add(currentTe);
					TMO.Entities.Add(currentTe);
					this.MacroName = label;
				}
				else if (this.MacroCount > 0)
				{
					TMO.SearchInTMO(this.MacroName).Body.Add(se);
				}
				else
				{
					throw new SPException("Некорректное количество директив MACRO и MEND");
				}
				this.MacroCount++;
			}
			else if (operation == "MEND")
			{
				if (this.MacroCount > 1)
				{
					TMO.SearchInTMO(this.MacroName).Body.Add(se);

				}
				CheckSourceEntityFirstRun.CheckMEND(se, MacroCount);
				this.MacroCount--;
				if (this.MacroCount == 0)
				{
					this.MacroName = null;
				}
			}
			else
			{
				if (this.MacroCount > 0)
				{
					TMO.SearchInTMO(this.MacroName).Body.Add(se);
				}
				else if (te == TMO.Root)
				{
					if (operation.In("WHILE", "ENDW", "IF", "ELSE", "ENDIF", "AIF", "AGO"))
					{
						throw new SPException("Использование директивы " + operation + " возможно только в теле макроса: " + se.SourceString);
					}
				}
			}
		}

		/// <summary>
		/// Шаг второго прохода
		/// </summary>
		public void SecondRunStep(SourceEntity se, TMOEntity te)
		{
			String operation = se.Operation;
			String label = se.Label;
			List<String> operands = se.Operands;

			if (operation == "GLOBAL" && this.MacroCountSecond == 0)
			{
				CheckSourceEntitySecondRun.CheckGLOBAL(se);
				if (operands.Count == 1)
					Global.Entities.Add(new GlobalEntity(operands[0], null));
				else
					Global.Entities.Add(new GlobalEntity(operands[0], Int32.Parse(operands[1])));
				return;
			}
			if (operation == "SET" && this.MacroCountSecond == 0)
			{
				CheckSourceEntitySecondRun.CheckSET(se, te);
				Global.SearchInGlobal(se.Operands[0]).Value = Int32.Parse(se.Operands[1]);
				return;
			}
			if (operation == "INC" && this.MacroCountSecond == 0)
			{
				CheckSourceEntitySecondRun.CheckINC(se, te);
				Global.SearchInGlobal(operands[0]).Value++;
				return;
			}
			if (operation == "MEND")
			{
				this.MacroCountSecond--;
				return;
			}
			if (operation == "MACRO")
			{
				this.MacroCountSecond++;
			}
			else if (operation == "END")
			{
				Result.Add(Utiliities.Print(se));
				CheckPseudoMacro();
				this._isEnd = true;
			}
			else
			{
				if (TMO.IsInTMO(operation) && this.MacroCountSecond == 0)
				{
					TMOEntity currentTe = TMO.SearchInTMO(operation);
					currentTe.PreviousMacro = te;
					CheckSourceEntitySecondRun.CheckMacroSubstitution(se, currentTe);
					CheckSourceEntitySecondRun.CheckMacroRun(se, te, currentTe);

					List<SourceEntity> res = currentTe.Invoke(operands);
					foreach (SourceEntity str in res)
					{
						Result.Add(Utiliities.Print(str));
					}
				}
				else
				{
					if (this.MacroCountSecond == 0)
					{
						Result.Add(Utiliities.Print(se));
					}
				}
			}
		}

		/// <summary> 
		/// Check all the source entities from the pseudo macro list, if they are macro call or not
		/// </summary>
		public void CheckPseudoMacro()
		{
			foreach (var se in Result)
			{
				if (!Utiliities.IsDirective(se.Operation) &&
					!Utiliities.IsAssemblerDirective(se.Operation) &&
					!(se.Operands.Count > 0 && se.Operands[0] == "START"))
				{
					throw new SPException($"Операция \"{se.Operation}\" не является ни оператором языка Ассемблера, ни оператором Макроязыка, ни макросом ({se.SourceString})");
				}
			}
		}

		#region Распечатка

		/// <summary>
		/// Распечатывает полностью ассемблерный код без макросов в таблицу
		/// </summary>
		public void PrintAsm(RichTextBox tb)
		{
			tb.Clear();
			foreach (SourceEntity se in this.Result)
			{
				tb.AppendText(se.ToString() + "\n");
			}
		}

		/// <summary>
		/// Распечатывает полностью ассемблерный код без макросов в консоль
		/// </summary>
		public void PrintAsm()
		{
			foreach (SourceEntity se in this.Result)
			{
				Console.WriteLine(se.ToString());
			}
		}

		#endregion
	}


	public static class CheckSourceEntityFirstRun
	{

		/// <summary>
		/// Проверка на метку (может быть пустая или много двоеточий)
		/// </summary>
		/// <param name="se">строка с операцией меткой</param>
		public static void CheckLabel(SourceEntity se)
		{
			if (se.SourceString.Split(':').Length > 2 && se.Operation != "BYTE")
			{
				throw new SPException("Слишком много двоеточий в строке: " + se.SourceString);
			}
			if (se.SourceString.Split(':').Length > 1 && String.IsNullOrEmpty(se.SourceString.Split(':')[0]))
			{
				throw new SPException("Слишком много двоеточий в строке: " + se.SourceString);
			}
		}

		/// <summary>
		/// Проверка строки с операцией MACRO
		/// </summary>
		public static void CheckMACRO(SourceEntity se, int macroCount)
		{
			if (se.SourceString.Contains(":"))
			{
				throw new SPException("При объявлении макроса не должно быть меток: " + se.SourceString);
			}
			if (String.IsNullOrEmpty(se.Label) || !Utiliities.IsLabel(se.Label))
			{
				throw new SPException("Имя макроса " + se.Label + " некорректно: " + se.SourceString);
			}
			if (TMO.IsInTMO(se.Label))
			{
				throw new SPException("Макрос " + se.Label + " уже описан: " + se.SourceString);
			}

			Utiliities.CheckNames(se.Label);
		}

		/// <summary>
		/// Проверка строки с операцией MEND
		/// </summary>
		/// <param name="se">строка с операцией MEND</param>
		public static void CheckMEND(SourceEntity se, int macroCount)
		{
			if (se.Operands.Count != 0)
			{
				throw new SPException("У директивы MEND не должно быть параметров: " + se.SourceString);
			}
			if (!String.IsNullOrEmpty(se.Label))
			{
				throw new SPException("У директивы MEND не должно быть метки: " + se.SourceString);
			}
			if (macroCount <= 0)
			{
				throw new SPException("Некорректное количество директив MACRO и MEND");
			}
		}

		/// <summary>
		/// Проверка строки с операцией END
		/// </summary>
		/// <param name="se">строка с операцией END</param>
		public static void CheckEND(SourceEntity se, int macroCount)
		{
			if (macroCount != 0)
			{
				throw new SPException("Количество директив MACRO и MEND не совпадает: " + se.SourceString);
			}
		}

	}

	public static class CheckSourceEntitySecondRun
	{
		/// <summary>
		/// Проверка макроподстановки
		/// </summary>
		public static void CheckMacroRun(SourceEntity se, TMOEntity parent, TMOEntity child)
		{
			TMOEntity current = parent;
			List<TMOEntity> list = new List<TMOEntity>();
			while (current.PreviousMacro != null)
			{
				if (list.Contains(current))
				{
					throw new SPException("Перекрестные ссылки и рекурсия запрещены.");
				}
				list.Add(current);
				current = current.PreviousMacro;
			}
			if (TMO.IsInTMO(child.Name) && parent.Name == child.Name)
			{
				throw new SPException("Макрос " + child.Name + " не может быть вызван из себя.");
			}
			if (TMO.IsInTMO(child.Name) && !parent.LocalTMO.Contains(child))
			{
				throw new SPException("Макрос " + child.Name + " не входит в область видимости " +
					(parent.Name == "root" ? "основной программы" : "тела макроса " + parent.Name) + ".");
			}
		}

		/// <summary>
		/// Проверка макроподстановки
		/// </summary>
		public static void CheckMacroSubstitution(SourceEntity se, TMOEntity te)
		{
			if (se.Operands.Count != te.Parameters.Count)
			{
				throw new SPException("Некорректное количество параметров. Введено: " + se.Operands.Count + ". Ожидается: " + te.Parameters.Count);
			}
			if (!String.IsNullOrEmpty(se.Label))
			{
				throw new SPException("При макровызове макроса не должно быть меток: " + se.SourceString);
			}
		}

		/// <summary>
		/// Проверка строки с операцией GLOBAL
		/// </summary>
		/// <param name="se">строка с операцией GLOBAL</param>
		public static void CheckGLOBAL(SourceEntity se)
		{
			if (se.Operands.Count > 0 && Global.IsInGlobal(se.Operands[0]))
			{
				throw new SPException("Повторное задание глобальной переменной: " + se.SourceString);
			}
			if (!String.IsNullOrEmpty(se.Label))
			{
				throw new SPException("В описании глобальной переменной метки не нужны: " + se.SourceString);
			}
			if (se.Operands.Count == 2)
			{
				if (!Utiliities.IsLabel(se.Operands[0]))
				{
					throw new SPException("Некорректное имя глобальной переменной: " + se.SourceString);
				}
				int temp;
				if (Int32.TryParse(se.Operands[1], out temp) == false)
				{
					throw new SPException("Некорректное значение глобальной переменной: " + se.SourceString);
				}
			}
			else if (se.Operands.Count == 1)
			{
				if (!Utiliities.IsLabel(se.Operands[0]))
				{
					throw new SPException("Некорректное имя глобальной переменной: " + se.SourceString);
				}
			}
			else
			{
				throw new SPException("Некорректное количество операндов в директиве GLOBAL: " + se.SourceString);
			}
			Utiliities.CheckNames(se.Label);
		}

		/// <summary>
		/// Проверка строки с операцией SET
		/// </summary>
		/// <param name="se">строка с операцией SET</param>
		public static void CheckSET(SourceEntity se, TMOEntity te)
		{
			if (!String.IsNullOrEmpty(se.Label))
			{
				throw new SPException("В директиве SET метки не должно быть: " + se.SourceString);
			}
			if (se.Operands.Count == 2)
			{
				if (!Global.IsInGlobal(se.Operands[0]))
				{
					throw new SPException("Некорректное имя глобальной переменной: " + se.SourceString);
				}
				int temp;
				if (Int32.TryParse(se.Operands[1], out temp) == false)
				{
					throw new SPException("Некорректное значение глобальной переменной: " + se.SourceString);
				}
				foreach (Dictionary<List<string>, TMOEntity> dict in Global.WhileVar)
				{
					if (dict.Keys.First().Contains(se.Operands[0]) && dict.Values.First() != te)
					{
						throw new SPException("Глобальная переменная " + se.Operands[0] + " используется как счетчик в цикле: " + se.SourceString);
					}
				}
			}
			else
			{
				throw new SPException("Некорректное количество операндов в директиве SET: " + se.SourceString);
			}
		}

		/// <summary>
		/// Проверка строки с операцией INC
		/// </summary>
		/// <param name="se">строка с операцией INC</param>
		public static void CheckINC(SourceEntity se, TMOEntity te)
		{
			if (!String.IsNullOrEmpty(se.Label))
			{
				throw new SPException("В директиве INC метки не должно быть: " + se.SourceString);
			}
			if (se.Operands.Count == 1)
			{
				if (!Global.IsInGlobal(se.Operands[0]))
				{
					throw new SPException("Некорректное имя глобальной переменной: " + se.SourceString);
				}
				if (Global.SearchInGlobal(se.Operands[0]).Value == null)
				{
					throw new SPException("Глобальной переменной " + se.Operands[0] + " не присвоено значение.");
				}
				foreach (Dictionary<List<string>, TMOEntity> dict in Global.WhileVar)
				{
					if (dict.Keys.First().Contains(se.Operands[0]) && dict.Values.First() != te)
					{
						throw new SPException("Глобальная переменная " + se.Operands[0] + " используется как счетчик в цикле: " + se.SourceString);
					}
				}
			}
			else
			{
				throw new SPException("Некорректное количество операндов в директиве INC: " + se.SourceString);
			}
		}
	}

}
