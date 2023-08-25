
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnGetOne = new System.Windows.Forms.Button();
            this.txtStockTicker = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGetAll = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lblVolatility = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label33 = new System.Windows.Forms.Label();
            this.lblDividend = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.lblOneYearTarget = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.lblEstimatedReturn = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.lblFairValue = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.picYearTrend = new System.Windows.Forms.PictureBox();
            this.picWeekTrend = new System.Windows.Forms.PictureBox();
            this.label22 = new System.Windows.Forms.Label();
            this.lblTicker = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label9 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblPrice = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblEPS = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.picMonthTrend = new System.Windows.Forms.PictureBox();
            this.picSpinner = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtTickerList = new System.Windows.Forms.TextBox();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtAnalysisOutput = new System.Windows.Forms.TextBox();
            this.lblSellQuantity = new System.Windows.Forms.Label();
            this.lblBuyQuantity = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.lblSellAt = new System.Windows.Forms.Label();
            this.lblBuyAt = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.txtMovementTargetPercent = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtTradePrice = new System.Windows.Forms.TextBox();
            this.radSell = new System.Windows.Forms.RadioButton();
            this.label18 = new System.Windows.Forms.Label();
            this.radBuy = new System.Windows.Forms.RadioButton();
            this.label16 = new System.Windows.Forms.Label();
            this.txtSharesTraded = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtSharesOwned = new System.Windows.Forms.TextBox();
            this.picUpTrend = new System.Windows.Forms.PictureBox();
            this.picDownTrend = new System.Windows.Forms.PictureBox();
            this.picSidewaysTrend = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picYearTrend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWeekTrend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMonthTrend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSpinner)).BeginInit();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUpTrend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDownTrend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSidewaysTrend)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGetOne
            // 
            this.btnGetOne.BackColor = System.Drawing.Color.Transparent;
            this.btnGetOne.ForeColor = System.Drawing.Color.Black;
            this.btnGetOne.Location = new System.Drawing.Point(33, 43);
            this.btnGetOne.Name = "btnGetOne";
            this.btnGetOne.Size = new System.Drawing.Size(213, 35);
            this.btnGetOne.TabIndex = 0;
            this.btnGetOne.Text = "Get Stock Info";
            this.btnGetOne.UseVisualStyleBackColor = false;
            this.btnGetOne.Click += new System.EventHandler(this.btnGetOne_click);
            // 
            // txtStockTicker
            // 
            this.txtStockTicker.BackColor = System.Drawing.SystemColors.Window;
            this.txtStockTicker.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStockTicker.Location = new System.Drawing.Point(80, 12);
            this.txtStockTicker.Name = "txtStockTicker";
            this.txtStockTicker.Size = new System.Drawing.Size(166, 23);
            this.txtStockTicker.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label1.Location = new System.Drawing.Point(33, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Ticker:";
            // 
            // btnGetAll
            // 
            this.btnGetAll.Location = new System.Drawing.Point(33, 327);
            this.btnGetAll.Name = "btnGetAll";
            this.btnGetAll.Size = new System.Drawing.Size(213, 33);
            this.btnGetAll.TabIndex = 3;
            this.btnGetAll.Text = "Get Mutiple Stock Info";
            this.btnGetAll.UseVisualStyleBackColor = true;
            this.btnGetAll.Click += new System.EventHandler(this.btnGetAll_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label2.Location = new System.Drawing.Point(15, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 22);
            this.label2.TabIndex = 4;
            this.label2.Text = "Volatility";
            // 
            // lblVolatility
            // 
            this.lblVolatility.BackColor = System.Drawing.Color.Transparent;
            this.lblVolatility.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblVolatility.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblVolatility.Location = new System.Drawing.Point(105, 94);
            this.lblVolatility.Name = "lblVolatility";
            this.lblVolatility.Size = new System.Drawing.Size(46, 22);
            this.lblVolatility.TabIndex = 5;
            this.lblVolatility.Text = "0.00";
            this.lblVolatility.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.label33);
            this.panel1.Controls.Add(this.lblDividend);
            this.panel1.Controls.Add(this.label35);
            this.panel1.Controls.Add(this.label53);
            this.panel1.Controls.Add(this.lblOneYearTarget);
            this.panel1.Controls.Add(this.label55);
            this.panel1.Controls.Add(this.label24);
            this.panel1.Controls.Add(this.lblEstimatedReturn);
            this.panel1.Controls.Add(this.label27);
            this.panel1.Controls.Add(this.label23);
            this.panel1.Controls.Add(this.lblFairValue);
            this.panel1.Controls.Add(this.label25);
            this.panel1.Controls.Add(this.picYearTrend);
            this.panel1.Controls.Add(this.picWeekTrend);
            this.panel1.Controls.Add(this.label22);
            this.panel1.Controls.Add(this.lblTicker);
            this.panel1.Controls.Add(this.label21);
            this.panel1.Controls.Add(this.label20);
            this.panel1.Controls.Add(this.label19);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.lblPrice);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.lblEPS);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lblVolatility);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.picMonthTrend);
            this.panel1.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel1.Location = new System.Drawing.Point(270, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(344, 480);
            this.panel1.TabIndex = 6;
            // 
            // label33
            // 
            this.label33.BackColor = System.Drawing.Color.Transparent;
            this.label33.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label33.Location = new System.Drawing.Point(15, 127);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(98, 22);
            this.label33.TabIndex = 33;
            this.label33.Text = "Dividend";
            // 
            // lblDividend
            // 
            this.lblDividend.BackColor = System.Drawing.Color.Transparent;
            this.lblDividend.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDividend.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblDividend.Location = new System.Drawing.Point(86, 127);
            this.lblDividend.Name = "lblDividend";
            this.lblDividend.Size = new System.Drawing.Size(65, 22);
            this.lblDividend.TabIndex = 34;
            this.lblDividend.Text = "0.00";
            this.lblDividend.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.BackColor = System.Drawing.Color.Transparent;
            this.label35.ForeColor = System.Drawing.Color.DarkGray;
            this.label35.Location = new System.Drawing.Point(15, 136);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(137, 15);
            this.label35.TabIndex = 35;
            this.label35.Text = "__________________________";
            // 
            // label53
            // 
            this.label53.BackColor = System.Drawing.Color.Transparent;
            this.label53.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label53.Location = new System.Drawing.Point(190, 128);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(92, 22);
            this.label53.TabIndex = 30;
            this.label53.Text = "One Year Target";
            // 
            // lblOneYearTarget
            // 
            this.lblOneYearTarget.BackColor = System.Drawing.Color.Transparent;
            this.lblOneYearTarget.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblOneYearTarget.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblOneYearTarget.Location = new System.Drawing.Point(280, 128);
            this.lblOneYearTarget.Name = "lblOneYearTarget";
            this.lblOneYearTarget.Size = new System.Drawing.Size(46, 22);
            this.lblOneYearTarget.TabIndex = 31;
            this.lblOneYearTarget.Text = "0.00";
            this.lblOneYearTarget.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.BackColor = System.Drawing.Color.Transparent;
            this.label55.ForeColor = System.Drawing.Color.DarkGray;
            this.label55.Location = new System.Drawing.Point(190, 137);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(137, 15);
            this.label55.TabIndex = 32;
            this.label55.Text = "__________________________";
            // 
            // label24
            // 
            this.label24.BackColor = System.Drawing.Color.Transparent;
            this.label24.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label24.Location = new System.Drawing.Point(15, 160);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(98, 22);
            this.label24.TabIndex = 27;
            this.label24.Text = "Estimated Return";
            // 
            // lblEstimatedReturn
            // 
            this.lblEstimatedReturn.BackColor = System.Drawing.Color.Transparent;
            this.lblEstimatedReturn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblEstimatedReturn.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblEstimatedReturn.Location = new System.Drawing.Point(105, 160);
            this.lblEstimatedReturn.Name = "lblEstimatedReturn";
            this.lblEstimatedReturn.Size = new System.Drawing.Size(46, 22);
            this.lblEstimatedReturn.TabIndex = 28;
            this.lblEstimatedReturn.Text = "0.00";
            this.lblEstimatedReturn.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.BackColor = System.Drawing.Color.Transparent;
            this.label27.ForeColor = System.Drawing.Color.DarkGray;
            this.label27.Location = new System.Drawing.Point(15, 169);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(137, 15);
            this.label27.TabIndex = 29;
            this.label27.Text = "__________________________";
            // 
            // label23
            // 
            this.label23.BackColor = System.Drawing.Color.Transparent;
            this.label23.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label23.Location = new System.Drawing.Point(191, 92);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(57, 22);
            this.label23.TabIndex = 24;
            this.label23.Text = "Fair Value";
            // 
            // lblFairValue
            // 
            this.lblFairValue.BackColor = System.Drawing.Color.Transparent;
            this.lblFairValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblFairValue.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblFairValue.Location = new System.Drawing.Point(248, 92);
            this.lblFairValue.Name = "lblFairValue";
            this.lblFairValue.Size = new System.Drawing.Size(79, 22);
            this.lblFairValue.TabIndex = 25;
            this.lblFairValue.Text = "0.00";
            this.lblFairValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.BackColor = System.Drawing.Color.Transparent;
            this.label25.ForeColor = System.Drawing.Color.DarkGray;
            this.label25.Location = new System.Drawing.Point(190, 101);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(137, 15);
            this.label25.TabIndex = 26;
            this.label25.Text = "__________________________";
            // 
            // picYearTrend
            // 
            this.picYearTrend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(13)))), ((int)(((byte)(20)))));
            this.picYearTrend.Image = global::StockApi.Properties.Resources.UpTrend1;
            this.picYearTrend.Location = new System.Drawing.Point(62, 429);
            this.picYearTrend.Name = "picYearTrend";
            this.picYearTrend.Size = new System.Drawing.Size(32, 29);
            this.picYearTrend.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picYearTrend.TabIndex = 19;
            this.picYearTrend.TabStop = false;
            // 
            // picWeekTrend
            // 
            this.picWeekTrend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(24)))), ((int)(((byte)(36)))));
            this.picWeekTrend.Image = global::StockApi.Properties.Resources.downTrend1;
            this.picWeekTrend.Location = new System.Drawing.Point(266, 428);
            this.picWeekTrend.Name = "picWeekTrend";
            this.picWeekTrend.Size = new System.Drawing.Size(33, 29);
            this.picWeekTrend.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picWeekTrend.TabIndex = 23;
            this.picWeekTrend.TabStop = false;
            // 
            // label22
            // 
            this.label22.BackColor = System.Drawing.Color.Transparent;
            this.label22.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label22.Location = new System.Drawing.Point(228, 436);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(50, 22);
            this.label22.TabIndex = 22;
            this.label22.Text = "Week";
            // 
            // lblTicker
            // 
            this.lblTicker.BackColor = System.Drawing.Color.Transparent;
            this.lblTicker.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTicker.Location = new System.Drawing.Point(9, 11);
            this.lblTicker.Name = "lblTicker";
            this.lblTicker.Size = new System.Drawing.Size(318, 32);
            this.lblTicker.TabIndex = 10;
            this.lblTicker.Text = "lblTicker";
            this.lblTicker.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label21
            // 
            this.label21.BackColor = System.Drawing.Color.Transparent;
            this.label21.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label21.Location = new System.Drawing.Point(124, 436);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(50, 22);
            this.label21.TabIndex = 20;
            this.label21.Text = "Month";
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.Transparent;
            this.label20.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label20.Location = new System.Drawing.Point(27, 436);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(38, 22);
            this.label20.TabIndex = 17;
            this.label20.Text = "Year";
            // 
            // label19
            // 
            this.label19.BackColor = System.Drawing.Color.Transparent;
            this.label19.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label19.Location = new System.Drawing.Point(51, 405);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(233, 25);
            this.label19.TabIndex = 16;
            this.label19.Text = "Price Trend";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.ForeColor = System.Drawing.Color.DarkGray;
            this.label11.Location = new System.Drawing.Point(14, 34);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(312, 15);
            this.label11.TabIndex = 15;
            this.label11.Text = "_____________________________________________________________";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(13)))), ((int)(((byte)(20)))));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ColumnHeadersVisible = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.Location = new System.Drawing.Point(9, 262);
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
            this.dataGridView1.Size = new System.Drawing.Size(312, 124);
            this.dataGridView1.TabIndex = 14;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label9.Location = new System.Drawing.Point(167, 237);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 22);
            this.label9.TabIndex = 11;
            this.label9.Text = "Price";
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.Color.Transparent;
            this.label14.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label14.Location = new System.Drawing.Point(256, 237);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(67, 22);
            this.label14.TabIndex = 7;
            this.label14.Text = "Volume";
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.Color.Transparent;
            this.label17.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label17.Location = new System.Drawing.Point(9, 237);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(57, 22);
            this.label17.TabIndex = 4;
            this.label17.Text = "Date";
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.Color.Transparent;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label13.Location = new System.Drawing.Point(49, 205);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(233, 29);
            this.label13.TabIndex = 10;
            this.label13.Text = "Historic Price";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.ForeColor = System.Drawing.Color.DarkGray;
            this.label12.Location = new System.Drawing.Point(9, 246);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(312, 15);
            this.label12.TabIndex = 13;
            this.label12.Text = "_____________________________________________________________";
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label8.Location = new System.Drawing.Point(14, 58);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 24);
            this.label8.TabIndex = 11;
            this.label8.Text = "Price";
            // 
            // lblPrice
            // 
            this.lblPrice.BackColor = System.Drawing.Color.Transparent;
            this.lblPrice.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblPrice.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblPrice.Location = new System.Drawing.Point(105, 60);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(46, 22);
            this.lblPrice.TabIndex = 12;
            this.lblPrice.Text = "0.00";
            this.lblPrice.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.ForeColor = System.Drawing.Color.DarkGray;
            this.label10.Location = new System.Drawing.Point(14, 69);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(137, 15);
            this.label10.TabIndex = 13;
            this.label10.Text = "__________________________";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label4.Location = new System.Drawing.Point(190, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "EPS (TTM)";
            // 
            // lblEPS
            // 
            this.lblEPS.BackColor = System.Drawing.Color.Transparent;
            this.lblEPS.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblEPS.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblEPS.Location = new System.Drawing.Point(280, 60);
            this.lblEPS.Name = "lblEPS";
            this.lblEPS.Size = new System.Drawing.Size(46, 15);
            this.lblEPS.TabIndex = 8;
            this.lblEPS.Text = "0.00";
            this.lblEPS.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.ForeColor = System.Drawing.Color.DarkGray;
            this.label6.Location = new System.Drawing.Point(190, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(137, 15);
            this.label6.TabIndex = 9;
            this.label6.Text = "__________________________";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.DarkGray;
            this.label3.Location = new System.Drawing.Point(15, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "__________________________";
            // 
            // picMonthTrend
            // 
            this.picMonthTrend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(18)))), ((int)(((byte)(26)))));
            this.picMonthTrend.Image = global::StockApi.Properties.Resources.RightArrow;
            this.picMonthTrend.Location = new System.Drawing.Point(171, 429);
            this.picMonthTrend.Name = "picMonthTrend";
            this.picMonthTrend.Size = new System.Drawing.Size(33, 29);
            this.picMonthTrend.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picMonthTrend.TabIndex = 21;
            this.picMonthTrend.TabStop = false;
            // 
            // picSpinner
            // 
            this.picSpinner.BackColor = System.Drawing.Color.Transparent;
            this.picSpinner.Image = global::StockApi.Properties.Resources.Spin1;
            this.picSpinner.Location = new System.Drawing.Point(33, 371);
            this.picSpinner.Name = "picSpinner";
            this.picSpinner.Size = new System.Drawing.Size(117, 73);
            this.picSpinner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picSpinner.TabIndex = 13;
            this.picSpinner.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label5.Location = new System.Drawing.Point(33, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(158, 15);
            this.label5.TabIndex = 7;
            this.label5.Text = "Tickers (CRLF delimeted list):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.ForeColor = System.Drawing.Color.DarkGray;
            this.label7.Location = new System.Drawing.Point(33, 118);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(217, 15);
            this.label7.TabIndex = 10;
            this.label7.Text = "__________________________________________";
            // 
            // txtTickerList
            // 
            this.txtTickerList.Location = new System.Drawing.Point(33, 163);
            this.txtTickerList.Multiline = true;
            this.txtTickerList.Name = "txtTickerList";
            this.txtTickerList.Size = new System.Drawing.Size(213, 152);
            this.txtTickerList.TabIndex = 11;
            this.txtTickerList.Text = "INTC";
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAnalyze.Location = new System.Drawing.Point(17, 149);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(260, 33);
            this.btnAnalyze.TabIndex = 12;
            this.btnAnalyze.Text = "Analyze for Next Trades";
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.txtAnalysisOutput);
            this.panel2.Controls.Add(this.lblSellQuantity);
            this.panel2.Controls.Add(this.lblBuyQuantity);
            this.panel2.Controls.Add(this.label32);
            this.panel2.Controls.Add(this.label31);
            this.panel2.Controls.Add(this.lblSellAt);
            this.panel2.Controls.Add(this.lblBuyAt);
            this.panel2.Controls.Add(this.label30);
            this.panel2.Controls.Add(this.label29);
            this.panel2.Controls.Add(this.label28);
            this.panel2.Controls.Add(this.txtMovementTargetPercent);
            this.panel2.Controls.Add(this.label26);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.btnAnalyze);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.txtSharesOwned);
            this.panel2.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel2.Location = new System.Drawing.Point(641, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(294, 480);
            this.panel2.TabIndex = 14;
            // 
            // txtAnalysisOutput
            // 
            this.txtAnalysisOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(13)))), ((int)(((byte)(20)))));
            this.txtAnalysisOutput.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.txtAnalysisOutput.Location = new System.Drawing.Point(20, 198);
            this.txtAnalysisOutput.Multiline = true;
            this.txtAnalysisOutput.Name = "txtAnalysisOutput";
            this.txtAnalysisOutput.Size = new System.Drawing.Size(257, 174);
            this.txtAnalysisOutput.TabIndex = 33;
            // 
            // lblSellQuantity
            // 
            this.lblSellQuantity.BackColor = System.Drawing.Color.Transparent;
            this.lblSellQuantity.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSellQuantity.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblSellQuantity.Location = new System.Drawing.Point(51, 439);
            this.lblSellQuantity.Name = "lblSellQuantity";
            this.lblSellQuantity.Size = new System.Drawing.Size(46, 15);
            this.lblSellQuantity.TabIndex = 32;
            this.lblSellQuantity.Text = "0";
            this.lblSellQuantity.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblBuyQuantity
            // 
            this.lblBuyQuantity.BackColor = System.Drawing.Color.Transparent;
            this.lblBuyQuantity.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblBuyQuantity.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblBuyQuantity.Location = new System.Drawing.Point(51, 421);
            this.lblBuyQuantity.Name = "lblBuyQuantity";
            this.lblBuyQuantity.Size = new System.Drawing.Size(46, 15);
            this.lblBuyQuantity.TabIndex = 31;
            this.lblBuyQuantity.Text = "0";
            this.lblBuyQuantity.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label32
            // 
            this.label32.BackColor = System.Drawing.Color.Transparent;
            this.label32.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label32.Location = new System.Drawing.Point(107, 439);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(30, 22);
            this.label32.TabIndex = 30;
            this.label32.Text = "@";
            // 
            // label31
            // 
            this.label31.BackColor = System.Drawing.Color.Transparent;
            this.label31.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label31.Location = new System.Drawing.Point(107, 422);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(30, 22);
            this.label31.TabIndex = 29;
            this.label31.Text = "@";
            // 
            // lblSellAt
            // 
            this.lblSellAt.BackColor = System.Drawing.Color.Transparent;
            this.lblSellAt.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSellAt.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblSellAt.Location = new System.Drawing.Point(135, 441);
            this.lblSellAt.Name = "lblSellAt";
            this.lblSellAt.Size = new System.Drawing.Size(46, 15);
            this.lblSellAt.TabIndex = 28;
            this.lblSellAt.Text = "0.00";
            this.lblSellAt.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblBuyAt
            // 
            this.lblBuyAt.BackColor = System.Drawing.Color.Transparent;
            this.lblBuyAt.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblBuyAt.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblBuyAt.Location = new System.Drawing.Point(135, 422);
            this.lblBuyAt.Name = "lblBuyAt";
            this.lblBuyAt.Size = new System.Drawing.Size(46, 15);
            this.lblBuyAt.TabIndex = 27;
            this.lblBuyAt.Text = "0.00";
            this.lblBuyAt.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label30
            // 
            this.label30.BackColor = System.Drawing.Color.Transparent;
            this.label30.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label30.Location = new System.Drawing.Point(18, 440);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(48, 22);
            this.label30.TabIndex = 26;
            this.label30.Text = "Sell";
            // 
            // label29
            // 
            this.label29.BackColor = System.Drawing.Color.Transparent;
            this.label29.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label29.Location = new System.Drawing.Point(17, 421);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(48, 22);
            this.label29.TabIndex = 25;
            this.label29.Text = "Buy ";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.BackColor = System.Drawing.Color.Transparent;
            this.label28.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label28.Location = new System.Drawing.Point(17, 125);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(155, 15);
            this.label28.TabIndex = 22;
            this.label28.Text = "Normal Trade Movement %:";
            // 
            // txtMovementTargetPercent
            // 
            this.txtMovementTargetPercent.Location = new System.Drawing.Point(187, 117);
            this.txtMovementTargetPercent.Name = "txtMovementTargetPercent";
            this.txtMovementTargetPercent.Size = new System.Drawing.Size(51, 23);
            this.txtMovementTargetPercent.TabIndex = 21;
            this.txtMovementTargetPercent.Text = "20";
            this.txtMovementTargetPercent.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label26
            // 
            this.label26.BackColor = System.Drawing.Color.Transparent;
            this.label26.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label26.Location = new System.Drawing.Point(24, 390);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(233, 29);
            this.label26.TabIndex = 20;
            this.label26.Text = "Next Trades";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtTradePrice);
            this.groupBox1.Controls.Add(this.radSell);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.radBuy);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.txtSharesTraded);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.groupBox1.Location = new System.Drawing.Point(20, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(257, 75);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Last Trade";
            // 
            // txtTradePrice
            // 
            this.txtTradePrice.Location = new System.Drawing.Point(167, 44);
            this.txtTradePrice.Name = "txtTradePrice";
            this.txtTradePrice.Size = new System.Drawing.Size(70, 23);
            this.txtTradePrice.TabIndex = 21;
            this.txtTradePrice.Text = "27";
            this.txtTradePrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // radSell
            // 
            this.radSell.AutoSize = true;
            this.radSell.Location = new System.Drawing.Point(14, 44);
            this.radSell.Name = "radSell";
            this.radSell.Size = new System.Drawing.Size(48, 19);
            this.radSell.TabIndex = 1;
            this.radSell.Text = "Sold";
            this.radSell.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.BackColor = System.Drawing.Color.Transparent;
            this.label18.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label18.Location = new System.Drawing.Point(125, 49);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(36, 15);
            this.label18.TabIndex = 20;
            this.label18.Text = "Price:";
            // 
            // radBuy
            // 
            this.radBuy.AutoSize = true;
            this.radBuy.Checked = true;
            this.radBuy.Location = new System.Drawing.Point(13, 22);
            this.radBuy.Name = "radBuy";
            this.radBuy.Size = new System.Drawing.Size(64, 19);
            this.radBuy.TabIndex = 0;
            this.radBuy.TabStop = true;
            this.radBuy.Text = "Bought";
            this.radBuy.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.Transparent;
            this.label16.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.label16.Location = new System.Drawing.Point(81, 20);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(82, 15);
            this.label16.TabIndex = 17;
            this.label16.Text = "Shares Traded:";
            // 
            // txtSharesTraded
            // 
            this.txtSharesTraded.Location = new System.Drawing.Point(167, 15);
            this.txtSharesTraded.Name = "txtSharesTraded";
            this.txtSharesTraded.Size = new System.Drawing.Size(70, 23);
            this.txtSharesTraded.TabIndex = 18;
            this.txtSharesTraded.Text = "20";
            this.txtSharesTraded.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.Transparent;
            this.label15.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label15.Location = new System.Drawing.Point(17, 17);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(85, 15);
            this.label15.TabIndex = 16;
            this.label15.Text = "Shares Owned:";
            // 
            // txtSharesOwned
            // 
            this.txtSharesOwned.Location = new System.Drawing.Point(106, 11);
            this.txtSharesOwned.Name = "txtSharesOwned";
            this.txtSharesOwned.Size = new System.Drawing.Size(51, 23);
            this.txtSharesOwned.TabIndex = 15;
            this.txtSharesOwned.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // picUpTrend
            // 
            this.picUpTrend.Image = global::StockApi.Properties.Resources.UpTrend1;
            this.picUpTrend.Location = new System.Drawing.Point(171, 371);
            this.picUpTrend.Name = "picUpTrend";
            this.picUpTrend.Size = new System.Drawing.Size(35, 34);
            this.picUpTrend.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picUpTrend.TabIndex = 18;
            this.picUpTrend.TabStop = false;
            // 
            // picDownTrend
            // 
            this.picDownTrend.Image = global::StockApi.Properties.Resources.downTrend1;
            this.picDownTrend.Location = new System.Drawing.Point(171, 423);
            this.picDownTrend.Name = "picDownTrend";
            this.picDownTrend.Size = new System.Drawing.Size(35, 34);
            this.picDownTrend.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picDownTrend.TabIndex = 19;
            this.picDownTrend.TabStop = false;
            // 
            // picSidewaysTrend
            // 
            this.picSidewaysTrend.Image = global::StockApi.Properties.Resources.RightArrow;
            this.picSidewaysTrend.Location = new System.Drawing.Point(192, 396);
            this.picSidewaysTrend.Name = "picSidewaysTrend";
            this.picSidewaysTrend.Size = new System.Drawing.Size(35, 34);
            this.picSidewaysTrend.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picSidewaysTrend.TabIndex = 20;
            this.picSidewaysTrend.TabStop = false;
            // 
            // Form1
            // 
            this.AcceptButton = this.btnGetOne;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.BackgroundImage = global::StockApi.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(965, 516);
            this.Controls.Add(this.picSidewaysTrend);
            this.Controls.Add(this.picDownTrend);
            this.Controls.Add(this.picUpTrend);
            this.Controls.Add(this.txtTickerList);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnGetAll);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtStockTicker);
            this.Controls.Add(this.btnGetOne);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.picSpinner);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Stock Data";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picYearTrend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWeekTrend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMonthTrend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSpinner)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUpTrend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDownTrend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSidewaysTrend)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGetOne;
        private System.Windows.Forms.TextBox txtStockTicker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGetAll;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblVolatility;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblEPS;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtTickerList;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.PictureBox picSpinner;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtSharesTraded;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtSharesOwned;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radSell;
        private System.Windows.Forms.RadioButton radBuy;
        private System.Windows.Forms.TextBox txtTradePrice;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.PictureBox picMonthTrend;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.PictureBox picYearTrend;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.PictureBox picUpTrend;
        private System.Windows.Forms.PictureBox picDownTrend;
        private System.Windows.Forms.PictureBox picWeekTrend;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label lblFairValue;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label lblEstimatedReturn;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Label lblOneYearTarget;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.Label lblTicker;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox txtMovementTargetPercent;
        private System.Windows.Forms.Label lblSellAt;
        private System.Windows.Forms.Label lblBuyAt;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label lblSellQuantity;
        private System.Windows.Forms.Label lblBuyQuantity;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label lblDividend;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.PictureBox picSidewaysTrend;
        private System.Windows.Forms.TextBox txtAnalysisOutput;
    }
}

