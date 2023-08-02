namespace MacroProcessor
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tb_error = new System.Windows.Forms.TextBox();
            this.btn_next_step = new System.Windows.Forms.Button();
            this.btn_refresh = new System.Windows.Forms.Button();
            this.btn_first_run = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_source_code = new System.Windows.Forms.RichTextBox();
            this.dgv_global = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_result = new System.Windows.Forms.RichTextBox();
            this.tb_output_file = new System.Windows.Forms.TextBox();
            this.btn_save = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.dgv_tmo = new System.Windows.Forms.DataGridView();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tb_input_file = new System.Windows.Forms.TextBox();
            this.btn_load = new System.Windows.Forms.Button();
            this.btn_second_run = new System.Windows.Forms.Button();
            this.gpb_result = new System.Windows.Forms.GroupBox();
            this.gpb_tables = new System.Windows.Forms.GroupBox();
            this.gpb_source_code = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_global)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_tmo)).BeginInit();
            this.gpb_result.SuspendLayout();
            this.gpb_tables.SuspendLayout();
            this.gpb_source_code.SuspendLayout();
            this.SuspendLayout();
            // 
            // tb_error
            // 
            this.tb_error.Enabled = false;
            this.tb_error.Location = new System.Drawing.Point(465, 15);
            this.tb_error.Margin = new System.Windows.Forms.Padding(4);
            this.tb_error.Multiline = true;
            this.tb_error.Name = "tb_error";
            this.tb_error.ReadOnly = true;
            this.tb_error.Size = new System.Drawing.Size(879, 70);
            this.tb_error.TabIndex = 16;
            // 
            // btn_next_step
            // 
            this.btn_next_step.Location = new System.Drawing.Point(297, 689);
            this.btn_next_step.Margin = new System.Windows.Forms.Padding(4);
            this.btn_next_step.Name = "btn_next_step";
            this.btn_next_step.Size = new System.Drawing.Size(157, 28);
            this.btn_next_step.TabIndex = 15;
            this.btn_next_step.Text = "Следующий шаг";
            this.btn_next_step.UseVisualStyleBackColor = true;
            this.btn_next_step.Click += new System.EventHandler(this.btn_next_step_Click);
            // 
            // btn_refresh
            // 
            this.btn_refresh.Location = new System.Drawing.Point(204, 689);
            this.btn_refresh.Margin = new System.Windows.Forms.Padding(4);
            this.btn_refresh.Name = "btn_refresh";
            this.btn_refresh.Size = new System.Drawing.Size(85, 28);
            this.btn_refresh.TabIndex = 14;
            this.btn_refresh.Text = "Заново";
            this.btn_refresh.UseVisualStyleBackColor = true;
            this.btn_refresh.Click += new System.EventHandler(this.btn_refresh_Click);
            // 
            // btn_first_run
            // 
            this.btn_first_run.Location = new System.Drawing.Point(16, 689);
            this.btn_first_run.Margin = new System.Windows.Forms.Padding(4);
            this.btn_first_run.Name = "btn_first_run";
            this.btn_first_run.Size = new System.Drawing.Size(85, 28);
            this.btn_first_run.TabIndex = 12;
            this.btn_first_run.Text = "1 проход";
            this.btn_first_run.UseVisualStyleBackColor = true;
            this.btn_first_run.Click += new System.EventHandler(this.btn_first_run_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(93, 32);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Исходный код";
            // 
            // tb_source_code
            // 
            this.tb_source_code.Location = new System.Drawing.Point(8, 60);
            this.tb_source_code.Margin = new System.Windows.Forms.Padding(4);
            this.tb_source_code.Name = "tb_source_code";
            this.tb_source_code.Size = new System.Drawing.Size(421, 598);
            this.tb_source_code.TabIndex = 4;
            this.tb_source_code.Text = "";
            this.tb_source_code.TextChanged += new System.EventHandler(this.btn_refresh_Click);
            // 
            // dgv_global
            // 
            this.dgv_global.AllowUserToAddRows = false;
            this.dgv_global.AllowUserToDeleteRows = false;
            this.dgv_global.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_global.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dgv_global.Location = new System.Drawing.Point(8, 476);
            this.dgv_global.Margin = new System.Windows.Forms.Padding(4);
            this.dgv_global.Name = "dgv_global";
            this.dgv_global.ReadOnly = true;
            this.dgv_global.RowHeadersVisible = false;
            this.dgv_global.RowHeadersWidth = 51;
            this.dgv_global.Size = new System.Drawing.Size(432, 132);
            this.dgv_global.TabIndex = 3;
            // 
            // Column1
            // 
            this.Column1.FillWeight = 50F;
            this.Column1.HeaderText = "Имя";
            this.Column1.MinimumWidth = 6;
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 125;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "Значение";
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(112, 457);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(230, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "Таблица глобальных переменных";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(101, 32);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Ассемблерный код";
            // 
            // tb_result
            // 
            this.tb_result.Enabled = false;
            this.tb_result.Location = new System.Drawing.Point(11, 60);
            this.tb_result.Margin = new System.Windows.Forms.Padding(4);
            this.tb_result.Name = "tb_result";
            this.tb_result.ReadOnly = true;
            this.tb_result.Size = new System.Drawing.Size(415, 547);
            this.tb_result.TabIndex = 5;
            this.tb_result.Text = "";
            // 
            // tb_output_file
            // 
            this.tb_output_file.Enabled = false;
            this.tb_output_file.Location = new System.Drawing.Point(45, 320);
            this.tb_output_file.Margin = new System.Windows.Forms.Padding(4);
            this.tb_output_file.Name = "tb_output_file";
            this.tb_output_file.ReadOnly = true;
            this.tb_output_file.Size = new System.Drawing.Size(377, 22);
            this.tb_output_file.TabIndex = 2;
            this.tb_output_file.Visible = false;
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(249, 26);
            this.btn_save.Margin = new System.Windows.Forms.Padding(4);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(105, 27);
            this.btn_save.TabIndex = 0;
            this.btn_save.Text = "Сохранить";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(204, 20);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = "ТМО";
            // 
            // dgv_tmo
            // 
            this.dgv_tmo.AllowUserToAddRows = false;
            this.dgv_tmo.AllowUserToDeleteRows = false;
            this.dgv_tmo.AllowUserToResizeColumns = false;
            this.dgv_tmo.AllowUserToResizeRows = false;
            this.dgv_tmo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_tmo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column3,
            this.Column4});
            this.dgv_tmo.Location = new System.Drawing.Point(8, 39);
            this.dgv_tmo.Margin = new System.Windows.Forms.Padding(4);
            this.dgv_tmo.Name = "dgv_tmo";
            this.dgv_tmo.ReadOnly = true;
            this.dgv_tmo.RowHeadersVisible = false;
            this.dgv_tmo.RowHeadersWidth = 51;
            this.dgv_tmo.Size = new System.Drawing.Size(432, 414);
            this.dgv_tmo.TabIndex = 0;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Имя макроса";
            this.Column3.MinimumWidth = 6;
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 125;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column4.HeaderText = "Тело макроса";
            this.Column4.MinimumWidth = 6;
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // tb_input_file
            // 
            this.tb_input_file.Enabled = false;
            this.tb_input_file.Location = new System.Drawing.Point(27, 154);
            this.tb_input_file.Margin = new System.Windows.Forms.Padding(4);
            this.tb_input_file.Name = "tb_input_file";
            this.tb_input_file.ReadOnly = true;
            this.tb_input_file.Size = new System.Drawing.Size(384, 22);
            this.tb_input_file.TabIndex = 3;
            // 
            // btn_load
            // 
            this.btn_load.Location = new System.Drawing.Point(207, 26);
            this.btn_load.Margin = new System.Windows.Forms.Padding(4);
            this.btn_load.Name = "btn_load";
            this.btn_load.Size = new System.Drawing.Size(97, 27);
            this.btn_load.TabIndex = 1;
            this.btn_load.Text = "Загрузить";
            this.btn_load.UseVisualStyleBackColor = true;
            this.btn_load.Click += new System.EventHandler(this.btn_load_Click);
            // 
            // btn_second_run
            // 
            this.btn_second_run.Location = new System.Drawing.Point(109, 689);
            this.btn_second_run.Margin = new System.Windows.Forms.Padding(4);
            this.btn_second_run.Name = "btn_second_run";
            this.btn_second_run.Size = new System.Drawing.Size(87, 28);
            this.btn_second_run.TabIndex = 13;
            this.btn_second_run.Text = "2 проход";
            this.btn_second_run.UseVisualStyleBackColor = true;
            this.btn_second_run.Click += new System.EventHandler(this.btn_second_run_Click);
            // 
            // gpb_result
            // 
            this.gpb_result.Controls.Add(this.label2);
            this.gpb_result.Controls.Add(this.tb_result);
            this.gpb_result.Controls.Add(this.tb_output_file);
            this.gpb_result.Controls.Add(this.btn_save);
            this.gpb_result.Location = new System.Drawing.Point(913, 97);
            this.gpb_result.Margin = new System.Windows.Forms.Padding(4);
            this.gpb_result.Name = "gpb_result";
            this.gpb_result.Padding = new System.Windows.Forms.Padding(4);
            this.gpb_result.Size = new System.Drawing.Size(432, 620);
            this.gpb_result.TabIndex = 11;
            this.gpb_result.TabStop = false;
            this.gpb_result.Text = "Результат";
            // 
            // gpb_tables
            // 
            this.gpb_tables.Controls.Add(this.dgv_global);
            this.gpb_tables.Controls.Add(this.label5);
            this.gpb_tables.Controls.Add(this.label4);
            this.gpb_tables.Controls.Add(this.dgv_tmo);
            this.gpb_tables.Location = new System.Drawing.Point(457, 97);
            this.gpb_tables.Margin = new System.Windows.Forms.Padding(4);
            this.gpb_tables.Name = "gpb_tables";
            this.gpb_tables.Padding = new System.Windows.Forms.Padding(4);
            this.gpb_tables.Size = new System.Drawing.Size(448, 620);
            this.gpb_tables.TabIndex = 10;
            this.gpb_tables.TabStop = false;
            this.gpb_tables.Text = "Таблицы";
            // 
            // gpb_source_code
            // 
            this.gpb_source_code.Controls.Add(this.label1);
            this.gpb_source_code.Controls.Add(this.tb_source_code);
            this.gpb_source_code.Controls.Add(this.tb_input_file);
            this.gpb_source_code.Controls.Add(this.btn_load);
            this.gpb_source_code.Location = new System.Drawing.Point(16, 15);
            this.gpb_source_code.Margin = new System.Windows.Forms.Padding(4);
            this.gpb_source_code.Name = "gpb_source_code";
            this.gpb_source_code.Padding = new System.Windows.Forms.Padding(4);
            this.gpb_source_code.Size = new System.Drawing.Size(439, 667);
            this.gpb_source_code.TabIndex = 9;
            this.gpb_source_code.TabStop = false;
            this.gpb_source_code.Text = "Исходный код";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1359, 711);
            this.Controls.Add(this.tb_error);
            this.Controls.Add(this.btn_next_step);
            this.Controls.Add(this.btn_refresh);
            this.Controls.Add(this.btn_first_run);
            this.Controls.Add(this.btn_second_run);
            this.Controls.Add(this.gpb_result);
            this.Controls.Add(this.gpb_tables);
            this.Controls.Add(this.gpb_source_code);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(1377, 758);
            this.MinimumSize = new System.Drawing.Size(1377, 758);
            this.Name = "MainForm";
            this.Text = "Макропроцессор";
            this.Load += new System.EventHandler(this.fm_main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_global)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_tmo)).EndInit();
            this.gpb_result.ResumeLayout(false);
            this.gpb_result.PerformLayout();
            this.gpb_tables.ResumeLayout(false);
            this.gpb_tables.PerformLayout();
            this.gpb_source_code.ResumeLayout(false);
            this.gpb_source_code.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tb_error;
        private System.Windows.Forms.Button btn_next_step;
        private System.Windows.Forms.Button btn_refresh;
        private System.Windows.Forms.Button btn_first_run;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox tb_source_code;
        private System.Windows.Forms.DataGridView dgv_global;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox tb_result;
        private System.Windows.Forms.TextBox tb_output_file;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dgv_tmo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.TextBox tb_input_file;
        private System.Windows.Forms.Button btn_load;
        private System.Windows.Forms.Button btn_second_run;
        private System.Windows.Forms.GroupBox gpb_result;
        private System.Windows.Forms.GroupBox gpb_tables;
        private System.Windows.Forms.GroupBox gpb_source_code;
    }
}