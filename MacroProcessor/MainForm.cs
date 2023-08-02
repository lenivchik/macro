using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MacroProcessor
{
	public partial class MainForm : Form
	{
		private string input_file = Utiliities.CurrentDirectory + "\\source.txt";
		private string output_file = Utiliities.CurrentDirectory + "\\result.txt";
		private GUIProgram program;
		private Boolean flag = false;
		private RichTextBox richTextBox1;
		public MainForm()
		{
			InitializeComponent();
			this.Show();
			this.Activate();
		}


		/// <summary>
		/// Следующий шаг выполнения проги
		/// </summary>
		private void btn_next_step_Click(object sender, EventArgs e)
		{

				try
			{
				this.btn_first_run.Enabled = false;
				this.btn_second_run.Enabled = false;
				this.btn_refresh.Enabled = true;

				// если исходный текст пуст
				if (program.SourceCode.Entities.Count == 0)
				{
					throw new SPException("Исходный текст должен содержать хотя бы одну строку");
				}
				if (program.Index == 0)
				{
					tb_error.Clear();
				}
				if ((program.Index + 1) == program.SourceCode.Entities.Count && program.Mode == true)
				{
					this.btn_next_step.Enabled = false;
				
				}
				if (program.Mode == false)
				{
					this.program.NextFirstStep();
					TMO.PrintTMO(this.dgv_tmo);
				}
				else
				{
					this.program.NextSecondStep(this.tb_result);
					Global.PrintGlobal(this.dgv_global);
					TMO.PrintTMO(this.dgv_tmo);
				}
			}
			catch (SPException ex)
			{
				this.tb_error.Text = ex.Message;
				this.disableButtons();
				this.btn_refresh.Enabled = true;
			}
			catch (Exception)
			{
				this.tb_error.Text = "Ошибка!";
				this.disableButtons();
				this.btn_refresh.Enabled = true;
			}
		}

		/// <summary>
		/// Первый проход
		/// </summary>
		private void btn_first_run_Click(object sender, EventArgs e)
		{
			
			this.btn_first_run.Enabled = false;
			this.btn_next_step.Enabled = false;
			this.btn_second_run.Enabled = true;
			this.btn_refresh.Enabled = true;

			while (true)
			{
				if (!String.IsNullOrEmpty(tb_error.Text))
				{
					break;
				}
				if (this.program.Mode == true)
				{
					this.btn_second_run.Enabled = true;
					break;
				}
				this.btn_next_step_Click(sender, e);
			}
		}

		/// <summary>
		/// Второй проход
		/// </summary>
		private void btn_second_run_Click(object sender, EventArgs e)
		{
			this.btn_second_run.Enabled = false;
			this.btn_next_step.Enabled = false;
			this.btn_first_run.Enabled = true;
			this.btn_refresh.Enabled = true;

			while (true)
			{
				if (!String.IsNullOrEmpty(tb_error.Text))
				{
					break;
				}
				if (this.program.Mode == false || this.program.Index == this.program.SourceCode.Entities.Count && this.program.Mode == true)
				{
					break;
				}
				this.btn_next_step_Click(sender, e);
			}
			flag = false;
		}

		#region 


		/// <summary>
		/// При загрузке формы заплняем TB исходниками, если можно
		/// </summary>
		private void fm_main_Load(object sender, EventArgs e)
		{
			this.tb_input_file.Text = this.input_file;
			this.tb_output_file.Text = this.output_file;
			disableButtons();

			if (!String.IsNullOrEmpty(this.tb_input_file.Text))
			{
				try
				{
					fillSourceTextBoxFromFile(this.tb_input_file.Text, this.tb_source_code);
					enableButtons();
					this.btn_second_run.Enabled = false;
					this.tb_error.Clear();
				}
				catch (SPException ex)
				{
					this.tb_error.Text = ex.Message;
					disableButtons();
				}
				catch (Exception)
				{
					this.tb_error.Text = "Не удалось загрузить данные с файла. Возможно путь к файлу указан неверно";
					disableButtons();
				}
			}
		}

		private void addEnd(RichTextBox rbtest)
        {
			if (richTextBox1==null)
            {
				richTextBox1 = new RichTextBox();

			}
			richTextBox1.Text = rbtest.Text;
			richTextBox1.AppendText("end");
			}

		/// <summary>
		/// Обновить все данные, заново начать прогу
		/// </summary>
		private void btn_refresh_Click(object sender, EventArgs e)
		{
			addEnd(tb_source_code);
			this.program = new GUIProgram(richTextBox1);
			this.tb_error.Clear();
			this.tb_result.Clear();
			TMO.PrintTMO(this.dgv_tmo);
			Global.PrintGlobal(this.dgv_global);

			this.enableButtons();
			this.btn_second_run.Enabled = false;
		}

		/// <summary>
		/// Загрузить из файла исходный код
		/// </summary>
		private void btn_load_Click(object sender, EventArgs e)
		{
			try
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.Filter = "Text Files (.txt)|*.txt";
				ofd.InitialDirectory = Utiliities.CurrentDirectory;
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					try
					{
						this.tb_source_code.Clear();
						this.tb_input_file.Text = ofd.FileName;
						enableButtons();
						this.btn_second_run.Enabled = false;
						fillSourceTextBoxFromFile(ofd.FileName, this.tb_source_code);
					}
					catch (SPException ex)
					{
						throw new Exception(ex.Message);
					}
					catch (Exception)
					{
						throw new Exception("Не удалось загрузить данные с файла. Возможно путь к файлу указан неверно");
					}
				}
			}
			catch (Exception ex)
			{
				this.tb_error.Text = ex.Message;
				disableButtons();
			}
		}

		/// <summary>
		/// Сохранение результата в файл
		/// </summary>
		private void btn_save_Click(object sender, EventArgs e)
		{
			try
			{
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.Filter = "Text Files (.txt)|*.txt";
				sfd.InitialDirectory = Utiliities.CurrentDirectory;
				if (sfd.ShowDialog() == DialogResult.OK)
				{
					this.tb_input_file.Text = sfd.FileName;
					List<String> temp = tb_result.Text.Split('\n').ToList<String>();
					StreamWriter sw = new StreamWriter(sfd.FileName);
					foreach (string str in temp)
					{
						sw.WriteLine(str);
					}
					sw.Close();
				}
			}
			catch (SPException ex)
			{
				this.tb_error.Text = ex.Message;
				disableButtons();
			}
			catch (Exception)
			{
				this.tb_error.Text = "Не удалось загрузить данные с файла. Возможно путь к файлу указан неверно";
				disableButtons();
			}
		}

		/// <summary>
		/// Открыть кнопки
		/// </summary>
		private void enableButtons()
		{
			this.btn_next_step.Enabled = true;
			this.btn_refresh.Enabled = true;
			this.btn_second_run.Enabled = true;
			this.btn_first_run.Enabled = true;
		}

		/// <summary>
		/// Закрыть кнопки
		/// </summary>
		private void disableButtons()
		{
			this.btn_next_step.Enabled = false;
			this.btn_refresh.Enabled = false;
			this.btn_second_run.Enabled = false;
			this.btn_first_run.Enabled = false;
		}

		/// <summary>
		/// Заполнить TеxtBox исходных данных, считав их из заданного файла
		/// </summary>
		/// <param name="file">Имя файла с исходниками</param>
		/// <param name="tb">TеxtBox исходных данных</param>
		private void fillSourceTextBoxFromFile(string file, RichTextBox tb)
		{
			try
			{
				String temp = String.Empty;
				StreamReader sr = new StreamReader(file);
				while ((temp = sr.ReadLine()) != null)
				{
					tb.AppendText(temp + Environment.NewLine);
				}
				sr.Close();
			}
			catch (Exception)
			{
				throw new SPException("Не удалось загрузить данные с файла. Возможно путь к файлу указан неверно");
			}
		}

		#endregion

	}
}
