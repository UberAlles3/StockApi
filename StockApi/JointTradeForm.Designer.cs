
namespace StockApi
{
    partial class JointTradeForm
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
            this.txtTickerList = new System.Windows.Forms.TextBox();
            this.btnGetTrades = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtTickerList
            // 
            this.txtTickerList.AcceptsReturn = true;
            this.txtTickerList.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtTickerList.Location = new System.Drawing.Point(12, 43);
            this.txtTickerList.Multiline = true;
            this.txtTickerList.Name = "txtTickerList";
            this.txtTickerList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTickerList.Size = new System.Drawing.Size(610, 640);
            this.txtTickerList.TabIndex = 13;
            // 
            // btnGetTrades
            // 
            this.btnGetTrades.Location = new System.Drawing.Point(12, 12);
            this.btnGetTrades.Name = "btnGetTrades";
            this.btnGetTrades.Size = new System.Drawing.Size(75, 23);
            this.btnGetTrades.TabIndex = 14;
            this.btnGetTrades.Text = "Get Trades";
            this.btnGetTrades.UseVisualStyleBackColor = true;
            this.btnGetTrades.Click += new System.EventHandler(this.btnGetTrades_Click);
            // 
            // JointTradeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 695);
            this.Controls.Add(this.btnGetTrades);
            this.Controls.Add(this.txtTickerList);
            this.Name = "JointTradeForm";
            this.Text = "Joint Trades";
            this.Load += new System.EventHandler(this.JointTradeForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtTickerList;
        private System.Windows.Forms.Button btnGetTrades;
    }
}