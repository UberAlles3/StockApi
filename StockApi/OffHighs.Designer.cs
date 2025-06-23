
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
            this.SuspendLayout();
            // 
            // txtTickerList
            // 
            this.txtTickerList.AcceptsReturn = true;
            this.txtTickerList.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtTickerList.Location = new System.Drawing.Point(12, 23);
            this.txtTickerList.Multiline = true;
            this.txtTickerList.Name = "txtTickerList";
            this.txtTickerList.Size = new System.Drawing.Size(477, 292);
            this.txtTickerList.TabIndex = 12;
            // 
            // OffHighs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 335);
            this.Controls.Add(this.txtTickerList);
            this.Name = "OffHighs";
            this.Text = "Off Highs";
            this.Load += new System.EventHandler(this.OffHighs_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtTickerList;
    }
}