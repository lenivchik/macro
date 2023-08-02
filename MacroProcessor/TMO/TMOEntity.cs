using System;
using System.Collections.Generic;
using System.Linq;
using MacroProcessor.Utils;

namespace MacroProcessor
{
	public class TMOEntity
	{
		public string Name { get; set; }
		public List<SourceEntity> Body { get; set; }
		public List<MacroParameter> Parameters { get; set; }

		/// <summary>
		/// Первый ключевой параметр.
		/// </summary>
		public int FirstKeyParameterIdx =>
			Array.IndexOf(Parameters?.ToArray() ?? new MacroParameter[] { }, Parameters?.FirstOrDefault(e => e.Type == MacroParameterTypes.Key));


		// для уникальных меток (Метки внутри макроса - да)
		public int UniqueLabelCounter { get; set; }
		// список меток, используемых при AGO
		public List<string> AgoLabels { get; set; }
		// если будет N итераций - считаем это бесконечным циклом:)
		private int Counter { get; set; }
		// При обработке вложенных макроописаний необходимы макрос-родитель и макросы-дети
		public TMOEntity ParentMacro { get; set; }
		public List<TMOEntity> Children { get; set; }
		// Локальная область видимости макросов
		public List<TMOEntity> LocalTMO { get; set; }
		// При обработке перекрестных ссылок необходим параметр, определяющий макрос, из которого бы вызван данный макрос
		public TMOEntity PreviousMacro { get; set; }

		public TMOEntity()
		{
			this.Body = new List<SourceEntity>();
			this.Parameters = new List<MacroParameter>();
			this.AgoLabels = new List<string>();
			this.Counter = 0;
			this.ParentMacro = null;
			this.PreviousMacro = null;
			this.Children = new List<TMOEntity>();
			this.LocalTMO = new List<TMOEntity>();
		}

