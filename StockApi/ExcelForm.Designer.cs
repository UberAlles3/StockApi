
namespace StockApi
{
    partial class ExcelForm
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnParse = new System.Windows.Forms.Button();
            this.radAccount1 = new System.Windows.Forms.RadioButton();
            this.radAccount2 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 83);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(776, 504);
            this.textBox1.TabIndex = 0;
            // 
            // btnParse
            // 
            this.btnParse.Location = new System.Drawing.Point(119, 24);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(136, 32);
            this.btnParse.TabIndex = 1;
            this.btnParse.Text = "Parse and Convert";
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // radAccount1
            // 
            this.radAccount1.AutoSize = true;
            this.radAccount1.Checked = true;
            this.radAccount1.ForeColor = System.Drawing.Color.White;
            this.radAccount1.Location = new System.Drawing.Point(12, 12);
            this.radAccount1.Name = "radAccount1";
            this.radAccount1.Size = new System.Drawing.Size(68, 19);
            this.radAccount1.TabIndex = 2;
            this.radAccount1.TabStop = true;
            this.radAccount1.Text = "Rollover";
            this.radAccount1.UseVisualStyleBackColor = true;
            this.radAccount1.CheckedChanged += new System.EventHandler(this.radAccount1_CheckedChanged);
            // 
            // radAccount2
            // 
            this.radAccount2.AutoSize = true;
            this.radAccount2.ForeColor = System.Drawing.Color.White;
            this.radAccount2.Location = new System.Drawing.Point(12, 37);
            this.radAccount2.Name = "radAccount2";
            this.radAccount2.Size = new System.Drawing.Size(50, 19);
            this.radAccount2.TabIndex = 3;
            this.radAccount2.Text = "Joint";
            this.radAccount2.UseVisualStyleBackColor = true;
            // 
            // ExcelForm
            // 
            this.AcceptButton = this.btnParse;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(64)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(800, 599);
            this.Controls.Add(this.radAccount2);
            this.Controls.Add(this.radAccount1);
            this.Controls.Add(this.btnParse);
            this.Controls.Add(this.textBox1);
            this.Name = "ExcelForm";
            this.Text = "Import from Excel export";
            this.Load += new System.EventHandler(this.ExeclForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnParse;
        private System.Windows.Forms.RadioButton radAccount1;
        private System.Windows.Forms.RadioButton radAccount2;
    }
}