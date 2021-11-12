
namespace FFRK_LabMem.Config.UI
{
    partial class ConfigListForm
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
            this.listBoxConfigs = new System.Windows.Forms.ListBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonRename = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.labelPath = new System.Windows.Forms.Label();
            this.buttonDupe = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(261, 286);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(106, 29);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Close";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // listBoxConfigs
            // 
            this.listBoxConfigs.FormattingEnabled = true;
            this.listBoxConfigs.Location = new System.Drawing.Point(12, 38);
            this.listBoxConfigs.Name = "listBoxConfigs";
            this.listBoxConfigs.Size = new System.Drawing.Size(244, 277);
            this.listBoxConfigs.TabIndex = 6;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAdd.Location = new System.Drawing.Point(261, 38);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(106, 29);
            this.buttonAdd.TabIndex = 7;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // buttonRename
            // 
            this.buttonRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRename.Location = new System.Drawing.Point(261, 73);
            this.buttonRename.Name = "buttonRename";
            this.buttonRename.Size = new System.Drawing.Size(106, 29);
            this.buttonRename.TabIndex = 8;
            this.buttonRename.Text = "Rename";
            this.buttonRename.UseVisualStyleBackColor = true;
            this.buttonRename.Click += new System.EventHandler(this.ButtonRename_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemove.Location = new System.Drawing.Point(260, 143);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(106, 29);
            this.buttonRemove.TabIndex = 9;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.ButtonRemove_Click);
            // 
            // labelPath
            // 
            this.labelPath.Location = new System.Drawing.Point(13, 13);
            this.labelPath.Name = "labelPath";
            this.labelPath.Size = new System.Drawing.Size(353, 18);
            this.labelPath.TabIndex = 10;
            this.labelPath.Text = "Config files path:";
            this.labelPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonDupe
            // 
            this.buttonDupe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDupe.Location = new System.Drawing.Point(260, 108);
            this.buttonDupe.Name = "buttonDupe";
            this.buttonDupe.Size = new System.Drawing.Size(106, 29);
            this.buttonDupe.TabIndex = 11;
            this.buttonDupe.Text = "Duplicate";
            this.buttonDupe.UseVisualStyleBackColor = true;
            this.buttonDupe.Click += new System.EventHandler(this.ButtonDupe_Click);
            // 
            // ConfigListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 328);
            this.Controls.Add(this.buttonDupe);
            this.Controls.Add(this.labelPath);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.buttonRename);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.listBoxConfigs);
            this.Controls.Add(this.buttonCancel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigListForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Lab Configurations";
            this.Load += new System.EventHandler(this.ConfigListForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ListBox listBoxConfigs;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonRename;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.Button buttonDupe;
    }
}