		/// <summary> Замена параметров в макросе на числа
		/// </summary>
		public List<SourceEntity> ProcessMacroParams(IEnumerable<string> passedParams)
		{
			// First key param index in the params list
			int firstKeyParamIndex = -1;
			var macro = this;

			#region Check correctness of passed params

			if (passedParams.Count() != macro.Parameters.Count)
			{
				throw new SPException(string.Format(
					Messages.IncorrectMacroCallParametersCount, macro.Name, passedParams.Count(), macro.Parameters.Count));
			}

			var first = passedParams.FirstOrDefault(x => x.Contains("="));
			if (first != null)
			{
				firstKeyParamIndex = Array.IndexOf(passedParams.ToArray(), first);
				if (passedParams.Any(x => !x.Contains("=") && Array.IndexOf(passedParams.ToArray(), x) > firstKeyParamIndex))
				{
					throw new SPException(string.Format(Messages.IncorrectMacroCallParameters, macro.Name));
				}
			}
			if (firstKeyParamIndex != macro.FirstKeyParameterIdx)
			{
				throw new SPException(string.Format(Messages.IncorrectMacroCallParameters, macro.Name));
			}

			#endregion

			// формируем локальную область видимости (параметры в виде key-value)
			Dictionary<string, int> dict = new Dictionary<string, int>();

			#region Parse local params

			// Delegate to process positioned params
			Action ParsePositionedParams = delegate ()
			{
				// If there is no key params, get all params as positioned
				int localFirstParamIndex = firstKeyParamIndex >= 0 ? firstKeyParamIndex : passedParams.Count();
				for (int i = 0; i < localFirstParamIndex; i++)
				{
					string currentParam = passedParams.ToArray()[i];
					var variable = Global.Entities.FirstOrDefault(e => e.Name.EqualsIgnoreCase(currentParam));
					if (variable == null && !int.TryParse(currentParam, out int temp))
					{
						throw new SPException(string.Format(Messages.IncorrectMacroParameterValue, currentParam));
					}
					if (variable != null && variable.Value == null)
					{
						throw new SPException(string.Format(Messages.MacroParameterIsEmptyVariable, currentParam));
					}

					int curerntParamValue = variable?.Value ?? int.Parse(currentParam);
					dict.Add(macro.Parameters[i].Name, curerntParamValue);
				}
			};

			// Delegate to process key params
			Action ParseKeyParams = delegate ()
			{
				// If there is no key params - exit
				if (firstKeyParamIndex < 0) return;

				for (int i = firstKeyParamIndex; i < passedParams.Count(); i++)
				{
					string currentParameter = passedParams.ToArray()[i];
					string[] vals = currentParameter.Split('=');
					if (vals.Length != 2)
					{
						throw new SPException(string.Format(Messages.IcorrectMacroCallKeyParameter, macro.Name, currentParameter));
					}

					var macroParameter = macro.Parameters.FirstOrDefault(e => e.Name == vals[0]);
					if (macroParameter == null)
					{
						throw new SPException(string.Format(Messages.ParameterDoesNotExists, vals[0]));
					}
					if (macroParameter.Type != MacroParameterTypes.Key)
					{
						throw new SPException(string.Format(Messages.IncorrectMacroParameterType, vals[0]));
					}

					var passedValue = vals[1];
					int value = 0;
					if (string.IsNullOrEmpty(passedValue))
					{
						// значение не указали
						value = macroParameter.DefaultValue ??
							throw new SPException(string.Format(Messages.IncorrectMacroParameterValue, currentParameter));
					}
					else
					{
						var variable = Global.Entities.FirstOrDefault(e => e.Name.EqualsIgnoreCase(vals[1]));
						if (variable == null && !int.TryParse(vals[1], out int temp))
						{
							throw new SPException(string.Format(Messages.IncorrectMacroParameterValue, currentParameter));
						}
						if (variable != null && variable.Value == null)
						{
							throw new SPException(string.Format(Messages.MacroParameterIsEmptyVariable, currentParameter));
						}
						value = variable?.Value ?? int.Parse(vals[1]);
					}

					if (dict.Keys.Contains(vals[0]))
					{
						throw new SPException(string.Format(Messages.DublicateMacroCallParameter, vals[0]));
					}

					dict.Add(vals[0], value);
				}
			};

			ParsePositionedParams();
			ParseKeyParams();

			#endregion

			var result = macro.Body.Select(e => (SourceEntity)e.Clone()).ToList();

			// замена параметров в макросе на числа
			int macroCount = 0;
			foreach (var sourceLine in result)
			{
				// Вложенные макросы не смотрим (Вложенные макроопределения - да)
				if (sourceLine.Operation == "MACRO") macroCount++;
				if (sourceLine.Operation == "MEND") macroCount--;
				if (macroCount != 0) continue;

				if (sourceLine.Operation.In("IF", "AIF", "WHILE"))
				{
					if (sourceLine.Operands.Count > 0)
					{
						foreach (string sign in Utiliities.ComparisonSigns)
						{
							string[] t = sourceLine.Operands[0].Split(new string[] { sign }, StringSplitOptions.None);
							if (t.Length == 2)
							{
								if (macro.Parameters.Any(e => e.Name == t[0].Trim()))
								{
									t[0] = dict[t[0].Trim()].ToString();
								}
								if (macro.Parameters.Any(e => e.Name == t[1].Trim()))
								{
									t[1] = dict[t[1].Trim()].ToString();
								}
								sourceLine.Operands[0] = t[0] + sign + t[1];
								break;
							}
						}
					}
					else
					{
						throw new SPException("Некорректное использование директивы " + sourceLine.Operation);
					}
				}
				else if (sourceLine.Operation.In("SET", "INC"))
				{
					// Ничего не делаем с операциями SET, INC - они умеют работать только с переменными
					continue;
				}
				else
				{
					for (int i = 0; i < sourceLine.Operands.Count; i++)
					{
						var currentOperand = sourceLine.Operands[i];
						if (dict.Keys.Contains(currentOperand))
						{
							sourceLine.Operands[i] = dict[currentOperand].ToString();
						}
						/**
							Parameters: mixed
							Situation: values of macro params (mixed) transferred to another macro as params (mixed too)
							Example: 
							
							mac macro a
								add r1 a
							mend

							mac2 macro b
								mac a=b
							mend
						*/
						if (currentOperand.Contains("="))
						{
							string[] t = currentOperand.Split(new string[] { "=" }, StringSplitOptions.None);
							if (t.Length == 2)
							{
								if (macro.Parameters.Any(e => e.Name == t[1].Trim()))
								{
									t[1] = dict[t[1].Trim()].ToString();
								}
								sourceLine.Operands[i] = t[0] + "=" + t[1];
							}
						}
					}
				}
			}

			return result;
		}

