
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnGetOne = new System.Windows.Forms.Button();
            this.txtStockTicker = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGetAll = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lblBeta = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label22 = new System.Windows.Forms.Label();
            this.picMonthTrend = new System.Windows.Forms.PictureBox();
            this.label21 = new System.Windows.Forms.Label();
            this.picYearTrend = new System.Windows.Forms.PictureBox();
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
            this.lblTicker = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblEPS = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.picSpinner = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtTickerList = new System.Windows.Forms.TextBox();
            this.btnGetHistory = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtTradePrice = new System.Windows.Forms.TextBox();
            this.radSell = new System.Windows.Forms.RadioButton();
            this.label18 = new System.Windows.Forms.Label();
            this.radBuy = new System.Windows.Forms.RadioButton();
            this.label16 = new System.Windows.Forms.Label();
            this.txtTraded = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtSharesOwned = new System.Windows.Forms.TextBox();
            this.picUpTrend = new System.Windows.Forms.PictureBox();
            this.picDownTrend = new System.Windows.Forms.PictureBox();
            this.picWeekTrend = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMonthTrend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picYearTrend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSpinner)).BeginInit();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUpTrend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDownTrend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWeekTrend)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGetOne
            // 
            this.btnGetOne.BackColor = System.Drawing.Color.Transparent;
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
            this.txtStockTicker.Text = "intc";
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
            this.btnGetAll.Location = new System.Drawing.Point(33, 305);
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
            // lblBeta
            // 
            this.lblBeta.BackColor = System.Drawing.Color.Transparent;
            this.lblBeta.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblBeta.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblBeta.Location = new System.Drawing.Point(105, 94);
            this.lblBeta.Name = "lblBeta";
            this.lblBeta.Size = new System.Drawing.Size(46, 22);
            this.lblBeta.TabIndex = 5;
            this.lblBeta.Text = "0.00";
            this.lblBeta.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.picWeekTrend);
            this.panel1.Controls.Add(this.label22);
            this.panel1.Controls.Add(this.picMonthTrend);
            this.panel1.Controls.Add(this.label21);
            this.panel1.Controls.Add(this.picYearTrend);
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
            this.panel1.Controls.Add(this.lblTicker);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.lblEPS);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lblBeta);
            this.panel1.Controls.Add(this.label3);
            this.panel1.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel1.Location = new System.Drawing.Point(270, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(332, 432);
            this.panel1.TabIndex = 6;
            // 
            // label22
            // 
            this.label22.BackColor = System.Drawing.Color.Transparent;
            this.label22.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label22.Location = new System.Drawing.Point(228, 402);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(50, 22);
            this.label22.TabIndex = 22;
            this.label22.Text = "Week";
            // 
            // picMonthTrend
            // 
            this.picMonthTrend.Image = global::StockApi.Properties.Resources.UpTrend1;
            this.picMonthTrend.Location = new System.Drawing.Point(171, 395);
            this.picMonthTrend.Name = "picMonthTrend";
            this.picMonthTrend.Size = new System.Drawing.Size(33, 29);
            this.picMonthTrend.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picMonthTrend.TabIndex = 21;
            this.picMonthTrend.TabStop = false;
            // 
            // label21
            // 
            this.label21.BackColor = System.Drawing.Color.Transparent;
            this.label21.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label21.Location = new System.Drawing.Point(124, 402);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(50, 22);
            this.label21.TabIndex = 20;
            this.label21.Text = "Month";
            // 
            // picYearTrend
            // 
            this.picYearTrend.Image = global::StockApi.Properties.Resources.UpTrend1;
            this.picYearTrend.Location = new System.Drawing.Point(62, 395);
            this.picYearTrend.Name = "picYearTrend";
            this.picYearTrend.Size = new System.Drawing.Size(32, 29);
            this.picYearTrend.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picYearTrend.TabIndex = 19;
            this.picYearTrend.TabStop = false;
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.Transparent;
            this.label20.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label20.Location = new System.Drawing.Point(27, 402);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(38, 22);
            this.label20.TabIndex = 17;
            this.label20.Text = "Year";
            // 
            // label19
            // 
            this.label19.BackColor = System.Drawing.Color.Transparent;
            this.label19.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label19.Location = new System.Drawing.Point(51, 371);
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
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ColumnHeadersVisible = false;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Location = new System.Drawing.Point(6, 251);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.Size = new System.Drawing.Size(312, 113);
            this.dataGridView1.TabIndex = 14;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label9.Location = new System.Drawing.Point(164, 226);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 22);
            this.label9.TabIndex = 11;
            this.label9.Text = "Price";
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.Color.Transparent;
            this.label14.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label14.Location = new System.Drawing.Point(253, 226);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(67, 22);
            this.label14.TabIndex = 7;
            this.label14.Text = "Volume";
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.Color.Transparent;
            this.label17.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label17.Location = new System.Drawing.Point(6, 226);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(57, 22);
            this.label17.TabIndex = 4;
            this.label17.Text = "Date";
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.Color.Transparent;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label13.Location = new System.Drawing.Point(49, 194);
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
            this.label12.Location = new System.Drawing.Point(6, 235);
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
            // lblTicker
            // 
            this.lblTicker.BackColor = System.Drawing.Color.Transparent;
            this.lblTicker.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTicker.Location = new System.Drawing.Point(49, 4);
            this.lblTicker.Name = "lblTicker";
            this.lblTicker.Size = new System.Drawing.Size(233, 32);
            this.lblTicker.TabIndex = 10;
            this.lblTicker.Text = "lblTicker";
            this.lblTicker.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.label5.Location = new System.Drawing.Point(33, 118);
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
            this.label7.Location = new System.Drawing.Point(33, 87);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(217, 15);
            this.label7.TabIndex = 10;
            this.label7.Text = "__________________________________________";
            // 
            // txtTickerList
            // 
            this.txtTickerList.Location = new System.Drawing.Point(33, 141);
            this.txtTickerList.Multiline = true;
            this.txtTickerList.Name = "txtTickerList";
            this.txtTickerList.Size = new System.Drawing.Size(213, 152);
            this.txtTickerList.TabIndex = 11;
            this.txtTickerList.Text = "INTC";
            // 
            // btnGetHistory
            // 
            this.btnGetHistory.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnGetHistory.Location = new System.Drawing.Point(17, 119);
            this.btnGetHistory.Name = "btnGetHistory";
            this.btnGetHistory.Size = new System.Drawing.Size(260, 33);
            this.btnGetHistory.TabIndex = 12;
            this.btnGetHistory.Text = "Analyze";
            this.btnGetHistory.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.btnGetHistory);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.txtSharesOwned);
            this.panel2.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel2.Location = new System.Drawing.Point(630, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(294, 432);
            this.panel2.TabIndex = 14;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtTradePrice);
            this.groupBox1.Controls.Add(this.radSell);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.radBuy);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.txtTraded);
            this.groupBox1.Location = new System.Drawing.Point(20, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(257, 75);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            // 
            // txtTradePrice
            // 
            this.txtTradePrice.Location = new System.Drawing.Point(167, 44);
            this.txtTradePrice.Name = "txtTradePrice";
            this.txtTradePrice.Size = new System.Drawing.Size(70, 23);
            this.txtTradePrice.TabIndex = 21;
            // 
            // radSell
            // 
            this.radSell.AutoSize = true;
            this.radSell.Location = new System.Drawing.Point(14, 44);
            this.radSell.Name = "radSell";
            this.radSell.Size = new System.Drawing.Size(43, 19);
            this.radSell.TabIndex = 1;
            this.radSell.Text = "Sell";
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
            this.radBuy.Size = new System.Drawing.Size(45, 19);
            this.radBuy.TabIndex = 0;
            this.radBuy.TabStop = true;
            this.radBuy.Text = "Buy";
            this.radBuy.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.Transparent;
            this.label16.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label16.Location = new System.Drawing.Point(81, 20);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(82, 15);
            this.label16.TabIndex = 17;
            this.label16.Text = "Shares Traded:";
            // 
            // txtTraded
            // 
            this.txtTraded.Location = new System.Drawing.Point(167, 15);
            this.txtTraded.Name = "txtTraded";
            this.txtTraded.Size = new System.Drawing.Size(70, 23);
            this.txtTraded.TabIndex = 18;
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
            // picWeekTrend
            // 
            this.picWeekTrend.Image = global::StockApi.Properties.Resources.downTrend1;
            this.picWeekTrend.Location = new System.Drawing.Point(266, 394);
            this.picWeekTrend.Name = "picWeekTrend";
            this.picWeekTrend.Size = new System.Drawing.Size(33, 29);
            this.picWeekTrend.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picWeekTrend.TabIndex = 23;
            this.picWeekTrend.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.BackgroundImage = global::StockApi.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(965, 477);
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Stock Data";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMonthTrend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picYearTrend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSpinner)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUpTrend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDownTrend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWeekTrend)).EndInit();
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
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblEPS;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtTickerList;
        private System.Windows.Forms.Label lblTicker;
        private System.Windows.Forms.Button btnGetHistory;
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
        private System.Windows.Forms.TextBox txtTraded;
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
    }
}

