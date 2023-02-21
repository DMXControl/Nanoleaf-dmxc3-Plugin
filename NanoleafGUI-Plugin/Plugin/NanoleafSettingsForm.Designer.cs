
using LumosControls.Controls;

namespace NanoleafGUI_Plugin
{
    partial class NanoleafSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NanoleafSettingsForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1 = new LumosControls.Controls.LumosTabControl();
            this.tabPage1 = new LumosControls.Controls.LumosTabPage();
            this.tabPage2 = new LumosControls.Controls.LumosTabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gbLayout = new LumosControls.Controls.LumosGroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tbName = new LumosControls.Controls.LumosTextBox_v2();
            this.lbName = new LumosControls.Controls.LumosLabel();
            this.tbHW = new LumosControls.Controls.LumosTextBox_v2();
            this.lbHW = new LumosControls.Controls.LumosLabel();
            this.tbModel = new LumosControls.Controls.LumosTextBox_v2();
            this.lbModel = new LumosControls.Controls.LumosLabel();
            this.tbFW = new LumosControls.Controls.LumosTextBox_v2();
            this.tbNS = new LumosControls.Controls.LumosTextBox_v2();
            this.tbAuthToken = new LumosControls.Controls.LumosTextBox_v2();
            this.tbIP = new LumosControls.Controls.LumosTextBox_v2();
            this.lbAuthToken = new LumosControls.Controls.LumosLabel();
            this.lbFW = new LumosControls.Controls.LumosLabel();
            this.lbSN = new LumosControls.Controls.LumosLabel();
            this.lbControllerIP = new LumosControls.Controls.LumosLabel();
            this.btRequestToken = new LumosControls.Controls.LumosButton();
            this.btRemoveController = new LumosControls.Controls.LumosButton();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.cbShowInInputAssignment = new LumosControls.Controls.LumosCheckBox();
            this.cbDiscover = new LumosControls.Controls.LumosCheckBox();
            this.cbAutoRequestToken = new LumosControls.Controls.LumosCheckBox();
            this.cbAutoConnect = new LumosControls.Controls.LumosCheckBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.nudRefreshRate = new LumosControls.Controls.LumosNumericUpDown();
            this.lbRefreshrate = new LumosControls.Controls.LumosLabel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.btClose = new LumosControls.Controls.LumosButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbDiscoveryState = new System.Windows.Forms.Label();
            this.btAddController = new LumosControls.Controls.LumosButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gbLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRefreshRate)).BeginInit();
            this.flowLayoutPanel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 79.52987F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.47013F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1021, 874);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.tabControl1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 53);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(805, 778);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tabControl1.HighlightColor = System.Drawing.Color.DimGray;
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(799, 26);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.OrderTag = null;
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(791, 0);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Controller 1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.OrderTag = null;
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(791, 0);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Controller 2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gbLayout);
            this.panel1.Controls.Add(this.tbName);
            this.panel1.Controls.Add(this.lbName);
            this.panel1.Controls.Add(this.tbHW);
            this.panel1.Controls.Add(this.lbHW);
            this.panel1.Controls.Add(this.tbModel);
            this.panel1.Controls.Add(this.lbModel);
            this.panel1.Controls.Add(this.tbFW);
            this.panel1.Controls.Add(this.tbNS);
            this.panel1.Controls.Add(this.tbAuthToken);
            this.panel1.Controls.Add(this.tbIP);
            this.panel1.Controls.Add(this.lbAuthToken);
            this.panel1.Controls.Add(this.lbFW);
            this.panel1.Controls.Add(this.lbSN);
            this.panel1.Controls.Add(this.lbControllerIP);
            this.panel1.Controls.Add(this.btRequestToken);
            this.panel1.Controls.Add(this.btRemoveController);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 35);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(799, 740);
            this.panel1.TabIndex = 1;
            // 
            // gbLayout
            // 
            this.gbLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbLayout.Controls.Add(this.pictureBox1);
            this.gbLayout.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.gbLayout.Location = new System.Drawing.Point(9, 198);
            this.gbLayout.Name = "gbLayout";
            this.gbLayout.Size = new System.Drawing.Size(785, 539);
            this.gbLayout.TabIndex = 15;
            this.gbLayout.TabStop = false;
            this.gbLayout.Text = "Layout";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(3, 18);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(779, 518);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // tbName
            // 
            this.tbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tbName.Location = new System.Drawing.Point(59, 60);
            this.tbName.Name = "tbName";
            this.tbName.ReadOnly = true;
            this.tbName.Size = new System.Drawing.Size(623, 20);
            this.tbName.TabIndex = 14;
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lbName.Location = new System.Drawing.Point(6, 63);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(43, 16);
            this.lbName.TabIndex = 13;
            this.lbName.Text = "Name:";
            // 
            // tbHW
            // 
            this.tbHW.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tbHW.Location = new System.Drawing.Point(59, 88);
            this.tbHW.Name = "tbHW";
            this.tbHW.ReadOnly = true;
            this.tbHW.Size = new System.Drawing.Size(623, 20);
            this.tbHW.TabIndex = 12;
            // 
            // lbHW
            // 
            this.lbHW.AutoSize = true;
            this.lbHW.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lbHW.Location = new System.Drawing.Point(6, 91);
            this.lbHW.Name = "lbHW";
            this.lbHW.Size = new System.Drawing.Size(30, 16);
            this.lbHW.TabIndex = 11;
            this.lbHW.Text = "HW:";
            // 
            // tbModel
            // 
            this.tbModel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tbModel.Location = new System.Drawing.Point(59, 172);
            this.tbModel.Name = "tbModel";
            this.tbModel.ReadOnly = true;
            this.tbModel.Size = new System.Drawing.Size(623, 20);
            this.tbModel.TabIndex = 10;
            // 
            // lbModel
            // 
            this.lbModel.AutoSize = true;
            this.lbModel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lbModel.Location = new System.Drawing.Point(6, 175);
            this.lbModel.Name = "lbModel";
            this.lbModel.Size = new System.Drawing.Size(44, 16);
            this.lbModel.TabIndex = 9;
            this.lbModel.Text = "Model:";
            // 
            // tbFW
            // 
            this.tbFW.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tbFW.Location = new System.Drawing.Point(59, 144);
            this.tbFW.Name = "tbFW";
            this.tbFW.ReadOnly = true;
            this.tbFW.Size = new System.Drawing.Size(623, 20);
            this.tbFW.TabIndex = 8;
            // 
            // tbNS
            // 
            this.tbNS.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tbNS.Location = new System.Drawing.Point(59, 116);
            this.tbNS.Name = "tbNS";
            this.tbNS.ReadOnly = true;
            this.tbNS.Size = new System.Drawing.Size(623, 20);
            this.tbNS.TabIndex = 7;
            // 
            // tbAuthToken
            // 
            this.tbAuthToken.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tbAuthToken.Location = new System.Drawing.Point(59, 32);
            this.tbAuthToken.Name = "tbAuthToken";
            this.tbAuthToken.Size = new System.Drawing.Size(623, 20);
            this.tbAuthToken.TabIndex = 6;
            // 
            // tbIP
            // 
            this.tbIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tbIP.Location = new System.Drawing.Point(59, 4);
            this.tbIP.Name = "tbIP";
            this.tbIP.ReadOnly = true;
            this.tbIP.Size = new System.Drawing.Size(623, 20);
            this.tbIP.TabIndex = 5;
            // 
            // lbAuthToken
            // 
            this.lbAuthToken.AutoSize = true;
            this.lbAuthToken.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lbAuthToken.Location = new System.Drawing.Point(6, 35);
            this.lbAuthToken.Name = "lbAuthToken";
            this.lbAuthToken.Size = new System.Drawing.Size(45, 16);
            this.lbAuthToken.TabIndex = 4;
            this.lbAuthToken.Text = "Token:";
            // 
            // lbFW
            // 
            this.lbFW.AutoSize = true;
            this.lbFW.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lbFW.Location = new System.Drawing.Point(6, 147);
            this.lbFW.Name = "lbFW";
            this.lbFW.Size = new System.Drawing.Size(28, 16);
            this.lbFW.TabIndex = 3;
            this.lbFW.Text = "FW:";
            // 
            // lbSN
            // 
            this.lbSN.AutoSize = true;
            this.lbSN.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lbSN.Location = new System.Drawing.Point(6, 119);
            this.lbSN.Name = "lbSN";
            this.lbSN.Size = new System.Drawing.Size(26, 16);
            this.lbSN.TabIndex = 2;
            this.lbSN.Text = "SN:";
            // 
            // lbControllerIP
            // 
            this.lbControllerIP.AutoSize = true;
            this.lbControllerIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lbControllerIP.Location = new System.Drawing.Point(6, 7);
            this.lbControllerIP.Name = "lbControllerIP";
            this.lbControllerIP.Size = new System.Drawing.Size(20, 16);
            this.lbControllerIP.TabIndex = 1;
            this.lbControllerIP.Text = "IP:";
            // 
            // btRequestToken
            // 
            this.btRequestToken.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.btRequestToken.Location = new System.Drawing.Point(688, 4);
            this.btRequestToken.Name = "btRequestToken";
            this.btRequestToken.Size = new System.Drawing.Size(106, 50);
            this.btRequestToken.TabIndex = 0;
            this.btRequestToken.Text = "Request Token";
            this.btRequestToken.UseVisualStyleBackColor = true;
            this.btRequestToken.Click += new System.EventHandler(this.btRequestToken_Click);
            // 
            // btRemoveController
            // 
            this.btRemoveController.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.btRemoveController.Location = new System.Drawing.Point(688, 147);
            this.btRemoveController.Name = "btRemoveController";
            this.btRemoveController.Size = new System.Drawing.Size(106, 50);
            this.btRemoveController.TabIndex = 0;
            this.btRemoveController.Text = "Remove Controller";
            this.btRemoveController.UseVisualStyleBackColor = true;
            this.btRemoveController.Click += new System.EventHandler(this.btRemoveController_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.cbShowInInputAssignment);
            this.flowLayoutPanel1.Controls.Add(this.cbDiscover);
            this.flowLayoutPanel1.Controls.Add(this.cbAutoRequestToken);
            this.flowLayoutPanel1.Controls.Add(this.cbAutoConnect);
            this.flowLayoutPanel1.Controls.Add(this.flowLayoutPanel2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(814, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.tableLayoutPanel1.SetRowSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(204, 828);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // cbShowInInputAssignment
            // 
            this.cbShowInInputAssignment.AutoSize = true;
            this.cbShowInInputAssignment.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cbShowInInputAssignment.Location = new System.Drawing.Point(3, 3);
            this.cbShowInInputAssignment.Name = "cbShowInInputAssignment";
            this.cbShowInInputAssignment.Size = new System.Drawing.Size(181, 21);
            this.cbShowInInputAssignment.TabIndex = 0;
            this.cbShowInInputAssignment.Text = "Show in InputAssignment";
            this.cbShowInInputAssignment.UseVisualStyleBackColor = true;
            this.cbShowInInputAssignment.CheckedChanged += new System.EventHandler(this.cbShowInInputAssignment_CheckedChanged);
            // 
            // cbDiscover
            // 
            this.cbDiscover.AutoSize = true;
            this.cbDiscover.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cbDiscover.Location = new System.Drawing.Point(3, 30);
            this.cbDiscover.Name = "cbDiscover";
            this.cbDiscover.Size = new System.Drawing.Size(88, 21);
            this.cbDiscover.TabIndex = 1;
            this.cbDiscover.Text = "Discover";
            this.cbDiscover.UseVisualStyleBackColor = true;
            this.cbDiscover.CheckedChanged += new System.EventHandler(this.cbDiscover_CheckedChanged);
            // 
            // cbAutoRequestToken
            // 
            this.cbAutoRequestToken.AutoSize = true;
            this.cbAutoRequestToken.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cbAutoRequestToken.Location = new System.Drawing.Point(3, 57);
            this.cbAutoRequestToken.Name = "cbAutoRequestToken";
            this.cbAutoRequestToken.Size = new System.Drawing.Size(151, 21);
            this.cbAutoRequestToken.TabIndex = 4;
            this.cbAutoRequestToken.Text = "AutoRequestToken";
            this.cbAutoRequestToken.UseVisualStyleBackColor = true;
            this.cbAutoRequestToken.CheckedChanged += new System.EventHandler(this.cbAutoRequestToken_CheckedChanged);
            // 
            // cbAutoConnect
            // 
            this.cbAutoConnect.AutoSize = true;
            this.cbAutoConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cbAutoConnect.Location = new System.Drawing.Point(3, 84);
            this.cbAutoConnect.Name = "cbAutoConnect";
            this.cbAutoConnect.Size = new System.Drawing.Size(110, 21);
            this.cbAutoConnect.TabIndex = 3;
            this.cbAutoConnect.Text = "AutoConnect";
            this.cbAutoConnect.UseVisualStyleBackColor = true;
            this.cbAutoConnect.CheckedChanged += new System.EventHandler(this.cbAutoConnect_CheckedChanged);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.nudRefreshRate);
            this.flowLayoutPanel2.Controls.Add(this.lbRefreshrate);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 111);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(200, 30);
            this.flowLayoutPanel2.TabIndex = 2;
            // 
            // nudRefreshRate
            // 
            this.nudRefreshRate.DecimalPlaces = 0;
            this.nudRefreshRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.nudRefreshRate.Hexadecimal = false;
            this.nudRefreshRate.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRefreshRate.Location = new System.Drawing.Point(3, 3);
            this.nudRefreshRate.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nudRefreshRate.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudRefreshRate.Name = "nudRefreshRate";
            this.nudRefreshRate.Size = new System.Drawing.Size(46, 20);
            this.nudRefreshRate.TabIndex = 0;
            this.nudRefreshRate.Text = "44";
            this.nudRefreshRate.Value = new decimal(new int[] {
            44,
            0,
            0,
            0});
            this.nudRefreshRate.ValueChanged += new System.EventHandler(this.nudRefreshRate_ValueChanged);
            // 
            // lbRefreshrate
            // 
            this.lbRefreshrate.AutoSize = true;
            this.lbRefreshrate.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbRefreshrate.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lbRefreshrate.Location = new System.Drawing.Point(55, 0);
            this.lbRefreshrate.Name = "lbRefreshrate";
            this.lbRefreshrate.Size = new System.Drawing.Size(77, 26);
            this.lbRefreshrate.TabIndex = 0;
            this.lbRefreshrate.Text = "Refreshrate";
            this.lbRefreshrate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flowLayoutPanel3
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel3, 2);
            this.flowLayoutPanel3.Controls.Add(this.btClose);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 837);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel3.Size = new System.Drawing.Size(1015, 34);
            this.flowLayoutPanel3.TabIndex = 2;
            // 
            // btClose
            // 
            this.btClose.AutoSize = true;
            this.btClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.btClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.btClose.Location = new System.Drawing.Point(888, 3);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(124, 30);
            this.btClose.TabIndex = 1;
            this.btClose.Text = "Close";
            this.btClose.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lbDiscoveryState);
            this.panel2.Controls.Add(this.btAddController);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(805, 44);
            this.panel2.TabIndex = 3;
            // 
            // lbDiscoveryState
            // 
            this.lbDiscoveryState.AutoSize = true;
            this.lbDiscoveryState.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbDiscoveryState.Location = new System.Drawing.Point(716, 0);
            this.lbDiscoveryState.Name = "lbDiscoveryState";
            this.lbDiscoveryState.Size = new System.Drawing.Size(89, 16);
            this.lbDiscoveryState.TabIndex = 18;
            this.lbDiscoveryState.Text = "Discovery {0}";
            // 
            // btAddController
            // 
            this.btAddController.Dock = System.Windows.Forms.DockStyle.Left;
            this.btAddController.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.btAddController.Location = new System.Drawing.Point(0, 0);
            this.btAddController.Name = "btAddController";
            this.btAddController.Size = new System.Drawing.Size(153, 44);
            this.btAddController.TabIndex = 17;
            this.btAddController.Text = "Add Controller";
            this.btAddController.UseVisualStyleBackColor = true;
            this.btAddController.Click += new System.EventHandler(this.btAddController_Click);
            // 
            // NanoleafSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btClose;
            this.ClientSize = new System.Drawing.Size(1021, 874);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "NanoleafSettingsForm";
            this.Text = "Nanoleaf Settings";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gbLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRefreshRate)).EndInit();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private LumosTabControl tabControl1;
        private LumosTabPage tabPage1;
        private LumosTabPage tabPage2;
        private LumosCheckBox cbShowInInputAssignment;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private LumosCheckBox cbDiscover;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private LumosLabel lbRefreshrate;
        private LumosNumericUpDown nudRefreshRate;
        private LumosTextBox_v2 tbNS;
        private LumosTextBox_v2 tbAuthToken;
        private LumosTextBox_v2 tbIP;
        private LumosLabel lbAuthToken;
        private LumosLabel lbFW;
        private LumosLabel lbSN;
        private LumosLabel lbControllerIP;
        private LumosButton btRequestToken;
        private LumosButton btRemoveController;
        private LumosTextBox_v2 tbFW;
        private LumosTextBox_v2 tbName;
        private LumosLabel lbName;
        private LumosTextBox_v2 tbHW;
        private LumosLabel lbHW;
        private LumosTextBox_v2 tbModel;
        private LumosLabel lbModel;
        private LumosCheckBox cbAutoRequestToken;
        private LumosCheckBox cbAutoConnect;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private LumosButton btClose;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbDiscoveryState;
        private LumosButton btAddController;
        private LumosGroupBox gbLayout;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}