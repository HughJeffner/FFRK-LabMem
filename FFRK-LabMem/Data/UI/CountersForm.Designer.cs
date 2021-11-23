
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Counters", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Runtime", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Hero Equipment", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Drops", System.Windows.Forms.HorizontalAlignment.Left);
            this.buttonCountersResetSession = new System.Windows.Forms.Button();
            this.buttonCountersResetLab = new System.Windows.Forms.Button();
            this.buttonCountersResetAll = new System.Windows.Forms.Button();
            this.listViewCounters = new System.Windows.Forms.ListView();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonSettings = new System.Windows.Forms.Button();
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
            this.buttonCountersResetAll.Location = new System.Drawing.Point(234, 470);
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
            this.columnHeader13});
            this.listViewCounters.FullRowSelect = true;
            listViewGroup1.Header = "Counters";
            listViewGroup1.Name = "Counters";
            listViewGroup2.Header = "Runtime";
            listViewGroup2.Name = "Runtime";
            listViewGroup3.Header = "Hero Equipment";
            listViewGroup3.Name = "HE";
            listViewGroup4.Header = "Drops";
            listViewGroup4.Name = "Drops";
            this.listViewCounters.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4});
            this.listViewCounters.HideSelection = false;
            this.listViewCounters.Location = new System.Drawing.Point(12, 12);
            this.listViewCounters.Name = "listViewCounters";
            this.listViewCounters.Size = new System.Drawing.Size(554, 452);
            this.listViewCounters.TabIndex = 8;
            this.listViewCounters.UseCompatibleStateImageBehavior = false;
            this.listViewCounters.View = System.Windows.Forms.View.Details;
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
            // columnHeader13
            // 
            this.columnHeader13.Text = "All-Time";
            this.columnHeader13.Width = 140;
            // 
            // buttonSettings
            // 
            this.buttonSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSettings.Location = new System.Drawing.Point(461, 470);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(105, 27);
            this.buttonSettings.TabIndex = 12;
            this.buttonSettings.Tag = "All";
            this.buttonSettings.Text = "Settings";
            this.buttonSettings.UseVisualStyleBackColor = true;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // CountersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 509);
            this.Controls.Add(this.buttonSettings);
            this.Controls.Add(this.buttonCountersResetSession);
            this.Controls.Add(this.buttonCountersResetLab);
            this.Controls.Add(this.buttonCountersResetAll);
            this.Controls.Add(this.listViewCounters);
            this.Name = "CountersForm";
            this.Text = "Counters";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CountersForm_FormClosed);
            this.Load += new System.EventHandler(this.CountersForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCountersResetSession;
        private System.Windows.Forms.Button buttonCountersResetLab;
        private System.Windows.Forms.Button buttonCountersResetAll;
        private System.Windows.Forms.ListView listViewCounters;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.Button buttonSettings;
    }
}