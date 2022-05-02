
namespace FFRK_LabMem.Config.UI
{
    partial class ConfigScheduleForm
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePickerEnable = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxEnableRepeat = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanelEnable = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxEnableSun = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableMon = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableTue = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableWed = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableThu = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableFri = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableSat = new System.Windows.Forms.CheckBox();
            this.checkBoxHardStart = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanelDisable = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxDisableSun = new System.Windows.Forms.CheckBox();
            this.checkBoxDisableMon = new System.Windows.Forms.CheckBox();
            this.checkBoxDisableTue = new System.Windows.Forms.CheckBox();
            this.checkBoxDisableWed = new System.Windows.Forms.CheckBox();
            this.checkBoxDisableThu = new System.Windows.Forms.CheckBox();
            this.checkBoxDisableFri = new System.Windows.Forms.CheckBox();
            this.checkBoxDisableSat = new System.Windows.Forms.CheckBox();
            this.checkBoxHardStop = new System.Windows.Forms.CheckBox();
            this.comboBoxDisableRepeat = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimePickerDisable = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.checkBoxForceStart = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanelEnable.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanelDisable.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(396, 452);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 29);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(315, 452);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 29);
            this.buttonOk.TabIndex = 4;
            this.buttonOk.Text = "Save";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "Start at:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dateTimePickerEnable
            // 
            this.dateTimePickerEnable.Checked = false;
            this.dateTimePickerEnable.CustomFormat = "MM/dd/yyyy hh:mm tt";
            this.dateTimePickerEnable.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerEnable.Location = new System.Drawing.Point(83, 25);
            this.dateTimePickerEnable.Name = "dateTimePickerEnable";
            this.dateTimePickerEnable.ShowCheckBox = true;
            this.dateTimePickerEnable.Size = new System.Drawing.Size(248, 20);
            this.dateTimePickerEnable.TabIndex = 8;
            this.dateTimePickerEnable.ValueChanged += new System.EventHandler(this.SetControlState);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(4, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "Repeats:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxEnableRepeat
            // 
            this.comboBoxEnableRepeat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEnableRepeat.FormattingEnabled = true;
            this.comboBoxEnableRepeat.Items.AddRange(new object[] {
            "Never",
            "Daily",
            "Weekly"});
            this.comboBoxEnableRepeat.Location = new System.Drawing.Point(84, 52);
            this.comboBoxEnableRepeat.Name = "comboBoxEnableRepeat";
            this.comboBoxEnableRepeat.Size = new System.Drawing.Size(247, 21);
            this.comboBoxEnableRepeat.TabIndex = 10;
            this.comboBoxEnableRepeat.SelectedIndexChanged += new System.EventHandler(this.SetControlState);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkBoxForceStart);
            this.groupBox1.Controls.Add(this.flowLayoutPanelEnable);
            this.groupBox1.Controls.Add(this.checkBoxHardStart);
            this.groupBox1.Controls.Add(this.comboBoxEnableRepeat);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dateTimePickerEnable);
            this.groupBox1.Location = new System.Drawing.Point(13, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(458, 207);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Start Options";
            // 
            // flowLayoutPanelEnable
            // 
            this.flowLayoutPanelEnable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanelEnable.Controls.Add(this.checkBoxEnableSun);
            this.flowLayoutPanelEnable.Controls.Add(this.checkBoxEnableMon);
            this.flowLayoutPanelEnable.Controls.Add(this.checkBoxEnableTue);
            this.flowLayoutPanelEnable.Controls.Add(this.checkBoxEnableWed);
            this.flowLayoutPanelEnable.Controls.Add(this.checkBoxEnableThu);
            this.flowLayoutPanelEnable.Controls.Add(this.checkBoxEnableFri);
            this.flowLayoutPanelEnable.Controls.Add(this.checkBoxEnableSat);
            this.flowLayoutPanelEnable.Enabled = false;
            this.flowLayoutPanelEnable.Location = new System.Drawing.Point(17, 79);
            this.flowLayoutPanelEnable.Name = "flowLayoutPanelEnable";
            this.flowLayoutPanelEnable.Padding = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanelEnable.Size = new System.Drawing.Size(435, 71);
            this.flowLayoutPanelEnable.TabIndex = 12;
            // 
            // checkBoxEnableSun
            // 
            this.checkBoxEnableSun.Location = new System.Drawing.Point(8, 8);
            this.checkBoxEnableSun.Name = "checkBoxEnableSun";
            this.checkBoxEnableSun.Size = new System.Drawing.Size(82, 18);
            this.checkBoxEnableSun.TabIndex = 0;
            this.checkBoxEnableSun.Tag = "0";
            this.checkBoxEnableSun.Text = "Sunday";
            this.checkBoxEnableSun.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableMon
            // 
            this.checkBoxEnableMon.Location = new System.Drawing.Point(96, 8);
            this.checkBoxEnableMon.Name = "checkBoxEnableMon";
            this.checkBoxEnableMon.Size = new System.Drawing.Size(82, 18);
            this.checkBoxEnableMon.TabIndex = 1;
            this.checkBoxEnableMon.Tag = "1";
            this.checkBoxEnableMon.Text = "Monday";
            this.checkBoxEnableMon.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableTue
            // 
            this.checkBoxEnableTue.Location = new System.Drawing.Point(184, 8);
            this.checkBoxEnableTue.Name = "checkBoxEnableTue";
            this.checkBoxEnableTue.Size = new System.Drawing.Size(82, 18);
            this.checkBoxEnableTue.TabIndex = 2;
            this.checkBoxEnableTue.Tag = "2";
            this.checkBoxEnableTue.Text = "Tuesday";
            this.checkBoxEnableTue.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableWed
            // 
            this.checkBoxEnableWed.Location = new System.Drawing.Point(272, 8);
            this.checkBoxEnableWed.Name = "checkBoxEnableWed";
            this.checkBoxEnableWed.Size = new System.Drawing.Size(96, 18);
            this.checkBoxEnableWed.TabIndex = 3;
            this.checkBoxEnableWed.Tag = "3";
            this.checkBoxEnableWed.Text = "Wednesday";
            this.checkBoxEnableWed.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableThu
            // 
            this.checkBoxEnableThu.Location = new System.Drawing.Point(8, 32);
            this.checkBoxEnableThu.Name = "checkBoxEnableThu";
            this.checkBoxEnableThu.Size = new System.Drawing.Size(82, 18);
            this.checkBoxEnableThu.TabIndex = 4;
            this.checkBoxEnableThu.Tag = "4";
            this.checkBoxEnableThu.Text = "Thursday";
            this.checkBoxEnableThu.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableFri
            // 
            this.checkBoxEnableFri.Location = new System.Drawing.Point(96, 32);
            this.checkBoxEnableFri.Name = "checkBoxEnableFri";
            this.checkBoxEnableFri.Size = new System.Drawing.Size(82, 18);
            this.checkBoxEnableFri.TabIndex = 5;
            this.checkBoxEnableFri.Tag = "5";
            this.checkBoxEnableFri.Text = "Friday";
            this.checkBoxEnableFri.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableSat
            // 
            this.checkBoxEnableSat.Location = new System.Drawing.Point(184, 32);
            this.checkBoxEnableSat.Name = "checkBoxEnableSat";
            this.checkBoxEnableSat.Size = new System.Drawing.Size(82, 18);
            this.checkBoxEnableSat.TabIndex = 6;
            this.checkBoxEnableSat.Tag = "6";
            this.checkBoxEnableSat.Text = "Saturday";
            this.checkBoxEnableSat.UseVisualStyleBackColor = true;
            // 
            // checkBoxHardStart
            // 
            this.checkBoxHardStart.AutoSize = true;
            this.checkBoxHardStart.Enabled = false;
            this.checkBoxHardStart.Location = new System.Drawing.Point(17, 156);
            this.checkBoxHardStart.Name = "checkBoxHardStart";
            this.checkBoxHardStart.Size = new System.Drawing.Size(201, 17);
            this.checkBoxHardStart.TabIndex = 11;
            this.checkBoxHardStart.Text = "Close and restart FFRK when starting";
            this.checkBoxHardStart.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.flowLayoutPanelDisable);
            this.groupBox2.Controls.Add(this.checkBoxHardStop);
            this.groupBox2.Controls.Add(this.comboBoxDisableRepeat);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.dateTimePickerDisable);
            this.groupBox2.Location = new System.Drawing.Point(12, 260);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(458, 186);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Stop Options";
            // 
            // flowLayoutPanelDisable
            // 
            this.flowLayoutPanelDisable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanelDisable.Controls.Add(this.checkBoxDisableSun);
            this.flowLayoutPanelDisable.Controls.Add(this.checkBoxDisableMon);
            this.flowLayoutPanelDisable.Controls.Add(this.checkBoxDisableTue);
            this.flowLayoutPanelDisable.Controls.Add(this.checkBoxDisableWed);
            this.flowLayoutPanelDisable.Controls.Add(this.checkBoxDisableThu);
            this.flowLayoutPanelDisable.Controls.Add(this.checkBoxDisableFri);
            this.flowLayoutPanelDisable.Controls.Add(this.checkBoxDisableSat);
            this.flowLayoutPanelDisable.Enabled = false;
            this.flowLayoutPanelDisable.Location = new System.Drawing.Point(17, 79);
            this.flowLayoutPanelDisable.Name = "flowLayoutPanelDisable";
            this.flowLayoutPanelDisable.Padding = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanelDisable.Size = new System.Drawing.Size(435, 71);
            this.flowLayoutPanelDisable.TabIndex = 12;
            // 
            // checkBoxDisableSun
            // 
            this.checkBoxDisableSun.Location = new System.Drawing.Point(8, 8);
            this.checkBoxDisableSun.Name = "checkBoxDisableSun";
            this.checkBoxDisableSun.Size = new System.Drawing.Size(82, 18);
            this.checkBoxDisableSun.TabIndex = 0;
            this.checkBoxDisableSun.Tag = "0";
            this.checkBoxDisableSun.Text = "Sunday";
            this.checkBoxDisableSun.UseVisualStyleBackColor = true;
            // 
            // checkBoxDisableMon
            // 
            this.checkBoxDisableMon.Location = new System.Drawing.Point(96, 8);
            this.checkBoxDisableMon.Name = "checkBoxDisableMon";
            this.checkBoxDisableMon.Size = new System.Drawing.Size(82, 18);
            this.checkBoxDisableMon.TabIndex = 1;
            this.checkBoxDisableMon.Tag = "1";
            this.checkBoxDisableMon.Text = "Monday";
            this.checkBoxDisableMon.UseVisualStyleBackColor = true;
            // 
            // checkBoxDisableTue
            // 
            this.checkBoxDisableTue.Location = new System.Drawing.Point(184, 8);
            this.checkBoxDisableTue.Name = "checkBoxDisableTue";
            this.checkBoxDisableTue.Size = new System.Drawing.Size(82, 18);
            this.checkBoxDisableTue.TabIndex = 2;
            this.checkBoxDisableTue.Tag = "2";
            this.checkBoxDisableTue.Text = "Tuesday";
            this.checkBoxDisableTue.UseVisualStyleBackColor = true;
            // 
            // checkBoxDisableWed
            // 
            this.checkBoxDisableWed.Location = new System.Drawing.Point(272, 8);
            this.checkBoxDisableWed.Name = "checkBoxDisableWed";
            this.checkBoxDisableWed.Size = new System.Drawing.Size(96, 18);
            this.checkBoxDisableWed.TabIndex = 3;
            this.checkBoxDisableWed.Tag = "3";
            this.checkBoxDisableWed.Text = "Wednesday";
            this.checkBoxDisableWed.UseVisualStyleBackColor = true;
            // 
            // checkBoxDisableThu
            // 
            this.checkBoxDisableThu.Location = new System.Drawing.Point(8, 32);
            this.checkBoxDisableThu.Name = "checkBoxDisableThu";
            this.checkBoxDisableThu.Size = new System.Drawing.Size(82, 18);
            this.checkBoxDisableThu.TabIndex = 4;
            this.checkBoxDisableThu.Tag = "4";
            this.checkBoxDisableThu.Text = "Thursday";
            this.checkBoxDisableThu.UseVisualStyleBackColor = true;
            // 
            // checkBoxDisableFri
            // 
            this.checkBoxDisableFri.Location = new System.Drawing.Point(96, 32);
            this.checkBoxDisableFri.Name = "checkBoxDisableFri";
            this.checkBoxDisableFri.Size = new System.Drawing.Size(82, 18);
            this.checkBoxDisableFri.TabIndex = 5;
            this.checkBoxDisableFri.Tag = "5";
            this.checkBoxDisableFri.Text = "Friday";
            this.checkBoxDisableFri.UseVisualStyleBackColor = true;
            // 
            // checkBoxDisableSat
            // 
            this.checkBoxDisableSat.Location = new System.Drawing.Point(184, 32);
            this.checkBoxDisableSat.Name = "checkBoxDisableSat";
            this.checkBoxDisableSat.Size = new System.Drawing.Size(82, 18);
            this.checkBoxDisableSat.TabIndex = 6;
            this.checkBoxDisableSat.Tag = "6";
            this.checkBoxDisableSat.Text = "Saturday";
            this.checkBoxDisableSat.UseVisualStyleBackColor = true;
            // 
            // checkBoxHardStop
            // 
            this.checkBoxHardStop.AutoSize = true;
            this.checkBoxHardStop.Enabled = false;
            this.checkBoxHardStop.Location = new System.Drawing.Point(17, 156);
            this.checkBoxHardStop.Name = "checkBoxHardStop";
            this.checkBoxHardStop.Size = new System.Drawing.Size(154, 17);
            this.checkBoxHardStop.TabIndex = 11;
            this.checkBoxHardStop.Text = "Close FFRK when stopping";
            this.checkBoxHardStop.UseVisualStyleBackColor = true;
            // 
            // comboBoxDisableRepeat
            // 
            this.comboBoxDisableRepeat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDisableRepeat.FormattingEnabled = true;
            this.comboBoxDisableRepeat.Items.AddRange(new object[] {
            "Never",
            "Daily",
            "Weekly"});
            this.comboBoxDisableRepeat.Location = new System.Drawing.Point(84, 52);
            this.comboBoxDisableRepeat.Name = "comboBoxDisableRepeat";
            this.comboBoxDisableRepeat.Size = new System.Drawing.Size(247, 21);
            this.comboBoxDisableRepeat.TabIndex = 10;
            this.comboBoxDisableRepeat.SelectedIndexChanged += new System.EventHandler(this.SetControlState);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(7, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Stop at:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(4, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Repeats:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dateTimePickerDisable
            // 
            this.dateTimePickerDisable.Checked = false;
            this.dateTimePickerDisable.CustomFormat = "MM/dd/yyyy hh:mm tt";
            this.dateTimePickerDisable.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerDisable.Location = new System.Drawing.Point(83, 25);
            this.dateTimePickerDisable.Name = "dateTimePickerDisable";
            this.dateTimePickerDisable.ShowCheckBox = true;
            this.dateTimePickerDisable.Size = new System.Drawing.Size(248, 20);
            this.dateTimePickerDisable.TabIndex = 8;
            this.dateTimePickerDisable.ValueChanged += new System.EventHandler(this.SetControlState);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Schedule Name:";
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.Location = new System.Drawing.Point(106, 10);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(365, 20);
            this.textBoxName.TabIndex = 14;
            // 
            // checkBoxForceStart
            // 
            this.checkBoxForceStart.AutoSize = true;
            this.checkBoxForceStart.Enabled = false;
            this.checkBoxForceStart.Location = new System.Drawing.Point(17, 179);
            this.checkBoxForceStart.Name = "checkBoxForceStart";
            this.checkBoxForceStart.Size = new System.Drawing.Size(162, 17);
            this.checkBoxForceStart.TabIndex = 13;
            this.checkBoxForceStart.Text = "Force start if already enabled";
            this.checkBoxForceStart.UseVisualStyleBackColor = true;
            // 
            // ConfigScheduleForm
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(483, 493);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigScheduleForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Schedule";
            this.Load += new System.EventHandler(this.ConfigScheduleForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.flowLayoutPanelEnable.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.flowLayoutPanelDisable.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxEnableRepeat;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelEnable;
        private System.Windows.Forms.CheckBox checkBoxEnableSun;
        private System.Windows.Forms.CheckBox checkBoxEnableMon;
        private System.Windows.Forms.CheckBox checkBoxEnableTue;
        private System.Windows.Forms.CheckBox checkBoxEnableWed;
        private System.Windows.Forms.CheckBox checkBoxEnableThu;
        private System.Windows.Forms.CheckBox checkBoxEnableFri;
        private System.Windows.Forms.CheckBox checkBoxEnableSat;
        private System.Windows.Forms.CheckBox checkBoxHardStart;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelDisable;
        private System.Windows.Forms.CheckBox checkBoxDisableSun;
        private System.Windows.Forms.CheckBox checkBoxDisableMon;
        private System.Windows.Forms.CheckBox checkBoxDisableTue;
        private System.Windows.Forms.CheckBox checkBoxDisableWed;
        private System.Windows.Forms.CheckBox checkBoxDisableThu;
        private System.Windows.Forms.CheckBox checkBoxDisableFri;
        private System.Windows.Forms.CheckBox checkBoxDisableSat;
        private System.Windows.Forms.CheckBox checkBoxHardStop;
        private System.Windows.Forms.ComboBox comboBoxDisableRepeat;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dateTimePickerDisable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.CheckBox checkBoxForceStart;
    }
}