		/// <summary> 
		/// Выполнение макровызовов и макроподстановок в макросах
		/// </summary>
		public List<SourceEntity> Invoke(IEnumerable<string> passedParams)
		{
			// Замена параметров в макросе на числа
			List<SourceEntity> bodyWithParams = this.ProcessMacroParams(passedParams);

			// Create new source code form current macro
			SourceCode macroSourceCode = new SourceCode(bodyWithParams);

			// первый проход с проверкой всех while-endw
			foreach (SourceEntity se in macroSourceCode.Entities)
			{
				macroSourceCode.FirstRunStep(se, this);
			}

			// образование локальной области видимости макросов
			CheckMacros.CheckLocalTMO();
			// Checks
			CheckMacros.CheckMacroLabels(this);

			#region Unique macro labels

			//заменяем метки на "крутые" уникальные метки (Метки внутри макроса - да)
			List<string> localMacroLabels = new List<string>();
			int macroCount = 0;
			foreach (SourceEntity currentLine in macroSourceCode.Entities)
			{
				if (currentLine.Operation == "MACRO") macroCount++;
				if (currentLine.Operation == "MEND") macroCount--;
				if (macroCount != 0) continue;

				if (!String.IsNullOrEmpty(currentLine.Label))
				{
					if (!Utiliities.IsMacroLabel(currentLine.Label))
					{
						throw new SPException("Некорректное задание метки в макросе: " + currentLine.SourceString);
					}
					if (localMacroLabels.Contains(currentLine.Label))
					{
						throw new SPException("Повторное задание метки в макросе: " + currentLine.SourceString);
					}
					localMacroLabels.Add(currentLine.Label);
				}
			}

			macroCount = 0;
			for (int i = 0; i < macroSourceCode.Entities.Count; i++)
			{
				SourceEntity currentLine = macroSourceCode.Entities[i];
				// Формирование уникальных меток
				if (currentLine.Operation == "MACRO") macroCount++;
				if (currentLine.Operation == "MEND") macroCount--;
				if (macroCount != 0) continue;

				if (!String.IsNullOrEmpty(currentLine.Label))
				{
					currentLine.Label = currentLine.Label + "_" + this.Name + "_" + this.UniqueLabelCounter;
				}
				for (int j = 0; j < currentLine.Operands.Count; j++)
				{
					if (localMacroLabels.Contains(currentLine.Operands[j]))
					{
						currentLine.Operands[j] = currentLine.Operands[j] + "_" + this.Name + "_" + this.UniqueLabelCounter;
					}
				}
			}

			this.UniqueLabelCounter++;

			#endregion

			// Second run
			#region while, if, go...

			// исполнять ли команду дальше
			Stack<bool> runStack = new Stack<bool>();
			runStack.Push(true);
			// стек комманд, появлявшихся ранее
			Stack<Command> commandStack = new Stack<Command>();
			// стек строк, куда надо вернуться при while
			Stack<int> whileStack = new Stack<int>();

			macroCount = 0;
			for (int i = 0; i < macroSourceCode.Entities.Count; i++)
			{
				SourceEntity current = macroSourceCode.Entities[i].Clone();

				this.Counter++;
				if (this.Counter == 100000)
				{
					throw new SPException("Обнаружен бесконечный цикл");
				}

				// Вложенные макросы не обрабатываем
				if (current.Operation == "MACRO")
				{
					macroCount++;
				}
				else if (current.Operation == "MEND")
				{
					macroCount--;
				}
				if (macroCount == 0)
				{
					CheckMacros.CheckInner(current, commandStack);

					if (current.Operation == "IF")
					{
						CheckMacros.CheckIF(this);
						commandStack.Push(Command.IF);
						runStack.Push(runStack.Peek() && Utiliities.Compare(current.Operands[0]));
						continue;
					}
					if (current.Operation == "ELSE")
					{
						CheckMacros.CheckIF(this);
						commandStack.Pop();
						commandStack.Push(Command.ELSE);
						bool elseFlag = runStack.Pop();
						runStack.Push(runStack.Peek() && !elseFlag);
						continue;
					}
					if (current.Operation == "ENDIF")
					{
						CheckMacros.CheckIF(this);
						commandStack.Pop();
						runStack.Pop();
						continue;
					}
					if (current.Operation == "WHILE")
					{
						CheckMacros.CheckWhileEndw(this);
						if (current.Operands.Count == 1)
							Utiliities.PushConditionArgs(current.Operands[0], this);

						commandStack.Push(Command.WHILE);
						runStack.Push(runStack.Peek() && Utiliities.Compare(current.Operands[0]));
						whileStack.Push(i);
						continue;
					}
					if (current.Operation == "ENDW")
					{
						CheckMacros.CheckWhileEndw(this);
						commandStack.Pop();
						int newI = whileStack.Pop() - 1;
						if (runStack.Pop())
						{
							i = newI;
						}
						continue;
					}
					if (current.Operation.In("AIF", "AGO"))
					{
						// If AIF condition is false, skip current line
						if (current.Operation == "AIF" && !Utiliities.Compare(current.Operands[0])) continue;

						CheckMacros.CheckAIF(this);
						if (runStack.Peek())
						{
							string label = current.Operation == "AIF" ? current.Operands[1] : current.Operands[0];
							// находим метку, чтобы туда прыгнуть
							bool ready = false;
							Stack<bool> agoStack = new Stack<bool>();

							// вверх
							int localMacroCount = 0;
							for (int j = i; j >= 0; j--)
							{
								// Вложенные макросы не смотрим
								if (macroSourceCode.Entities[j].Operation == "MACRO") localMacroCount++;
								if (macroSourceCode.Entities[j].Operation == "MEND") localMacroCount--;
								if (localMacroCount != 0) continue;

								if (macroSourceCode.Entities[j].Operation?.In("WHILE", "IF") == true)
								{
									if (agoStack.Count > 0)
									{
										agoStack.Pop();
									}
								}
								if (macroSourceCode.Entities[j].Operation == "ELSE")
								{
									if (agoStack.Count > 0)
									{
										agoStack.Pop();
									}
									agoStack.Push(false);
								}
								if (macroSourceCode.Entities[j].Operation?.In("ENDIF", "ENDW") == true)
								{
									agoStack.Push(false);
								}
								if (macroSourceCode.Entities[j].Label == label && (agoStack.Count == 0 || agoStack.Peek()))
								{
									i = j - 1;
									ready = true;
									break;
								}
							}

							// вниз
							if (!ready)
							{
								localMacroCount = 0;
								for (int j = i; j < macroSourceCode.Entities.Count; j++)
								{
									// Вложенные макросы не смотрим
									if (macroSourceCode.Entities[j].Operation == "MACRO") localMacroCount++;
									if (macroSourceCode.Entities[j].Operation == "MEND") localMacroCount--;
									if (localMacroCount != 0) continue;

									if (macroSourceCode.Entities[j].Operation?.In("WHILE", "IF") == true)
									{
										agoStack.Push(false);
									}
									if (macroSourceCode.Entities[j].Operation == "ELSE")
									{
										if (agoStack.Count > 0)
										{
											agoStack.Pop();
										}
										agoStack.Push(false);
									}
									if (macroSourceCode.Entities[j].Operation?.In("ENDIF", "ENDW") == true)
									{
										if (agoStack.Count > 0)
										{
											agoStack.Pop();
										}
									}
									if (macroSourceCode.Entities[j].Label == label && (agoStack.Count == 0 || agoStack.Peek()))
									{
										i = j - 1;
										ready = true;
										break;
									}
								}
							}
							if (!ready)
							{
								throw new SPException("Метка " + label + " при директиве " + current.Operation +
									" находится вне зоны видимости или не описана");
							}
						}
						continue;
					}
				}
				if (runStack.Peek())
				{
					macroSourceCode.SecondRunStep(current, this);
				}
			}

			#endregion

			// Список меток, которые уже найдены
			List<string> markedLabels = new List<string>();
			macroCount = 0;
			foreach (SourceEntity se in macroSourceCode.Result)
			{
				if (se.Operation == "MACRO") macroCount++;
				if (se.Operation == "MEND") macroCount--;
				if (macroCount != 0) continue;

				if (!String.IsNullOrEmpty(se.Label))
				{
					if (markedLabels.Contains(se.Label))
					{
						throw new SPException("Повторное описание метки " + se.Label + " в макросе " + this.Name);
					}
					markedLabels.Add(se.Label);
				}
			}

			return macroSourceCode.Result;
		}

	}

