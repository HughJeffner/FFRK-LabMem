
namespace FFRK_LabMem.Config
{
    partial class ConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.listCategory = new System.Windows.Forms.ListBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.numericUpDownScreenBottom = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownScreenTop = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxDatalog = new System.Windows.Forms.CheckBox();
            this.checkBoxPrerelease = new System.Windows.Forms.CheckBox();
            this.checkBoxUpdates = new System.Windows.Forms.CheckBox();
            this.checkBoxDebug = new System.Windows.Forms.CheckBox();
            this.checkBoxTimestamps = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.buttonProxyBlocklist = new System.Windows.Forms.Button();
            this.textBoxProxyBlocklist = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxProxySecure = new System.Windows.Forms.CheckBox();
            this.numericUpDownProxyPort = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.textBoxAdbHost = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxAdbPath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDownWatchdog = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBoxLabUsePotions = new System.Windows.Forms.CheckBox();
            this.checkBoxLabRestart = new System.Windows.Forms.CheckBox();
            this.checkBoxLabStopOnMasterPainting = new System.Windows.Forms.CheckBox();
            this.checkBoxLabRestartFailedBattle = new System.Windows.Forms.CheckBox();
            this.checkBoxLabAvoidPortal = new System.Windows.Forms.CheckBox();
            this.checkBoxLabAvoidExplore = new System.Windows.Forms.CheckBox();
            this.checkBoxLabDoors = new System.Windows.Forms.CheckBox();
            this.checkBoxLabDebug = new System.Windows.Forms.CheckBox();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.checkBoxSwap = new System.Windows.Forms.CheckBox();
            this.buttonPaintingMoveUp = new System.Windows.Forms.Button();
            this.buttonPaintingMoveDown = new System.Windows.Forms.Button();
            this.listViewPaintings = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxKeys = new System.Windows.Forms.ComboBox();
            this.buttonTreasureUp = new System.Windows.Forms.Button();
            this.buttonTreasureDown = new System.Windows.Forms.Button();
            this.listViewTreasures = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.comboBoxLab = new System.Windows.Forms.ComboBox();
            this.lblRestart = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScreenBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScreenTop)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownProxyPort)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWatchdog)).BeginInit();
            this.tabPage6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(442, 375);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 29);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "Save";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(523, 375);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 29);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // listCategory
            // 
            this.listCategory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listCategory.FormattingEnabled = true;
            this.listCategory.ItemHeight = 20;
            this.listCategory.Items.AddRange(new object[] {
            "General",
            "Proxy",
            "Adb",
            "Lab"});
            this.listCategory.Location = new System.Drawing.Point(15, 12);
            this.listCategory.Name = "listCategory";
            this.listCategory.Size = new System.Drawing.Size(120, 362);
            this.listCategory.TabIndex = 25;
            this.listCategory.SelectedIndexChanged += new System.EventHandler(this.ListCategory_SelectedIndexChanged);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Controls.Add(this.tabPage4);
            this.tabControl.Location = new System.Drawing.Point(141, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(457, 363);
            this.tabControl.TabIndex = 26;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.numericUpDownScreenBottom);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.numericUpDownScreenTop);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.checkBoxDatalog);
            this.tabPage1.Controls.Add(this.checkBoxPrerelease);
            this.tabPage1.Controls.Add(this.checkBoxUpdates);
            this.tabPage1.Controls.Add(this.checkBoxDebug);
            this.tabPage1.Controls.Add(this.checkBoxTimestamps);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(449, 337);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            // 
            // numericUpDownScreenBottom
            // 
            this.numericUpDownScreenBottom.Location = new System.Drawing.Point(128, 140);
            this.numericUpDownScreenBottom.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownScreenBottom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numericUpDownScreenBottom.Name = "numericUpDownScreenBottom";
            this.numericUpDownScreenBottom.Size = new System.Drawing.Size(94, 20);
            this.numericUpDownScreenBottom.TabIndex = 8;
            this.numericUpDownScreenBottom.ValueChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(-3, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Screen bottom offset:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numericUpDownScreenTop
            // 
            this.numericUpDownScreenTop.Location = new System.Drawing.Point(128, 114);
            this.numericUpDownScreenTop.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownScreenTop.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numericUpDownScreenTop.Name = "numericUpDownScreenTop";
            this.numericUpDownScreenTop.Size = new System.Drawing.Size(94, 20);
            this.numericUpDownScreenTop.TabIndex = 6;
            this.numericUpDownScreenTop.ValueChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(-3, 114);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Screen top offset:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBoxDatalog
            // 
            this.checkBoxDatalog.AutoSize = true;
            this.checkBoxDatalog.Location = new System.Drawing.Point(0, 92);
            this.checkBoxDatalog.Name = "checkBoxDatalog";
            this.checkBoxDatalog.Size = new System.Drawing.Size(120, 17);
            this.checkBoxDatalog.TabIndex = 4;
            this.checkBoxDatalog.Text = "Enable data logging";
            this.checkBoxDatalog.UseVisualStyleBackColor = true;
            this.checkBoxDatalog.CheckedChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // checkBoxPrerelease
            // 
            this.checkBoxPrerelease.AutoSize = true;
            this.checkBoxPrerelease.Enabled = false;
            this.checkBoxPrerelease.Location = new System.Drawing.Point(18, 69);
            this.checkBoxPrerelease.Name = "checkBoxPrerelease";
            this.checkBoxPrerelease.Size = new System.Drawing.Size(153, 17);
            this.checkBoxPrerelease.TabIndex = 3;
            this.checkBoxPrerelease.Text = "Include pre-release verions";
            this.checkBoxPrerelease.UseVisualStyleBackColor = true;
            // 
            // checkBoxUpdates
            // 
            this.checkBoxUpdates.AutoSize = true;
            this.checkBoxUpdates.Location = new System.Drawing.Point(0, 46);
            this.checkBoxUpdates.Name = "checkBoxUpdates";
            this.checkBoxUpdates.Size = new System.Drawing.Size(187, 17);
            this.checkBoxUpdates.TabIndex = 2;
            this.checkBoxUpdates.Text = "Check for new versions on startup";
            this.checkBoxUpdates.UseVisualStyleBackColor = true;
            this.checkBoxUpdates.CheckedChanged += new System.EventHandler(this.checkBoxUpdates_CheckedChanged);
            // 
            // checkBoxDebug
            // 
            this.checkBoxDebug.AutoSize = true;
            this.checkBoxDebug.Location = new System.Drawing.Point(0, 23);
            this.checkBoxDebug.Name = "checkBoxDebug";
            this.checkBoxDebug.Size = new System.Drawing.Size(136, 17);
            this.checkBoxDebug.TabIndex = 1;
            this.checkBoxDebug.Text = "Show debug messages";
            this.checkBoxDebug.UseVisualStyleBackColor = true;
            this.checkBoxDebug.CheckedChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // checkBoxTimestamps
            // 
            this.checkBoxTimestamps.AutoSize = true;
            this.checkBoxTimestamps.Location = new System.Drawing.Point(0, 0);
            this.checkBoxTimestamps.Name = "checkBoxTimestamps";
            this.checkBoxTimestamps.Size = new System.Drawing.Size(108, 17);
            this.checkBoxTimestamps.TabIndex = 0;
            this.checkBoxTimestamps.Text = "Show timestamps";
            this.checkBoxTimestamps.UseVisualStyleBackColor = true;
            this.checkBoxTimestamps.CheckedChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.buttonProxyBlocklist);
            this.tabPage2.Controls.Add(this.textBoxProxyBlocklist);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.checkBoxProxySecure);
            this.tabPage2.Controls.Add(this.numericUpDownProxyPort);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(449, 337);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Proxy";
            // 
            // buttonProxyBlocklist
            // 
            this.buttonProxyBlocklist.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonProxyBlocklist.Image = global::FFRK_LabMem.Properties.Resources.folder;
            this.buttonProxyBlocklist.Location = new System.Drawing.Point(378, 25);
            this.buttonProxyBlocklist.Name = "buttonProxyBlocklist";
            this.buttonProxyBlocklist.Size = new System.Drawing.Size(25, 20);
            this.buttonProxyBlocklist.TabIndex = 50;
            this.buttonProxyBlocklist.UseVisualStyleBackColor = true;
            this.buttonProxyBlocklist.Visible = false;
            this.buttonProxyBlocklist.Click += new System.EventHandler(this.BtnBrowseIn_Click);
            // 
            // textBoxProxyBlocklist
            // 
            this.textBoxProxyBlocklist.Location = new System.Drawing.Point(131, 26);
            this.textBoxProxyBlocklist.Name = "textBoxProxyBlocklist";
            this.textBoxProxyBlocklist.Size = new System.Drawing.Size(241, 20);
            this.textBoxProxyBlocklist.TabIndex = 11;
            this.textBoxProxyBlocklist.TextChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(0, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 20);
            this.label4.TabIndex = 10;
            this.label4.Text = "Proxy blocklist:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBoxProxySecure
            // 
            this.checkBoxProxySecure.AutoSize = true;
            this.checkBoxProxySecure.Location = new System.Drawing.Point(3, 61);
            this.checkBoxProxySecure.Name = "checkBoxProxySecure";
            this.checkBoxProxySecure.Size = new System.Drawing.Size(140, 17);
            this.checkBoxProxySecure.TabIndex = 9;
            this.checkBoxProxySecure.Text = "Use secure proxy (https)";
            this.checkBoxProxySecure.UseVisualStyleBackColor = true;
            this.checkBoxProxySecure.CheckedChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // numericUpDownProxyPort
            // 
            this.numericUpDownProxyPort.Location = new System.Drawing.Point(131, 0);
            this.numericUpDownProxyPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownProxyPort.Minimum = new decimal(new int[] {
            1025,
            0,
            0,
            0});
            this.numericUpDownProxyPort.Name = "numericUpDownProxyPort";
            this.numericUpDownProxyPort.Size = new System.Drawing.Size(94, 20);
            this.numericUpDownProxyPort.TabIndex = 8;
            this.numericUpDownProxyPort.Value = new decimal(new int[] {
            8081,
            0,
            0,
            0});
            this.numericUpDownProxyPort.ValueChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Proxy port:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.textBoxAdbHost);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.textBoxAdbPath);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(449, 337);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Adb";
            // 
            // textBoxAdbHost
            // 
            this.textBoxAdbHost.Location = new System.Drawing.Point(131, 26);
            this.textBoxAdbHost.Name = "textBoxAdbHost";
            this.textBoxAdbHost.Size = new System.Drawing.Size(241, 20);
            this.textBoxAdbHost.TabIndex = 55;
            this.textBoxAdbHost.TextChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(0, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 20);
            this.label6.TabIndex = 54;
            this.label6.Text = "Adb host:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxAdbPath
            // 
            this.textBoxAdbPath.Location = new System.Drawing.Point(131, -1);
            this.textBoxAdbPath.Name = "textBoxAdbPath";
            this.textBoxAdbPath.Size = new System.Drawing.Size(241, 20);
            this.textBoxAdbPath.TabIndex = 52;
            this.textBoxAdbPath.TextChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 20);
            this.label5.TabIndex = 51;
            this.label5.Text = "Adb path:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.tabControl1);
            this.tabPage4.Controls.Add(this.comboBoxLab);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(449, 337);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Lab";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Location = new System.Drawing.Point(3, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(443, 305);
            this.tabControl1.TabIndex = 18;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.label8);
            this.tabPage5.Controls.Add(this.numericUpDownWatchdog);
            this.tabPage5.Controls.Add(this.label7);
            this.tabPage5.Controls.Add(this.checkBoxLabUsePotions);
            this.tabPage5.Controls.Add(this.checkBoxLabRestart);
            this.tabPage5.Controls.Add(this.checkBoxLabStopOnMasterPainting);
            this.tabPage5.Controls.Add(this.checkBoxLabRestartFailedBattle);
            this.tabPage5.Controls.Add(this.checkBoxLabAvoidPortal);
            this.tabPage5.Controls.Add(this.checkBoxLabAvoidExplore);
            this.tabPage5.Controls.Add(this.checkBoxLabDoors);
            this.tabPage5.Controls.Add(this.checkBoxLabDebug);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(435, 279);
            this.tabPage5.TabIndex = 0;
            this.tabPage5.Text = "Options";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(176, 197);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(147, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "minute(s) pass with no activity";
            // 
            // numericUpDownWatchdog
            // 
            this.numericUpDownWatchdog.Location = new System.Drawing.Point(115, 195);
            this.numericUpDownWatchdog.Name = "numericUpDownWatchdog";
            this.numericUpDownWatchdog.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownWatchdog.TabIndex = 12;
            this.numericUpDownWatchdog.ValueChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 197);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Restart FFRK when ";
            // 
            // checkBoxLabUsePotions
            // 
            this.checkBoxLabUsePotions.AutoSize = true;
            this.checkBoxLabUsePotions.Location = new System.Drawing.Point(18, 167);
            this.checkBoxLabUsePotions.Name = "checkBoxLabUsePotions";
            this.checkBoxLabUsePotions.Size = new System.Drawing.Size(121, 17);
            this.checkBoxLabUsePotions.TabIndex = 10;
            this.checkBoxLabUsePotions.Text = "Use stamina potions";
            this.checkBoxLabUsePotions.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabRestart
            // 
            this.checkBoxLabRestart.AutoSize = true;
            this.checkBoxLabRestart.Location = new System.Drawing.Point(6, 144);
            this.checkBoxLabRestart.Name = "checkBoxLabRestart";
            this.checkBoxLabRestart.Size = new System.Drawing.Size(158, 17);
            this.checkBoxLabRestart.TabIndex = 9;
            this.checkBoxLabRestart.Text = "Restart lab when completed";
            this.checkBoxLabRestart.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabStopOnMasterPainting
            // 
            this.checkBoxLabStopOnMasterPainting.AutoSize = true;
            this.checkBoxLabStopOnMasterPainting.Location = new System.Drawing.Point(6, 121);
            this.checkBoxLabStopOnMasterPainting.Name = "checkBoxLabStopOnMasterPainting";
            this.checkBoxLabStopOnMasterPainting.Size = new System.Drawing.Size(205, 17);
            this.checkBoxLabStopOnMasterPainting.TabIndex = 8;
            this.checkBoxLabStopOnMasterPainting.Text = "Stop when Master Painting is reached";
            this.checkBoxLabStopOnMasterPainting.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabRestartFailedBattle
            // 
            this.checkBoxLabRestartFailedBattle.AutoSize = true;
            this.checkBoxLabRestartFailedBattle.Location = new System.Drawing.Point(6, 98);
            this.checkBoxLabRestartFailedBattle.Name = "checkBoxLabRestartFailedBattle";
            this.checkBoxLabRestartFailedBattle.Size = new System.Drawing.Size(168, 17);
            this.checkBoxLabRestartFailedBattle.TabIndex = 7;
            this.checkBoxLabRestartFailedBattle.Text = "Restart battles when defeated";
            this.checkBoxLabRestartFailedBattle.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabAvoidPortal
            // 
            this.checkBoxLabAvoidPortal.AutoSize = true;
            this.checkBoxLabAvoidPortal.Location = new System.Drawing.Point(6, 75);
            this.checkBoxLabAvoidPortal.Name = "checkBoxLabAvoidPortal";
            this.checkBoxLabAvoidPortal.Size = new System.Drawing.Size(422, 17);
            this.checkBoxLabAvoidPortal.TabIndex = 6;
            this.checkBoxLabAvoidPortal.Text = "Avoid the portal if an exploration is visible behind it, or if there are unknown " +
    "paintings";
            this.checkBoxLabAvoidPortal.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabAvoidExplore
            // 
            this.checkBoxLabAvoidExplore.AutoSize = true;
            this.checkBoxLabAvoidExplore.Location = new System.Drawing.Point(6, 52);
            this.checkBoxLabAvoidExplore.Name = "checkBoxLabAvoidExplore";
            this.checkBoxLabAvoidExplore.Size = new System.Drawing.Size(243, 17);
            this.checkBoxLabAvoidExplore.TabIndex = 5;
            this.checkBoxLabAvoidExplore.Text = "Avoid exploration paintings if treasure is visible";
            this.checkBoxLabAvoidExplore.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabDoors
            // 
            this.checkBoxLabDoors.AutoSize = true;
            this.checkBoxLabDoors.Location = new System.Drawing.Point(6, 29);
            this.checkBoxLabDoors.Name = "checkBoxLabDoors";
            this.checkBoxLabDoors.Size = new System.Drawing.Size(115, 17);
            this.checkBoxLabDoors.TabIndex = 4;
            this.checkBoxLabDoors.Text = "Open sealed doors";
            this.checkBoxLabDoors.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabDebug
            // 
            this.checkBoxLabDebug.AutoSize = true;
            this.checkBoxLabDebug.Location = new System.Drawing.Point(6, 6);
            this.checkBoxLabDebug.Name = "checkBoxLabDebug";
            this.checkBoxLabDebug.Size = new System.Drawing.Size(136, 17);
            this.checkBoxLabDebug.TabIndex = 3;
            this.checkBoxLabDebug.Text = "Show debug messages";
            this.checkBoxLabDebug.UseVisualStyleBackColor = true;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.checkBoxSwap);
            this.tabPage6.Controls.Add(this.buttonPaintingMoveUp);
            this.tabPage6.Controls.Add(this.buttonPaintingMoveDown);
            this.tabPage6.Controls.Add(this.listViewPaintings);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(435, 279);
            this.tabPage6.TabIndex = 1;
            this.tabPage6.Text = "Painting Priority";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // checkBoxSwap
            // 
            this.checkBoxSwap.AutoSize = true;
            this.checkBoxSwap.Checked = true;
            this.checkBoxSwap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSwap.Location = new System.Drawing.Point(332, 249);
            this.checkBoxSwap.Name = "checkBoxSwap";
            this.checkBoxSwap.Size = new System.Drawing.Size(97, 17);
            this.checkBoxSwap.TabIndex = 3;
            this.checkBoxSwap.Text = "Swap positions";
            this.checkBoxSwap.UseVisualStyleBackColor = true;
            // 
            // buttonPaintingMoveUp
            // 
            this.buttonPaintingMoveUp.Location = new System.Drawing.Point(6, 243);
            this.buttonPaintingMoveUp.Name = "buttonPaintingMoveUp";
            this.buttonPaintingMoveUp.Size = new System.Drawing.Size(75, 23);
            this.buttonPaintingMoveUp.TabIndex = 2;
            this.buttonPaintingMoveUp.Text = "Move Up";
            this.buttonPaintingMoveUp.UseVisualStyleBackColor = true;
            this.buttonPaintingMoveUp.Click += new System.EventHandler(this.buttonPaintingUp_Click);
            // 
            // buttonPaintingMoveDown
            // 
            this.buttonPaintingMoveDown.Location = new System.Drawing.Point(87, 243);
            this.buttonPaintingMoveDown.Name = "buttonPaintingMoveDown";
            this.buttonPaintingMoveDown.Size = new System.Drawing.Size(75, 23);
            this.buttonPaintingMoveDown.TabIndex = 1;
            this.buttonPaintingMoveDown.Text = "Move Down";
            this.buttonPaintingMoveDown.UseVisualStyleBackColor = true;
            this.buttonPaintingMoveDown.Click += new System.EventHandler(this.buttonPaintingDown_Click);
            // 
            // listViewPaintings
            // 
            this.listViewPaintings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewPaintings.FullRowSelect = true;
            this.listViewPaintings.GridLines = true;
            this.listViewPaintings.HideSelection = false;
            this.listViewPaintings.Location = new System.Drawing.Point(6, 6);
            this.listViewPaintings.Name = "listViewPaintings";
            this.listViewPaintings.Size = new System.Drawing.Size(423, 232);
            this.listViewPaintings.TabIndex = 0;
            this.listViewPaintings.UseCompatibleStateImageBehavior = false;
            this.listViewPaintings.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Priority";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Painting Type";
            this.columnHeader2.Width = 323;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.label9);
            this.tabPage7.Controls.Add(this.comboBoxKeys);
            this.tabPage7.Controls.Add(this.buttonTreasureUp);
            this.tabPage7.Controls.Add(this.buttonTreasureDown);
            this.tabPage7.Controls.Add(this.listViewTreasures);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Size = new System.Drawing.Size(435, 279);
            this.tabPage7.TabIndex = 2;
            this.tabPage7.Text = "Treasure Priority";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(187, 248);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(115, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Max keys for selection:";
            // 
            // comboBoxKeys
            // 
            this.comboBoxKeys.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxKeys.FormattingEnabled = true;
            this.comboBoxKeys.Items.AddRange(new object[] {
            "0",
            "1",
            "3"});
            this.comboBoxKeys.Location = new System.Drawing.Point(308, 245);
            this.comboBoxKeys.Name = "comboBoxKeys";
            this.comboBoxKeys.Size = new System.Drawing.Size(121, 21);
            this.comboBoxKeys.TabIndex = 6;
            this.comboBoxKeys.SelectedIndexChanged += new System.EventHandler(this.comboBoxKeys_SelectedIndexChanged);
            // 
            // buttonTreasureUp
            // 
            this.buttonTreasureUp.Location = new System.Drawing.Point(6, 243);
            this.buttonTreasureUp.Name = "buttonTreasureUp";
            this.buttonTreasureUp.Size = new System.Drawing.Size(75, 23);
            this.buttonTreasureUp.TabIndex = 5;
            this.buttonTreasureUp.Text = "Move Up";
            this.buttonTreasureUp.UseVisualStyleBackColor = true;
            this.buttonTreasureUp.Click += new System.EventHandler(this.buttonTreasureUp_Click);
            // 
            // buttonTreasureDown
            // 
            this.buttonTreasureDown.Location = new System.Drawing.Point(87, 243);
            this.buttonTreasureDown.Name = "buttonTreasureDown";
            this.buttonTreasureDown.Size = new System.Drawing.Size(75, 23);
            this.buttonTreasureDown.TabIndex = 4;
            this.buttonTreasureDown.Text = "Move Down";
            this.buttonTreasureDown.UseVisualStyleBackColor = true;
            this.buttonTreasureDown.Click += new System.EventHandler(this.buttonTreasureDown_Click);
            // 
            // listViewTreasures
            // 
            this.listViewTreasures.CheckBoxes = true;
            this.listViewTreasures.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listViewTreasures.FullRowSelect = true;
            this.listViewTreasures.GridLines = true;
            this.listViewTreasures.HideSelection = false;
            this.listViewTreasures.Location = new System.Drawing.Point(6, 6);
            this.listViewTreasures.Name = "listViewTreasures";
            this.listViewTreasures.Size = new System.Drawing.Size(423, 232);
            this.listViewTreasures.TabIndex = 3;
            this.listViewTreasures.UseCompatibleStateImageBehavior = false;
            this.listViewTreasures.View = System.Windows.Forms.View.Details;
            this.listViewTreasures.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewTreasures_ItemChecked);
            this.listViewTreasures.SelectedIndexChanged += new System.EventHandler(this.listViewTreasures_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Priority";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Max Keys";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Treasure Type";
            this.columnHeader5.Width = 267;
            // 
            // comboBoxLab
            // 
            this.comboBoxLab.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLab.FormattingEnabled = true;
            this.comboBoxLab.Location = new System.Drawing.Point(0, 0);
            this.comboBoxLab.Name = "comboBoxLab";
            this.comboBoxLab.Size = new System.Drawing.Size(446, 21);
            this.comboBoxLab.TabIndex = 17;
            this.comboBoxLab.SelectedIndexChanged += new System.EventHandler(this.comboBoxLab_SelectedIndexChanged);
            // 
            // lblRestart
            // 
            this.lblRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRestart.BackColor = System.Drawing.SystemColors.Control;
            this.lblRestart.Image = global::FFRK_LabMem.Properties.Resources.error;
            this.lblRestart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRestart.Location = new System.Drawing.Point(12, 375);
            this.lblRestart.Name = "lblRestart";
            this.lblRestart.Size = new System.Drawing.Size(293, 29);
            this.lblRestart.TabIndex = 18;
            this.lblRestart.Text = "Restart of the app is required for changes to take effect.";
            this.lblRestart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblRestart.Visible = false;
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Choose blocklist file";
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyDocuments;
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(610, 416);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.listCategory);
            this.Controls.Add(this.lblRestart);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigForm";
            this.Text = "FFRK LabMem Configuration";
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScreenBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScreenTop)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownProxyPort)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWatchdog)).EndInit();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        internal System.Windows.Forms.Label lblRestart;
        internal System.Windows.Forms.ListBox listCategory;
        internal System.Windows.Forms.TabControl tabControl;
        internal System.Windows.Forms.TabPage tabPage1;
        internal System.Windows.Forms.TabPage tabPage2;
        internal System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.CheckBox checkBoxTimestamps;
        private System.Windows.Forms.CheckBox checkBoxDebug;
        private System.Windows.Forms.CheckBox checkBoxPrerelease;
        private System.Windows.Forms.CheckBox checkBoxUpdates;
        private System.Windows.Forms.CheckBox checkBoxDatalog;
        private System.Windows.Forms.NumericUpDown numericUpDownScreenBottom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownScreenTop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownProxyPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxProxySecure;
        internal System.Windows.Forms.Button buttonProxyBlocklist;
        private System.Windows.Forms.TextBox textBoxProxyBlocklist;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox textBoxAdbPath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxAdbHost;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxLab;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.CheckBox checkBoxLabAvoidExplore;
        private System.Windows.Forms.CheckBox checkBoxLabDoors;
        private System.Windows.Forms.CheckBox checkBoxLabDebug;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.CheckBox checkBoxLabAvoidPortal;
        private System.Windows.Forms.CheckBox checkBoxLabRestartFailedBattle;
        private System.Windows.Forms.CheckBox checkBoxLabStopOnMasterPainting;
        private System.Windows.Forms.CheckBox checkBoxLabRestart;
        private System.Windows.Forms.CheckBox checkBoxLabUsePotions;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericUpDownWatchdog;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonPaintingMoveUp;
        private System.Windows.Forms.Button buttonPaintingMoveDown;
        private System.Windows.Forms.ListView listViewPaintings;
        private System.Windows.Forms.CheckBox checkBoxSwap;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBoxKeys;
        private System.Windows.Forms.Button buttonTreasureUp;
        private System.Windows.Forms.Button buttonTreasureDown;
        private System.Windows.Forms.ListView listViewTreasures;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
    }
}