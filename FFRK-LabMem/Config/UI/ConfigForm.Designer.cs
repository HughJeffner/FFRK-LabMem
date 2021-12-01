﻿
namespace FFRK_LabMem.Config.UI
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("General", 0);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Proxy", 1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Adb", 2);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Lab", 3);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Timings", 4);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Schedule", 5);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Counters", 6);
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("Notifications", 7);
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.checkBoxLogging = new System.Windows.Forms.CheckBox();
            this.buttonDebug = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.numericUpDownWatchdogCrash = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDownWatchdogHang = new System.Windows.Forms.NumericUpDown();
            this.buttonCheckForUpdates = new System.Windows.Forms.Button();
            this.numericUpDownScreenBottom = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownScreenTop = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxDatalog = new System.Windows.Forms.CheckBox();
            this.checkBoxPrerelease = new System.Windows.Forms.CheckBox();
            this.checkBoxUpdates = new System.Windows.Forms.CheckBox();
            this.checkBoxTimestamps = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBoxProxyConnectionPool = new System.Windows.Forms.CheckBox();
            this.checkBoxProxyAutoConfig = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonProxyReset = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonProxyRegenCert = new System.Windows.Forms.Button();
            this.textBoxProxyBlocklist = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxProxySecure = new System.Windows.Forms.CheckBox();
            this.numericUpDownProxyPort = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonProxyBlocklist = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.checkBoxAdbClose = new System.Windows.Forms.CheckBox();
            this.comboBoxAdbHost = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxAdbPath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.buttonLabConfigurations = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.checkBoxLabAutoStart = new System.Windows.Forms.CheckBox();
            this.checkBoxLabUseTeleport = new System.Windows.Forms.CheckBox();
            this.checkBoxLabUsePotions = new System.Windows.Forms.CheckBox();
            this.checkBoxLabRestart = new System.Windows.Forms.CheckBox();
            this.checkBoxLabStopOnMasterPainting = new System.Windows.Forms.CheckBox();
            this.checkBoxLabRestartFailedBattle = new System.Windows.Forms.CheckBox();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.checkBoxLabScreenshotRadiant = new System.Windows.Forms.CheckBox();
            this.checkBoxSlot5 = new System.Windows.Forms.CheckBox();
            this.checkBoxSlot4 = new System.Windows.Forms.CheckBox();
            this.checkBoxSlot3 = new System.Windows.Forms.CheckBox();
            this.checkBoxSlot2 = new System.Windows.Forms.CheckBox();
            this.checkBoxSlot1 = new System.Windows.Forms.CheckBox();
            this.numericUpDownFatigue = new System.Windows.Forms.NumericUpDown();
            this.checkBoxLabUseLetheTears = new System.Windows.Forms.CheckBox();
            this.checkBoxLabAvoidPortal = new System.Windows.Forms.CheckBox();
            this.checkBoxLabAvoidExplore = new System.Windows.Forms.CheckBox();
            this.checkBoxLabDoors = new System.Windows.Forms.CheckBox();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.checkBoxSwap = new System.Windows.Forms.CheckBox();
            this.buttonPaintingMoveUp = new System.Windows.Forms.Button();
            this.buttonPaintingMoveDown = new System.Windows.Forms.Button();
            this.listViewPaintings = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxKeys = new System.Windows.Forms.ComboBox();
            this.buttonTreasureUp = new System.Windows.Forms.Button();
            this.buttonTreasureDown = new System.Windows.Forms.Button();
            this.listViewTreasures = new System.Windows.Forms.ListView();
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage10 = new System.Windows.Forms.TabPage();
            this.checkBoxLabBlockListOverride = new System.Windows.Forms.CheckBox();
            this.buttonRemoveBlocklist = new System.Windows.Forms.Button();
            this.buttonAddBlocklist = new System.Windows.Forms.Button();
            this.checkedListBoxBlocklist = new System.Windows.Forms.CheckedListBox();
            this.comboBoxLab = new System.Windows.Forms.ComboBox();
            this.tabPage13 = new System.Windows.Forms.TabPage();
            this.buttonTimingDefaults = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Timing = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Delay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Jitter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage11 = new System.Windows.Forms.TabPage();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.buttonScheduleAdd = new System.Windows.Forms.Button();
            this.buttonScheduleDelete = new System.Windows.Forms.Button();
            this.listViewSchedule = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage12 = new System.Windows.Forms.TabPage();
            this.buttonShowCounters = new System.Windows.Forms.Button();
            this.checkedListBoxDropCategories = new System.Windows.Forms.CheckedListBox();
            this.checkBoxCountersLogDropsTotal = new System.Windows.Forms.CheckBox();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.panelSMTP = new System.Windows.Forms.Panel();
            this.label17 = new System.Windows.Forms.Label();
            this.textBoxSMTPTo = new System.Windows.Forms.TextBox();
            this.textBoxSMTPFrom = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.checkBoxSMTPSSL = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.textBoxSMTPPassword = new System.Windows.Forms.TextBox();
            this.textBoxSMTPUser = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.numericUpDownSMTPPort = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxSMTPServer = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.checkBoxNotifcationEmail = new System.Windows.Forms.CheckBox();
            this.checkBoxNotificationFlashTaskbar = new System.Windows.Forms.CheckBox();
            this.checkBoxNotificationConsole = new System.Windows.Forms.CheckBox();
            this.textBoxNotificationSound = new System.Windows.Forms.TextBox();
            this.buttonNotificationSoundBrowse = new System.Windows.Forms.Button();
            this.checkBoxNotificationSound = new System.Windows.Forms.CheckBox();
            this.buttonNotificationTest = new System.Windows.Forms.Button();
            this.comboBoxNotificationEvents = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblRestart = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonApply = new System.Windows.Forms.Button();
            this.openFileDialogSound = new System.Windows.Forms.OpenFileDialog();
            this.label19 = new System.Windows.Forms.Label();
            this.numericUpDownCountersRarity = new System.Windows.Forms.NumericUpDown();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWatchdogCrash)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWatchdogHang)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScreenBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScreenTop)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownProxyPort)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFatigue)).BeginInit();
            this.tabPage6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.tabPage10.SuspendLayout();
            this.tabPage13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabPage11.SuspendLayout();
            this.tabPage12.SuspendLayout();
            this.tabPage9.SuspendLayout();
            this.panelSMTP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSMTPPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCountersRarity)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(411, 517);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(87, 33);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "Save";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(505, 517);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(87, 33);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Controls.Add(this.tabPage4);
            this.tabControl.Controls.Add(this.tabPage13);
            this.tabControl.Controls.Add(this.tabPage11);
            this.tabControl.Controls.Add(this.tabPage12);
            this.tabControl.Controls.Add(this.tabPage9);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(512, 494);
            this.tabControl.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.checkBoxLogging);
            this.tabPage1.Controls.Add(this.buttonDebug);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.numericUpDownWatchdogCrash);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.numericUpDownWatchdogHang);
            this.tabPage1.Controls.Add(this.buttonCheckForUpdates);
            this.tabPage1.Controls.Add(this.numericUpDownScreenBottom);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.numericUpDownScreenTop);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.checkBoxDatalog);
            this.tabPage1.Controls.Add(this.checkBoxPrerelease);
            this.tabPage1.Controls.Add(this.checkBoxUpdates);
            this.tabPage1.Controls.Add(this.checkBoxTimestamps);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(504, 466);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            // 
            // checkBoxLogging
            // 
            this.checkBoxLogging.AutoSize = true;
            this.checkBoxLogging.Location = new System.Drawing.Point(0, 78);
            this.checkBoxLogging.Name = "checkBoxLogging";
            this.checkBoxLogging.Size = new System.Drawing.Size(192, 19);
            this.checkBoxLogging.TabIndex = 23;
            this.checkBoxLogging.Text = "Enable console output logging";
            this.toolTip1.SetToolTip(this.checkBoxLogging, "Logs all program output to timestamped files in the Logs directory");
            this.checkBoxLogging.UseVisualStyleBackColor = true;
            // 
            // buttonDebug
            // 
            this.buttonDebug.BackColor = System.Drawing.SystemColors.Control;
            this.buttonDebug.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.buttonDebug.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDebug.Location = new System.Drawing.Point(149, 131);
            this.buttonDebug.Name = "buttonDebug";
            this.buttonDebug.Size = new System.Drawing.Size(233, 26);
            this.buttonDebug.TabIndex = 22;
            this.buttonDebug.Text = "Adb,Proxy";
            this.buttonDebug.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonDebug.UseVisualStyleBackColor = false;
            this.buttonDebug.Click += new System.EventHandler(this.ButtonDebug_Click);
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(-3, 132);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(143, 23);
            this.label12.TabIndex = 21;
            this.label12.Text = "Show debug info:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(-3, 250);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(143, 23);
            this.label11.TabIndex = 20;
            this.label11.Text = "Check every:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(212, 254);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(231, 15);
            this.label10.TabIndex = 19;
            this.label10.Text = "second(s) if FFRK is running and restart it";
            // 
            // numericUpDownWatchdogCrash
            // 
            this.numericUpDownWatchdogCrash.Location = new System.Drawing.Point(149, 250);
            this.numericUpDownWatchdogCrash.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numericUpDownWatchdogCrash.Name = "numericUpDownWatchdogCrash";
            this.numericUpDownWatchdogCrash.Size = new System.Drawing.Size(56, 21);
            this.numericUpDownWatchdogCrash.TabIndex = 18;
            this.toolTip1.SetToolTip(this.numericUpDownWatchdogCrash, "Checks if FFRK is running and starts it if not. \r\nSet to \'0\' to disable");
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(-3, 221);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(146, 23);
            this.label7.TabIndex = 17;
            this.label7.Text = "Restart FFRK when:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(212, 223);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(168, 15);
            this.label8.TabIndex = 16;
            this.label8.Text = "minute(s) pass with no activity";
            // 
            // numericUpDownWatchdogHang
            // 
            this.numericUpDownWatchdogHang.Location = new System.Drawing.Point(149, 221);
            this.numericUpDownWatchdogHang.Name = "numericUpDownWatchdogHang";
            this.numericUpDownWatchdogHang.Size = new System.Drawing.Size(56, 21);
            this.numericUpDownWatchdogHang.TabIndex = 14;
            this.toolTip1.SetToolTip(this.numericUpDownWatchdogHang, "If an action doesn\'t complete in this number of minutes, FFRK restart is performe" +
        "d.\r\nSet to \'0\' to disable");
            // 
            // buttonCheckForUpdates
            // 
            this.buttonCheckForUpdates.Location = new System.Drawing.Point(236, 22);
            this.buttonCheckForUpdates.Name = "buttonCheckForUpdates";
            this.buttonCheckForUpdates.Size = new System.Drawing.Size(87, 27);
            this.buttonCheckForUpdates.TabIndex = 9;
            this.buttonCheckForUpdates.Text = "Check Now";
            this.buttonCheckForUpdates.UseVisualStyleBackColor = true;
            this.buttonCheckForUpdates.Click += new System.EventHandler(this.ButtonCheckForUpdates_Click);
            // 
            // numericUpDownScreenBottom
            // 
            this.numericUpDownScreenBottom.Location = new System.Drawing.Point(149, 192);
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
            this.numericUpDownScreenBottom.Size = new System.Drawing.Size(110, 21);
            this.numericUpDownScreenBottom.TabIndex = 8;
            this.toolTip1.SetToolTip(this.numericUpDownScreenBottom, "Number of pixels of the gray bar at the bottom of FFRK\r\n0 for none, -1 to prompt " +
        "auto-detect");
            this.numericUpDownScreenBottom.ValueChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(-3, 192);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(146, 23);
            this.label2.TabIndex = 7;
            this.label2.Text = "Screen bottom offset:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numericUpDownScreenTop
            // 
            this.numericUpDownScreenTop.Location = new System.Drawing.Point(149, 163);
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
            this.numericUpDownScreenTop.Size = new System.Drawing.Size(110, 21);
            this.numericUpDownScreenTop.TabIndex = 6;
            this.toolTip1.SetToolTip(this.numericUpDownScreenTop, "Number of pixels of the gray bar at the top of FFRK\r\n0 for none, -1 to prompt aut" +
        "o-detect");
            this.numericUpDownScreenTop.ValueChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(-3, 163);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 23);
            this.label1.TabIndex = 5;
            this.label1.Text = "Screen top offset:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBoxDatalog
            // 
            this.checkBoxDatalog.AutoSize = true;
            this.checkBoxDatalog.Location = new System.Drawing.Point(0, 103);
            this.checkBoxDatalog.Name = "checkBoxDatalog";
            this.checkBoxDatalog.Size = new System.Drawing.Size(136, 19);
            this.checkBoxDatalog.TabIndex = 4;
            this.checkBoxDatalog.Text = "Enable data logging";
            this.toolTip1.SetToolTip(this.checkBoxDatalog, "Logs battle drops, found items, painting rates and other data in the DataLog dire" +
        "ctory");
            this.checkBoxDatalog.UseVisualStyleBackColor = true;
            this.checkBoxDatalog.CheckedChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // checkBoxPrerelease
            // 
            this.checkBoxPrerelease.AutoSize = true;
            this.checkBoxPrerelease.Enabled = false;
            this.checkBoxPrerelease.Location = new System.Drawing.Point(21, 53);
            this.checkBoxPrerelease.Name = "checkBoxPrerelease";
            this.checkBoxPrerelease.Size = new System.Drawing.Size(174, 19);
            this.checkBoxPrerelease.TabIndex = 3;
            this.checkBoxPrerelease.Text = "Include pre-release verions";
            this.checkBoxPrerelease.UseVisualStyleBackColor = true;
            // 
            // checkBoxUpdates
            // 
            this.checkBoxUpdates.AutoSize = true;
            this.checkBoxUpdates.Location = new System.Drawing.Point(0, 27);
            this.checkBoxUpdates.Name = "checkBoxUpdates";
            this.checkBoxUpdates.Size = new System.Drawing.Size(208, 19);
            this.checkBoxUpdates.TabIndex = 2;
            this.checkBoxUpdates.Text = "Check for new versions on startup";
            this.toolTip1.SetToolTip(this.checkBoxUpdates, "Checks the github repository for new releases");
            this.checkBoxUpdates.UseVisualStyleBackColor = true;
            this.checkBoxUpdates.CheckedChanged += new System.EventHandler(this.CheckBoxUpdates_CheckedChanged);
            // 
            // checkBoxTimestamps
            // 
            this.checkBoxTimestamps.AutoSize = true;
            this.checkBoxTimestamps.Location = new System.Drawing.Point(0, 0);
            this.checkBoxTimestamps.Name = "checkBoxTimestamps";
            this.checkBoxTimestamps.Size = new System.Drawing.Size(124, 19);
            this.checkBoxTimestamps.TabIndex = 0;
            this.checkBoxTimestamps.Text = "Show timestamps";
            this.checkBoxTimestamps.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.checkBoxProxyConnectionPool);
            this.tabPage2.Controls.Add(this.checkBoxProxyAutoConfig);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.textBoxProxyBlocklist);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.checkBoxProxySecure);
            this.tabPage2.Controls.Add(this.numericUpDownProxyPort);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.buttonProxyBlocklist);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(504, 468);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Proxy";
            // 
            // checkBoxProxyConnectionPool
            // 
            this.checkBoxProxyConnectionPool.AutoSize = true;
            this.checkBoxProxyConnectionPool.Location = new System.Drawing.Point(3, 122);
            this.checkBoxProxyConnectionPool.Name = "checkBoxProxyConnectionPool";
            this.checkBoxProxyConnectionPool.Size = new System.Drawing.Size(155, 19);
            this.checkBoxProxyConnectionPool.TabIndex = 8;
            this.checkBoxProxyConnectionPool.Text = "Use connection pooling";
            this.toolTip1.SetToolTip(this.checkBoxProxyConnectionPool, "May improve performance by reusing existing connections.  \r\nDisable if you experi" +
        "ence frequent connection issues");
            this.checkBoxProxyConnectionPool.UseVisualStyleBackColor = true;
            this.checkBoxProxyConnectionPool.CheckedChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // checkBoxProxyAutoConfig
            // 
            this.checkBoxProxyAutoConfig.AutoSize = true;
            this.checkBoxProxyAutoConfig.Location = new System.Drawing.Point(3, 96);
            this.checkBoxProxyAutoConfig.Name = "checkBoxProxyAutoConfig";
            this.checkBoxProxyAutoConfig.Size = new System.Drawing.Size(303, 19);
            this.checkBoxProxyAutoConfig.TabIndex = 7;
            this.checkBoxProxyAutoConfig.Text = "Auto-configure device system proxy settings via Adb";
            this.toolTip1.SetToolTip(this.checkBoxProxyAutoConfig, "Attempts to automatically configure proxy settings on the device. \r\nThis does not" +
        " show in the wifi settings UI!  Use the button below to revert.");
            this.checkBoxProxyAutoConfig.UseVisualStyleBackColor = true;
            this.checkBoxProxyAutoConfig.CheckedChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonProxyReset);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.buttonProxyRegenCert);
            this.groupBox1.Location = new System.Drawing.Point(0, 183);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(300, 202);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Actions";
            // 
            // buttonProxyReset
            // 
            this.buttonProxyReset.Location = new System.Drawing.Point(7, 55);
            this.buttonProxyReset.Name = "buttonProxyReset";
            this.buttonProxyReset.Size = new System.Drawing.Size(267, 27);
            this.buttonProxyReset.TabIndex = 2;
            this.buttonProxyReset.Text = "Reset System Proxy";
            this.buttonProxyReset.UseVisualStyleBackColor = true;
            this.buttonProxyReset.Click += new System.EventHandler(this.ButtonProxyReset_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(7, 22);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(267, 27);
            this.button1.TabIndex = 0;
            this.button1.Text = "Copy Proxy Bypass to Clipboard\r\n";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // buttonProxyRegenCert
            // 
            this.buttonProxyRegenCert.Location = new System.Drawing.Point(7, 88);
            this.buttonProxyRegenCert.Name = "buttonProxyRegenCert";
            this.buttonProxyRegenCert.Size = new System.Drawing.Size(267, 27);
            this.buttonProxyRegenCert.TabIndex = 1;
            this.buttonProxyRegenCert.Text = "Regenerate Certificate";
            this.buttonProxyRegenCert.UseVisualStyleBackColor = true;
            this.buttonProxyRegenCert.Click += new System.EventHandler(this.ButtonProxyRegenCert_Click);
            // 
            // textBoxProxyBlocklist
            // 
            this.textBoxProxyBlocklist.Location = new System.Drawing.Point(153, 30);
            this.textBoxProxyBlocklist.Name = "textBoxProxyBlocklist";
            this.textBoxProxyBlocklist.Size = new System.Drawing.Size(280, 21);
            this.textBoxProxyBlocklist.TabIndex = 3;
            this.toolTip1.SetToolTip(this.textBoxProxyBlocklist, "Specifies a text file to block connections.  \r\nThe file should contain a domain n" +
        "ame on each line of the file.");
            this.textBoxProxyBlocklist.TextChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(0, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(146, 23);
            this.label4.TabIndex = 2;
            this.label4.Text = "Proxy blocklist:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBoxProxySecure
            // 
            this.checkBoxProxySecure.AutoSize = true;
            this.checkBoxProxySecure.Location = new System.Drawing.Point(3, 70);
            this.checkBoxProxySecure.Name = "checkBoxProxySecure";
            this.checkBoxProxySecure.Size = new System.Drawing.Size(157, 19);
            this.checkBoxProxySecure.TabIndex = 5;
            this.checkBoxProxySecure.Text = "Use secure proxy (https)";
            this.toolTip1.SetToolTip(this.checkBoxProxySecure, "Use HTTPS interception, this is required for FFRK version 8.0.0+");
            this.checkBoxProxySecure.UseVisualStyleBackColor = true;
            this.checkBoxProxySecure.CheckedChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // numericUpDownProxyPort
            // 
            this.numericUpDownProxyPort.Location = new System.Drawing.Point(153, 0);
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
            this.numericUpDownProxyPort.Size = new System.Drawing.Size(110, 21);
            this.numericUpDownProxyPort.TabIndex = 1;
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
            this.label3.Size = new System.Drawing.Size(146, 23);
            this.label3.TabIndex = 0;
            this.label3.Text = "Proxy port:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonProxyBlocklist
            // 
            this.buttonProxyBlocklist.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonProxyBlocklist.Image = global::FFRK_LabMem.Properties.Resources.folder;
            this.buttonProxyBlocklist.Location = new System.Drawing.Point(441, 29);
            this.buttonProxyBlocklist.Name = "buttonProxyBlocklist";
            this.buttonProxyBlocklist.Size = new System.Drawing.Size(29, 23);
            this.buttonProxyBlocklist.TabIndex = 4;
            this.buttonProxyBlocklist.UseVisualStyleBackColor = true;
            this.buttonProxyBlocklist.Visible = false;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.checkBoxAdbClose);
            this.tabPage3.Controls.Add(this.comboBoxAdbHost);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.textBoxAdbPath);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(504, 468);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Adb";
            // 
            // checkBoxAdbClose
            // 
            this.checkBoxAdbClose.AutoSize = true;
            this.checkBoxAdbClose.Location = new System.Drawing.Point(3, 70);
            this.checkBoxAdbClose.Name = "checkBoxAdbClose";
            this.checkBoxAdbClose.Size = new System.Drawing.Size(120, 19);
            this.checkBoxAdbClose.TabIndex = 6;
            this.checkBoxAdbClose.Text = "Close Adb on exit";
            this.checkBoxAdbClose.UseVisualStyleBackColor = true;
            // 
            // comboBoxAdbHost
            // 
            this.comboBoxAdbHost.FormattingEnabled = true;
            this.comboBoxAdbHost.Items.AddRange(new object[] {
            "127.0.0.1:7555",
            "127.0.0.1:62001",
            "127.0.0.1:21503",
            "127.0.0.1:21513",
            "127.0.0.1:21523",
            "127.0.0.1:5555"});
            this.comboBoxAdbHost.Location = new System.Drawing.Point(153, 29);
            this.comboBoxAdbHost.Name = "comboBoxAdbHost";
            this.comboBoxAdbHost.Size = new System.Drawing.Size(280, 23);
            this.comboBoxAdbHost.TabIndex = 3;
            this.toolTip1.SetToolTip(this.comboBoxAdbHost, "Host and port for connecting to the device adb service. \r\nNot needed if connected" +
        " via USB");
            this.comboBoxAdbHost.TextChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(0, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(146, 23);
            this.label6.TabIndex = 2;
            this.label6.Text = "Adb host:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxAdbPath
            // 
            this.textBoxAdbPath.Location = new System.Drawing.Point(153, 0);
            this.textBoxAdbPath.Name = "textBoxAdbPath";
            this.textBoxAdbPath.Size = new System.Drawing.Size(280, 21);
            this.textBoxAdbPath.TabIndex = 1;
            this.toolTip1.SetToolTip(this.textBoxAdbPath, "Custom adb path if not using the bundled adb.exe");
            this.textBoxAdbPath.TextChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(0, 1);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(146, 23);
            this.label5.TabIndex = 0;
            this.label5.Text = "Adb path:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage4.Controls.Add(this.buttonLabConfigurations);
            this.tabPage4.Controls.Add(this.tabControl1);
            this.tabPage4.Controls.Add(this.comboBoxLab);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(504, 468);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Lab";
            // 
            // buttonLabConfigurations
            // 
            this.buttonLabConfigurations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLabConfigurations.Location = new System.Drawing.Point(471, -1);
            this.buttonLabConfigurations.Name = "buttonLabConfigurations";
            this.buttonLabConfigurations.Size = new System.Drawing.Size(33, 25);
            this.buttonLabConfigurations.TabIndex = 2;
            this.buttonLabConfigurations.Text = "...";
            this.buttonLabConfigurations.UseVisualStyleBackColor = true;
            this.buttonLabConfigurations.Click += new System.EventHandler(this.ButtonLabConfigurations_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage8);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Controls.Add(this.tabPage10);
            this.tabControl1.Location = new System.Drawing.Point(3, 35);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(500, 426);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.TabControl1_SelectedIndexChanged);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.checkBoxLabAutoStart);
            this.tabPage5.Controls.Add(this.checkBoxLabUseTeleport);
            this.tabPage5.Controls.Add(this.checkBoxLabUsePotions);
            this.tabPage5.Controls.Add(this.checkBoxLabRestart);
            this.tabPage5.Controls.Add(this.checkBoxLabStopOnMasterPainting);
            this.tabPage5.Controls.Add(this.checkBoxLabRestartFailedBattle);
            this.tabPage5.Location = new System.Drawing.Point(4, 24);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(492, 398);
            this.tabPage5.TabIndex = 0;
            this.tabPage5.Text = "Control";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabAutoStart
            // 
            this.checkBoxLabAutoStart.AutoSize = true;
            this.checkBoxLabAutoStart.Location = new System.Drawing.Point(7, 137);
            this.checkBoxLabAutoStart.Name = "checkBoxLabAutoStart";
            this.checkBoxLabAutoStart.Size = new System.Drawing.Size(158, 19);
            this.checkBoxLabAutoStart.TabIndex = 15;
            this.checkBoxLabAutoStart.Text = "Auto-start when enabled";
            this.toolTip1.SetToolTip(this.checkBoxLabAutoStart, "Attempts to automaticaly get things going");
            this.checkBoxLabAutoStart.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabUseTeleport
            // 
            this.checkBoxLabUseTeleport.AutoSize = true;
            this.checkBoxLabUseTeleport.Location = new System.Drawing.Point(7, 59);
            this.checkBoxLabUseTeleport.Name = "checkBoxLabUseTeleport";
            this.checkBoxLabUseTeleport.Size = new System.Drawing.Size(307, 19);
            this.checkBoxLabUseTeleport.TabIndex = 3;
            this.checkBoxLabUseTeleport.Text = "Use teleport stone when Master Painting is reached";
            this.toolTip1.SetToolTip(this.checkBoxLabUseTeleport, "Escapes the dungeon without fighting the master painting");
            this.checkBoxLabUseTeleport.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabUsePotions
            // 
            this.checkBoxLabUsePotions.AutoSize = true;
            this.checkBoxLabUsePotions.Location = new System.Drawing.Point(21, 111);
            this.checkBoxLabUsePotions.Name = "checkBoxLabUsePotions";
            this.checkBoxLabUsePotions.Size = new System.Drawing.Size(138, 19);
            this.checkBoxLabUsePotions.TabIndex = 5;
            this.checkBoxLabUsePotions.Text = "Use stamina potions";
            this.checkBoxLabUsePotions.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabRestart
            // 
            this.checkBoxLabRestart.AutoSize = true;
            this.checkBoxLabRestart.Location = new System.Drawing.Point(7, 85);
            this.checkBoxLabRestart.Name = "checkBoxLabRestart";
            this.checkBoxLabRestart.Size = new System.Drawing.Size(179, 19);
            this.checkBoxLabRestart.TabIndex = 4;
            this.checkBoxLabRestart.Text = "Restart lab when completed";
            this.toolTip1.SetToolTip(this.checkBoxLabRestart, "Restarts the lab run once completed, use for degenerate 24/7 farming");
            this.checkBoxLabRestart.UseVisualStyleBackColor = true;
            this.checkBoxLabRestart.CheckedChanged += new System.EventHandler(this.CheckBoxLabRestart_CheckedChanged);
            // 
            // checkBoxLabStopOnMasterPainting
            // 
            this.checkBoxLabStopOnMasterPainting.AutoSize = true;
            this.checkBoxLabStopOnMasterPainting.Location = new System.Drawing.Point(7, 33);
            this.checkBoxLabStopOnMasterPainting.Name = "checkBoxLabStopOnMasterPainting";
            this.checkBoxLabStopOnMasterPainting.Size = new System.Drawing.Size(233, 19);
            this.checkBoxLabStopOnMasterPainting.TabIndex = 2;
            this.checkBoxLabStopOnMasterPainting.Text = "Stop when Master Painting is reached";
            this.toolTip1.SetToolTip(this.checkBoxLabStopOnMasterPainting, "Disables the bot when the Master painting is reached");
            this.checkBoxLabStopOnMasterPainting.UseVisualStyleBackColor = true;
            this.checkBoxLabStopOnMasterPainting.CheckedChanged += new System.EventHandler(this.CheckBoxLabStopOnMasterPainting_CheckedChanged);
            // 
            // checkBoxLabRestartFailedBattle
            // 
            this.checkBoxLabRestartFailedBattle.AutoSize = true;
            this.checkBoxLabRestartFailedBattle.Location = new System.Drawing.Point(7, 7);
            this.checkBoxLabRestartFailedBattle.Name = "checkBoxLabRestartFailedBattle";
            this.checkBoxLabRestartFailedBattle.Size = new System.Drawing.Size(188, 19);
            this.checkBoxLabRestartFailedBattle.TabIndex = 1;
            this.checkBoxLabRestartFailedBattle.Text = "Restart battles when defeated";
            this.toolTip1.SetToolTip(this.checkBoxLabRestartFailedBattle, "Restarts the battle if you are defeated.  \r\nIf not set then plays a tone and wait" +
        "s for your input.");
            this.checkBoxLabRestartFailedBattle.UseVisualStyleBackColor = true;
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.checkBoxLabScreenshotRadiant);
            this.tabPage8.Controls.Add(this.checkBoxSlot5);
            this.tabPage8.Controls.Add(this.checkBoxSlot4);
            this.tabPage8.Controls.Add(this.checkBoxSlot3);
            this.tabPage8.Controls.Add(this.checkBoxSlot2);
            this.tabPage8.Controls.Add(this.checkBoxSlot1);
            this.tabPage8.Controls.Add(this.numericUpDownFatigue);
            this.tabPage8.Controls.Add(this.checkBoxLabUseLetheTears);
            this.tabPage8.Controls.Add(this.checkBoxLabAvoidPortal);
            this.tabPage8.Controls.Add(this.checkBoxLabAvoidExplore);
            this.tabPage8.Controls.Add(this.checkBoxLabDoors);
            this.tabPage8.Location = new System.Drawing.Point(4, 22);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Size = new System.Drawing.Size(492, 400);
            this.tabPage8.TabIndex = 3;
            this.tabPage8.Text = "Options";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabScreenshotRadiant
            // 
            this.checkBoxLabScreenshotRadiant.AutoSize = true;
            this.checkBoxLabScreenshotRadiant.Location = new System.Drawing.Point(7, 137);
            this.checkBoxLabScreenshotRadiant.Name = "checkBoxLabScreenshotRadiant";
            this.checkBoxLabScreenshotRadiant.Size = new System.Drawing.Size(293, 19);
            this.checkBoxLabScreenshotRadiant.TabIndex = 15;
            this.checkBoxLabScreenshotRadiant.Text = "Take screenshot when a radiant painting is found";
            this.toolTip1.SetToolTip(this.checkBoxLabScreenshotRadiant, "Saves a PNG file to the bot\'s current directory");
            this.checkBoxLabScreenshotRadiant.UseVisualStyleBackColor = true;
            // 
            // checkBoxSlot5
            // 
            this.checkBoxSlot5.AutoSize = true;
            this.checkBoxSlot5.Location = new System.Drawing.Point(198, 111);
            this.checkBoxSlot5.Name = "checkBoxSlot5";
            this.checkBoxSlot5.Size = new System.Drawing.Size(33, 19);
            this.checkBoxSlot5.TabIndex = 9;
            this.checkBoxSlot5.Text = "5";
            this.toolTip1.SetToolTip(this.checkBoxSlot5, "Party slot 5");
            this.checkBoxSlot5.UseVisualStyleBackColor = true;
            // 
            // checkBoxSlot4
            // 
            this.checkBoxSlot4.AutoSize = true;
            this.checkBoxSlot4.Location = new System.Drawing.Point(154, 111);
            this.checkBoxSlot4.Name = "checkBoxSlot4";
            this.checkBoxSlot4.Size = new System.Drawing.Size(33, 19);
            this.checkBoxSlot4.TabIndex = 8;
            this.checkBoxSlot4.Text = "4";
            this.toolTip1.SetToolTip(this.checkBoxSlot4, "Party slot 4");
            this.checkBoxSlot4.UseVisualStyleBackColor = true;
            // 
            // checkBoxSlot3
            // 
            this.checkBoxSlot3.AutoSize = true;
            this.checkBoxSlot3.Location = new System.Drawing.Point(110, 111);
            this.checkBoxSlot3.Name = "checkBoxSlot3";
            this.checkBoxSlot3.Size = new System.Drawing.Size(33, 19);
            this.checkBoxSlot3.TabIndex = 7;
            this.checkBoxSlot3.Text = "3";
            this.toolTip1.SetToolTip(this.checkBoxSlot3, "Party slot 3");
            this.checkBoxSlot3.UseVisualStyleBackColor = true;
            // 
            // checkBoxSlot2
            // 
            this.checkBoxSlot2.AutoSize = true;
            this.checkBoxSlot2.Location = new System.Drawing.Point(65, 111);
            this.checkBoxSlot2.Name = "checkBoxSlot2";
            this.checkBoxSlot2.Size = new System.Drawing.Size(33, 19);
            this.checkBoxSlot2.TabIndex = 6;
            this.checkBoxSlot2.Text = "2";
            this.toolTip1.SetToolTip(this.checkBoxSlot2, "Party slot 2");
            this.checkBoxSlot2.UseVisualStyleBackColor = true;
            // 
            // checkBoxSlot1
            // 
            this.checkBoxSlot1.AutoSize = true;
            this.checkBoxSlot1.Location = new System.Drawing.Point(21, 111);
            this.checkBoxSlot1.Name = "checkBoxSlot1";
            this.checkBoxSlot1.Size = new System.Drawing.Size(33, 19);
            this.checkBoxSlot1.TabIndex = 5;
            this.checkBoxSlot1.Text = "1";
            this.toolTip1.SetToolTip(this.checkBoxSlot1, "Party slot 1");
            this.checkBoxSlot1.UseVisualStyleBackColor = true;
            // 
            // numericUpDownFatigue
            // 
            this.numericUpDownFatigue.Location = new System.Drawing.Point(259, 85);
            this.numericUpDownFatigue.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numericUpDownFatigue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownFatigue.Name = "numericUpDownFatigue";
            this.numericUpDownFatigue.Size = new System.Drawing.Size(56, 21);
            this.numericUpDownFatigue.TabIndex = 4;
            this.toolTip1.SetToolTip(this.numericUpDownFatigue, "Fatigue that must be reached before Lethe Tears are used");
            this.numericUpDownFatigue.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // checkBoxLabUseLetheTears
            // 
            this.checkBoxLabUseLetheTears.AutoSize = true;
            this.checkBoxLabUseLetheTears.Location = new System.Drawing.Point(7, 85);
            this.checkBoxLabUseLetheTears.Name = "checkBoxLabUseLetheTears";
            this.checkBoxLabUseLetheTears.Size = new System.Drawing.Size(236, 19);
            this.checkBoxLabUseLetheTears.TabIndex = 3;
            this.checkBoxLabUseLetheTears.Text = "Use Lethe Tears when fatigue reaches";
            this.checkBoxLabUseLetheTears.UseVisualStyleBackColor = true;
            this.checkBoxLabUseLetheTears.CheckedChanged += new System.EventHandler(this.CheckBoxLabUseLetheTears_CheckedChanged);
            // 
            // checkBoxLabAvoidPortal
            // 
            this.checkBoxLabAvoidPortal.AutoSize = true;
            this.checkBoxLabAvoidPortal.Location = new System.Drawing.Point(7, 59);
            this.checkBoxLabAvoidPortal.Name = "checkBoxLabAvoidPortal";
            this.checkBoxLabAvoidPortal.Size = new System.Drawing.Size(482, 19);
            this.checkBoxLabAvoidPortal.TabIndex = 2;
            this.checkBoxLabAvoidPortal.Text = "Avoid the portal if an exploration is visible behind it, or if there are unknown " +
    "paintings";
            this.toolTip1.SetToolTip(this.checkBoxLabAvoidPortal, "Overrides the painting priority and avoids Portal paintings if a treasure is visi" +
        "ble in the background\r\nor there are more paintings to reveal.\r\n");
            this.checkBoxLabAvoidPortal.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabAvoidExplore
            // 
            this.checkBoxLabAvoidExplore.AutoSize = true;
            this.checkBoxLabAvoidExplore.Location = new System.Drawing.Point(7, 33);
            this.checkBoxLabAvoidExplore.Name = "checkBoxLabAvoidExplore";
            this.checkBoxLabAvoidExplore.Size = new System.Drawing.Size(278, 19);
            this.checkBoxLabAvoidExplore.TabIndex = 1;
            this.checkBoxLabAvoidExplore.Text = "Avoid exploration paintings if treasure is visible";
            this.toolTip1.SetToolTip(this.checkBoxLabAvoidExplore, "Overrides the painting priority and avoids exploration paintings if a treasure is" +
        " visible in the background\r\nto eliminate the chance of getting a Portal");
            this.checkBoxLabAvoidExplore.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabDoors
            // 
            this.checkBoxLabDoors.AutoSize = true;
            this.checkBoxLabDoors.Location = new System.Drawing.Point(7, 7);
            this.checkBoxLabDoors.Name = "checkBoxLabDoors";
            this.checkBoxLabDoors.Size = new System.Drawing.Size(130, 19);
            this.checkBoxLabDoors.TabIndex = 0;
            this.checkBoxLabDoors.Text = "Open sealed doors";
            this.checkBoxLabDoors.UseVisualStyleBackColor = true;
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
            this.tabPage6.Size = new System.Drawing.Size(492, 400);
            this.tabPage6.TabIndex = 1;
            this.tabPage6.Text = "Paintings";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // checkBoxSwap
            // 
            this.checkBoxSwap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxSwap.AutoSize = true;
            this.checkBoxSwap.Checked = true;
            this.checkBoxSwap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSwap.Location = new System.Drawing.Point(374, 365);
            this.checkBoxSwap.Name = "checkBoxSwap";
            this.checkBoxSwap.Size = new System.Drawing.Size(109, 19);
            this.checkBoxSwap.TabIndex = 3;
            this.checkBoxSwap.Text = "Swap positions";
            this.checkBoxSwap.UseVisualStyleBackColor = true;
            // 
            // buttonPaintingMoveUp
            // 
            this.buttonPaintingMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPaintingMoveUp.Location = new System.Drawing.Point(7, 357);
            this.buttonPaintingMoveUp.Name = "buttonPaintingMoveUp";
            this.buttonPaintingMoveUp.Size = new System.Drawing.Size(87, 27);
            this.buttonPaintingMoveUp.TabIndex = 1;
            this.buttonPaintingMoveUp.Text = "Move Up";
            this.buttonPaintingMoveUp.UseVisualStyleBackColor = true;
            this.buttonPaintingMoveUp.Click += new System.EventHandler(this.ButtonPaintingUp_Click);
            // 
            // buttonPaintingMoveDown
            // 
            this.buttonPaintingMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPaintingMoveDown.Location = new System.Drawing.Point(101, 357);
            this.buttonPaintingMoveDown.Name = "buttonPaintingMoveDown";
            this.buttonPaintingMoveDown.Size = new System.Drawing.Size(87, 27);
            this.buttonPaintingMoveDown.TabIndex = 2;
            this.buttonPaintingMoveDown.Text = "Move Down";
            this.buttonPaintingMoveDown.UseVisualStyleBackColor = true;
            this.buttonPaintingMoveDown.Click += new System.EventHandler(this.ButtonPaintingDown_Click);
            // 
            // listViewPaintings
            // 
            this.listViewPaintings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewPaintings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewPaintings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewPaintings.FullRowSelect = true;
            this.listViewPaintings.HideSelection = false;
            this.listViewPaintings.Location = new System.Drawing.Point(7, 7);
            this.listViewPaintings.Name = "listViewPaintings";
            this.listViewPaintings.Size = new System.Drawing.Size(476, 340);
            this.listViewPaintings.SmallImageList = this.imageList2;
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
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "treasure-16.png");
            this.imageList2.Images.SetKeyName(1, "painting-16.png");
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
            this.tabPage7.Size = new System.Drawing.Size(492, 400);
            this.tabPage7.TabIndex = 2;
            this.tabPage7.Text = "Treasures";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(201, 363);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(130, 15);
            this.label9.TabIndex = 3;
            this.label9.Text = "Max keys for selection:";
            // 
            // comboBoxKeys
            // 
            this.comboBoxKeys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxKeys.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxKeys.FormattingEnabled = true;
            this.comboBoxKeys.Items.AddRange(new object[] {
            "0",
            "1",
            "3"});
            this.comboBoxKeys.Location = new System.Drawing.Point(342, 359);
            this.comboBoxKeys.Name = "comboBoxKeys";
            this.comboBoxKeys.Size = new System.Drawing.Size(140, 23);
            this.comboBoxKeys.TabIndex = 4;
            this.comboBoxKeys.SelectedIndexChanged += new System.EventHandler(this.ComboBoxKeys_SelectedIndexChanged);
            // 
            // buttonTreasureUp
            // 
            this.buttonTreasureUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonTreasureUp.Location = new System.Drawing.Point(7, 357);
            this.buttonTreasureUp.Name = "buttonTreasureUp";
            this.buttonTreasureUp.Size = new System.Drawing.Size(87, 27);
            this.buttonTreasureUp.TabIndex = 1;
            this.buttonTreasureUp.Text = "Move Up";
            this.buttonTreasureUp.UseVisualStyleBackColor = true;
            this.buttonTreasureUp.Click += new System.EventHandler(this.ButtonTreasureUp_Click);
            // 
            // buttonTreasureDown
            // 
            this.buttonTreasureDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonTreasureDown.Location = new System.Drawing.Point(101, 357);
            this.buttonTreasureDown.Name = "buttonTreasureDown";
            this.buttonTreasureDown.Size = new System.Drawing.Size(87, 27);
            this.buttonTreasureDown.TabIndex = 2;
            this.buttonTreasureDown.Text = "Move Down";
            this.buttonTreasureDown.UseVisualStyleBackColor = true;
            this.buttonTreasureDown.Click += new System.EventHandler(this.ButtonTreasureDown_Click);
            // 
            // listViewTreasures
            // 
            this.listViewTreasures.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewTreasures.CheckBoxes = true;
            this.listViewTreasures.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader14,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listViewTreasures.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewTreasures.FullRowSelect = true;
            this.listViewTreasures.HideSelection = false;
            this.listViewTreasures.Location = new System.Drawing.Point(7, 7);
            this.listViewTreasures.Name = "listViewTreasures";
            this.listViewTreasures.Size = new System.Drawing.Size(476, 340);
            this.listViewTreasures.SmallImageList = this.imageList2;
            this.listViewTreasures.TabIndex = 0;
            this.listViewTreasures.UseCompatibleStateImageBehavior = false;
            this.listViewTreasures.View = System.Windows.Forms.View.Details;
            this.listViewTreasures.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ListViewTreasures_ItemChecked);
            this.listViewTreasures.SelectedIndexChanged += new System.EventHandler(this.ListViewTreasures_SelectedIndexChanged);
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "";
            this.columnHeader14.Width = 40;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Priority";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Max Keys";
            this.columnHeader4.Width = 75;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Treasure Type";
            this.columnHeader5.Width = 250;
            // 
            // tabPage10
            // 
            this.tabPage10.Controls.Add(this.checkBoxLabBlockListOverride);
            this.tabPage10.Controls.Add(this.buttonRemoveBlocklist);
            this.tabPage10.Controls.Add(this.buttonAddBlocklist);
            this.tabPage10.Controls.Add(this.checkedListBoxBlocklist);
            this.tabPage10.Location = new System.Drawing.Point(4, 22);
            this.tabPage10.Name = "tabPage10";
            this.tabPage10.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage10.Size = new System.Drawing.Size(492, 400);
            this.tabPage10.TabIndex = 5;
            this.tabPage10.Text = "Blocklist";
            this.tabPage10.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabBlockListOverride
            // 
            this.checkBoxLabBlockListOverride.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxLabBlockListOverride.AutoSize = true;
            this.checkBoxLabBlockListOverride.Location = new System.Drawing.Point(337, 365);
            this.checkBoxLabBlockListOverride.Name = "checkBoxLabBlockListOverride";
            this.checkBoxLabBlockListOverride.Size = new System.Drawing.Size(147, 19);
            this.checkBoxLabBlockListOverride.TabIndex = 4;
            this.checkBoxLabBlockListOverride.Text = "Override avoid options";
            this.toolTip1.SetToolTip(this.checkBoxLabBlockListOverride, "When enabled, the enemy blocklist takes priority over the avoidance options in th" +
        "e Options tab");
            this.checkBoxLabBlockListOverride.UseVisualStyleBackColor = true;
            // 
            // buttonRemoveBlocklist
            // 
            this.buttonRemoveBlocklist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRemoveBlocklist.Enabled = false;
            this.buttonRemoveBlocklist.Location = new System.Drawing.Point(101, 357);
            this.buttonRemoveBlocklist.Name = "buttonRemoveBlocklist";
            this.buttonRemoveBlocklist.Size = new System.Drawing.Size(87, 27);
            this.buttonRemoveBlocklist.TabIndex = 3;
            this.buttonRemoveBlocklist.Text = "Remove";
            this.buttonRemoveBlocklist.UseVisualStyleBackColor = true;
            this.buttonRemoveBlocklist.Click += new System.EventHandler(this.ButtonRemoveBlocklist_Click);
            // 
            // buttonAddBlocklist
            // 
            this.buttonAddBlocklist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddBlocklist.Location = new System.Drawing.Point(7, 357);
            this.buttonAddBlocklist.Name = "buttonAddBlocklist";
            this.buttonAddBlocklist.Size = new System.Drawing.Size(87, 27);
            this.buttonAddBlocklist.TabIndex = 2;
            this.buttonAddBlocklist.Text = "Add";
            this.buttonAddBlocklist.UseVisualStyleBackColor = true;
            this.buttonAddBlocklist.Click += new System.EventHandler(this.ButtonAddBlocklist_Click);
            // 
            // checkedListBoxBlocklist
            // 
            this.checkedListBoxBlocklist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBoxBlocklist.CheckOnClick = true;
            this.checkedListBoxBlocklist.FormattingEnabled = true;
            this.checkedListBoxBlocklist.IntegralHeight = false;
            this.checkedListBoxBlocklist.Location = new System.Drawing.Point(7, 7);
            this.checkedListBoxBlocklist.Name = "checkedListBoxBlocklist";
            this.checkedListBoxBlocklist.Size = new System.Drawing.Size(476, 340);
            this.checkedListBoxBlocklist.TabIndex = 0;
            this.checkedListBoxBlocklist.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkedListBoxBlocklist_MouseMove);
            // 
            // comboBoxLab
            // 
            this.comboBoxLab.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLab.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxLab.FormattingEnabled = true;
            this.comboBoxLab.Location = new System.Drawing.Point(3, 0);
            this.comboBoxLab.Name = "comboBoxLab";
            this.comboBoxLab.Size = new System.Drawing.Size(460, 24);
            this.comboBoxLab.TabIndex = 0;
            this.comboBoxLab.SelectedIndexChanged += new System.EventHandler(this.ComboBoxLab_SelectedIndexChanged);
            // 
            // tabPage13
            // 
            this.tabPage13.Controls.Add(this.buttonTimingDefaults);
            this.tabPage13.Controls.Add(this.dataGridView1);
            this.tabPage13.Location = new System.Drawing.Point(4, 22);
            this.tabPage13.Name = "tabPage13";
            this.tabPage13.Size = new System.Drawing.Size(504, 468);
            this.tabPage13.TabIndex = 6;
            this.tabPage13.Text = "Timings";
            this.tabPage13.UseVisualStyleBackColor = true;
            // 
            // buttonTimingDefaults
            // 
            this.buttonTimingDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonTimingDefaults.Location = new System.Drawing.Point(0, 426);
            this.buttonTimingDefaults.Name = "buttonTimingDefaults";
            this.buttonTimingDefaults.Size = new System.Drawing.Size(140, 27);
            this.buttonTimingDefaults.TabIndex = 3;
            this.buttonTimingDefaults.Text = "Reset Defaults";
            this.buttonTimingDefaults.UseVisualStyleBackColor = true;
            this.buttonTimingDefaults.Click += new System.EventHandler(this.ButtonTimingDefaults_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Timing,
            this.Delay,
            this.Jitter});
            this.dataGridView1.Location = new System.Drawing.Point(3, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(495, 418);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.DataGridView1_CellValidating);
            // 
            // Timing
            // 
            this.Timing.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Timing.HeaderText = "Timing";
            this.Timing.Name = "Timing";
            this.Timing.ReadOnly = true;
            // 
            // Delay
            // 
            this.Delay.HeaderText = "Delay (ms)";
            this.Delay.Name = "Delay";
            // 
            // Jitter
            // 
            this.Jitter.HeaderText = "Jitter (ms)";
            this.Jitter.Name = "Jitter";
            // 
            // tabPage11
            // 
            this.tabPage11.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage11.Controls.Add(this.linkLabel1);
            this.tabPage11.Controls.Add(this.buttonScheduleAdd);
            this.tabPage11.Controls.Add(this.buttonScheduleDelete);
            this.tabPage11.Controls.Add(this.listViewSchedule);
            this.tabPage11.Location = new System.Drawing.Point(4, 22);
            this.tabPage11.Name = "tabPage11";
            this.tabPage11.Size = new System.Drawing.Size(504, 468);
            this.tabPage11.TabIndex = 4;
            this.tabPage11.Text = "Schedule";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(306, 432);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(175, 15);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Click here for more information";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.linkLabel1, "https://github.com/HughJeffner/FFRK-LabMem/wiki/Scheduler");
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel1_LinkClicked);
            // 
            // buttonScheduleAdd
            // 
            this.buttonScheduleAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonScheduleAdd.Location = new System.Drawing.Point(0, 426);
            this.buttonScheduleAdd.Name = "buttonScheduleAdd";
            this.buttonScheduleAdd.Size = new System.Drawing.Size(87, 27);
            this.buttonScheduleAdd.TabIndex = 3;
            this.buttonScheduleAdd.Text = "Add";
            this.buttonScheduleAdd.UseVisualStyleBackColor = true;
            this.buttonScheduleAdd.Click += new System.EventHandler(this.buttonScheduleAdd_Click);
            // 
            // buttonScheduleDelete
            // 
            this.buttonScheduleDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonScheduleDelete.Location = new System.Drawing.Point(94, 426);
            this.buttonScheduleDelete.Name = "buttonScheduleDelete";
            this.buttonScheduleDelete.Size = new System.Drawing.Size(87, 27);
            this.buttonScheduleDelete.TabIndex = 4;
            this.buttonScheduleDelete.Text = "Delete";
            this.buttonScheduleDelete.UseVisualStyleBackColor = true;
            this.buttonScheduleDelete.Click += new System.EventHandler(this.buttonScheduleDelete_Click);
            // 
            // listViewSchedule
            // 
            this.listViewSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewSchedule.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.listViewSchedule.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewSchedule.FullRowSelect = true;
            this.listViewSchedule.HideSelection = false;
            this.listViewSchedule.Location = new System.Drawing.Point(3, 0);
            this.listViewSchedule.Name = "listViewSchedule";
            this.listViewSchedule.Size = new System.Drawing.Size(495, 418);
            this.listViewSchedule.TabIndex = 1;
            this.listViewSchedule.UseCompatibleStateImageBehavior = false;
            this.listViewSchedule.View = System.Windows.Forms.View.Details;
            this.listViewSchedule.DoubleClick += new System.EventHandler(this.listViewSchedule_DoubleClick);
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Name";
            this.columnHeader7.Width = 140;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Start";
            this.columnHeader8.Width = 150;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Stop";
            this.columnHeader9.Width = 150;
            // 
            // tabPage12
            // 
            this.tabPage12.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage12.Controls.Add(this.numericUpDownCountersRarity);
            this.tabPage12.Controls.Add(this.label19);
            this.tabPage12.Controls.Add(this.buttonShowCounters);
            this.tabPage12.Controls.Add(this.checkedListBoxDropCategories);
            this.tabPage12.Controls.Add(this.checkBoxCountersLogDropsTotal);
            this.tabPage12.Location = new System.Drawing.Point(4, 24);
            this.tabPage12.Name = "tabPage12";
            this.tabPage12.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage12.Size = new System.Drawing.Size(504, 466);
            this.tabPage12.TabIndex = 5;
            this.tabPage12.Text = "Counters";
            // 
            // buttonShowCounters
            // 
            this.buttonShowCounters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonShowCounters.Location = new System.Drawing.Point(6, 424);
            this.buttonShowCounters.Name = "buttonShowCounters";
            this.buttonShowCounters.Size = new System.Drawing.Size(108, 27);
            this.buttonShowCounters.TabIndex = 4;
            this.buttonShowCounters.Text = "Open Counters";
            this.buttonShowCounters.UseVisualStyleBackColor = true;
            this.buttonShowCounters.Click += new System.EventHandler(this.buttonShowCounters_Click);
            // 
            // checkedListBoxDropCategories
            // 
            this.checkedListBoxDropCategories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBoxDropCategories.CheckOnClick = true;
            this.checkedListBoxDropCategories.FormattingEnabled = true;
            this.checkedListBoxDropCategories.IntegralHeight = false;
            this.checkedListBoxDropCategories.Location = new System.Drawing.Point(3, 61);
            this.checkedListBoxDropCategories.Name = "checkedListBoxDropCategories";
            this.checkedListBoxDropCategories.Size = new System.Drawing.Size(495, 355);
            this.checkedListBoxDropCategories.TabIndex = 0;
            // 
            // checkBoxCountersLogDropsTotal
            // 
            this.checkBoxCountersLogDropsTotal.AutoSize = true;
            this.checkBoxCountersLogDropsTotal.Location = new System.Drawing.Point(6, 0);
            this.checkBoxCountersLogDropsTotal.Name = "checkBoxCountersLogDropsTotal";
            this.checkBoxCountersLogDropsTotal.Size = new System.Drawing.Size(340, 19);
            this.checkBoxCountersLogDropsTotal.TabIndex = 1;
            this.checkBoxCountersLogDropsTotal.Text = "Log drops to All-Time counters (may grow large over time)";
            this.checkBoxCountersLogDropsTotal.UseVisualStyleBackColor = true;
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.panelSMTP);
            this.tabPage9.Controls.Add(this.checkBoxNotifcationEmail);
            this.tabPage9.Controls.Add(this.checkBoxNotificationFlashTaskbar);
            this.tabPage9.Controls.Add(this.checkBoxNotificationConsole);
            this.tabPage9.Controls.Add(this.textBoxNotificationSound);
            this.tabPage9.Controls.Add(this.buttonNotificationSoundBrowse);
            this.tabPage9.Controls.Add(this.checkBoxNotificationSound);
            this.tabPage9.Controls.Add(this.buttonNotificationTest);
            this.tabPage9.Controls.Add(this.comboBoxNotificationEvents);
            this.tabPage9.Location = new System.Drawing.Point(4, 24);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage9.Size = new System.Drawing.Size(504, 466);
            this.tabPage9.TabIndex = 7;
            this.tabPage9.Text = "Notifications";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // panelSMTP
            // 
            this.panelSMTP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSMTP.Controls.Add(this.label17);
            this.panelSMTP.Controls.Add(this.textBoxSMTPTo);
            this.panelSMTP.Controls.Add(this.textBoxSMTPFrom);
            this.panelSMTP.Controls.Add(this.label18);
            this.panelSMTP.Controls.Add(this.checkBoxSMTPSSL);
            this.panelSMTP.Controls.Add(this.label16);
            this.panelSMTP.Controls.Add(this.textBoxSMTPPassword);
            this.panelSMTP.Controls.Add(this.textBoxSMTPUser);
            this.panelSMTP.Controls.Add(this.label15);
            this.panelSMTP.Controls.Add(this.numericUpDownSMTPPort);
            this.panelSMTP.Controls.Add(this.label14);
            this.panelSMTP.Controls.Add(this.textBoxSMTPServer);
            this.panelSMTP.Controls.Add(this.label13);
            this.panelSMTP.Enabled = false;
            this.panelSMTP.Location = new System.Drawing.Point(22, 174);
            this.panelSMTP.Name = "panelSMTP";
            this.panelSMTP.Size = new System.Drawing.Size(476, 145);
            this.panelSMTP.TabIndex = 11;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(4, 114);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(24, 15);
            this.label17.TabIndex = 12;
            this.label17.Text = "To:";
            // 
            // textBoxSMTPTo
            // 
            this.textBoxSMTPTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSMTPTo.Location = new System.Drawing.Point(112, 111);
            this.textBoxSMTPTo.Name = "textBoxSMTPTo";
            this.textBoxSMTPTo.Size = new System.Drawing.Size(202, 21);
            this.textBoxSMTPTo.TabIndex = 11;
            // 
            // textBoxSMTPFrom
            // 
            this.textBoxSMTPFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSMTPFrom.Location = new System.Drawing.Point(112, 84);
            this.textBoxSMTPFrom.Name = "textBoxSMTPFrom";
            this.textBoxSMTPFrom.Size = new System.Drawing.Size(202, 21);
            this.textBoxSMTPFrom.TabIndex = 10;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(4, 87);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(39, 15);
            this.label18.TabIndex = 9;
            this.label18.Text = "From:";
            // 
            // checkBoxSMTPSSL
            // 
            this.checkBoxSMTPSSL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxSMTPSSL.AutoSize = true;
            this.checkBoxSMTPSSL.Location = new System.Drawing.Point(333, 32);
            this.checkBoxSMTPSSL.Name = "checkBoxSMTPSSL";
            this.checkBoxSMTPSSL.Size = new System.Drawing.Size(137, 19);
            this.checkBoxSMTPSSL.TabIndex = 8;
            this.checkBoxSMTPSSL.Text = "Use SSL connection";
            this.checkBoxSMTPSSL.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(4, 60);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(64, 15);
            this.label16.TabIndex = 7;
            this.label16.Text = "Password:";
            // 
            // textBoxSMTPPassword
            // 
            this.textBoxSMTPPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSMTPPassword.Location = new System.Drawing.Point(112, 57);
            this.textBoxSMTPPassword.Name = "textBoxSMTPPassword";
            this.textBoxSMTPPassword.PasswordChar = '*';
            this.textBoxSMTPPassword.Size = new System.Drawing.Size(202, 21);
            this.textBoxSMTPPassword.TabIndex = 6;
            // 
            // textBoxSMTPUser
            // 
            this.textBoxSMTPUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSMTPUser.Location = new System.Drawing.Point(112, 30);
            this.textBoxSMTPUser.Name = "textBoxSMTPUser";
            this.textBoxSMTPUser.Size = new System.Drawing.Size(202, 21);
            this.textBoxSMTPUser.TabIndex = 5;
            this.textBoxSMTPUser.TextChanged += new System.EventHandler(this.TextBoxSMTPUser_TextChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(4, 33);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(73, 15);
            this.label15.TabIndex = 4;
            this.label15.Text = "User Name:";
            // 
            // numericUpDownSMTPPort
            // 
            this.numericUpDownSMTPPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownSMTPPort.Location = new System.Drawing.Point(389, 3);
            this.numericUpDownSMTPPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownSMTPPort.Name = "numericUpDownSMTPPort";
            this.numericUpDownSMTPPort.Size = new System.Drawing.Size(81, 21);
            this.numericUpDownSMTPPort.TabIndex = 3;
            this.numericUpDownSMTPPort.Value = new decimal(new int[] {
            587,
            0,
            0,
            0});
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(330, 4);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 15);
            this.label14.TabIndex = 2;
            this.label14.Text = "Port:";
            // 
            // textBoxSMTPServer
            // 
            this.textBoxSMTPServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSMTPServer.Location = new System.Drawing.Point(112, 3);
            this.textBoxSMTPServer.Name = "textBoxSMTPServer";
            this.textBoxSMTPServer.Size = new System.Drawing.Size(202, 21);
            this.textBoxSMTPServer.TabIndex = 1;
            this.textBoxSMTPServer.Text = "smtp.gmail.com";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(4, 4);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(82, 15);
            this.label13.TabIndex = 0;
            this.label13.Text = "SMTP Server:";
            // 
            // checkBoxNotifcationEmail
            // 
            this.checkBoxNotifcationEmail.AutoSize = true;
            this.checkBoxNotifcationEmail.Location = new System.Drawing.Point(3, 148);
            this.checkBoxNotifcationEmail.Name = "checkBoxNotifcationEmail";
            this.checkBoxNotifcationEmail.Size = new System.Drawing.Size(106, 19);
            this.checkBoxNotifcationEmail.TabIndex = 10;
            this.checkBoxNotifcationEmail.Text = "Send an email";
            this.checkBoxNotifcationEmail.UseVisualStyleBackColor = true;
            this.checkBoxNotifcationEmail.CheckedChanged += new System.EventHandler(this.CheckBoxNotifcationEmail_CheckedChanged);
            // 
            // checkBoxNotificationFlashTaskbar
            // 
            this.checkBoxNotificationFlashTaskbar.AutoSize = true;
            this.checkBoxNotificationFlashTaskbar.Location = new System.Drawing.Point(3, 123);
            this.checkBoxNotificationFlashTaskbar.Name = "checkBoxNotificationFlashTaskbar";
            this.checkBoxNotificationFlashTaskbar.Size = new System.Drawing.Size(119, 19);
            this.checkBoxNotificationFlashTaskbar.TabIndex = 9;
            this.checkBoxNotificationFlashTaskbar.Text = "Flash the taskbar";
            this.checkBoxNotificationFlashTaskbar.UseVisualStyleBackColor = true;
            // 
            // checkBoxNotificationConsole
            // 
            this.checkBoxNotificationConsole.AutoSize = true;
            this.checkBoxNotificationConsole.Location = new System.Drawing.Point(3, 98);
            this.checkBoxNotificationConsole.Name = "checkBoxNotificationConsole";
            this.checkBoxNotificationConsole.Size = new System.Drawing.Size(132, 19);
            this.checkBoxNotificationConsole.TabIndex = 8;
            this.checkBoxNotificationConsole.Text = "Play console beeps";
            this.checkBoxNotificationConsole.UseVisualStyleBackColor = true;
            // 
            // textBoxNotificationSound
            // 
            this.textBoxNotificationSound.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxNotificationSound.Enabled = false;
            this.textBoxNotificationSound.Location = new System.Drawing.Point(3, 65);
            this.textBoxNotificationSound.Name = "textBoxNotificationSound";
            this.textBoxNotificationSound.Size = new System.Drawing.Size(460, 21);
            this.textBoxNotificationSound.TabIndex = 6;
            this.toolTip1.SetToolTip(this.textBoxNotificationSound, "Specifies a text file to block connections.  \r\nThe file should contain a domain n" +
        "ame on each line of the file.");
            // 
            // buttonNotificationSoundBrowse
            // 
            this.buttonNotificationSoundBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNotificationSoundBrowse.Enabled = false;
            this.buttonNotificationSoundBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonNotificationSoundBrowse.Image = global::FFRK_LabMem.Properties.Resources.folder;
            this.buttonNotificationSoundBrowse.Location = new System.Drawing.Point(469, 65);
            this.buttonNotificationSoundBrowse.Name = "buttonNotificationSoundBrowse";
            this.buttonNotificationSoundBrowse.Size = new System.Drawing.Size(29, 21);
            this.buttonNotificationSoundBrowse.TabIndex = 7;
            this.buttonNotificationSoundBrowse.UseVisualStyleBackColor = true;
            this.buttonNotificationSoundBrowse.Click += new System.EventHandler(this.ButtonNotificationSoundBrowse_Click);
            // 
            // checkBoxNotificationSound
            // 
            this.checkBoxNotificationSound.AutoSize = true;
            this.checkBoxNotificationSound.Location = new System.Drawing.Point(3, 40);
            this.checkBoxNotificationSound.Name = "checkBoxNotificationSound";
            this.checkBoxNotificationSound.Size = new System.Drawing.Size(131, 19);
            this.checkBoxNotificationSound.TabIndex = 5;
            this.checkBoxNotificationSound.Text = "Play a sound (.wav)";
            this.checkBoxNotificationSound.UseVisualStyleBackColor = true;
            this.checkBoxNotificationSound.CheckedChanged += new System.EventHandler(this.CheckBoxNotificationSound_CheckedChanged);
            // 
            // buttonNotificationTest
            // 
            this.buttonNotificationTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNotificationTest.Location = new System.Drawing.Point(439, 0);
            this.buttonNotificationTest.Name = "buttonNotificationTest";
            this.buttonNotificationTest.Size = new System.Drawing.Size(63, 25);
            this.buttonNotificationTest.TabIndex = 4;
            this.buttonNotificationTest.Text = "Test";
            this.buttonNotificationTest.UseVisualStyleBackColor = true;
            this.buttonNotificationTest.Click += new System.EventHandler(this.ButtonNotificationTest_Click);
            // 
            // comboBoxNotificationEvents
            // 
            this.comboBoxNotificationEvents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxNotificationEvents.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNotificationEvents.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxNotificationEvents.FormattingEnabled = true;
            this.comboBoxNotificationEvents.Location = new System.Drawing.Point(3, 0);
            this.comboBoxNotificationEvents.Name = "comboBoxNotificationEvents";
            this.comboBoxNotificationEvents.Size = new System.Drawing.Size(426, 24);
            this.comboBoxNotificationEvents.TabIndex = 3;
            this.comboBoxNotificationEvents.SelectedIndexChanged += new System.EventHandler(this.ComboBoxNotificationEvents_SelectedIndexChanged);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.HideSelection = false;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8});
            this.listView1.LabelWrap = false;
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(0, 5);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Scrollable = false;
            this.listView1.ShowGroups = false;
            this.listView1.Size = new System.Drawing.Size(158, 483);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 19;
            this.listView1.TileSize = new System.Drawing.Size(120, 32);
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Category";
            this.columnHeader6.Width = 150;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "general-32.png");
            this.imageList1.Images.SetKeyName(1, "proxy-32.png");
            this.imageList1.Images.SetKeyName(2, "adb-32.png");
            this.imageList1.Images.SetKeyName(3, "lab-32.png");
            this.imageList1.Images.SetKeyName(4, "timings-32.png");
            this.imageList1.Images.SetKeyName(5, "schedule-32.png");
            this.imageList1.Images.SetKeyName(6, "counters-32.png");
            this.imageList1.Images.SetKeyName(7, "notification-32.png");
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(14, 14);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listView1);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 6);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl);
            this.splitContainer1.Size = new System.Drawing.Size(675, 494);
            this.splitContainer1.SplitterDistance = 158;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 20;
            // 
            // lblRestart
            // 
            this.lblRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRestart.BackColor = System.Drawing.SystemColors.Control;
            this.lblRestart.Image = global::FFRK_LabMem.Properties.Resources.error;
            this.lblRestart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRestart.Location = new System.Drawing.Point(17, 515);
            this.lblRestart.Name = "lblRestart";
            this.lblRestart.Size = new System.Drawing.Size(341, 33);
            this.lblRestart.TabIndex = 18;
            this.lblRestart.Text = "Restart of the app is required for changes to take effect.";
            this.lblRestart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblRestart.Visible = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 10);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(57, 6);
            // 
            // buttonApply
            // 
            this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonApply.Location = new System.Drawing.Point(598, 517);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(87, 33);
            this.buttonApply.TabIndex = 21;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // openFileDialogSound
            // 
            this.openFileDialogSound.DefaultExt = "wav";
            this.openFileDialogSound.Filter = "WAV files|*.wav";
            this.openFileDialogSound.Title = "Choose sound file";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(3, 31);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(279, 15);
            this.label19.TabIndex = 5;
            this.label19.Text = "Don\'t count drops for materials with rarity under: ★";
            // 
            // numericUpDownCountersRarity
            // 
            this.numericUpDownCountersRarity.Location = new System.Drawing.Point(288, 29);
            this.numericUpDownCountersRarity.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDownCountersRarity.Name = "numericUpDownCountersRarity";
            this.numericUpDownCountersRarity.Size = new System.Drawing.Size(52, 21);
            this.numericUpDownCountersRarity.TabIndex = 6;
            this.numericUpDownCountersRarity.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(703, 562);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.lblRestart);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FFRK LabMem Configuration";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConfigForm_FormClosed);
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWatchdogCrash)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWatchdogHang)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScreenBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScreenTop)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownProxyPort)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tabPage8.ResumeLayout(false);
            this.tabPage8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFatigue)).EndInit();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            this.tabPage10.ResumeLayout(false);
            this.tabPage10.PerformLayout();
            this.tabPage13.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabPage11.ResumeLayout(false);
            this.tabPage11.PerformLayout();
            this.tabPage12.ResumeLayout(false);
            this.tabPage12.PerformLayout();
            this.tabPage9.ResumeLayout(false);
            this.tabPage9.PerformLayout();
            this.panelSMTP.ResumeLayout(false);
            this.panelSMTP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSMTPPort)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCountersRarity)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        internal System.Windows.Forms.Label lblRestart;
        internal System.Windows.Forms.TabControl tabControl;
        internal System.Windows.Forms.TabPage tabPage1;
        internal System.Windows.Forms.TabPage tabPage2;
        internal System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.CheckBox checkBoxTimestamps;
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
        private System.Windows.Forms.TextBox textBoxAdbPath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxLab;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.CheckBox checkBoxLabRestartFailedBattle;
        private System.Windows.Forms.CheckBox checkBoxLabStopOnMasterPainting;
        private System.Windows.Forms.CheckBox checkBoxLabRestart;
        private System.Windows.Forms.CheckBox checkBoxLabUsePotions;
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
        private System.Windows.Forms.Button buttonProxyRegenCert;
        private System.Windows.Forms.ComboBox comboBoxAdbHost;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBoxLabUseTeleport;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.CheckBox checkBoxLabAvoidPortal;
        private System.Windows.Forms.CheckBox checkBoxLabAvoidExplore;
        private System.Windows.Forms.CheckBox checkBoxLabDoors;
        private System.Windows.Forms.CheckBox checkBoxSlot5;
        private System.Windows.Forms.CheckBox checkBoxSlot4;
        private System.Windows.Forms.CheckBox checkBoxSlot3;
        private System.Windows.Forms.CheckBox checkBoxSlot2;
        private System.Windows.Forms.CheckBox checkBoxSlot1;
        private System.Windows.Forms.NumericUpDown numericUpDownFatigue;
        private System.Windows.Forms.CheckBox checkBoxLabUseLetheTears;
        private System.Windows.Forms.Button buttonCheckForUpdates;
        private System.Windows.Forms.TabPage tabPage10;
        private System.Windows.Forms.Button buttonRemoveBlocklist;
        private System.Windows.Forms.Button buttonAddBlocklist;
        private System.Windows.Forms.CheckedListBox checkedListBoxBlocklist;
        private System.Windows.Forms.CheckBox checkBoxLabBlockListOverride;
        private System.Windows.Forms.Button buttonLabConfigurations;
        private System.Windows.Forms.CheckBox checkBoxLabAutoStart;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TabPage tabPage11;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button buttonScheduleAdd;
        private System.Windows.Forms.Button buttonScheduleDelete;
        private System.Windows.Forms.ListView listViewSchedule;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.CheckBox checkBoxProxyAutoConfig;
        private System.Windows.Forms.Button buttonProxyReset;
        private System.Windows.Forms.TabPage tabPage12;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericUpDownWatchdogHang;
        private System.Windows.Forms.TabPage tabPage13;
        private System.Windows.Forms.Button buttonTimingDefaults;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Timing;
        private System.Windows.Forms.DataGridViewTextBoxColumn Delay;
        private System.Windows.Forms.DataGridViewTextBoxColumn Jitter;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numericUpDownWatchdogCrash;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox checkBoxProxyConnectionPool;
        private System.Windows.Forms.CheckBox checkBoxAdbClose;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.CheckBox checkBoxLabScreenshotRadiant;
        private System.Windows.Forms.CheckedListBox checkedListBoxDropCategories;
        private System.Windows.Forms.CheckBox checkBoxCountersLogDropsTotal;
        private System.Windows.Forms.Button buttonShowCounters;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.Button buttonDebug;
        private System.Windows.Forms.TabPage tabPage9;
        private System.Windows.Forms.Button buttonNotificationTest;
        private System.Windows.Forms.ComboBox comboBoxNotificationEvents;
        private System.Windows.Forms.CheckBox checkBoxNotificationConsole;
        private System.Windows.Forms.TextBox textBoxNotificationSound;
        internal System.Windows.Forms.Button buttonNotificationSoundBrowse;
        private System.Windows.Forms.CheckBox checkBoxNotificationSound;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.OpenFileDialog openFileDialogSound;
        private System.Windows.Forms.CheckBox checkBoxLogging;
        private System.Windows.Forms.CheckBox checkBoxNotificationFlashTaskbar;
        private System.Windows.Forms.Panel panelSMTP;
        private System.Windows.Forms.NumericUpDown numericUpDownSMTPPort;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxSMTPServer;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox checkBoxNotifcationEmail;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox textBoxSMTPTo;
        private System.Windows.Forms.TextBox textBoxSMTPFrom;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.CheckBox checkBoxSMTPSSL;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBoxSMTPPassword;
        private System.Windows.Forms.TextBox textBoxSMTPUser;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown numericUpDownCountersRarity;
        private System.Windows.Forms.Label label19;
    }
}