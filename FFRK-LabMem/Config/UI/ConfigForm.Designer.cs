
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Counters", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Runtime", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Hero Equipment", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("General", 0);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Proxy", 1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Adb", 2);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Lab", 3);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Timings", 4);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Schedule", 5);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Counters", 6);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
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
            this.checkBoxDebug = new System.Windows.Forms.CheckBox();
            this.checkBoxTimestamps = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBoxProxyAutoConfig = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonProxyReset = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonProxyRegenCert = new System.Windows.Forms.Button();
            this.buttonProxyBlocklist = new System.Windows.Forms.Button();
            this.textBoxProxyBlocklist = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxProxySecure = new System.Windows.Forms.CheckBox();
            this.numericUpDownProxyPort = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.comboBoxAdbHost = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxAdbPath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.buttonLabConfigurations = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.checkBoxLabAutoStart = new System.Windows.Forms.CheckBox();
            this.checkBoxLabScreenshotRadiant = new System.Windows.Forms.CheckBox();
            this.checkBoxLabUseTeleport = new System.Windows.Forms.CheckBox();
            this.checkBoxLabUsePotions = new System.Windows.Forms.CheckBox();
            this.checkBoxLabRestart = new System.Windows.Forms.CheckBox();
            this.checkBoxLabStopOnMasterPainting = new System.Windows.Forms.CheckBox();
            this.checkBoxLabRestartFailedBattle = new System.Windows.Forms.CheckBox();
            this.checkBoxLabDebug = new System.Windows.Forms.CheckBox();
            this.tabPage8 = new System.Windows.Forms.TabPage();
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
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxKeys = new System.Windows.Forms.ComboBox();
            this.buttonTreasureUp = new System.Windows.Forms.Button();
            this.buttonTreasureDown = new System.Windows.Forms.Button();
            this.listViewTreasures = new System.Windows.Forms.ListView();
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
            this.buttonScheduleAdd = new System.Windows.Forms.Button();
            this.buttonScheduleDelete = new System.Windows.Forms.Button();
            this.listViewSchedule = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage12 = new System.Windows.Forms.TabPage();
            this.buttonCountersResetSession = new System.Windows.Forms.Button();
            this.buttonCountersResetLab = new System.Windows.Forms.Button();
            this.buttonCountersResetAll = new System.Windows.Forms.Button();
            this.listViewCounters = new System.Windows.Forms.ListView();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblRestart = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label10 = new System.Windows.Forms.Label();
            this.numericUpDownWatchdogCrash = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
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
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWatchdogCrash)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(442, 375);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 29);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "Save";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(523, 375);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 29);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Controls.Add(this.tabPage4);
            this.tabControl.Controls.Add(this.tabPage13);
            this.tabControl.Controls.Add(this.tabPage11);
            this.tabControl.Controls.Add(this.tabPage12);
            this.tabControl.Location = new System.Drawing.Point(141, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(457, 363);
            this.tabControl.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
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
            this.tabPage1.Controls.Add(this.checkBoxDebug);
            this.tabPage1.Controls.Add(this.checkBoxTimestamps);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(449, 337);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(-3, 166);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(125, 20);
            this.label7.TabIndex = 17;
            this.label7.Text = "Restart FFRK when:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(182, 170);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(147, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "minute(s) pass with no activity";
            // 
            // numericUpDownWatchdogHang
            // 
            this.numericUpDownWatchdogHang.Location = new System.Drawing.Point(128, 166);
            this.numericUpDownWatchdogHang.Name = "numericUpDownWatchdogHang";
            this.numericUpDownWatchdogHang.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownWatchdogHang.TabIndex = 14;
            this.toolTip1.SetToolTip(this.numericUpDownWatchdogHang, "If an action doesn\'t complete in this number of minutes, FFRK restart is performe" +
        "d. Set to \'0\' to disable");
            // 
            // buttonCheckForUpdates
            // 
            this.buttonCheckForUpdates.Location = new System.Drawing.Point(202, 42);
            this.buttonCheckForUpdates.Name = "buttonCheckForUpdates";
            this.buttonCheckForUpdates.Size = new System.Drawing.Size(75, 23);
            this.buttonCheckForUpdates.TabIndex = 9;
            this.buttonCheckForUpdates.Text = "Check Now";
            this.buttonCheckForUpdates.UseVisualStyleBackColor = true;
            this.buttonCheckForUpdates.Click += new System.EventHandler(this.ButtonCheckForUpdates_Click);
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
            this.toolTip1.SetToolTip(this.numericUpDownScreenBottom, "Number of pixels of the gray bar at the bottom of FFRK, 0 for none, -1 to prompt " +
        "auto-detect");
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
            this.toolTip1.SetToolTip(this.numericUpDownScreenTop, "Number of pixels of the gray bar at the top of FFRK, 0 for none, -1 to prompt aut" +
        "o-detect");
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
            this.toolTip1.SetToolTip(this.checkBoxDatalog, "Logs battle drops, found items, painting rates and other data in the DataLog dire" +
        "ctory");
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
            this.toolTip1.SetToolTip(this.checkBoxUpdates, "Checks the github repository for new releases");
            this.checkBoxUpdates.UseVisualStyleBackColor = true;
            this.checkBoxUpdates.CheckedChanged += new System.EventHandler(this.CheckBoxUpdates_CheckedChanged);
            // 
            // checkBoxDebug
            // 
            this.checkBoxDebug.AutoSize = true;
            this.checkBoxDebug.Location = new System.Drawing.Point(0, 23);
            this.checkBoxDebug.Name = "checkBoxDebug";
            this.checkBoxDebug.Size = new System.Drawing.Size(136, 17);
            this.checkBoxDebug.TabIndex = 1;
            this.checkBoxDebug.Text = "Show debug messages";
            this.toolTip1.SetToolTip(this.checkBoxDebug, "Shows general program debug information including URLs connected");
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
            this.tabPage2.Controls.Add(this.checkBoxProxyAutoConfig);
            this.tabPage2.Controls.Add(this.groupBox1);
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
            // checkBoxProxyAutoConfig
            // 
            this.checkBoxProxyAutoConfig.AutoSize = true;
            this.checkBoxProxyAutoConfig.Location = new System.Drawing.Point(3, 84);
            this.checkBoxProxyAutoConfig.Name = "checkBoxProxyAutoConfig";
            this.checkBoxProxyAutoConfig.Size = new System.Drawing.Size(274, 17);
            this.checkBoxProxyAutoConfig.TabIndex = 7;
            this.checkBoxProxyAutoConfig.Text = "Auto-configure device system proxy settings via ADB";
            this.toolTip1.SetToolTip(this.checkBoxProxyAutoConfig, "Attempts to automatically configure proxy settings on the device.  This does not " +
        "show in the wifi settings UI!  Use the button below to revert.");
            this.checkBoxProxyAutoConfig.UseVisualStyleBackColor = true;
            this.checkBoxProxyAutoConfig.CheckedChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonProxyReset);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.buttonProxyRegenCert);
            this.groupBox1.Location = new System.Drawing.Point(0, 131);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(257, 200);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Actions";
            // 
            // buttonProxyReset
            // 
            this.buttonProxyReset.Location = new System.Drawing.Point(6, 48);
            this.buttonProxyReset.Name = "buttonProxyReset";
            this.buttonProxyReset.Size = new System.Drawing.Size(229, 23);
            this.buttonProxyReset.TabIndex = 2;
            this.buttonProxyReset.Text = "Reset System Proxy";
            this.buttonProxyReset.UseVisualStyleBackColor = true;
            this.buttonProxyReset.Click += new System.EventHandler(this.ButtonProxyReset_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(229, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Copy Proxy Bypass to Clipboard\r\n";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // buttonProxyRegenCert
            // 
            this.buttonProxyRegenCert.Location = new System.Drawing.Point(6, 77);
            this.buttonProxyRegenCert.Name = "buttonProxyRegenCert";
            this.buttonProxyRegenCert.Size = new System.Drawing.Size(229, 23);
            this.buttonProxyRegenCert.TabIndex = 1;
            this.buttonProxyRegenCert.Text = "Regenerate Certificate";
            this.buttonProxyRegenCert.UseVisualStyleBackColor = true;
            this.buttonProxyRegenCert.Click += new System.EventHandler(this.ButtonProxyRegenCert_Click);
            // 
            // buttonProxyBlocklist
            // 
            this.buttonProxyBlocklist.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonProxyBlocklist.Image = global::FFRK_LabMem.Properties.Resources.folder;
            this.buttonProxyBlocklist.Location = new System.Drawing.Point(378, 25);
            this.buttonProxyBlocklist.Name = "buttonProxyBlocklist";
            this.buttonProxyBlocklist.Size = new System.Drawing.Size(25, 20);
            this.buttonProxyBlocklist.TabIndex = 4;
            this.buttonProxyBlocklist.UseVisualStyleBackColor = true;
            this.buttonProxyBlocklist.Visible = false;
            // 
            // textBoxProxyBlocklist
            // 
            this.textBoxProxyBlocklist.Location = new System.Drawing.Point(131, 26);
            this.textBoxProxyBlocklist.Name = "textBoxProxyBlocklist";
            this.textBoxProxyBlocklist.Size = new System.Drawing.Size(241, 20);
            this.textBoxProxyBlocklist.TabIndex = 3;
            this.toolTip1.SetToolTip(this.textBoxProxyBlocklist, "Specifies a text file to block connections.  The file should contain a domain nam" +
        "e on each line of the file.");
            this.textBoxProxyBlocklist.TextChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(0, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 20);
            this.label4.TabIndex = 2;
            this.label4.Text = "Proxy blocklist:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBoxProxySecure
            // 
            this.checkBoxProxySecure.AutoSize = true;
            this.checkBoxProxySecure.Location = new System.Drawing.Point(3, 61);
            this.checkBoxProxySecure.Name = "checkBoxProxySecure";
            this.checkBoxProxySecure.Size = new System.Drawing.Size(140, 17);
            this.checkBoxProxySecure.TabIndex = 5;
            this.checkBoxProxySecure.Text = "Use secure proxy (https)";
            this.toolTip1.SetToolTip(this.checkBoxProxySecure, "Use HTTPS interception, this is required for FFRK version 8.0.0+");
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
            this.label3.Size = new System.Drawing.Size(125, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "Proxy port:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.comboBoxAdbHost);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.textBoxAdbPath);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(449, 337);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Adb";
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
            this.comboBoxAdbHost.Location = new System.Drawing.Point(131, 25);
            this.comboBoxAdbHost.Name = "comboBoxAdbHost";
            this.comboBoxAdbHost.Size = new System.Drawing.Size(241, 21);
            this.comboBoxAdbHost.TabIndex = 3;
            this.toolTip1.SetToolTip(this.comboBoxAdbHost, "Host and port for connecting to the device adb service.  Not needed if connected " +
        "via USB");
            this.comboBoxAdbHost.TextChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(0, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 20);
            this.label6.TabIndex = 2;
            this.label6.Text = "Adb host:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxAdbPath
            // 
            this.textBoxAdbPath.Location = new System.Drawing.Point(131, 0);
            this.textBoxAdbPath.Name = "textBoxAdbPath";
            this.textBoxAdbPath.Size = new System.Drawing.Size(241, 20);
            this.textBoxAdbPath.TabIndex = 1;
            this.toolTip1.SetToolTip(this.textBoxAdbPath, "Custom adb path if not using the bundled adb.exe");
            this.textBoxAdbPath.TextChanged += new System.EventHandler(this.NeedsRestart_Changed);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(0, 1);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 20);
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
            this.tabPage4.Size = new System.Drawing.Size(449, 337);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Lab";
            // 
            // buttonLabConfigurations
            // 
            this.buttonLabConfigurations.Location = new System.Drawing.Point(424, -1);
            this.buttonLabConfigurations.Name = "buttonLabConfigurations";
            this.buttonLabConfigurations.Size = new System.Drawing.Size(22, 22);
            this.buttonLabConfigurations.TabIndex = 2;
            this.buttonLabConfigurations.Text = "...";
            this.buttonLabConfigurations.UseVisualStyleBackColor = true;
            this.buttonLabConfigurations.Click += new System.EventHandler(this.ButtonLabConfigurations_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage8);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Controls.Add(this.tabPage10);
            this.tabControl1.Location = new System.Drawing.Point(3, 30);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(443, 305);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.TabControl1_SelectedIndexChanged);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.checkBoxLabAutoStart);
            this.tabPage5.Controls.Add(this.checkBoxLabScreenshotRadiant);
            this.tabPage5.Controls.Add(this.checkBoxLabUseTeleport);
            this.tabPage5.Controls.Add(this.checkBoxLabUsePotions);
            this.tabPage5.Controls.Add(this.checkBoxLabRestart);
            this.tabPage5.Controls.Add(this.checkBoxLabStopOnMasterPainting);
            this.tabPage5.Controls.Add(this.checkBoxLabRestartFailedBattle);
            this.tabPage5.Controls.Add(this.checkBoxLabDebug);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(435, 279);
            this.tabPage5.TabIndex = 0;
            this.tabPage5.Text = "Control";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabAutoStart
            // 
            this.checkBoxLabAutoStart.AutoSize = true;
            this.checkBoxLabAutoStart.Location = new System.Drawing.Point(6, 168);
            this.checkBoxLabAutoStart.Name = "checkBoxLabAutoStart";
            this.checkBoxLabAutoStart.Size = new System.Drawing.Size(141, 17);
            this.checkBoxLabAutoStart.TabIndex = 15;
            this.checkBoxLabAutoStart.Text = "Auto-start when enabled";
            this.toolTip1.SetToolTip(this.checkBoxLabAutoStart, "Attempts to automaticaly get things going");
            this.checkBoxLabAutoStart.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabScreenshotRadiant
            // 
            this.checkBoxLabScreenshotRadiant.AutoSize = true;
            this.checkBoxLabScreenshotRadiant.Location = new System.Drawing.Point(6, 145);
            this.checkBoxLabScreenshotRadiant.Name = "checkBoxLabScreenshotRadiant";
            this.checkBoxLabScreenshotRadiant.Size = new System.Drawing.Size(259, 17);
            this.checkBoxLabScreenshotRadiant.TabIndex = 14;
            this.checkBoxLabScreenshotRadiant.Text = "Take screenshot when a radiant painting is found";
            this.toolTip1.SetToolTip(this.checkBoxLabScreenshotRadiant, "Saves a PNG file to the bot\'s current directory");
            this.checkBoxLabScreenshotRadiant.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabUseTeleport
            // 
            this.checkBoxLabUseTeleport.AutoSize = true;
            this.checkBoxLabUseTeleport.Location = new System.Drawing.Point(6, 75);
            this.checkBoxLabUseTeleport.Name = "checkBoxLabUseTeleport";
            this.checkBoxLabUseTeleport.Size = new System.Drawing.Size(269, 17);
            this.checkBoxLabUseTeleport.TabIndex = 3;
            this.checkBoxLabUseTeleport.Text = "Use teleport stone when Master Painting is reached";
            this.toolTip1.SetToolTip(this.checkBoxLabUseTeleport, "Escapes the dungeon without fighting the master painting");
            this.checkBoxLabUseTeleport.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabUsePotions
            // 
            this.checkBoxLabUsePotions.AutoSize = true;
            this.checkBoxLabUsePotions.Location = new System.Drawing.Point(18, 122);
            this.checkBoxLabUsePotions.Name = "checkBoxLabUsePotions";
            this.checkBoxLabUsePotions.Size = new System.Drawing.Size(121, 17);
            this.checkBoxLabUsePotions.TabIndex = 5;
            this.checkBoxLabUsePotions.Text = "Use stamina potions";
            this.checkBoxLabUsePotions.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabRestart
            // 
            this.checkBoxLabRestart.AutoSize = true;
            this.checkBoxLabRestart.Location = new System.Drawing.Point(6, 98);
            this.checkBoxLabRestart.Name = "checkBoxLabRestart";
            this.checkBoxLabRestart.Size = new System.Drawing.Size(158, 17);
            this.checkBoxLabRestart.TabIndex = 4;
            this.checkBoxLabRestart.Text = "Restart lab when completed";
            this.toolTip1.SetToolTip(this.checkBoxLabRestart, "Restarts the lab run once completed, use for degenerate 24/7 farming");
            this.checkBoxLabRestart.UseVisualStyleBackColor = true;
            this.checkBoxLabRestart.CheckedChanged += new System.EventHandler(this.CheckBoxLabRestart_CheckedChanged);
            // 
            // checkBoxLabStopOnMasterPainting
            // 
            this.checkBoxLabStopOnMasterPainting.AutoSize = true;
            this.checkBoxLabStopOnMasterPainting.Location = new System.Drawing.Point(6, 52);
            this.checkBoxLabStopOnMasterPainting.Name = "checkBoxLabStopOnMasterPainting";
            this.checkBoxLabStopOnMasterPainting.Size = new System.Drawing.Size(205, 17);
            this.checkBoxLabStopOnMasterPainting.TabIndex = 2;
            this.checkBoxLabStopOnMasterPainting.Text = "Stop when Master Painting is reached";
            this.toolTip1.SetToolTip(this.checkBoxLabStopOnMasterPainting, "Disables the bot when the Master painting is reached");
            this.checkBoxLabStopOnMasterPainting.UseVisualStyleBackColor = true;
            this.checkBoxLabStopOnMasterPainting.CheckedChanged += new System.EventHandler(this.CheckBoxLabStopOnMasterPainting_CheckedChanged);
            // 
            // checkBoxLabRestartFailedBattle
            // 
            this.checkBoxLabRestartFailedBattle.AutoSize = true;
            this.checkBoxLabRestartFailedBattle.Location = new System.Drawing.Point(6, 29);
            this.checkBoxLabRestartFailedBattle.Name = "checkBoxLabRestartFailedBattle";
            this.checkBoxLabRestartFailedBattle.Size = new System.Drawing.Size(168, 17);
            this.checkBoxLabRestartFailedBattle.TabIndex = 1;
            this.checkBoxLabRestartFailedBattle.Text = "Restart battles when defeated";
            this.toolTip1.SetToolTip(this.checkBoxLabRestartFailedBattle, "Restarts the battle if you are defeated.  If not set then plays a tone and waits " +
        "for your input.");
            this.checkBoxLabRestartFailedBattle.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabDebug
            // 
            this.checkBoxLabDebug.AutoSize = true;
            this.checkBoxLabDebug.Location = new System.Drawing.Point(6, 6);
            this.checkBoxLabDebug.Name = "checkBoxLabDebug";
            this.checkBoxLabDebug.Size = new System.Drawing.Size(136, 17);
            this.checkBoxLabDebug.TabIndex = 0;
            this.checkBoxLabDebug.Text = "Show debug messages";
            this.toolTip1.SetToolTip(this.checkBoxLabDebug, "Shows lab-specific debug information including button-finding info");
            this.checkBoxLabDebug.UseVisualStyleBackColor = true;
            // 
            // tabPage8
            // 
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
            this.tabPage8.Size = new System.Drawing.Size(435, 279);
            this.tabPage8.TabIndex = 3;
            this.tabPage8.Text = "Options";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // checkBoxSlot5
            // 
            this.checkBoxSlot5.AutoSize = true;
            this.checkBoxSlot5.Location = new System.Drawing.Point(170, 98);
            this.checkBoxSlot5.Name = "checkBoxSlot5";
            this.checkBoxSlot5.Size = new System.Drawing.Size(32, 17);
            this.checkBoxSlot5.TabIndex = 9;
            this.checkBoxSlot5.Text = "5";
            this.toolTip1.SetToolTip(this.checkBoxSlot5, "Party slot 5");
            this.checkBoxSlot5.UseVisualStyleBackColor = true;
            // 
            // checkBoxSlot4
            // 
            this.checkBoxSlot4.AutoSize = true;
            this.checkBoxSlot4.Location = new System.Drawing.Point(132, 98);
            this.checkBoxSlot4.Name = "checkBoxSlot4";
            this.checkBoxSlot4.Size = new System.Drawing.Size(32, 17);
            this.checkBoxSlot4.TabIndex = 8;
            this.checkBoxSlot4.Text = "4";
            this.toolTip1.SetToolTip(this.checkBoxSlot4, "Party slot 4");
            this.checkBoxSlot4.UseVisualStyleBackColor = true;
            // 
            // checkBoxSlot3
            // 
            this.checkBoxSlot3.AutoSize = true;
            this.checkBoxSlot3.Location = new System.Drawing.Point(94, 98);
            this.checkBoxSlot3.Name = "checkBoxSlot3";
            this.checkBoxSlot3.Size = new System.Drawing.Size(32, 17);
            this.checkBoxSlot3.TabIndex = 7;
            this.checkBoxSlot3.Text = "3";
            this.toolTip1.SetToolTip(this.checkBoxSlot3, "Party slot 3");
            this.checkBoxSlot3.UseVisualStyleBackColor = true;
            // 
            // checkBoxSlot2
            // 
            this.checkBoxSlot2.AutoSize = true;
            this.checkBoxSlot2.Location = new System.Drawing.Point(56, 98);
            this.checkBoxSlot2.Name = "checkBoxSlot2";
            this.checkBoxSlot2.Size = new System.Drawing.Size(32, 17);
            this.checkBoxSlot2.TabIndex = 6;
            this.checkBoxSlot2.Text = "2";
            this.toolTip1.SetToolTip(this.checkBoxSlot2, "Party slot 2");
            this.checkBoxSlot2.UseVisualStyleBackColor = true;
            // 
            // checkBoxSlot1
            // 
            this.checkBoxSlot1.AutoSize = true;
            this.checkBoxSlot1.Location = new System.Drawing.Point(18, 98);
            this.checkBoxSlot1.Name = "checkBoxSlot1";
            this.checkBoxSlot1.Size = new System.Drawing.Size(32, 17);
            this.checkBoxSlot1.TabIndex = 5;
            this.checkBoxSlot1.Text = "1";
            this.toolTip1.SetToolTip(this.checkBoxSlot1, "Party slot 1");
            this.checkBoxSlot1.UseVisualStyleBackColor = true;
            // 
            // numericUpDownFatigue
            // 
            this.numericUpDownFatigue.Location = new System.Drawing.Point(222, 74);
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
            this.numericUpDownFatigue.Size = new System.Drawing.Size(48, 20);
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
            this.checkBoxLabUseLetheTears.Location = new System.Drawing.Point(6, 75);
            this.checkBoxLabUseLetheTears.Name = "checkBoxLabUseLetheTears";
            this.checkBoxLabUseLetheTears.Size = new System.Drawing.Size(210, 17);
            this.checkBoxLabUseLetheTears.TabIndex = 3;
            this.checkBoxLabUseLetheTears.Text = "Use Lethe Tears when fatigue reaches";
            this.checkBoxLabUseLetheTears.UseVisualStyleBackColor = true;
            this.checkBoxLabUseLetheTears.CheckedChanged += new System.EventHandler(this.CheckBoxLabUseLetheTears_CheckedChanged);
            // 
            // checkBoxLabAvoidPortal
            // 
            this.checkBoxLabAvoidPortal.AutoSize = true;
            this.checkBoxLabAvoidPortal.Location = new System.Drawing.Point(6, 52);
            this.checkBoxLabAvoidPortal.Name = "checkBoxLabAvoidPortal";
            this.checkBoxLabAvoidPortal.Size = new System.Drawing.Size(422, 17);
            this.checkBoxLabAvoidPortal.TabIndex = 2;
            this.checkBoxLabAvoidPortal.Text = "Avoid the portal if an exploration is visible behind it, or if there are unknown " +
    "paintings";
            this.toolTip1.SetToolTip(this.checkBoxLabAvoidPortal, "Overrides the painting priority and avoids Portal paintings if a treasure is visi" +
        "ble in the background or there are more paintings to reveal.\r\n");
            this.checkBoxLabAvoidPortal.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabAvoidExplore
            // 
            this.checkBoxLabAvoidExplore.AutoSize = true;
            this.checkBoxLabAvoidExplore.Location = new System.Drawing.Point(6, 29);
            this.checkBoxLabAvoidExplore.Name = "checkBoxLabAvoidExplore";
            this.checkBoxLabAvoidExplore.Size = new System.Drawing.Size(243, 17);
            this.checkBoxLabAvoidExplore.TabIndex = 1;
            this.checkBoxLabAvoidExplore.Text = "Avoid exploration paintings if treasure is visible";
            this.toolTip1.SetToolTip(this.checkBoxLabAvoidExplore, "Overrides the painting priority and avoids exploration paintings if a treasure is" +
        " visible in the background to eliminate the chance of getting a Portal");
            this.checkBoxLabAvoidExplore.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabDoors
            // 
            this.checkBoxLabDoors.AutoSize = true;
            this.checkBoxLabDoors.Location = new System.Drawing.Point(6, 6);
            this.checkBoxLabDoors.Name = "checkBoxLabDoors";
            this.checkBoxLabDoors.Size = new System.Drawing.Size(115, 17);
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
            this.tabPage6.Size = new System.Drawing.Size(435, 279);
            this.tabPage6.TabIndex = 1;
            this.tabPage6.Text = "Paintings";
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
            this.buttonPaintingMoveUp.TabIndex = 1;
            this.buttonPaintingMoveUp.Text = "Move Up";
            this.buttonPaintingMoveUp.UseVisualStyleBackColor = true;
            this.buttonPaintingMoveUp.Click += new System.EventHandler(this.ButtonPaintingUp_Click);
            // 
            // buttonPaintingMoveDown
            // 
            this.buttonPaintingMoveDown.Location = new System.Drawing.Point(87, 243);
            this.buttonPaintingMoveDown.Name = "buttonPaintingMoveDown";
            this.buttonPaintingMoveDown.Size = new System.Drawing.Size(75, 23);
            this.buttonPaintingMoveDown.TabIndex = 2;
            this.buttonPaintingMoveDown.Text = "Move Down";
            this.buttonPaintingMoveDown.UseVisualStyleBackColor = true;
            this.buttonPaintingMoveDown.Click += new System.EventHandler(this.ButtonPaintingDown_Click);
            // 
            // listViewPaintings
            // 
            this.listViewPaintings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewPaintings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewPaintings.FullRowSelect = true;
            this.listViewPaintings.GridLines = true;
            this.listViewPaintings.HideSelection = false;
            this.listViewPaintings.Location = new System.Drawing.Point(6, 6);
            this.listViewPaintings.Name = "listViewPaintings";
            this.listViewPaintings.Size = new System.Drawing.Size(423, 229);
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
            this.tabPage7.Text = "Treasures";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(187, 248);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(115, 13);
            this.label9.TabIndex = 3;
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
            this.comboBoxKeys.TabIndex = 4;
            this.comboBoxKeys.SelectedIndexChanged += new System.EventHandler(this.ComboBoxKeys_SelectedIndexChanged);
            // 
            // buttonTreasureUp
            // 
            this.buttonTreasureUp.Location = new System.Drawing.Point(6, 243);
            this.buttonTreasureUp.Name = "buttonTreasureUp";
            this.buttonTreasureUp.Size = new System.Drawing.Size(75, 23);
            this.buttonTreasureUp.TabIndex = 1;
            this.buttonTreasureUp.Text = "Move Up";
            this.buttonTreasureUp.UseVisualStyleBackColor = true;
            this.buttonTreasureUp.Click += new System.EventHandler(this.ButtonTreasureUp_Click);
            // 
            // buttonTreasureDown
            // 
            this.buttonTreasureDown.Location = new System.Drawing.Point(87, 243);
            this.buttonTreasureDown.Name = "buttonTreasureDown";
            this.buttonTreasureDown.Size = new System.Drawing.Size(75, 23);
            this.buttonTreasureDown.TabIndex = 2;
            this.buttonTreasureDown.Text = "Move Down";
            this.buttonTreasureDown.UseVisualStyleBackColor = true;
            this.buttonTreasureDown.Click += new System.EventHandler(this.ButtonTreasureDown_Click);
            // 
            // listViewTreasures
            // 
            this.listViewTreasures.CheckBoxes = true;
            this.listViewTreasures.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listViewTreasures.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewTreasures.FullRowSelect = true;
            this.listViewTreasures.GridLines = true;
            this.listViewTreasures.HideSelection = false;
            this.listViewTreasures.Location = new System.Drawing.Point(6, 6);
            this.listViewTreasures.Name = "listViewTreasures";
            this.listViewTreasures.Size = new System.Drawing.Size(423, 229);
            this.listViewTreasures.TabIndex = 0;
            this.listViewTreasures.UseCompatibleStateImageBehavior = false;
            this.listViewTreasures.View = System.Windows.Forms.View.Details;
            this.listViewTreasures.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ListViewTreasures_ItemChecked);
            this.listViewTreasures.SelectedIndexChanged += new System.EventHandler(this.ListViewTreasures_SelectedIndexChanged);
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
            this.columnHeader5.Width = 267;
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
            this.tabPage10.Size = new System.Drawing.Size(435, 279);
            this.tabPage10.TabIndex = 5;
            this.tabPage10.Text = "Blocklist";
            this.tabPage10.UseVisualStyleBackColor = true;
            // 
            // checkBoxLabBlockListOverride
            // 
            this.checkBoxLabBlockListOverride.AutoSize = true;
            this.checkBoxLabBlockListOverride.Location = new System.Drawing.Point(297, 249);
            this.checkBoxLabBlockListOverride.Name = "checkBoxLabBlockListOverride";
            this.checkBoxLabBlockListOverride.Size = new System.Drawing.Size(132, 17);
            this.checkBoxLabBlockListOverride.TabIndex = 4;
            this.checkBoxLabBlockListOverride.Text = "Override avoid options";
            this.toolTip1.SetToolTip(this.checkBoxLabBlockListOverride, "When enabled, the enemy blocklist takes priority over the avoidance options in th" +
        "e Options tab");
            this.checkBoxLabBlockListOverride.UseVisualStyleBackColor = true;
            // 
            // buttonRemoveBlocklist
            // 
            this.buttonRemoveBlocklist.Enabled = false;
            this.buttonRemoveBlocklist.Location = new System.Drawing.Point(87, 243);
            this.buttonRemoveBlocklist.Name = "buttonRemoveBlocklist";
            this.buttonRemoveBlocklist.Size = new System.Drawing.Size(75, 23);
            this.buttonRemoveBlocklist.TabIndex = 3;
            this.buttonRemoveBlocklist.Text = "Remove";
            this.buttonRemoveBlocklist.UseVisualStyleBackColor = true;
            this.buttonRemoveBlocklist.Click += new System.EventHandler(this.ButtonRemoveBlocklist_Click);
            // 
            // buttonAddBlocklist
            // 
            this.buttonAddBlocklist.Location = new System.Drawing.Point(6, 243);
            this.buttonAddBlocklist.Name = "buttonAddBlocklist";
            this.buttonAddBlocklist.Size = new System.Drawing.Size(75, 23);
            this.buttonAddBlocklist.TabIndex = 2;
            this.buttonAddBlocklist.Text = "Add";
            this.buttonAddBlocklist.UseVisualStyleBackColor = true;
            this.buttonAddBlocklist.Click += new System.EventHandler(this.ButtonAddBlocklist_Click);
            // 
            // checkedListBoxBlocklist
            // 
            this.checkedListBoxBlocklist.CheckOnClick = true;
            this.checkedListBoxBlocklist.FormattingEnabled = true;
            this.checkedListBoxBlocklist.Location = new System.Drawing.Point(6, 6);
            this.checkedListBoxBlocklist.Name = "checkedListBoxBlocklist";
            this.checkedListBoxBlocklist.Size = new System.Drawing.Size(423, 229);
            this.checkedListBoxBlocklist.TabIndex = 0;
            this.checkedListBoxBlocklist.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkedListBoxBlocklist_MouseMove);
            // 
            // comboBoxLab
            // 
            this.comboBoxLab.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLab.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxLab.FormattingEnabled = true;
            this.comboBoxLab.Location = new System.Drawing.Point(3, 0);
            this.comboBoxLab.Name = "comboBoxLab";
            this.comboBoxLab.Size = new System.Drawing.Size(416, 21);
            this.comboBoxLab.TabIndex = 0;
            this.comboBoxLab.SelectedIndexChanged += new System.EventHandler(this.ComboBoxLab_SelectedIndexChanged);
            // 
            // tabPage13
            // 
            this.tabPage13.Controls.Add(this.buttonTimingDefaults);
            this.tabPage13.Controls.Add(this.dataGridView1);
            this.tabPage13.Location = new System.Drawing.Point(4, 22);
            this.tabPage13.Name = "tabPage13";
            this.tabPage13.Size = new System.Drawing.Size(449, 337);
            this.tabPage13.TabIndex = 6;
            this.tabPage13.Text = "Timings";
            this.tabPage13.UseVisualStyleBackColor = true;
            // 
            // buttonTimingDefaults
            // 
            this.buttonTimingDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonTimingDefaults.Location = new System.Drawing.Point(3, 311);
            this.buttonTimingDefaults.Name = "buttonTimingDefaults";
            this.buttonTimingDefaults.Size = new System.Drawing.Size(120, 23);
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
            this.dataGridView1.Size = new System.Drawing.Size(443, 305);
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
            this.tabPage11.Controls.Add(this.buttonScheduleAdd);
            this.tabPage11.Controls.Add(this.buttonScheduleDelete);
            this.tabPage11.Controls.Add(this.listViewSchedule);
            this.tabPage11.Location = new System.Drawing.Point(4, 22);
            this.tabPage11.Name = "tabPage11";
            this.tabPage11.Size = new System.Drawing.Size(449, 337);
            this.tabPage11.TabIndex = 4;
            this.tabPage11.Text = "Schedule";
            // 
            // buttonScheduleAdd
            // 
            this.buttonScheduleAdd.Location = new System.Drawing.Point(3, 311);
            this.buttonScheduleAdd.Name = "buttonScheduleAdd";
            this.buttonScheduleAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonScheduleAdd.TabIndex = 3;
            this.buttonScheduleAdd.Text = "Add";
            this.buttonScheduleAdd.UseVisualStyleBackColor = true;
            this.buttonScheduleAdd.Click += new System.EventHandler(this.buttonScheduleAdd_Click);
            // 
            // buttonScheduleDelete
            // 
            this.buttonScheduleDelete.Location = new System.Drawing.Point(84, 311);
            this.buttonScheduleDelete.Name = "buttonScheduleDelete";
            this.buttonScheduleDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonScheduleDelete.TabIndex = 4;
            this.buttonScheduleDelete.Text = "Delete";
            this.buttonScheduleDelete.UseVisualStyleBackColor = true;
            this.buttonScheduleDelete.Click += new System.EventHandler(this.buttonScheduleDelete_Click);
            // 
            // listViewSchedule
            // 
            this.listViewSchedule.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.listViewSchedule.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewSchedule.FullRowSelect = true;
            this.listViewSchedule.HideSelection = false;
            this.listViewSchedule.Location = new System.Drawing.Point(3, 0);
            this.listViewSchedule.Name = "listViewSchedule";
            this.listViewSchedule.Size = new System.Drawing.Size(443, 305);
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
            this.columnHeader8.Width = 130;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Stop";
            this.columnHeader9.Width = 130;
            // 
            // tabPage12
            // 
            this.tabPage12.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage12.Controls.Add(this.buttonCountersResetSession);
            this.tabPage12.Controls.Add(this.buttonCountersResetLab);
            this.tabPage12.Controls.Add(this.buttonCountersResetAll);
            this.tabPage12.Controls.Add(this.listViewCounters);
            this.tabPage12.Location = new System.Drawing.Point(4, 22);
            this.tabPage12.Name = "tabPage12";
            this.tabPage12.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage12.Size = new System.Drawing.Size(449, 337);
            this.tabPage12.TabIndex = 5;
            this.tabPage12.Text = "Counters";
            // 
            // buttonCountersResetSession
            // 
            this.buttonCountersResetSession.Location = new System.Drawing.Point(99, 311);
            this.buttonCountersResetSession.Name = "buttonCountersResetSession";
            this.buttonCountersResetSession.Size = new System.Drawing.Size(90, 23);
            this.buttonCountersResetSession.TabIndex = 7;
            this.buttonCountersResetSession.Tag = "Session";
            this.buttonCountersResetSession.Text = "Reset Session";
            this.buttonCountersResetSession.UseVisualStyleBackColor = true;
            this.buttonCountersResetSession.Click += new System.EventHandler(this.ButtonCountersReset_Click);
            // 
            // buttonCountersResetLab
            // 
            this.buttonCountersResetLab.Location = new System.Drawing.Point(3, 311);
            this.buttonCountersResetLab.Name = "buttonCountersResetLab";
            this.buttonCountersResetLab.Size = new System.Drawing.Size(90, 23);
            this.buttonCountersResetLab.TabIndex = 5;
            this.buttonCountersResetLab.Tag = "CurrentLab";
            this.buttonCountersResetLab.Text = "Reset Lab";
            this.buttonCountersResetLab.UseVisualStyleBackColor = true;
            this.buttonCountersResetLab.Click += new System.EventHandler(this.ButtonCountersReset_Click);
            // 
            // buttonCountersResetAll
            // 
            this.buttonCountersResetAll.Location = new System.Drawing.Point(195, 311);
            this.buttonCountersResetAll.Name = "buttonCountersResetAll";
            this.buttonCountersResetAll.Size = new System.Drawing.Size(90, 23);
            this.buttonCountersResetAll.TabIndex = 6;
            this.buttonCountersResetAll.Tag = "All";
            this.buttonCountersResetAll.Text = "Reset All";
            this.buttonCountersResetAll.UseVisualStyleBackColor = true;
            this.buttonCountersResetAll.Click += new System.EventHandler(this.ButtonCountersReset_Click);
            // 
            // listViewCounters
            // 
            this.listViewCounters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader12,
            this.columnHeader11,
            this.columnHeader13});
            this.listViewCounters.FullRowSelect = true;
            listViewGroup1.Header = "Counters";
            listViewGroup1.Name = "Counters";
            listViewGroup2.Header = "Runtime";
            listViewGroup2.Name = "Runtime";
            listViewGroup3.Header = "Hero Equipment";
            listViewGroup3.Name = "HE";
            this.listViewCounters.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
            this.listViewCounters.HideSelection = false;
            this.listViewCounters.Location = new System.Drawing.Point(3, 0);
            this.listViewCounters.Name = "listViewCounters";
            this.listViewCounters.Size = new System.Drawing.Size(443, 305);
            this.listViewCounters.TabIndex = 0;
            this.listViewCounters.UseCompatibleStateImageBehavior = false;
            this.listViewCounters.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "";
            this.columnHeader10.Width = 140;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Session";
            this.columnHeader12.Width = 90;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Current Lab";
            this.columnHeader11.Width = 90;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "All-Time";
            this.columnHeader13.Width = 90;
            // 
            // lblRestart
            // 
            this.lblRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRestart.BackColor = System.Drawing.SystemColors.Control;
            this.lblRestart.Image = global::FFRK_LabMem.Properties.Resources.error;
            this.lblRestart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRestart.Location = new System.Drawing.Point(15, 375);
            this.lblRestart.Name = "lblRestart";
            this.lblRestart.Size = new System.Drawing.Size(301, 29);
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
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6});
            this.listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            listViewItem7});
            this.listView1.LabelWrap = false;
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(13, 12);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Scrollable = false;
            this.listView1.ShowGroups = false;
            this.listView1.Size = new System.Drawing.Size(120, 360);
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
            this.columnHeader6.Width = 115;
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
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(182, 196);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(200, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "second(s) if FFRK is running and restart it";
            // 
            // numericUpDownWatchdogCrash
            // 
            this.numericUpDownWatchdogCrash.Location = new System.Drawing.Point(128, 192);
            this.numericUpDownWatchdogCrash.Name = "numericUpDownWatchdogCrash";
            this.numericUpDownWatchdogCrash.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownWatchdogCrash.TabIndex = 18;
            this.toolTip1.SetToolTip(this.numericUpDownWatchdogCrash, "Checks if FFRK is running and starts it if not.  Set to \'0\' to disable");
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(-3, 192);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(123, 20);
            this.label11.TabIndex = 20;
            this.label11.Text = "Check every";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(610, 416);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.lblRestart);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FFRK LabMem Configuration";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConfigForm_FormClosed);
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
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
            this.tabPage12.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWatchdogCrash)).EndInit();
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
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxLab;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.CheckBox checkBoxLabDebug;
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
        private System.Windows.Forms.CheckBox checkBoxLabScreenshotRadiant;
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
        private System.Windows.Forms.ListView listViewCounters;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.Button buttonCountersResetLab;
        private System.Windows.Forms.Button buttonCountersResetAll;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericUpDownWatchdogHang;
        private System.Windows.Forms.TabPage tabPage13;
        private System.Windows.Forms.Button buttonTimingDefaults;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Timing;
        private System.Windows.Forms.DataGridViewTextBoxColumn Delay;
        private System.Windows.Forms.DataGridViewTextBoxColumn Jitter;
        private System.Windows.Forms.Button buttonCountersResetSession;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numericUpDownWatchdogCrash;
        private System.Windows.Forms.Label label11;
    }
}