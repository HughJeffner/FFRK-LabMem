using FFRK_LabMem.Machines;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;

namespace FFRK_LabMem.Config
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
            var form = new ConfigListForm();
            form.currentConfig = currentConfig;
            form.ShowDialog();
        }

        private void ConfigListForm_Load(object sender, EventArgs e)
        {

            foreach (var item in Directory.GetFiles(ConfigForm.CONFIG_FOLDER, "*.json"))
            {
                listBoxConfigs.Items.Add(item.Replace(ConfigForm.CONFIG_FOLDER,""));
            }
            listBoxConfigs.SelectedIndex = 0;

        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var input = Interaction.InputBox("Enter new name", "Add Configuration", "lab.new.json");
            if (!String.IsNullOrEmpty(input))
            {
                try
                {
                    if(File.Exists(ConfigForm.CONFIG_FOLDER + input)){
                        MessageBox.Show(this, "File already exists!  Try a different name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    File.WriteAllText(ConfigForm.CONFIG_FOLDER + input, JsonConvert.SerializeObject(new LabConfiguration(), Formatting.Indented));
                    listBoxConfigs.Items.Add(input);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error adding configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void buttonRename_Click(object sender, EventArgs e)
        {
            var target = listBoxConfigs.SelectedItem.ToString();
            var input = Interaction.InputBox("Enter new name", "Rename Configuration", target);
            if (!String.IsNullOrEmpty(input))
            {
                try
                {
                    File.Move(ConfigForm.CONFIG_FOLDER + target, ConfigForm.CONFIG_FOLDER + input);
                    listBoxConfigs.Items[listBoxConfigs.SelectedIndex] = input;
                } catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error renaming configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {

            if (listBoxConfigs.SelectedItem.ToString().ToLower().Equals(currentConfig))
            {
                MessageBox.Show(this, "Can't delete current config!","Warning",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var target = listBoxConfigs.SelectedItem.ToString();
            var result = MessageBox.Show(this, "Are you sure?", "Remove Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    File.Delete(ConfigForm.CONFIG_FOLDER + target);
                    listBoxConfigs.Items.Remove(listBoxConfigs.SelectedItem);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error deleting configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
        }
    }
}
