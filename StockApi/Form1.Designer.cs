
namespace StockApi
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnGetOne = new System.Windows.Forms.Button();
            this.txtStockTicker = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGetAll = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lblBeta = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGetOne
            // 
            this.btnGetOne.BackColor = System.Drawing.Color.Transparent;
            this.btnGetOne.Location = new System.Drawing.Point(33, 43);
            this.btnGetOne.Name = "btnGetOne";
            this.btnGetOne.Size = new System.Drawing.Size(161, 35);
            this.btnGetOne.TabIndex = 0;
            this.btnGetOne.Text = "Get individual stock data";
            this.btnGetOne.UseVisualStyleBackColor = false;
            this.btnGetOne.Click += new System.EventHandler(this.btnGetOne_click);
            // 
            // txtStockTicker
            // 
            this.txtStockTicker.BackColor = System.Drawing.SystemColors.Window;
            this.txtStockTicker.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStockTicker.Location = new System.Drawing.Point(80, 12);
            this.txtStockTicker.Name = "txtStockTicker";
            this.txtStockTicker.Size = new System.Drawing.Size(114, 23);
            this.txtStockTicker.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(23)))), ((int)(((byte)(32)))));
            this.label1.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label1.Location = new System.Drawing.Point(33, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Ticker:";
            // 
            // btnGetAll
            // 
            this.btnGetAll.Location = new System.Drawing.Point(33, 101);
            this.btnGetAll.Name = "btnGetAll";
            this.btnGetAll.Size = new System.Drawing.Size(161, 33);
            this.btnGetAll.TabIndex = 3;
            this.btnGetAll.Text = "Get all stock data";
            this.btnGetAll.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(13, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 34);
            this.label2.TabIndex = 4;
            this.label2.Text = "Beta (5Y Monthly)";
            // 
            // lblBeta
            // 
            this.lblBeta.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblBeta.Location = new System.Drawing.Point(103, 15);
            this.lblBeta.Name = "lblBeta";
            this.lblBeta.Size = new System.Drawing.Size(46, 15);
            this.lblBeta.TabIndex = 5;
            this.lblBeta.Text = "0.00";
            this.lblBeta.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lblBeta);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(260, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(313, 101);
            this.panel1.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.DarkGray;
            this.label3.Location = new System.Drawing.Point(12, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "__________________________";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.BackgroundImage = global::StockApi.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(611, 354);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnGetAll);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtStockTicker);
            this.Controls.Add(this.btnGetOne);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Stock Data";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGetOne;
        private System.Windows.Forms.TextBox txtStockTicker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGetAll;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblBeta;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
    }
}