	public enum Command
	{
		IF,
		ELSE,
		WHILE,
		AGO,
		EMPTY
	}

	public static class CheckMacros
	{
		/// <summary>
		/// Проверяет макрос на наличие меток
		/// </summary>
		public static void CheckMacroLabels(TMOEntity te)
		{
			List<SourceEntity> result = new List<SourceEntity>();

			// Вложенные макросы не обрабатываем
			int macroCount = 0;
			foreach (SourceEntity se in te.Body)
			{
				if (se.Operation == "MACRO")
				{
					macroCount++;
				}
				else if (se.Operation == "MEND")
				{
					macroCount--;
				}
				if (macroCount == 0)
				{
					result.Add(se.Clone());
				}
			}

			// Определяем метки, используемые при AIF-AGO
			foreach (SourceEntity se in result)
			{
				if (se.Operation == "AGO" && se.Operands.Count > 0 && !te.AgoLabels.Contains(se.Operands[0]))
				{
					te.AgoLabels.Add(se.Operands[0]);
				}
				if (se.Operation == "AIF" && se.Operands.Count > 1 && !te.AgoLabels.Contains(se.Operands[1]))
				{
					te.AgoLabels.Add(se.Operands[1]);
				}
			}

			// Список меток, которые являются частью AGO, и уже найдены
			List<string> markedLabels = new List<string>();
			foreach (SourceEntity se in result)
			{
				if (!String.IsNullOrEmpty(se.Label) && se.Operation?.ToUpper() != "MACRO")
				{
					if (markedLabels.Contains(se.Label))
					{
						throw new SPException("Повторное описание метки " + se.Label + " в макросе " + te.Name);
					}
					markedLabels.Add(se.Label);
				}
			}

			// Все ли метки найдены
			foreach (string agoLabel in te.AgoLabels)
			{
				if (!markedLabels.Contains(agoLabel))
				{
					throw new SPException("Ошибка использования директивы AGO или AIF. " +
						"Метка в пределах макроса " + te.Name + " не найдена.");
				}
			}
		}

