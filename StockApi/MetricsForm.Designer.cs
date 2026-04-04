
namespace StockApi
{
    partial class MetricsForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTicker = new System.Windows.Forms.TextBox();
            this.chkBigChanges = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBeginLetter = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtEndLetter = new System.Windows.Forms.TextBox();
            this.btnRunMetrics = new System.Windows.Forms.Button();
            this.txtTickerList = new System.Windows.Forms.RichTextBox();
            this.btnCancelMetrics = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSearch.Location = new System.Drawing.Point(349, 27);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(96, 40);
            this.btnSearch.TabIndex = 17;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(16)))), ((int)(((byte)(32)))));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.InactiveCaption;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.Size = new System.Drawing.Size(990, 577);
            this.dataGridView1.TabIndex = 18;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 44);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(174, 23);
            this.comboBox1.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 29);
            this.label1.TabIndex = 48;
            this.label1.Text = "Date Range";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label2.Location = new System.Drawing.Point(219, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 29);
            this.label2.TabIndex = 49;
            this.label2.Text = "Ticker";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtTicker
            // 
            this.txtTicker.Location = new System.Drawing.Point(219, 44);
            this.txtTicker.Name = "txtTicker";
            this.txtTicker.Size = new System.Drawing.Size(100, 23);
            this.txtTicker.TabIndex = 50;
            // 
            // chkBigChanges
            // 
            this.chkBigChanges.AutoSize = true;
            this.chkBigChanges.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.chkBigChanges.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.chkBigChanges.Location = new System.Drawing.Point(474, 47);
            this.chkBigChanges.Name = "chkBigChanges";
            this.chkBigChanges.Size = new System.Drawing.Size(125, 21);
            this.chkBigChanges.TabIndex = 51;
            this.chkBigChanges.Text = "Big changes only";
            this.chkBigChanges.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label4.Location = new System.Drawing.Point(1045, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(156, 16);
            this.label4.TabIndex = 52;
            this.label4.Text = "Run Metrics for Stocks...";
            // 
            // txtBeginLetter
            // 
            this.txtBeginLetter.Location = new System.Drawing.Point(1045, 47);
            this.txtBeginLetter.Name = "txtBeginLetter";
            this.txtBeginLetter.Size = new System.Drawing.Size(62, 23);
            this.txtBeginLetter.TabIndex = 53;
            this.txtBeginLetter.Text = "A";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label3.Location = new System.Drawing.Point(1128, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 16);
            this.label3.TabIndex = 54;
            this.label3.Text = "through";
            // 
            // txtEndLetter
            // 
            this.txtEndLetter.Location = new System.Drawing.Point(1045, 76);
            this.txtEndLetter.Name = "txtEndLetter";
            this.txtEndLetter.Size = new System.Drawing.Size(62, 23);
            this.txtEndLetter.TabIndex = 55;
            this.txtEndLetter.Text = "B";
            // 
            // btnRunMetrics
            // 
            this.btnRunMetrics.Location = new System.Drawing.Point(1045, 124);
            this.btnRunMetrics.Name = "btnRunMetrics";
            this.btnRunMetrics.Size = new System.Drawing.Size(75, 27);
            this.btnRunMetrics.TabIndex = 56;
            this.btnRunMetrics.Text = "Run";
            this.btnRunMetrics.UseVisualStyleBackColor = true;
            this.btnRunMetrics.Click += new System.EventHandler(this.btnRunMetrics_Click);
            // 
            // txtTickerList
            // 
            this.txtTickerList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(16)))), ((int)(((byte)(32)))));
            this.txtTickerList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTickerList.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.txtTickerList.Location = new System.Drawing.Point(1045, 162);
            this.txtTickerList.Margin = new System.Windows.Forms.Padding(8);
            this.txtTickerList.Name = "txtTickerList";
            this.txtTickerList.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtTickerList.ShortcutsEnabled = false;
            this.txtTickerList.Size = new System.Drawing.Size(189, 517);
            this.txtTickerList.TabIndex = 57;
            this.txtTickerList.Text = "";
            // 
            // btnCancelMetrics
            // 
            this.btnCancelMetrics.Location = new System.Drawing.Point(1139, 124);
            this.btnCancelMetrics.Name = "btnCancelMetrics";
            this.btnCancelMetrics.Size = new System.Drawing.Size(75, 27);
            this.btnCancelMetrics.TabIndex = 58;
            this.btnCancelMetrics.Text = "Cancel";
            this.btnCancelMetrics.UseVisualStyleBackColor = true;
            this.btnCancelMetrics.Click += new System.EventHandler(this.btnCancelMetrics_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Location = new System.Drawing.Point(12, 84);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(996, 595);
            this.panel1.TabIndex = 59;
            // 
            // MetricsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(48)))), ((int)(((byte)(96)))));
            this.ClientSize = new System.Drawing.Size(1246, 696);
            this.Controls.Add(this.btnCancelMetrics);
            this.Controls.Add(this.txtTickerList);
            this.Controls.Add(this.btnRunMetrics);
            this.Controls.Add(this.txtEndLetter);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtBeginLetter);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkBigChanges);
            this.Controls.Add(this.txtTicker);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.panel1);
            this.Name = "MetricsForm";
            this.Text = "Metrics";
            this.Load += new System.EventHandler(this.MetricsForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MetricsForm_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTicker;
        private System.Windows.Forms.CheckBox chkBigChanges;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBeginLetter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtEndLetter;
        private System.Windows.Forms.Button btnRunMetrics;
        private System.Windows.Forms.RichTextBox txtTickerList;
        private System.Windows.Forms.Button btnCancelMetrics;
        private System.Windows.Forms.Panel panel1;
    }
}