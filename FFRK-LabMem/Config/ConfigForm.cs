using FFRK_LabMem.Machines;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections;
using FFRK_Machines;
using System.Diagnostics;

namespace FFRK_LabMem.Config
{
    public partial class ConfigForm : Form
    {

        private Dictionary<String, String> paintingLookup = new Dictionary<string, string>() {
            {"1.1", "Combatant Painting (Green)"},
            {"1.2", "Combatant Painting (Orange)"},
            {"1.3", "Combatant Painting (Red)"},
            {"2", "Master Painting"},
            {"3", "Treasure Painting"},
            {"4", "Exploration Painting"},
            {"5", "Onslaught Painting"},
            {"6", "Portal Painting"},
            {"7", "Restoration Painting"}
        };

        private Dictionary<String, String> treasureLookup = new Dictionary<string, string>() {
            {"5", "Hero Equipment"},
            {"4", "Anima Lenses, Bookmark, 5* Rat Tails, Map x2, Teleport Stone"},
            {"3", "6* Motes, 4* Rat Tails, Magic Key, Treasure Map, Lethe Potion"},
            {"2", "6* Crystals, Rainbow Crystal, Rosetta Stone of Wisdom"},
            {"1", "5* Orbs, 5* Motes"}
        };


        public ConfigHelper configHelper = null;
        public LabController controller = null;
        public Lab.Configuration labConfig = new Lab.Configuration();

        public ConfigForm()
        {
            InitializeComponent();
        }

        public static void CreateAndShow(ConfigHelper configHelper, LabController controller)
        {
            // Disable Lab
            controller.Disable();

            // Show form
            Application.EnableVisualStyles();
            var form = new ConfigForm();
            form.configHelper = configHelper;
            form.controller = controller;
            form.ShowDialog();
        }

        private void ListCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControl.SelectedIndex = listCategory.SelectedIndex;
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            // Tab fakery
            listCategory.SelectedIndex = 0;
            tabControl.Top -= tabControl.ItemSize.Height;
            tabControl.Height += tabControl.ItemSize.Height;
            tabControl.Region = new Region(new RectangleF(tabPage1.Left, tabPage1.Top, tabPage1.Width, tabPage1.Height + tabControl.ItemSize.Height-20));

            // Values
            checkBoxTimestamps.Checked = configHelper.GetBool("console.timestamps", true);
            checkBoxDebug.Checked = configHelper.GetBool("console.debug", false);
            checkBoxUpdates.Checked = configHelper.GetBool("updates.checkForUpdates", true);
            checkBoxPrerelease.Checked = configHelper.GetBool("updates.includePrerelease", false);
            checkBoxDatalog.Checked = configHelper.GetBool("datalogger.enabled", false);
            numericUpDownScreenTop.Value = configHelper.GetInt("screen.topOffset", -1);
            numericUpDownScreenBottom.Value = configHelper.GetInt("screen.bottomOffset", -1);
            numericUpDownProxyPort.Value = configHelper.GetInt("proxy.port", 8081);
            checkBoxProxySecure.Checked = configHelper.GetBool("proxy.secure", true);
            textBoxProxyBlocklist.Text = configHelper.GetString("proxy.blocklist", "");
            textBoxAdbPath.Text = configHelper.GetString("adb.path", "adb.exe");
            textBoxAdbHost.Text = configHelper.GetString("adb.host", "127.0.0.1:7555");
            
            // Load lab .json
            foreach (var item in Directory.GetFiles("./Config/", "*.json"))
            {
                comboBoxLab.Items.Add(item);
                if (item.EndsWith(configHelper.GetString("lab.configFile", "config/lab.balanced.json"))) comboBoxLab.SelectedIndex = comboBoxLab.Items.Count - 1;
            }
            if (comboBoxLab.SelectedItem != null)
            {
                labConfig = JsonConvert.DeserializeObject<Lab.Configuration>(File.ReadAllText(comboBoxLab.SelectedItem.ToString()));
            }

            // List sorting
            listViewPaintings.ListViewItemSorter = new PaintingSorter();
            listViewTreasures.ListViewItemSorter = new TreasureSorter();