		/// <summary>
		/// Локальная область видимости макросов. Из parent можно вызвать только макросы localTMO
		/// </summary>
		public static void CheckLocalTMO()
		{
			foreach (TMOEntity te in TMO.Entities)
			{
				te.LocalTMO.Clear();
				TMOEntity current = te;
				while (current != TMO.Root)
				{
					te.LocalTMO.AddRange(current.Children);
					current = current.ParentMacro;
				}
				te.LocalTMO.AddRange(current.Children);
				te.LocalTMO.Remove(te);
			}
			TMO.Root.LocalTMO = TMO.Root.Children;
		}

		/// <summary>
		/// Проверка макроса на WHILE-ENDW
		/// </summary>
		public static void CheckWhileEndw(TMOEntity te)
		{
			int whileCount = 0;

			// Вложенные макросы не обрабатываем
			List<SourceEntity> result = new List<SourceEntity>();
			int macroCount = 0;
			foreach (SourceEntity se in te.Body)
			{
				if (se.Operation == "MACRO")
				{
					macroCount++;
				}
				else if (se.Operation == "MEND")
				{
					macroCount--;
				}
				if (macroCount == 0)
				{
					result.Add(se.Clone());
				}
			}

			//проверка корректности WHILE-ENDW
			try
			{
				foreach (SourceEntity str in result)
				{
					if (str.Operation == "WHILE")
					{
						if (str.Operands.Count != 1)
						{
							throw new SPException("Некорректное количество операндов директивы WHILE: " + str.SourceString);
						}
						if (!String.IsNullOrEmpty(str.Label))
						{
							throw new SPException("При директиве WHILE метки быть не должно: " + str.SourceString);
						}
						whileCount++;
					}
					else if (str.Operation == "ENDW")
					{
						if (str.Operands.Count != 0)
						{
							throw new SPException("Некорректное количество операндов директивы ENDW: " + str.SourceString);
						}
						if (!String.IsNullOrEmpty(str.Label))
						{
							throw new SPException("При директиве ENDW метки быть не должно: " + str.SourceString);
						}
						whileCount--;
						if (whileCount < 0)
						{
							throw new SPException("Некорректное использование директив WHILE и ENDW");
						}
					}
					else if ((str.Operation == "MACRO" || str.Operation == "MEND") && whileCount > 0)
					{
						throw new SPException("Объявление макросов в цикле запрещено");
					}
					else if (str.Operation == "GLOBAL" && whileCount > 0)
					{
						throw new SPException("Объявление глобальных переменных в цикле запрещено");
					}
					else if (!string.IsNullOrEmpty(str.Label) && str.Operation != "MACRO" && whileCount > 0)
					{
						throw new SPException("Использование меток в цикле запрещено");
					}
				}

				if (whileCount != 0)
				{
					throw new SPException("Некорректное использование директив WHILE и ENDW");
				}
			}
			catch (SPException ex)
			{
				throw new SPException(ex.Message, ex.errorString);
			}
			catch (Exception)
			{
				throw new SPException("Некорректное использование директив WHILE и ENDW");
			}
		}

