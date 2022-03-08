
namespace FFRK_LabMem.Data.UI
{
    partial class CountersForm
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
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Counters", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Runtime", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("Hero Equipment", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup8 = new System.Windows.Forms.ListViewGroup("Drops", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CountersForm));
            this.buttonCountersResetSession = new System.Windows.Forms.Button();
            this.buttonCountersResetLab = new System.Windows.Forms.Button();
            this.buttonCountersResetAll = new System.Windows.Forms.Button();
            this.listViewCounters = new Controls.ListViewExtended();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonExport = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.countersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runtimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.heroEquipmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dropsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.qEDropsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBoxLab = new System.Windows.Forms.ComboBox();
            this.comboBoxQE = new System.Windows.Forms.ComboBox();
            this.buttonCountersResetGroup = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCountersResetSession
            // 
            this.buttonCountersResetSession.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCountersResetSession.Location = new System.Drawing.Point(12, 470);
            this.buttonCountersResetSession.Name = "buttonCountersResetSession";
            this.buttonCountersResetSession.Size = new System.Drawing.Size(105, 27);
            this.buttonCountersResetSession.TabIndex = 11;
            this.buttonCountersResetSession.Tag = "Session";
            this.buttonCountersResetSession.Text = "Reset Session";
            this.buttonCountersResetSession.UseVisualStyleBackColor = true;
            this.buttonCountersResetSession.Click += new System.EventHandler(this.ButtonCountersReset_Click);
            // 
            // buttonCountersResetLab
            // 
            this.buttonCountersResetLab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCountersResetLab.Location = new System.Drawing.Point(123, 470);
            this.buttonCountersResetLab.Name = "buttonCountersResetLab";
            this.buttonCountersResetLab.Size = new System.Drawing.Size(105, 27);
            this.buttonCountersResetLab.TabIndex = 9;
            this.buttonCountersResetLab.Tag = "CurrentLab";
            this.buttonCountersResetLab.Text = "Reset Lab";
            this.buttonCountersResetLab.UseVisualStyleBackColor = true;
            this.buttonCountersResetLab.Click += new System.EventHandler(this.ButtonCountersReset_Click);
            // 
            // buttonCountersResetAll
            // 
            this.buttonCountersResetAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCountersResetAll.Location = new System.Drawing.Point(345, 470);
            this.buttonCountersResetAll.Name = "buttonCountersResetAll";
            this.buttonCountersResetAll.Size = new System.Drawing.Size(105, 27);
            this.buttonCountersResetAll.TabIndex = 10;
            this.buttonCountersResetAll.Tag = "All";
            this.buttonCountersResetAll.Text = "Reset All";
            this.buttonCountersResetAll.UseVisualStyleBackColor = true;
            this.buttonCountersResetAll.Click += new System.EventHandler(this.ButtonCountersReset_Click);
            // 
            // listViewCounters
            // 
            this.listViewCounters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewCounters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader12,
            this.columnHeader11,
            this.columnHeader1,
            this.columnHeader13});
            this.listViewCounters.FullRowSelect = true;
            listViewGroup5.Header = "Counters";
            listViewGroup5.Name = "Counters";
            listViewGroup6.Header = "Runtime";
            listViewGroup6.Name = "Runtime";
            listViewGroup7.Header = "Hero Equipment";
            listViewGroup7.Name = "HE";
            listViewGroup8.Header = "Drops";
            listViewGroup8.Name = "Drops";
            this.listViewCounters.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup5,
            listViewGroup6,
            listViewGroup7,
            listViewGroup8});
            this.listViewCounters.HideSelection = false;
            this.listViewCounters.Location = new System.Drawing.Point(12, 42);
            this.listViewCounters.Name = "listViewCounters";
            this.listViewCounters.Size = new System.Drawing.Size(580, 422);
            this.listViewCounters.TabIndex = 8;
            this.listViewCounters.UseCompatibleStateImageBehavior = false;
            this.listViewCounters.View = System.Windows.Forms.View.Details;
            this.listViewCounters.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listViewCounters_KeyUp);
            this.listViewCounters.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listViewCounters_MouseUp);
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "";
            this.columnHeader10.Width = 180;
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
            // columnHeader1
            // 
            this.columnHeader1.Text = "Group";
            this.columnHeader1.Width = 90;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "All-Time";
            this.columnHeader13.Width = 90;
            // 
            // buttonExport
            // 
            this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExport.Location = new System.Drawing.Point(487, 470);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(105, 27);
            this.buttonExport.TabIndex = 12;
            this.buttonExport.Tag = "All";
            this.buttonExport.Text = "Export";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.ButtonExport_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.countersToolStripMenuItem,
            this.runtimeToolStripMenuItem,
            this.heroEquipmentToolStripMenuItem,
            this.dropsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.qEDropsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.allToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(162, 148);
            // 
            // countersToolStripMenuItem
            // 
            this.countersToolStripMenuItem.Name = "countersToolStripMenuItem";
            this.countersToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.countersToolStripMenuItem.Tag = "Counters";
            this.countersToolStripMenuItem.Text = "Counters";
            this.countersToolStripMenuItem.Click += new System.EventHandler(this.ResetToolStripMenuItem_Click);
            // 
            // runtimeToolStripMenuItem
            // 
            this.runtimeToolStripMenuItem.Name = "runtimeToolStripMenuItem";
            this.runtimeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.runtimeToolStripMenuItem.Tag = "Runtime";
            this.runtimeToolStripMenuItem.Text = "Runtime";
            this.runtimeToolStripMenuItem.Click += new System.EventHandler(this.ResetToolStripMenuItem_Click);
            // 
            // heroEquipmentToolStripMenuItem
            // 
            this.heroEquipmentToolStripMenuItem.Name = "heroEquipmentToolStripMenuItem";
            this.heroEquipmentToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.heroEquipmentToolStripMenuItem.Tag = "HeroEquipment";
            this.heroEquipmentToolStripMenuItem.Text = "Hero Equipment";
            this.heroEquipmentToolStripMenuItem.Click += new System.EventHandler(this.ResetToolStripMenuItem_Click);
            // 
            // dropsToolStripMenuItem
            // 
            this.dropsToolStripMenuItem.Name = "dropsToolStripMenuItem";
            this.dropsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.dropsToolStripMenuItem.Tag = "Drops";
            this.dropsToolStripMenuItem.Text = "Drops";
            this.dropsToolStripMenuItem.Click += new System.EventHandler(this.ResetToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(158, 6);
            // 
            // qEDropsToolStripMenuItem
            // 
            this.qEDropsToolStripMenuItem.Name = "qEDropsToolStripMenuItem";
            this.qEDropsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.qEDropsToolStripMenuItem.Tag = "QEDrops";
            this.qEDropsToolStripMenuItem.Text = "QE Drops";
            this.qEDropsToolStripMenuItem.Click += new System.EventHandler(this.ResetToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(158, 6);
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            this.allToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.allToolStripMenuItem.Tag = "All";
            this.allToolStripMenuItem.Text = "All";
            this.allToolStripMenuItem.Click += new System.EventHandler(this.ResetToolStripMenuItem_Click);
            // 
            // comboBoxLab
            // 
            this.comboBoxLab.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLab.FormattingEnabled = true;
            this.comboBoxLab.Location = new System.Drawing.Point(13, 13);
            this.comboBoxLab.Name = "comboBoxLab";
            this.comboBoxLab.Size = new System.Drawing.Size(411, 21);
            this.comboBoxLab.TabIndex = 13;
            this.comboBoxLab.SelectedIndexChanged += new System.EventHandler(this.comboBoxLab_SelectedIndexChanged);
            // 
            // comboBoxQE
            // 
            this.comboBoxQE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxQE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxQE.FormattingEnabled = true;
            this.comboBoxQE.Items.AddRange(new object[] {
            "Exclude QE Drops",
            "Include QE Drops",
            "Only QE Drops"});
            this.comboBoxQE.Location = new System.Drawing.Point(431, 13);
            this.comboBoxQE.Name = "comboBoxQE";
            this.comboBoxQE.Size = new System.Drawing.Size(161, 21);
            this.comboBoxQE.TabIndex = 14;
            this.comboBoxQE.SelectedIndexChanged += new System.EventHandler(this.comboBoxLab_SelectedIndexChanged);
            // 
            // buttonCountersResetGroup
            // 
            this.buttonCountersResetGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCountersResetGroup.Location = new System.Drawing.Point(234, 470);
            this.buttonCountersResetGroup.Name = "buttonCountersResetGroup";
            this.buttonCountersResetGroup.Size = new System.Drawing.Size(105, 27);
            this.buttonCountersResetGroup.TabIndex = 15;
            this.buttonCountersResetGroup.Tag = "Group";
            this.buttonCountersResetGroup.Text = "Reset Group";
            this.buttonCountersResetGroup.UseVisualStyleBackColor = true;
            this.buttonCountersResetGroup.Click += new System.EventHandler(this.ButtonCountersReset_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Comma Separated Values (*.csv) |*.csv";
            this.saveFileDialog1.Title = "Choose export file";
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem8});
            this.contextMenuStrip2.Name = "contextMenuStrip1";
            this.contextMenuStrip2.Size = new System.Drawing.Size(207, 48);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.CheckOnClick = true;
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(206, 22);
            this.toolStripMenuItem8.Tag = "";
            this.toolStripMenuItem8.Text = "Perfect Passive Acquired ";
            this.toolStripMenuItem8.Click += new System.EventHandler(this.toolStripMenuItem8_Click);
            // 
            // CountersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 509);
            this.Controls.Add(this.buttonCountersResetGroup);
            this.Controls.Add(this.comboBoxQE);
            this.Controls.Add(this.comboBoxLab);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.buttonCountersResetSession);
            this.Controls.Add(this.buttonCountersResetLab);
            this.Controls.Add(this.buttonCountersResetAll);
            this.Controls.Add(this.listViewCounters);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(620, 39);
            this.Name = "CountersForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Counters";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CountersForm_FormClosed);
            this.Load += new System.EventHandler(this.CountersForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCountersResetSession;
        private System.Windows.Forms.Button buttonCountersResetLab;
        private System.Windows.Forms.Button buttonCountersResetAll;
        private Controls.ListViewExtended listViewCounters;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem countersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runtimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem heroEquipmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dropsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBoxLab;
        private System.Windows.Forms.ComboBox comboBoxQE;
        private System.Windows.Forms.ToolStripMenuItem qEDropsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button buttonCountersResetGroup;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
    }
}