            // Hide restart warning
            lblRestart.Visible = false;

        }
        private void buttonOk_Click(object sender, EventArgs e)
        {
            ColorConsole.Write("Saving configuration... ");

            // General
            configHelper.SetValue("console.timestamps", checkBoxTimestamps.Checked ? "true" : "false");
            configHelper.SetValue("console.debug", checkBoxDebug.Checked ? "true" : "false");
            configHelper.SetValue("updates.checkForUpdates", checkBoxUpdates.Checked ? "true" : "false");
            configHelper.SetValue("updates.includePrerelease", checkBoxPrerelease.Checked ? "true" : "false");
            configHelper.SetValue("datalogger.enabled", checkBoxDatalog.Checked ? "true" : "false");
            configHelper.SetValue("screen.topOffset", numericUpDownScreenTop.Value.ToString());
            configHelper.SetValue("screen.bottomOffset", numericUpDownScreenBottom.Value.ToString());
            configHelper.SetValue("proxy.port", numericUpDownProxyPort.Value.ToString());
            configHelper.SetValue("proxy.secure", checkBoxProxySecure.Checked ? "true" : "false");
            configHelper.SetValue("proxy.blocklist", textBoxProxyBlocklist.Text);
            configHelper.SetValue("adb.path", textBoxAdbPath.Text);
            configHelper.SetValue("adb.host", textBoxAdbHost.Text);
            configHelper.SetValue("lab.configFile",comboBoxLab.SelectedItem.ToString());

            // Lab
            labConfig.Debug = checkBoxLabDebug.Checked;
            labConfig.OpenDoors = checkBoxLabDoors.Checked;
            labConfig.AvoidExploreIfTreasure = checkBoxLabAvoidExplore.Checked;
            labConfig.AvoidPortal = checkBoxLabAvoidPortal.Checked;
            labConfig.RestartFailedBattle = checkBoxLabRestartFailedBattle.Checked;
            labConfig.StopOnMasterPainting = checkBoxLabStopOnMasterPainting.Checked;
            labConfig.RestartLab = checkBoxLabRestart.Checked;
            labConfig.UsePotions = checkBoxLabUsePotions.Checked;
            labConfig.WatchdogMinutes = (int)numericUpDownWatchdog.Value;
            labConfig.PaintingPriorityMap.Clear();
            foreach (ListViewItem item in listViewPaintings.Items)
            {
                labConfig.PaintingPriorityMap.Add(item.Tag.ToString(), int.Parse(item.Text));
            }
            labConfig.TreasureFilterMap.Clear();
            foreach (ListViewItem item in listViewTreasures.Items)
            {
                var value = new Lab.Configuration.TreasureFilter();
                value.Priority = int.Parse(item.Text);
                value.MaxKeys = int.Parse(item.SubItems[1].Text);
                labConfig.TreasureFilterMap.Add(item.Tag.ToString(), value);
            }

            // Save to .json
            File.WriteAllText(comboBoxLab.SelectedItem.ToString(), JsonConvert.SerializeObject(labConfig,Formatting.Indented));

            // Update machine
            controller.Machine.Config = labConfig;
            
            ColorConsole.WriteLine("Done!");

            // Restart warning
            if (lblRestart.Visible)
            {
                var ret = MessageBox.Show(this, "Restart now?", "Restart Required", MessageBoxButtons.YesNo);
                if (ret == DialogResult.Yes)
                {
                    Application.Restart();
                    Environment.Exit(0);
                }
            }

            this.Close();
        }

        private void BtnBrowseIn_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBoxProxyBlocklist.Text;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxProxyBlocklist.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void comboBoxLab_SelectedIndexChanged(object sender, EventArgs e)
        {
            labConfig = JsonConvert.DeserializeObject<Lab.Configuration>(File.ReadAllText(comboBoxLab.SelectedItem.ToString()));
            checkBoxLabDebug.Checked = labConfig.Debug;
            checkBoxLabDoors.Checked = labConfig.OpenDoors;
            checkBoxLabAvoidExplore.Checked = labConfig.AvoidExploreIfTreasure;
            checkBoxLabAvoidPortal.Checked = labConfig.AvoidPortal;
            checkBoxLabRestartFailedBattle.Checked = labConfig.RestartFailedBattle;
            checkBoxLabStopOnMasterPainting.Checked = labConfig.StopOnMasterPainting;
            checkBoxLabRestart.Checked = labConfig.RestartLab;
            checkBoxLabUsePotions.Checked = labConfig.UsePotions;
            numericUpDownWatchdog.Value = labConfig.WatchdogMinutes;

            listViewPaintings.Items.Clear();
            foreach (var item in labConfig.PaintingPriorityMap)
            {
                var newItem = new ListViewItem(item.Value.ToString());
                newItem.SubItems.Add(paintingLookup[item.Key]);
                newItem.Tag = item.Key;
                listViewPaintings.Items.Add(newItem);
            }

            listViewTreasures.Items.Clear();
            foreach (var item in labConfig.TreasureFilterMap)
            {
                var newItem = new ListViewItem(item.Value.Priority.ToString());
                newItem.SubItems.Add(item.Value.MaxKeys.ToString());
                newItem.SubItems.Add(treasureLookup[item.Key]);
                newItem.Checked = item.Value.Priority > 0;
                newItem.Tag = item.Key;
                listViewTreasures.Items.Add(newItem);
            }

        }