		/// <summary>
		/// Проверка макроса на AIF-AGO
		/// </summary>
		/// <param name="body"></param>
		public static void CheckAIF(TMOEntity te)
		{
			// Вложенные макросы не обрабатываем
			List<SourceEntity> result = new List<SourceEntity>();
			int macroCount = 0;
			foreach (SourceEntity se in te.Body)
			{
				if (se.Operation == "MACRO")
				{
					macroCount++;
				}
				else if (se.Operation == "MEND")
				{
					macroCount--;
				}
				if (macroCount == 0)
				{
					result.Add(se.Clone());
				}
			}

			try
			{
				foreach (SourceEntity str in result)
				{
					if (str.Operation == "AIF")
					{
						if (str.Operands.Count != 2)
						{
							throw new SPException("Некорректное использование директивы AIF: " + str.SourceString);
						}
						if (!String.IsNullOrEmpty(str.Label))
						{
							throw new SPException("При директиве AIF метки быть не должно: " + str.SourceString);
						}
						if (!Utiliities.IsMacroLabel(str.Operands[1]))
						{
							throw new SPException("При директиве AIF отсутствует метка для ссылки: " + str.SourceString);
						}
					}
					if (str.Operation == "AGO")
					{
						if (str.Operands.Count != 1)
						{
							throw new SPException("Некорректное использование директивы AGO: " + str.SourceString);
						}
						if (!String.IsNullOrEmpty(str.Label))
						{
							throw new SPException("При директиве AGO метки быть не должно: " + str.SourceString);
						}
						if (!Utiliities.IsMacroLabel(str.Operands[0]))
						{
							throw new SPException("При директиве AGO отсутствует метка для ссылки: " + str.SourceString);
						}
					}
				}
			}
			catch (SPException ex)
			{
				throw new SPException(ex.Message, ex.errorString);
			}
			catch (Exception)
			{
				throw new SPException("Некорректное использование директив AIF-AGO");
			}
		}

