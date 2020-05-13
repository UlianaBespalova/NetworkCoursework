namespace ComForm
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.b_con = new System.Windows.Forms.Button();
            this.cb_PortNames = new System.Windows.Forms.ComboBox();
            this.b_OpenPort = new System.Windows.Forms.Button();
            this.b_ChooseFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.b_About = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.b_Connection = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(21, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(512, 380);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // b_con
            // 
            this.b_con.Location = new System.Drawing.Point(557, 117);
            this.b_con.Name = "b_con";
            this.b_con.Size = new System.Drawing.Size(141, 23);
            this.b_con.TabIndex = 4;
            this.b_con.Text = "Проверить соединение";
            this.b_con.UseVisualStyleBackColor = true;
            this.b_con.Click += new System.EventHandler(this.b_con_Click);
            // 
            // cb_PortNames
            // 
            this.cb_PortNames.FormattingEnabled = true;
            this.cb_PortNames.Location = new System.Drawing.Point(557, 32);
            this.cb_PortNames.Name = "cb_PortNames";
            this.cb_PortNames.Size = new System.Drawing.Size(141, 21);
            this.cb_PortNames.TabIndex = 6;
            // 
            // b_OpenPort
            // 
            this.b_OpenPort.Location = new System.Drawing.Point(557, 59);
            this.b_OpenPort.Name = "b_OpenPort";
            this.b_OpenPort.Size = new System.Drawing.Size(141, 23);
            this.b_OpenPort.TabIndex = 7;
            this.b_OpenPort.Text = "Открыть порт";
            this.b_OpenPort.UseVisualStyleBackColor = true;
            this.b_OpenPort.Click += new System.EventHandler(this.b_OpenPort_Click);
            // 
            // b_ChooseFile
            // 
            this.b_ChooseFile.Location = new System.Drawing.Point(557, 177);
            this.b_ChooseFile.Name = "b_ChooseFile";
            this.b_ChooseFile.Size = new System.Drawing.Size(141, 23);
            this.b_ChooseFile.TabIndex = 8;
            this.b_ChooseFile.Text = "Выбрать файл...";
            this.b_ChooseFile.UseVisualStyleBackColor = true;
            this.b_ChooseFile.Click += new System.EventHandler(this.b_ChooseFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(554, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Выбранный порт:";
            // 
            // b_About
            // 
            this.b_About.Location = new System.Drawing.Point(557, 369);
            this.b_About.Name = "b_About";
            this.b_About.Size = new System.Drawing.Size(141, 23);
            this.b_About.TabIndex = 11;
            this.b_About.Text = "О программе";
            this.b_About.UseVisualStyleBackColor = true;
            this.b_About.Click += new System.EventHandler(this.b_About_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(557, 148);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(141, 23);
            this.progressBar1.TabIndex = 12;
            this.progressBar1.Visible = false;
            // 
            // b_Connection
            // 
            this.b_Connection.Location = new System.Drawing.Point(557, 88);
            this.b_Connection.Name = "b_Connection";
            this.b_Connection.Size = new System.Drawing.Size(141, 23);
            this.b_Connection.TabIndex = 13;
            this.b_Connection.Text = "Установить соединение";
            this.b_Connection.UseVisualStyleBackColor = true;
            this.b_Connection.Click += new System.EventHandler(this.b_Connection_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 413);
            this.Controls.Add(this.b_Connection);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.b_About);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.b_ChooseFile);
            this.Controls.Add(this.b_OpenPort);
            this.Controls.Add(this.cb_PortNames);
            this.Controls.Add(this.b_con);
            this.Controls.Add(this.richTextBox1);
            this.Name = "Form1";
            this.Text = "COMpass";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button b_con;
        private System.Windows.Forms.ComboBox cb_PortNames;
        private System.Windows.Forms.Button b_OpenPort;
        private System.Windows.Forms.Button b_ChooseFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button b_About;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button b_Connection;
    }
}