        private void listViewTreasures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewTreasures.SelectedItems.Count > 0)
                comboBoxKeys.SelectedIndex = comboBoxKeys.FindString(listViewTreasures.SelectedItems[0].SubItems[1].Text);
        }

        private void buttonPaintingUp_Click(object sender, EventArgs e)
        {
            if (listViewPaintings.SelectedItems.Count == 0) return;
            var p = int.Parse(listViewPaintings.SelectedItems[0].Text) - 1;
            if (p <= 0) p = 0;
            listViewPaintings.SelectedItems[0].Text = p.ToString();

            if (checkBoxSwap.Checked)
            {
                // Get previous item
                var currentIndex = listViewPaintings.SelectedItems[0].Index;
                if (currentIndex != 0)
                {
                    var prevItem = listViewPaintings.Items[currentIndex - 1];
                    if (int.Parse(prevItem.Text) == p)
                    {
                        prevItem.Text = (p + 1).ToString();
                    }
                }
            }

            listViewPaintings.Sort();
            listViewPaintings.Focus();
        }

        private void buttonPaintingDown_Click(object sender, EventArgs e)
        {
            if (listViewPaintings.SelectedItems.Count == 0) return;
            var p = int.Parse(listViewPaintings.SelectedItems[0].Text) + 1;
            if (p >= 255 ) p = 255;
            listViewPaintings.SelectedItems[0].Text = p.ToString();

            if (checkBoxSwap.Checked)
            {
                // Get next item
                var currentIndex = listViewPaintings.SelectedItems[0].Index;
                if (currentIndex < listViewPaintings.Items.Count -1)
                {
                    var nextItem = listViewPaintings.Items[currentIndex + 1];
                    if (int.Parse(nextItem.Text) == p)
                    {
                        nextItem.Text = (p - 1).ToString();
                    }
                }
            }

            listViewPaintings.Sort();
            listViewPaintings.Focus();
        }

        private void buttonTreasureUp_Click(object sender, EventArgs e)
        {
            if (listViewTreasures.SelectedItems.Count == 0) return;
            var p = int.Parse(listViewTreasures.SelectedItems[0].Text) - 1;
            if (p <= 1) p = 1;
            listViewTreasures.SelectedItems[0].Text = p.ToString();
            listViewTreasures.Sort();
            listViewTreasures.Focus();
        }

        private void buttonTreasureDown_Click(object sender, EventArgs e)
        {
            if (listViewTreasures.SelectedItems.Count == 0) return;
            var p = int.Parse(listViewTreasures.SelectedItems[0].Text) + 1;
            if (p >= 255) p = 255;
            listViewTreasures.SelectedItems[0].Text = p.ToString();
            listViewTreasures.SelectedItems[0].Checked = p > 0;
            listViewTreasures.Sort();
            listViewTreasures.Focus();
        }

        private void listViewTreasures_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Checked)
            {
                e.Item.Text = "1";
            }
            else
            {
                e.Item.Text = "0";
            }
            listViewTreasures.Sort();
        }

        private void comboBoxKeys_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (listViewTreasures.SelectedItems.Count == 0) return;
            listViewTreasures.SelectedItems[0].SubItems[1].Text = comboBoxKeys.Text;
            listViewTreasures.Sort();
            listViewTreasures.Focus();

        }

        public class PaintingSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                ListViewItem listviewX, listviewY;

                // Cast the objects to be compared to ListViewItem objects
                listviewX = (ListViewItem)x;
                listviewY = (ListViewItem)y;

                // Get values
                int priorityX = int.Parse(listviewX.Text);
                int priorityY = int.Parse(listviewY.Text);

                return priorityX.CompareTo(priorityY);

            }
        }

        public class TreasureSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                ListViewItem listviewX, listviewY;

                // Cast the objects to be compared to ListViewItem objects
                listviewX = (ListViewItem)x;
                listviewY = (ListViewItem)y;

                // Get values
                int priorityX = int.Parse(listviewX.Text);
                int priorityY = int.Parse(listviewY.Text);
                int keysX = int.Parse(listviewX.SubItems[1].Text);
                int keysY = int.Parse(listviewY.SubItems[1].Text);

                if (priorityX == 0) priorityX = 1000;
                if (priorityY == 0) priorityY = 1000;

                var cmp1 = priorityX.CompareTo(priorityY);
                if (cmp1 == 0)
                {
                    return keysX.CompareTo(keysY);
                } else
                {
                    return cmp1;
                }

            }
        }

        private void NeedsRestart_Changed(object sender, EventArgs e)
        {
            lblRestart.Visible = true;

        }

        private void checkBoxUpdates_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxPrerelease.Enabled = checkBoxUpdates.Checked;
        }

        private void buttonProxyRegenCert_Click(object sender, EventArgs e)
        {
            var ret = MessageBox.Show(this, "Regenerate the proxy HTTPS certificate?  (This will delete your existing certificate and you will have to install the new one on your device)", "Regenerate Certificate", MessageBoxButtons.OKCancel);
            if (ret == DialogResult.OK)
            {
                lblRestart.Visible = true;
                File.Delete("rootCert.pfx");
            }
        }
    }
}