		/// <summary>
		/// Проверка макроса на IF-ELSE-ENDIF
		/// </summary>
		/// <param name="body"></param>
		public static void CheckIF(TMOEntity te)
		{
			Stack<bool> stackIfHasElse = new Stack<bool>();

			// Вложенные макросы не обрабатываем
			List<SourceEntity> result = new List<SourceEntity>();
			int macroCount = 0;
			foreach (SourceEntity se in te.Body)
			{
				if (se.Operation == "MACRO")
				{
					macroCount++;
				}
				else if (se.Operation == "MEND")
				{
					macroCount--;
				}
				if (macroCount == 0)
				{
					result.Add(se.Clone());
				}
			}

			//проверка корректности IF-ELSE-ENDIF
			try
			{
				foreach (SourceEntity str in result)
				{
					if (str.Operation == "IF")
					{
						if (str.Operands.Count != 1)
						{
							throw new SPException("Некорректное использование директивы IF: " + str.SourceString);
						}
						if (!String.IsNullOrEmpty(str.Label))
						{
							throw new SPException("При директиве IF метки быть не должно: " + str.SourceString);
						}
						stackIfHasElse.Push(false);
					}
					if (str.Operation == "ELSE")
					{
						if (str.Operands.Count != 0)
						{
							throw new SPException("Некорректное использование директивы ELSE: " + str.SourceString);
						}
						if (!String.IsNullOrEmpty(str.Label))
						{
							throw new SPException("При директиве ELSE метки быть не должно: " + str.SourceString);
						}
						if (stackIfHasElse.Peek() == true)
						{
							throw new SPException("Лишняя ветка ELSE: " + str.SourceString);
						}
						else
						{
							stackIfHasElse.Pop();
							stackIfHasElse.Push(true);
						}
					}
					if (str.Operation == "ENDIF")
					{
						if (str.Operands.Count != 0)
						{
							throw new SPException("Некорректное использование директивы ENDIF: " + str.SourceString);
						}
						if (!String.IsNullOrEmpty(str.Label))
						{
							throw new SPException("При директиве ENDIF метки быть не должно: " + str.SourceString);
						}
						stackIfHasElse.Pop();
					}
				}

				if (stackIfHasElse.Count > 0)
				{
					throw new SPException("Отсутствует директива ENDIF");
				}
			}
			catch (SPException ex)
			{
				throw new SPException(ex.Message, ex.errorString);
			}
			catch (Exception)
			{
				throw new SPException("Некорректное использование директив IF - ENDIF");
			}
		}

		/// <summary>
		/// Проверка вложенностей
		/// </summary>
		/// <param name="current"></param>
		/// <param name="stack"></param>
		public static void CheckInner(SourceEntity current, Stack<Command> stack)
		{
			if (current.Operation == "IF")
			{
				return;
			}
			if (current.Operation == "ELSE")
			{
				if (stack.Count > 0 && stack.Peek() != Command.IF)
				{
					throw new SPException("Некорректное использование директивы ELSE");
				}
				return;
			}
			if (current.Operation == "ENDIF")
			{
				if (stack.Count > 0 && stack.Peek() != Command.IF && stack.Peek() != Command.ELSE)
				{
					throw new SPException("Некорректное использование директивы ENDIF");
				}
				return;
			}
			if (current.Operation == "WHILE")
			{
				return;
			}
			if (current.Operation == "ENDW")
			{
				if (stack.Count > 0 && stack.Peek() != Command.WHILE)
				{
					throw new SPException("Некорректное использование директивы ENDW");
				}
				return;
			}
		}

	}
}
