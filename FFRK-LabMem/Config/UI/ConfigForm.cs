using FFRK_LabMem.Machines;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using FFRK_Machines;
using Microsoft.VisualBasic;
using FFRK_LabMem.Services;
using System.Threading;
using System.Linq;

namespace FFRK_LabMem.Config.UI
{
    public partial class ConfigForm : Form
    {

        private LabTimings.TimingDictionary DefaultTimings = LabTimings.GetDefaultTimings();
        private bool treasuresTabLoaded = false;
        private bool treasuresLoaded = false;

        public ConfigHelper configHelper = null;
        public LabController controller = null;
        public LabConfiguration labConfig = new LabConfiguration();
        protected Scheduler scheduler = null;

        public ConfigForm()
        {
            InitializeComponent();
        }

        public static async void CreateAndShow(ConfigHelper configHelper, LabController controller)
        {

            bool initalState = controller.Enabled;
            var defaultScheduler = Scheduler.Default(controller);

            // Disable Lab
            if (controller.Enabled) controller.Disable();
            await defaultScheduler.Stop();

            // Show form
            var form = new ConfigForm
            {
                configHelper = configHelper,
                controller = controller,
                scheduler = defaultScheduler
            };
            form.ShowDialog();

            // Re-enable if needed
            if (initalState) controller.Enable();
            await defaultScheduler.Start();

        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            tabControl.SelectedIndex = listView1.SelectedItems[0].Index;
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            // Tab fakery
            listView1.Items[0].Selected = true;
            listView1.Items[0].Focused = true;
            tabControl.Top -= tabControl.ItemSize.Height;
            tabControl.Height += tabControl.ItemSize.Height;
            tabControl.Region = new Region(new RectangleF(tabPage1.Left, tabPage1.Top, tabPage1.Width, tabPage1.Height + tabControl.ItemSize.Height-20));

            // Debug values
            foreach (var item in ColorConsole.GetCategories())
            {
                comboBoxDebug.Items.Add(item);
            }
            comboBoxDebug.Items[0] = String.Format("<{0}>", ColorConsole.GetSelectedCategories(ColorConsole.DebugCategories));
            comboBoxDebug.SelectedIndex = 0;
            comboBoxDebug.Tag = ColorConsole.DebugCategories;

            // Values
            checkBoxTimestamps.Checked = configHelper.GetBool("console.timestamps", true);
            checkBoxUpdates.Checked = configHelper.GetBool("updates.checkForUpdates", true);
            checkBoxPrerelease.Checked = configHelper.GetBool("updates.includePrerelease", false);
            checkBoxDatalog.Checked = configHelper.GetBool("datalogger.enabled", false);
            numericUpDownScreenTop.Value = configHelper.GetInt("screen.topOffset", -1);
            numericUpDownScreenBottom.Value = configHelper.GetInt("screen.bottomOffset", -1);
            numericUpDownWatchdogHang.Value = configHelper.GetInt("lab.watchdogHangMinutes", 10);
            numericUpDownWatchdogCrash.Value = configHelper.GetInt("lab.watchdogCrashSeconds", 30);
            numericUpDownProxyPort.Value = configHelper.GetInt("proxy.port", 8081);
            checkBoxProxySecure.Checked = configHelper.GetBool("proxy.secure", true);
            textBoxProxyBlocklist.Text = configHelper.GetString("proxy.blocklist", "");
            checkBoxProxyAutoConfig.Checked = configHelper.GetBool("proxy.autoconfig", false);
            checkBoxProxyConnectionPool.Checked = configHelper.GetBool("proxy.connectionpooling", false);
            textBoxAdbPath.Text = configHelper.GetString("adb.path", "adb.exe");
            comboBoxAdbHost.DataSource = Lookups.AdbHosts;
            comboBoxAdbHost.DisplayMember = "Display";
            comboBoxAdbHost.ValueMember = "Value";
            comboBoxAdbHost.SelectedValue = configHelper.GetString("adb.host", "127.0.0.1:7555");
            if (comboBoxAdbHost.SelectedItem == null) comboBoxAdbHost.Text = configHelper.GetString("adb.host", "127.0.0.1:7555");

            // Load lab .json
            LoadConfigs();

            // Timings
            LoadTimings();

            // Schedules
            foreach (var schedule in scheduler.Schedules)
            {
                AddScheduleListViewItem(schedule);
            }

            // Counters
            Data.Counters.OnUpdated += LoadCounters;
            LoadCounters(sender, e);

            // List sorting
            listViewPaintings.ListViewItemSorter = new Sorters.PaintingSorter();
            listViewTreasures.ListViewItemSorter = new Sorters.TreasureSorter();

            // Hide restart warning
            lblRestart.Visible = false;

        }

