using System;
using System.Collections.Generic;
using System.Linq;
using MacroProcessor.Utils;

namespace MacroProcessor
{
	/// <summary>
	/// Утилита для парсинга параметров из строки.
	/// </summary>
	public abstract class MacroParametersParser
	{
		protected readonly SourceCode Processor;

		public MacroParametersParser(SourceCode processor)
		{
			Processor = processor;
		}

		/// <summary>
		/// Распарсить строку в параметры.
		/// </summary>
		/// <param name="operands">Операнды.</param>
		/// <returns>Список параметров.</returns>
		public abstract List<MacroParameter> Parse(List<string> operands, string macroName);
	}

	/// <summary>
	/// Утилита для парсинга позиционных параметров.
	/// </summary>
	public class PositionMacroParametersParser : MacroParametersParser
	{
		public PositionMacroParametersParser(SourceCode processor)
			: base(processor)
		{

		}

		/// <summary>
		/// Распарсить строку в параметры.
		/// </summary>
		/// <param name="operands">Операнды.</param>
		/// <returns>Список параметров.</returns>
		public override List<MacroParameter> Parse(List<string> operands, string macroName)
		{
			if (!(operands?.Any() ?? false))
			{
				return new List<MacroParameter>();
			}

			// Проверить корректность имени
			foreach (string currentOperand in operands)
			{
				Utiliities.CheckNames(currentOperand);

				if (operands.Count(x => x == currentOperand) > 1)
				{
					throw new SPException(string.Format(Messages.MacroParameterDublicate, currentOperand));
				}
				if (!Utiliities.IsLabel(currentOperand))
				{
					throw new SPException(string.Format(Messages.IncorrectMacroParameterName, currentOperand));
				}
			}

			return operands
				.Select(e => new MacroParameter(e, MacroParameterTypes.Position))
				.ToList();
		}
	}

	/// <summary>
	/// Утилита для парсинга ключевых параметров.
	/// </summary>
	public class KeyMacroParametersParser : MacroParametersParser
	{
		public KeyMacroParametersParser(SourceCode processor)
			: base(processor)
		{

		}

		/// <summary>
		/// Распарсить строку в параметры.
		/// </summary>
		/// <param name="operands">Операнды.</param>
		/// <returns>Список параметров.</returns>
		public override List<MacroParameter> Parse(List<string> operands, string macroName)
		{
			if (!(operands?.Any() ?? false))
			{
				return new List<MacroParameter>();
			}

			var parameters = new List<MacroParameter>();

			// Проверить корректность имени
			foreach (string currentOperand in operands)
			{
				Utiliities.CheckNames(currentOperand);

				var vals = currentOperand.Split('=');
				if (vals.Length != 2)
				{
					throw new SPException(string.Format(Messages.IcorrectMacroDefinitionParameter, currentOperand));
				}
				var parameterName = vals[0];
				var defaultValue = vals[1];

				if (parameters.Any(e => e.Name == parameterName))
				{
					throw new SPException(string.Format(Messages.MacroParameterDublicate, currentOperand));
				}
				if (!Utiliities.IsLabel(parameterName))
				{
					throw new SPException(string.Format(Messages.IcorrectMacroDefinitionParameter, currentOperand));
				}

				//if (!string.IsNullOrEmpty(defaultValue))
				//{
				//	throw new SPException(string.Format(Messages.IcorrectMacroDefinitionParameter, currentOperand));
				//}

				if (!string.IsNullOrEmpty(defaultValue) && !int.TryParse(defaultValue, out int temp))
				{
					throw new SPException(string.Format(Messages.IcorrectMacroDefinitionParameterDefault, currentOperand));
				}

				parameters.Add(new MacroParameter
				{
					Name = parameterName,
					Type = MacroParameterTypes.Key,
					DefaultValue = !string.IsNullOrEmpty(defaultValue)
						? int.Parse(defaultValue)
						: (int?)null
				});
			}

			return parameters;
		}
	}

	/// <summary>
	/// Утилита для парсинга смешанных параметров.
	/// </summary>
	public class MixedMacroParametersParser : MacroParametersParser
	{
		public MixedMacroParametersParser(SourceCode processor)
			: base(processor)
		{

		}

		/// <summary>
		/// Распарсить строку в параметры.
		/// </summary>
		/// <param name="operands">Операнды.</param>
		/// <returns>Список параметров.</returns>
		public override List<MacroParameter> Parse(List<string> operands, string macroName)
		{
			if (!(operands?.Any() ?? false))
			{
				return new List<MacroParameter>();
			}

			var parameters = new List<MacroParameter>();

			var firstKeyParameterIdx = Array.IndexOf(operands.ToArray(), operands.FirstOrDefault(x => x.Contains("=")));
			if (firstKeyParameterIdx != -1)
			{
				if (operands.Any(x => !x.Contains("=") && Array.IndexOf(operands.ToArray(), x) > firstKeyParameterIdx))
				{
					throw new SPException(string.Format(Messages.IcorrectMacroDefinitionParameters, macroName));
				}
			}

			// Проверить корректность позиционных параметров
			var lastPositionParameterIdx = firstKeyParameterIdx >= 0 ? firstKeyParameterIdx : operands.Count();
			for (int i = 0; i < lastPositionParameterIdx; i++)
			{
				var currentOperand = operands.ToArray()[i];

				Utiliities.CheckNames(currentOperand);

				if (parameters.Any(e => e.Name == currentOperand))
				{
					throw new SPException(string.Format(Messages.MacroParameterDublicate, currentOperand));
				}
				if (!Utiliities.IsLabel(currentOperand))
				{
					throw new SPException(string.Format(Messages.IncorrectMacroParameterName, currentOperand));
				}

				parameters.Add(new MacroParameter
				{
					Name = currentOperand,
					Type = MacroParameterTypes.Position
				});
			}

			// Проверить корректность ключевых параметров
			if (firstKeyParameterIdx >= 0)
			{
				for (int i = firstKeyParameterIdx; i < operands.Count(); i++)
				{
					var currentOperand = operands.ToArray()[i];

					Utiliities.CheckNames(currentOperand);

					var vals = currentOperand.Split('=');
					if (vals.Length != 2)
					{
						throw new SPException(string.Format(Messages.IcorrectMacroDefinitionParameter, currentOperand));
					}
					var parameterName = vals[0];
					var defaultValue = vals[1];

					if (parameters.Any(e => e.Name == parameterName))
					{
						throw new SPException(string.Format(Messages.MacroParameterDublicate, currentOperand));
					}
					if (!Utiliities.IsLabel(parameterName))
					{
						throw new SPException(string.Format(Messages.IcorrectMacroDefinitionParameter, currentOperand));
					}

					//if (!string.IsNullOrEmpty(defaultValue))
					//{
					//	throw new SPException(string.Format(Messages.IcorrectMacroDefinitionParameter, currentOperand));
					//}

					if (!string.IsNullOrEmpty(defaultValue) && !int.TryParse(defaultValue, out int temp))
					{
						throw new SPException(string.Format(Messages.IcorrectMacroDefinitionParameterDefault, currentOperand));
					}

					parameters.Add(new MacroParameter
					{
						Name = parameterName,
						Type = MacroParameterTypes.Key,
						DefaultValue = !string.IsNullOrEmpty(defaultValue)
							? int.Parse(defaultValue)
							: (int?)null
					});
				}
			}

			return parameters;
		}
	}
}
