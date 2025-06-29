
namespace StockApi
{
    partial class OffHighs
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
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtTickerList
            // 
            this.txtTickerList.AcceptsReturn = true;
            this.txtTickerList.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtTickerList.Location = new System.Drawing.Point(12, 32);
            this.txtTickerList.Multiline = true;
            this.txtTickerList.Name = "txtTickerList";
            this.txtTickerList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTickerList.Size = new System.Drawing.Size(477, 491);
            this.txtTickerList.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(271, 15);
            this.label1.TabIndex = 13;
            this.label1.Text = "Ticker                  High            Current               Target";
            // 
            // OffHighs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 535);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTickerList);
            this.Name = "OffHighs";
            this.Text = "Off Highs";
            this.Load += new System.EventHandler(this.OffHighs_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtTickerList;
        private System.Windows.Forms.Label label1;
    }
}