        private void ConfigForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Data.Counters.OnUpdated -= LoadCounters;
        }

        private void LoadTimings()
        {
            dataGridView1.Rows.Clear();
            foreach (KeyValuePair<string, LabTimings.Timing> item in LabTimings.Timings)
            {
                dataGridView1.Rows.Add(item.Key, item.Value.Delay, item.Value.Jitter);
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var key = row.Cells[0].Value.ToString();
                if (Lookups.Timings.ContainsKey(key)) row.Cells[0].ToolTipText = Lookups.Timings[key];
            }
        }

        private void LoadCounters(object sender, EventArgs e)
        {
            listViewCounters.Items.Clear();

            // Counters
            var sessionCounters = Data.Counters.Default.CounterSets["Session"].Counters.ToList();
            foreach (var item in sessionCounters)
            {
                var newItem = new ListViewItem();
                newItem.Group = listViewCounters.Groups["Counters"];
                if (Lookups.Counters.ContainsKey(item.Key))
                {
                    newItem.Text = Lookups.Counters[item.Key];
                }
                else
                {
                    newItem.Text = item.Key;
                }
                newItem.SubItems.Add(item.Value.ToString());
                newItem.SubItems.Add(Data.Counters.Default.CounterSets["CurrentLab"].Counters[item.Key].ToString());
                newItem.SubItems.Add(Data.Counters.Default.CounterSets["Total"].Counters[item.Key].ToString());
                listViewCounters.Items.Add(newItem);  
            }

            // Runtime
            string runtimeFormat = @"d\.hh\:mm\:ss";
            var sessionRuntime = Data.Counters.Default.CounterSets["Session"].Runtime.ToList();
            foreach (var item in sessionRuntime)
            {
                var newItem = new ListViewItem();
                newItem.Group = listViewCounters.Groups["Runtime"];
                newItem.Text = item.Key;
                newItem.SubItems.Add(item.Value.ToString(runtimeFormat));
                newItem.SubItems.Add(Data.Counters.Default.CounterSets["CurrentLab"].Runtime[item.Key].ToString(runtimeFormat));
                newItem.SubItems.Add(Data.Counters.Default.CounterSets["Total"].Runtime[item.Key].ToString(runtimeFormat));
                listViewCounters.Items.Add(newItem);
            }

            // Merge HE Keys
            var heKeys = Data.Counters.Default.CounterSets.Values.SelectMany(s => s.HeroEquipment.Keys).Distinct().OrderBy(s => s);
            foreach (var item in heKeys)
            {
                var newItem = new ListViewItem();
                newItem.Group = listViewCounters.Groups["HE"];
                newItem.Text = item;
                if (Data.Counters.Default.CounterSets["Session"].HeroEquipment.ContainsKey(item))
                {
                    newItem.SubItems.Add(Data.Counters.Default.CounterSets["Session"].HeroEquipment[item].ToString());
                } else
                {
                    newItem.SubItems.Add("-");
                }
                if (Data.Counters.Default.CounterSets["CurrentLab"].HeroEquipment.ContainsKey(item))
                {
                    newItem.SubItems.Add(Data.Counters.Default.CounterSets["CurrentLab"].HeroEquipment[item].ToString());
                }
                else
                {
                    newItem.SubItems.Add("-");
                }
                newItem.SubItems.Add("-");
                listViewCounters.Items.Add(newItem);
            }

            // Merge Drops
            var dropKeys = Data.Counters.Default.CounterSets.Values.SelectMany(s => s.Drops.Keys).Distinct().OrderBy(s => s);
            foreach (var item in dropKeys)
            {
                var newItem = new ListViewItem();
                newItem.Group = listViewCounters.Groups["Drops"];
                newItem.Text = item;
                if (Data.Counters.Default.CounterSets["Session"].Drops.ContainsKey(item))
                {
                    newItem.SubItems.Add(Data.Counters.Default.CounterSets["Session"].Drops[item].ToString());
                }
                else
                {
                    newItem.SubItems.Add("-");
                }
                if (Data.Counters.Default.CounterSets["CurrentLab"].Drops.ContainsKey(item))
                {
                    newItem.SubItems.Add(Data.Counters.Default.CounterSets["CurrentLab"].Drops[item].ToString());
                }
                else
                {
                    newItem.SubItems.Add("-");
                }
                newItem.SubItems.Add("-");
                listViewCounters.Items.Add(newItem);
            }

        }

