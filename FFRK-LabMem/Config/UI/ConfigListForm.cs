using FFRK_LabMem.Machines;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;

namespace FFRK_LabMem.Config.UI
{
    public partial class ConfigListForm : Form
    {

        private string currentConfig;

        public ConfigListForm()
        {
            InitializeComponent();
        }

        public static void CreateAndShow(string currentConfig)
        {
            // Show form
            var form = new ConfigListForm
            {
                currentConfig = currentConfig
            };
            form.ShowDialog();
        }

        private void ConfigListForm_Load(object sender, EventArgs e)
        {

            labelPath.Text = String.Format("Config files path: {0}", ConfigFile.CONFIG_FOLDER);
            foreach (var item in ConfigFile.GetFiles())
            {
                listBoxConfigs.Items.Add(item);
            }
            listBoxConfigs.SelectedIndex = 0;

        }

        private async void ButtonAdd_Click(object sender, EventArgs e)
        {
            var input = Interaction.InputBox("Enter new name", "Add Configuration", "New");
            if (!String.IsNullOrEmpty(input))
            {
                var file = ConfigFile.FromName(input);
                try
                {
                    if(File.Exists(file.Path)){
                        MessageBox.Show(this, "File already exists!  Try a different name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    await new LabConfiguration().Save(file.Path);
                    listBoxConfigs.Items.Add(file);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error adding configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void ButtonRename_Click(object sender, EventArgs e)
        {
            var target = (ConfigFile)listBoxConfigs.SelectedItem;
            var input = Interaction.InputBox("Enter new name", "Rename Configuration", target.Name);
            if (!String.IsNullOrEmpty(input))
            {
                var file = ConfigFile.FromName(input);
                try
                {
                    File.Move(target.Path, file.Path);
                    listBoxConfigs.Items[listBoxConfigs.SelectedIndex] = file;
                } catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error renaming configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {

            if (ConfigFile.FromObject(listBoxConfigs.SelectedItem).Path.ToLower().Equals(currentConfig.ToLower()))
            {
                MessageBox.Show(this, "Can't delete current config!","Warning",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var target = (ConfigFile)listBoxConfigs.SelectedItem;
            var result = MessageBox.Show(this, "Are you sure you wish to delete " + target.Name  + "?", "Remove Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    File.Delete(target.Path);
                    listBoxConfigs.Items.Remove(listBoxConfigs.SelectedItem);
                    listBoxConfigs.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error deleting configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
        }

        private void ButtonDupe_Click(object sender, EventArgs e)
        {

            var target = (ConfigFile)listBoxConfigs.SelectedItem;
            var input = Interaction.InputBox("Enter new name", "Duplicate Configuration", target.Name + " Copy");
            if (!String.IsNullOrEmpty(input))
            {
                var file = ConfigFile.FromName(input);
                try
                {
                    File.Copy(target.Path, file.Path);
                    listBoxConfigs.Items.Add(file);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error duplicating configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }


        }

    }
}
