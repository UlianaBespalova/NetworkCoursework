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
            this.b_open5 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.b_open6 = new System.Windows.Forms.Button();
            this.b_con = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
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
            // b_open5
            // 
            this.b_open5.Location = new System.Drawing.Point(557, 12);
            this.b_open5.Name = "b_open5";
            this.b_open5.Size = new System.Drawing.Size(141, 23);
            this.b_open5.TabIndex = 1;
            this.b_open5.Text = "open COM1";
            this.b_open5.UseVisualStyleBackColor = true;
            this.b_open5.Click += new System.EventHandler(this.b_open5_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(557, 122);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(141, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Send \"Hello\" as msg";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // b_open6
            // 
            this.b_open6.Location = new System.Drawing.Point(557, 42);
            this.b_open6.Name = "b_open6";
            this.b_open6.Size = new System.Drawing.Size(141, 23);
            this.b_open6.TabIndex = 3;
            this.b_open6.Text = "open COM2";
            this.b_open6.UseVisualStyleBackColor = true;
            this.b_open6.Click += new System.EventHandler(this.b_open6_Click);
            // 
            // b_con
            // 
            this.b_con.Location = new System.Drawing.Point(557, 72);
            this.b_con.Name = "b_con";
            this.b_con.Size = new System.Drawing.Size(141, 23);
            this.b_con.TabIndex = 4;
            this.b_con.Text = "is connection";
            this.b_con.UseVisualStyleBackColor = true;
            this.b_con.Click += new System.EventHandler(this.b_con_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(557, 151);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(141, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Send \"Hello\" as file";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(729, 413);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.b_con);
            this.Controls.Add(this.b_open6);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.b_open5);
            this.Controls.Add(this.richTextBox1);
            this.Name = "Form1";
            this.Text = "example";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button b_open5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button b_open6;
        private System.Windows.Forms.Button b_con;
        private System.Windows.Forms.Button button2;
    }
}