        private void LoadConfigs()
        {
            var targetConfig = configHelper.GetString("lab.configFile", "config/lab.balanced.json").ToLower();
            comboBoxLab.Items.Clear();
            foreach (var item in ConfigFile.GetFiles())
            {
                comboBoxLab.Items.Add(item);
                if (item.Path.ToLower().EndsWith(targetConfig)) comboBoxLab.SelectedIndex = comboBoxLab.Items.Count - 1;
            }
            if (comboBoxLab.SelectedItem == null) comboBoxLab.SelectedIndex = 0;
        }

        private async void ButtonOk_Click(object sender, EventArgs e)
        {
            ColorConsole.Write("Saving configuration... ");

            // General
            configHelper.SetValue("console.timestamps", checkBoxTimestamps.Checked);
            configHelper.SetValue("console.debugCategories", (short)comboBoxDebug.Tag);
            ColorConsole.DebugCategories = (ColorConsole.DebugCategory)comboBoxDebug.Tag;
            configHelper.SetValue("updates.checkForUpdates", checkBoxUpdates.Checked);
            configHelper.SetValue("updates.includePrerelease", checkBoxPrerelease.Checked);
            configHelper.SetValue("datalogger.enabled", checkBoxDatalog.Checked);
            configHelper.SetValue("screen.topOffset", numericUpDownScreenTop.Value);
            configHelper.SetValue("screen.bottomOffset", numericUpDownScreenBottom.Value);
            configHelper.SetValue("proxy.port", numericUpDownProxyPort.Value);
            configHelper.SetValue("proxy.secure", checkBoxProxySecure.Checked);
            configHelper.SetValue("proxy.blocklist", textBoxProxyBlocklist.Text);
            configHelper.SetValue("proxy.autoconfig", checkBoxProxyAutoConfig.Checked);
            configHelper.SetValue("proxy.connectionpooling", checkBoxProxyConnectionPool.Checked);
            configHelper.SetValue("adb.path", textBoxAdbPath.Text);
            configHelper.SetValue("adb.host", (comboBoxAdbHost.SelectedItem != null) ? ((AdbHostItem)comboBoxAdbHost.SelectedItem).Value : comboBoxAdbHost.Text);
            configHelper.SetValue("lab.configFile", ConfigFile.FromObject(comboBoxLab.SelectedItem).Path);
            configHelper.SetValue("lab.watchdogHangMinutes", (int)numericUpDownWatchdogHang.Value);
            configHelper.SetValue("lab.watchdogCrashSeconds", (int)numericUpDownWatchdogCrash.Value);

            // Lab
            labConfig.OpenDoors = checkBoxLabDoors.Checked;
            labConfig.AvoidExploreIfTreasure = checkBoxLabAvoidExplore.Checked;
            labConfig.AvoidPortal = checkBoxLabAvoidPortal.Checked;
            labConfig.RestartFailedBattle = checkBoxLabRestartFailedBattle.Checked;
            labConfig.StopOnMasterPainting = checkBoxLabStopOnMasterPainting.Checked;
            labConfig.RestartLab = checkBoxLabRestart.Checked;
            labConfig.UsePotions = checkBoxLabUsePotions.Checked;
            labConfig.UseLetheTears = checkBoxLabUseLetheTears.Checked;
            labConfig.LetheTearsFatigue = (int)numericUpDownFatigue.Value;
            labConfig.LetheTearsSlot = 0;
            if (checkBoxSlot1.Checked) labConfig.LetheTearsSlot |= (1 << 4);
            if (checkBoxSlot2.Checked) labConfig.LetheTearsSlot |= (1 << 3);
            if (checkBoxSlot3.Checked) labConfig.LetheTearsSlot |= (1 << 2);
            if (checkBoxSlot4.Checked) labConfig.LetheTearsSlot |= (1 << 1);
            if (checkBoxSlot5.Checked) labConfig.LetheTearsSlot |= (1 << 0);
            labConfig.UseTeleportStoneOnMasterPainting = checkBoxLabUseTeleport.Checked;
            labConfig.ScreenshotRadiantPainting = checkBoxLabScreenshotRadiant.Checked;
            labConfig.EnemyBlocklistAvoidOptionOverride = checkBoxLabBlockListOverride.Checked;
            labConfig.AutoStart = checkBoxLabAutoStart.Checked;

            // Paintings
            labConfig.PaintingPriorityMap.Clear();
            foreach (ListViewItem item in listViewPaintings.Items)
            {
                labConfig.PaintingPriorityMap.Add(item.Tag.ToString(), int.Parse(item.Text));
            }

            // Treasures
            labConfig.TreasureFilterMap.Clear();
            foreach (ListViewItem item in listViewTreasures.Items)
            {
                var value = new LabConfiguration.TreasureFilter();
                value.Priority = int.Parse(item.Text);
                value.MaxKeys = int.Parse(item.SubItems[1].Text);
                labConfig.TreasureFilterMap.Add(item.Tag.ToString(), value);
            }

            // Enemy blocklist
            labConfig.EnemyBlocklist.Clear();
            for (int i = 0; i < checkedListBoxBlocklist.Items.Count; i++)
            {
                LabConfiguration.EnemyBlocklistEntry item = (LabConfiguration.EnemyBlocklistEntry)checkedListBoxBlocklist.Items[i];
                item.Enabled = checkedListBoxBlocklist.GetItemChecked(i);
                labConfig.EnemyBlocklist.Add(item);
            }

            // Save Lab to .json
            await labConfig.Save(ConfigFile.FromObject(comboBoxLab.SelectedItem).Path);

            // Save Timings
            LabTimings.Timings.Clear();
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                LabTimings.Timings.Add(item.Cells[0].Value.ToString(), new LabTimings.Timing()
                {
                    Delay = int.Parse(item.Cells[1].Value.ToString()),
                    Jitter = int.Parse(item.Cells[2].Value.ToString())
                });
            }
            await LabTimings.Save();

            // Save Schedule
            scheduler.Schedules.Clear();
            foreach (ListViewItem item in listViewSchedule.Items)
            {
                var schedule = (Scheduler.Schedule)item.Tag;
                scheduler.Schedules.Add(schedule);
            }
            await scheduler.Save();
            
            ColorConsole.WriteLine("Done!");

            // Update machine
            controller.Machine.Config = labConfig;
            controller.Machine.Watchdog.Update((int)numericUpDownWatchdogHang.Value, (int)numericUpDownWatchdogCrash.Value);

            // Restart warning
            if (lblRestart.Visible)
            {
                var ret = MessageBox.Show(this, "Changes require the app to restart, restart now?", 
                    "Restart Required", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning, 
                    MessageBoxDefaultButton.Button1);

                if (ret == DialogResult.Yes)
                {
                    Application.Restart();
                    Environment.Exit(0);
                }
            }

            this.Close();
        }

        private async void ComboBoxLab_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Options
            labConfig = await LabConfiguration.Load<LabConfiguration>(ConfigFile.FromObject(comboBoxLab.SelectedItem).Path);
            checkBoxLabDoors.Checked = labConfig.OpenDoors;
            checkBoxLabAvoidExplore.Checked = labConfig.AvoidExploreIfTreasure;
            checkBoxLabAvoidPortal.Checked = labConfig.AvoidPortal;
            checkBoxLabRestartFailedBattle.Checked = labConfig.RestartFailedBattle;
            checkBoxLabStopOnMasterPainting.Checked = labConfig.StopOnMasterPainting;
            CheckBoxLabStopOnMasterPainting_CheckedChanged(sender, e);
            checkBoxLabRestart.Checked = labConfig.RestartLab;
            CheckBoxLabRestart_CheckedChanged(sender, e);
            checkBoxLabUsePotions.Checked = labConfig.UsePotions;
            checkBoxLabUseLetheTears.Checked = labConfig.UseLetheTears;
            CheckBoxLabUseLetheTears_CheckedChanged(sender, e);
            numericUpDownFatigue.Value = labConfig.LetheTearsFatigue;
            checkBoxSlot1.Checked = ((labConfig.LetheTearsSlot >> 4) & 1) != 0;
            checkBoxSlot2.Checked = ((labConfig.LetheTearsSlot >> 3) & 1) != 0;
            checkBoxSlot3.Checked = ((labConfig.LetheTearsSlot >> 2) & 1) != 0;
            checkBoxSlot4.Checked = ((labConfig.LetheTearsSlot >> 1) & 1) != 0;
            checkBoxSlot5.Checked = ((labConfig.LetheTearsSlot >> 0) & 1) != 0;
            checkBoxLabUseTeleport.Checked = labConfig.UseTeleportStoneOnMasterPainting;
            checkBoxLabScreenshotRadiant.Checked = labConfig.ScreenshotRadiantPainting;
            checkBoxLabBlockListOverride.Checked = labConfig.EnemyBlocklistAvoidOptionOverride;
            checkBoxLabAutoStart.Checked = labConfig.AutoStart;
            
            // Painting priorities
            listViewPaintings.Items.Clear();
            foreach (var item in labConfig.PaintingPriorityMap)
            {
                var newItem = new ListViewItem(item.Value.ToString());
                newItem.SubItems.Add(Lookups.Paintings[item.Key]);
                newItem.Tag = item.Key;
                listViewPaintings.Items.Add(newItem);
            }

            // Treasure priorities
            treasuresLoaded = false;
            listViewTreasures.Items.Clear();
            foreach (var item in labConfig.TreasureFilterMap)
            {
                var newItem = new ListViewItem();
                newItem.Text = item.Value.Priority.ToString();
                newItem.SubItems.Add(item.Value.MaxKeys.ToString());
                newItem.SubItems.Add(Lookups.Treasures[item.Key]);
                newItem.Checked = item.Value.Priority > 0;
                newItem.Tag = item.Key;
                listViewTreasures.Items.Add(newItem);
            }
            treasuresLoaded = true;

            // Enemy blocklist
            checkedListBoxBlocklist.Items.Clear();
            foreach (LabConfiguration.EnemyBlocklistEntry entry in labConfig.EnemyBlocklist)
            {
                checkedListBoxBlocklist.Items.Add(entry, entry.Enabled);
            }
            buttonRemoveBlocklist.Enabled = checkedListBoxBlocklist.Items.Count > 0;

        }

        private void ListViewTreasures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewTreasures.SelectedItems.Count == 0) return;
            comboBoxKeys.SelectedIndex = comboBoxKeys.FindString(listViewTreasures.SelectedItems[0].SubItems[1].Text);
            buttonTreasureUp.Enabled = listViewTreasures.SelectedItems[0].Checked;
            buttonTreasureDown.Enabled = listViewTreasures.SelectedItems[0].Checked;

        }

        private void ButtonPaintingUp_Click(object sender, EventArgs e)
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

        private void ButtonPaintingDown_Click(object sender, EventArgs e)
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

        private void ButtonTreasureUp_Click(object sender, EventArgs e)
        {
            if (listViewTreasures.SelectedItems.Count == 0) return;
            var p = int.Parse(listViewTreasures.SelectedItems[0].Text) - 1;
            if (p < 0) return;
            if (p <= 1) p = 1;
            listViewTreasures.SelectedItems[0].Text = p.ToString();
            listViewTreasures.Sort();
            listViewTreasures.Focus();
        }

        private void ButtonTreasureDown_Click(object sender, EventArgs e)
        {
            if (listViewTreasures.SelectedItems.Count == 0) return;
            var p = int.Parse(listViewTreasures.SelectedItems[0].Text) + 1;
            if (p >= 255) p = 255;
            listViewTreasures.SelectedItems[0].Text = p.ToString();
            listViewTreasures.SelectedItems[0].Checked = p > 0;
            listViewTreasures.Sort();
            listViewTreasures.Focus();
        }

        private void ListViewTreasures_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!treasuresLoaded || !treasuresTabLoaded) return;
            if (e.Item.Checked)
            {
                e.Item.Text = "1";
            }
            else
            {
                e.Item.Text = "0";
            }
            buttonTreasureUp.Enabled = e.Item.Checked;
            buttonTreasureDown.Enabled = e.Item.Checked;
            listViewTreasures.Sort();
        }

        private void ComboBoxKeys_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (listViewTreasures.SelectedItems.Count == 0) return;
            listViewTreasures.SelectedItems[0].SubItems[1].Text = comboBoxKeys.Text;
            listViewTreasures.Sort();
            listViewTreasures.Focus();

        }

        private void NeedsRestart_Changed(object sender, EventArgs e)
        {

            var changed = (
                checkBoxTimestamps.Checked != configHelper.GetBool("console.timestamps", true) |
                (short)comboBoxDebug.Tag != configHelper.GetShort("console.debugCategories", 0) |
                checkBoxDatalog.Checked != configHelper.GetBool("datalogger.enabled", false) |
                numericUpDownProxyPort.Value != configHelper.GetInt("proxy.port", 8081) |
                checkBoxProxySecure.Checked != configHelper.GetBool("proxy.secure", true) |
                textBoxProxyBlocklist.Text != configHelper.GetString("proxy.blocklist", "") |
                checkBoxProxyAutoConfig.Checked != configHelper.GetBool("proxy.autoconfig", false) |
                checkBoxProxyConnectionPool.Checked != configHelper.GetBool("proxy.connectionpooling", false) |
                textBoxAdbPath.Text != configHelper.GetString("adb.path", "adb.exe") |
                ((comboBoxAdbHost.SelectedItem != null) ? ((AdbHostItem)comboBoxAdbHost.SelectedItem).Value : comboBoxAdbHost.Text) != configHelper.GetString("adb.host", "127.0.0.1:7555")
            );

            lblRestart.Visible = changed;

        }

        private void CheckBoxUpdates_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxPrerelease.Enabled = checkBoxUpdates.Checked;
        }

        private void ButtonProxyRegenCert_Click(object sender, EventArgs e)
        {
            var ret = MessageBox.Show(this, 
                "Regenerate the proxy HTTPS certificate?  (This will delete your existing certificate and you will have to install the new one on your device)", 
                "Regenerate Certificate", 
                MessageBoxButtons.OKCancel, 
                MessageBoxIcon.Warning, 
                MessageBoxDefaultButton.Button2);

            if (ret == DialogResult.OK)
            {
                lblRestart.Visible = true;
                File.Delete("rootCert.pfx");
            }
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage7) treasuresTabLoaded = true;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Services.Clipboard.CopyProxyBypassToClipboard();
            MessageBox.Show(this, "Copied!","", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void ButtonProxyReset_Click(object sender, EventArgs e)
        {
            var ret = MessageBox.Show(this,
                "Remove system proxy settings?  (Requires device restart)",
                "Proxy Auto-Configure",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (ret == DialogResult.OK)
            {
                await controller.Adb.SetProxySettings(0, CancellationToken.None);
            }
        }

        private void CheckBoxLabRestart_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxLabUsePotions.Enabled = checkBoxLabRestart.Checked;
        }

        private void CheckBoxLabUseLetheTears_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownFatigue.Enabled = checkBoxLabUseLetheTears.Checked;
            checkBoxSlot1.Enabled = checkBoxLabUseLetheTears.Checked;
            checkBoxSlot2.Enabled = checkBoxLabUseLetheTears.Checked;
            checkBoxSlot3.Enabled = checkBoxLabUseLetheTears.Checked;
            checkBoxSlot4.Enabled = checkBoxLabUseLetheTears.Checked;
            checkBoxSlot5.Enabled = checkBoxLabUseLetheTears.Checked;

        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

            if (e.ColumnIndex >= 1)
            {
                int i;
                if (!int.TryParse(Convert.ToString(e.FormattedValue), out i))
                {
                    e.Cancel = true;
                }
                else
                {
                    if (i < 0) e.Cancel = true;
                }
            }

        }
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var row = dataGridView1.Rows[e.RowIndex];
            var key = row.Cells[0].Value.ToString();

            // Featured timings
            if (Lookups.Timings.ContainsKey(key)) e.CellStyle.BackColor = SystemColors.Menu;

            // Changed timings
            int i;
            if (e.ColumnIndex == 1 && int.TryParse(row.Cells[1].Value.ToString(), out i))
            {
                if (DefaultTimings[key].Delay != i) e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
            }
            if (e.ColumnIndex == 2 && int.TryParse(row.Cells[2].Value.ToString(), out i))
            {
                if (DefaultTimings[key].Jitter != i) e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
            }

        }

        private async void ButtonTimingDefaults_Click(object sender, EventArgs e)
        {

            var ret = MessageBox.Show(this, "Are you sure you want to reset all timings to the defaults?",
                   "Reset Timings",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Warning,
                   MessageBoxDefaultButton.Button1);

            if (ret == DialogResult.Yes)
            {
                await LabTimings.ResetToDefaults();
                LoadTimings();
            }
        }

        private void CheckBoxLabStopOnMasterPainting_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxLabUseTeleport.Enabled = !checkBoxLabStopOnMasterPainting.Checked;
        }

        private async void ButtonCheckForUpdates_Click(object sender, EventArgs e)
        {

            if (await Services.Updates.Check(checkBoxPrerelease.Checked))
            {
                var ret = MessageBox.Show(this, "There is a new version available, download and install it now?", "Check For Updates", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            
                if (ret == DialogResult.Yes)
                {
                    Services.Updates.DownloadInstallerAndRun(checkBoxPrerelease.Checked, false);
                    this.Close();
                }
            
            } else
            {
                MessageBox.Show(this, "This version is up-to-date!", "Check For Updates", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void ButtonAddBlocklist_Click(object sender, EventArgs e)
        {
            var input = Interaction.InputBox("Enter enemy name (does not have to inlude Labyrinth)", "Add Blocklist Entry");
            if (!String.IsNullOrEmpty(input))
            {
                var newItem = new LabConfiguration.EnemyBlocklistEntry() { Name = input, Enabled = true };
                checkedListBoxBlocklist.Items.Add(newItem, true);
                buttonRemoveBlocklist.Enabled = true;
            }
        }

        private void ButtonRemoveBlocklist_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(this, "Are you sure?", "Remove Blocklist Entry", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                checkedListBoxBlocklist.Items.Remove(checkedListBoxBlocklist.SelectedItem);
            }
        }

        private void ButtonLabConfigurations_Click(object sender, EventArgs e)
        {
            ConfigListForm.CreateAndShow(configHelper.GetString("lab.configFile", "config/lab.balanced.json").ToLower());
            LoadConfigs();
        }

        // Class variable to keep track of which row is currently selected:
        int hoveredIndex = -1;
        private void checkedListBoxBlocklist_MouseMove(object sender, MouseEventArgs e)
        {
            // See which row is currently under the mouse:
            int newHoveredIndex = checkedListBoxBlocklist.IndexFromPoint(e.Location);

            // If the row has changed since last moving the mouse:
            if (hoveredIndex != newHoveredIndex)
            {
                // Change the variable for the next time we move the mouse:
                hoveredIndex = newHoveredIndex;

                // If over a row showing data (rather than blank space):
                if (hoveredIndex > -1)
                {
                    //Set tooltip text for the row now under the mouse:
                    toolTip1.Active = false;
                    var name = ((LabConfiguration.EnemyBlocklistEntry)checkedListBoxBlocklist.Items[hoveredIndex]).Name;
                    if (Lookups.Blocklist.ContainsKey(name))
                    {
                        toolTip1.SetToolTip(checkedListBoxBlocklist, Lookups.Blocklist[name]);
                    } else
                    {
                        toolTip1.SetToolTip(checkedListBoxBlocklist, "User-defined");
                    }
                    toolTip1.Active = true;
                }
            }
        }

        private void buttonScheduleAdd_Click(object sender, EventArgs e)
        {
            var item = ConfigScheduleForm.EditSchedule(null);
            if (item!=null) AddScheduleListViewItem(item);
        }

        private void listViewSchedule_DoubleClick(object sender, EventArgs e)
        {
            var item = listViewSchedule.SelectedItems[0];
            if (item == null) return;
            var newItem = ConfigScheduleForm.EditSchedule((Scheduler.Schedule)item.Tag);
            if (newItem != null)
            {
                listViewSchedule.Items.Remove(item);
                AddScheduleListViewItem(newItem);
            }
        }

        private void buttonScheduleDelete_Click(object sender, EventArgs e)
        {
            var item = listViewSchedule.SelectedItems[0];
            if (item == null) return;
            var result = MessageBox.Show(this, "Are you sure?", "Remove Schedule", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                listViewSchedule.Items.Remove(item);
            }
        }

        private void AddScheduleListViewItem(Scheduler.Schedule schedule)
        {
            var newItem = new ListViewItem(schedule.Name);
            if (schedule.EnableEnabled)
            {
                if (String.IsNullOrEmpty(schedule.EnableCronTab))
                {
                    newItem.SubItems.Add(schedule.EnableDate.ToString());
                } else
                {
                    newItem.SubItems.Add(schedule.EnableDate.ToString("hh:mm tt"));
                }
            } else
            {
                newItem.SubItems.Add("-");
            }
            if (schedule.DisableEnabled)
            {
                if (String.IsNullOrEmpty(schedule.DisableCronTab))
                {
                    newItem.SubItems.Add(schedule.DisableDate.ToString());
                }
                else
                {
                    newItem.SubItems.Add(schedule.DisableDate.ToString("hh:mm tt"));
                }
            }
            else
            {
                newItem.SubItems.Add("-");
            }
            newItem.SubItems.Add(schedule.EnableCronTab);
            newItem.Tag = schedule;
            listViewSchedule.Items.Add(newItem);
        }

        private async void ButtonCountersReset_Click(object sender, EventArgs e)
        {
            String tag = (string)((Button)sender).Tag;
            var result = MessageBox.Show(this, $"Are you sure you want to reset {tag} counters?", "Reset Counters", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                var target = tag.Equals("All") ? null : tag;
                await Data.Counters.Reset(target);
            }
        }

        private void ComboBoxDebug_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDebug.SelectedIndex == 0) return;
            ColorConsole.DebugCategory t = (ColorConsole.DebugCategory)comboBoxDebug.Tag;
            ColorConsole.DebugCategory target = (ColorConsole.DebugCategory)comboBoxDebug.SelectedItem;
            t ^= target;
            comboBoxDebug.Tag = t;
            comboBoxDebug.SelectedIndex = 0;
            comboBoxDebug.Items[0] = String.Format("<{0}>", ColorConsole.GetSelectedCategories(t));
        }
    }
}
