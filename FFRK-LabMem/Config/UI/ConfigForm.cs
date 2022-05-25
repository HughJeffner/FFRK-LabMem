using FFRK_LabMem.Machines;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using FFRK_Machines;
using FFRK_LabMem.Services;
using System.Threading;
using System.Linq;
using FFRK_LabMem.Data;
using FFRK_LabMem.Data.UI;
using FFRK_Machines.Services.Notifications;
using System.Threading.Tasks;
using FFRK_Machines.Threading;
using FFRK_Machines.Services.Adb;

namespace FFRK_LabMem.Config.UI
{
    public partial class ConfigForm : Form
    {
        public static bool IsLoaded { get; set; } = false;

        private bool treasuresTabLoaded = false;
        private bool treasuresLoaded = false;
        private ConfigHelper configHelper = null;
        private LabController controller = null;
        private LabConfiguration labConfig = new LabConfiguration();
        private int initalTabIndex = 0;
        protected Scheduler scheduler = null;
        private Notifications.EventList notificationEvents = null;
        private Notifications.EventType? selectedNotificationEvent = null;

        public ConfigForm()
        {
            InitializeComponent();
        }

        public static void CreateAndShow(ConfigHelper configHelper, LabController controller, int initalTabIndex = 0)
        {
            // Show form
            if (IsLoaded == false)
            {
                IsLoaded = true;
                
                Task mytask = Utility.StartSTATask(async () =>
                {
                    var form = new ConfigForm
                    {
                        configHelper = configHelper,
                        controller = controller,
                        scheduler = Scheduler.Default,
                        initalTabIndex = initalTabIndex
                    };
                    form.ShowDialog();
                    await Task.CompletedTask;

                });
            }

        }

        private void ConfigForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            IsLoaded = false;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            tabControl.SelectedIndex = listView1.SelectedItems[0].Index;
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {

            // Title
            this.Text += " " + Updates.GetVersionCode();
            if (this.controller.Machine == null) this.Text += " (Config Only)";

            // Tab fakery
            listView1.Items[0].Selected = true;
            listView1.Items[0].Focused = true;
            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.ItemSize = new Size(0, 1);
            tabControl.SizeMode = TabSizeMode.Fixed;
            foreach (TabPage tab in tabControl.TabPages)
            {
                tab.Text = "";
            }

            // Debug values
            foreach (var item in ColorConsole.GetCategories())
            {
                ToolStripMenuItem toolStripItem = new ToolStripMenuItem(item.ToString());
                toolStripItem.Tag = item;
                toolStripItem.Checked = ColorConsole.DebugCategories.HasFlag(item);
                toolStripItem.CheckOnClick = true;
                toolStripItem.Click += DebugToolStripMenuItem_SelectedIndexChanged;
                contextMenuStrip1.Items.Add(toolStripItem);
            }
            buttonDebug.Text = String.Format("{0}", ColorConsole.GetSelectedCategories(ColorConsole.DebugCategories));
            buttonDebug.Tag = ColorConsole.DebugCategories;

            // Values
            checkBoxTimestamps.Checked = configHelper.GetBool("console.timestamps", true);
            checkBoxLogging.Checked = configHelper.GetBool("console.logging", true);
            checkBoxUpdates.Checked = configHelper.GetBool("updates.checkForUpdates", true);
            checkBoxPrerelease.Checked = configHelper.GetBool("updates.includePrerelease", false);
            checkBoxDatalog.Checked = configHelper.GetBool("datalogger.enabled", false);
            numericUpDownScreenTop.Value = configHelper.GetInt("screen.topOffset", -1);
            numericUpDownScreenBottom.Value = configHelper.GetInt("screen.bottomOffset", -1);
            numericUpDownWatchdogHang.Value = configHelper.GetInt("lab.watchdogHangSeconds", 120);
            numericUpDownWatchdogHangWarning.Value = configHelper.GetInt("lab.watchdogHangWarningSeconds", 60);
            numericUpDownHangLoopThreshold.Value = configHelper.GetInt("lab.watchdogHangWarningLoopThreshold", 60);
            numericUpDownWatchdogBattle.Value = configHelper.GetInt("lab.watchdogBattleMinutes", 15);
            numericUpDownWatchdogCrash.Value = configHelper.GetInt("lab.watchdogCrashSeconds", 30);
            numericUpDownRestartLoopThreshold.Value = configHelper.GetInt("lab.watchdogLoopDetectionThreshold", 6);
            numericUpDownRestartLoopWindow.Value = configHelper.GetInt("lab.watchdogLoopDetectionWindowMinutes", 60);
            numericUpDownRestartMaxRetries.Value = configHelper.GetInt("lab.watchdogMaxRetries", 5);
            numericUpDownBattleMaxRetries.Value = configHelper.GetInt("lab.watchdogBattleMaxRetries", 5);
            checkBoxWatchdogScreenshot.Checked = configHelper.GetBool("lab.watchdogHangScreenshot", false);
            numericUpDownProxyPort.Value = configHelper.GetInt("proxy.port", 8081);
            checkBoxProxySecure.Checked = configHelper.GetBool("proxy.secure", true);
            textBoxProxyBlocklist.Text = configHelper.GetString("proxy.blocklist", "");
            checkBoxProxyAutoConfig.Checked = configHelper.GetBool("proxy.autoconfig", false);
            checkBoxProxyConnectionPool.Checked = configHelper.GetBool("proxy.connectionPooling", false);
            textBoxAdbPath.Text = configHelper.GetString("adb.path", "adb.exe");
            comboBoxAdbHost.DataSource = Lookups.AdbHosts;
            comboBoxAdbHost.DisplayMember = "Display";
            comboBoxAdbHost.ValueMember = "Value";
            comboBoxAdbHost.SelectedValue = configHelper.GetString("adb.host", "127.0.0.1:7555");
            if (comboBoxAdbHost.SelectedItem == null) comboBoxAdbHost.Text = configHelper.GetString("adb.host", "127.0.0.1:7555");
            checkBoxAdbClose.Checked = configHelper.GetBool("adb.closeOnExit", false);
            comboBoxCapture.SelectedIndex = configHelper.GetInt("adb.capture", 1);
            trackBarCaptureRate.Value = configHelper.GetInt("adb.captureRate", 200) / 10;
            trackBarFindPrecision.Value = 10 - (int)(configHelper.GetDouble("adb.findPrecision", 0.5) * 10);
            trackBarFindAccuracy.Value = configHelper.GetInt("adb.FindAccuracy", 0);
            comboBoxInput.SelectedIndex = configHelper.GetInt("adb.input", 1);
            trackBarTapDelay.Value = configHelper.GetInt("adb.tapDelay", 30) / 10;
            trackBarTapDuration.Value = configHelper.GetInt("adb.tapDuration", 0) / 10;
            numericUpDownTapPressure.Value = configHelper.GetInt("adb.tapPressure", 50);
            checkBoxCountersLogDropsTotal.Checked = configHelper.GetBool("counters.logDropsToTotal", false);
            numericUpDownCountersRarity.Value = configHelper.GetInt("counters.materialsRarityFilter", 6);
            textBoxLogFolder.Text = configHelper.GetString("console.logFolder", "");
            textBoxScreenshotFolder.Text = configHelper.GetString("adb.screenshotFolder", "");

            // Load lab .json
            LoadConfigs();

            // Timings
            LoadTimings();

            // Schedules
            foreach (var schedule in scheduler.Schedules)
            {
                AddScheduleListViewItem(schedule);
            }

            // Drop categories
            LoadDropCategories();

            // Notifications
            LoadNotifications();

            // List sorting
            listViewPaintings.ListViewItemSorter = new Sorters.PaintingSorter();
            listViewTreasures.ListViewItemSorter = new Sorters.TreasureSorter();

            // Hide restart warning
            lblRestart.Visible = false;

            // Inital tab
            listView1.Items[initalTabIndex].Selected = true;

        }

        private async void LoadNotifications()
        {

            notificationEvents = await Notifications.GetEvents();

            comboBoxNotificationEvents.Items.Clear();
            foreach (var item in Enum.GetValues(typeof(Notifications.EventType)).Cast<Notifications.EventType>())
            {
                comboBoxNotificationEvents.Items.Add(Lookups.NotificationEvents[item]);
            }
            comboBoxNotificationEvents.SelectedIndex = 0;
            selectedNotificationEvent = Notifications.EventType.LAB_COMPLETED;
        }

        private void LoadDropCategories()
        {
            checkedListBoxDropCategories.Items.Clear();
            foreach (var item in Enum.GetValues(typeof(Counters.DropCategory)).Cast<Counters.DropCategory>())
            {
                if (item != Counters.DropCategory.UNKNOWN)
                    checkedListBoxDropCategories.Items.Add(Lookups.DropCategories[item], Counters.Default.DropCategories.HasFlag((Enum)item));
            }
            buttonShowCounters.Visible = !CountersForm.IsLoaded;

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

            // Console
            configHelper.SetValue("console.timestamps", checkBoxTimestamps.Checked);
            configHelper.SetValue("console.logging", checkBoxLogging.Checked);
            configHelper.SetValue("console.debugCategories", (short)buttonDebug.Tag);
            configHelper.SetValue("console.logFolder", textBoxLogFolder.Text);
            ColorConsole.Timestamps = checkBoxTimestamps.Checked;
            ColorConsole.LogBuffer.Enabled = checkBoxLogging.Checked;
            ColorConsole.LogBuffer.UpdateFolderOrDefault(textBoxLogFolder.Text);
            ColorConsole.DebugCategories = (ColorConsole.DebugCategory)buttonDebug.Tag;

            // Other setting
            configHelper.SetValue("updates.checkForUpdates", checkBoxUpdates.Checked);
            configHelper.SetValue("updates.includePrerelease", checkBoxPrerelease.Checked);
            configHelper.SetValue("datalogger.enabled", checkBoxDatalog.Checked);
            configHelper.SetValue("screen.topOffset", numericUpDownScreenTop.Value);
            configHelper.SetValue("screen.bottomOffset", numericUpDownScreenBottom.Value);
            configHelper.SetValue("proxy.port", numericUpDownProxyPort.Value);
            configHelper.SetValue("proxy.secure", checkBoxProxySecure.Checked);
            configHelper.SetValue("proxy.blocklist", textBoxProxyBlocklist.Text);
            configHelper.SetValue("proxy.autoconfig", checkBoxProxyAutoConfig.Checked);
            configHelper.SetValue("proxy.connectionPooling", checkBoxProxyConnectionPool.Checked);
            configHelper.SetValue("adb.path", textBoxAdbPath.Text);
            configHelper.SetValue("adb.host", (comboBoxAdbHost.SelectedItem != null) ? ((AdbHostItem)comboBoxAdbHost.SelectedItem).Value : comboBoxAdbHost.Text);
            configHelper.SetValue("adb.capture", comboBoxCapture.SelectedIndex);
            configHelper.SetValue("adb.captureRate", trackBarCaptureRate.Value * 10);
            configHelper.SetValue("adb.findPrecision", ((double)(10 - trackBarFindPrecision.Value))/ 10);
            configHelper.SetValue("adb.findAccuracy", trackBarFindAccuracy.Value);
            configHelper.SetValue("adb.input", comboBoxInput.SelectedIndex);
            configHelper.SetValue("adb.tapDelay", trackBarTapDelay.Value * 10);
            configHelper.SetValue("adb.tapDuration", trackBarTapDuration.Value * 10);
            configHelper.SetValue("adb.tapPressure", numericUpDownTapPressure.Value);
            configHelper.SetValue("adb.closeOnExit", checkBoxAdbClose.Checked);
            configHelper.SetValue("lab.configFile", ConfigFile.FromObject(comboBoxLab.SelectedItem).Path);
            configHelper.SetValue("lab.watchdogHangSeconds", (int)numericUpDownWatchdogHang.Value);
            configHelper.SetValue("lab.watchdogHangWarningSeconds", (int)numericUpDownWatchdogHangWarning.Value);
            configHelper.SetValue("lab.watchdogHangWarningLoopThreshold", (int)numericUpDownHangLoopThreshold.Value);
            configHelper.SetValue("lab.watchdogBattleMinutes", (int)numericUpDownWatchdogBattle.Value);
            configHelper.SetValue("lab.watchdogCrashSeconds", (int)numericUpDownWatchdogCrash.Value);
            configHelper.SetValue("lab.watchdogLoopDetectionThreshold", (int)numericUpDownRestartLoopThreshold.Value);
            configHelper.SetValue("lab.watchdogLoopDetectionWindowMinutes", (int)numericUpDownRestartLoopWindow.Value);
            configHelper.SetValue("lab.watchdogMaxRetries", (int)numericUpDownRestartMaxRetries.Value);
            configHelper.SetValue("lab.watchdogBattleMaxRetries", (int)numericUpDownBattleMaxRetries.Value);
            configHelper.SetValue("lab.watchdogHangScreenshot", checkBoxWatchdogScreenshot.Checked);
            configHelper.SetValue("counters.logDropsToTotal", checkBoxCountersLogDropsTotal.Checked);
            configHelper.SetValue("counters.materialsRarityFilter", numericUpDownCountersRarity.Value);
            configHelper.SetValue("adb.screenshotFolder", textBoxScreenshotFolder.Text);

            // Drop categories
            Counters.DropCategory cats = 0;
            foreach (var item in checkedListBoxDropCategories.CheckedItems)
            {
                cats |= Lookups.DropCategoriesInverse[item.ToString()];
            }
            configHelper.SetValue("counters.dropCategories", (int)cats);
            Counters.Default.DropCategories = cats;
            Counters.Default.LogDropsToTotalCounters = checkBoxCountersLogDropsTotal.Checked;
            Counters.Default.MaterialsRarityFilter = (int)numericUpDownCountersRarity.Value;

            // Lab
            labConfig.PartyIndex = (LabConfiguration.PartyIndexOption)comboBoxLabParty.SelectedIndex;
            labConfig.OpenDoors = checkBoxLabDoors.Checked;
            labConfig.AvoidExploreIfTreasure = checkBoxLabAvoidExplore.Checked;
            labConfig.AvoidPortal = checkBoxLabAvoidPortal.Checked;
            labConfig.AvoidPortalIfExplore = checkBoxLabAvoidPortalExplore.Checked;
            labConfig.AvoidPortalIfMore = checkBoxLabAvoidPortalMore.Checked;
            labConfig.AvoidMasterIfTreasure = checkBoxLabAvoidMaster.Checked;
            labConfig.AvoidMasterIfExplore = checkBoxLabAvoidMasterExplore.Checked;
            labConfig.AvoidMasterIfMore = checkBoxLabAvoidMasterMore.Checked;
            labConfig.RestartFailedBattle = checkBoxLabRestartFailedBattle.Checked;
            labConfig.StopOnMasterPainting = checkBoxLabStopOnMasterPainting.Checked;
            labConfig.RestartLab = checkBoxLabRestart.Checked;
            labConfig.UsePotions = checkBoxLabUsePotions.Checked;
            labConfig.WaitForStamina = checkBoxLabWaitForStamina.Checked;
            labConfig.UseLetheTears = checkBoxLabUseLetheTears.Checked;
            labConfig.LetheTearsFatigue = (int)numericUpDownFatigue.Value;
            labConfig.LetheTearsSlots[0] = 0;
            labConfig.LetheTearsSlots[1] = 0;
            labConfig.LetheTearsSlots[2] = 0;
            if (checkBoxSlot1.Checked) labConfig.LetheTearsSlots[0] |= (1 << 4);
            if (checkBoxSlot2.Checked) labConfig.LetheTearsSlots[0] |= (1 << 3);
            if (checkBoxSlot3.Checked) labConfig.LetheTearsSlots[0] |= (1 << 2);
            if (checkBoxSlot4.Checked) labConfig.LetheTearsSlots[0] |= (1 << 1);
            if (checkBoxSlot5.Checked) labConfig.LetheTearsSlots[0] |= (1 << 0);
            if (checkBoxSlot6.Checked) labConfig.LetheTearsSlots[1] |= (1 << 4);
            if (checkBoxSlot7.Checked) labConfig.LetheTearsSlots[1] |= (1 << 3);
            if (checkBoxSlot8.Checked) labConfig.LetheTearsSlots[1] |= (1 << 2);
            if (checkBoxSlot9.Checked) labConfig.LetheTearsSlots[1] |= (1 << 1);
            if (checkBoxSlot10.Checked) labConfig.LetheTearsSlots[1] |= (1 << 0);
            if (checkBoxSlot11.Checked) labConfig.LetheTearsSlots[2] |= (1 << 4);
            if (checkBoxSlot12.Checked) labConfig.LetheTearsSlots[2] |= (1 << 3);
            if (checkBoxSlot13.Checked) labConfig.LetheTearsSlots[2] |= (1 << 2);
            if (checkBoxSlot14.Checked) labConfig.LetheTearsSlots[2] |= (1 << 1);
            if (checkBoxSlot15.Checked) labConfig.LetheTearsSlots[2] |= (1 << 0);
            labConfig.LetheTearsMasterOnly = checkBoxLetheTearsMasterOnly.Checked;
            labConfig.UseTeleportStoneOnMasterPainting = checkBoxLabUseTeleport.Checked;
            labConfig.CompleteDailyMission = (LabConfiguration.CompleteMissionOption)comboBoxLabMission.SelectedIndex;
            labConfig.ScreenshotRadiantPainting = checkBoxLabScreenshotRadiant.Checked;
            labConfig.EnemyBlocklistAvoidOptionOverride = checkBoxLabBlockListOverride.Checked;
            labConfig.AutoStart = checkBoxLabAutoStart.Checked;
            labConfig.BoostRestoreEnabled = checkBoxBoostRestore.Checked;
            labConfig.BoostRestoreFatigue = (int)numericUpDownRestoreFatigue.Value;

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
                value.Priority = int.Parse(item.SubItems[1].Text);
                value.MaxKeys = int.Parse(item.SubItems[2].Text);
                labConfig.TreasureFilterMap.Add(item.Tag.ToString(), value);
            }

            // Enemy blocklist
            labConfig.EnemyPriorityList.Clear();
            foreach (DataGridViewRow item in dataGridViewEnemies.Rows)
            {
                if (!String.IsNullOrEmpty(item.Cells[3].Value?.ToString()))
                {
                    var entry = new LabConfiguration.EnemyPriority();
                    if (item.Cells[0].Value != null) entry.Enabled = (bool)item.Cells[0].Value;
                    if (item.Cells[1].Value != null) entry.PriorityAdjust = ((DataGridViewComboBoxCell)item.Cells[1]).Items.IndexOf(item.Cells[1].Value) - 3;
                    if (item.Cells[2].Value != null && !item.Cells[2].Value.Equals("Default")) entry.Parties.Add(((DataGridViewComboBoxCell)item.Cells[2]).Items.IndexOf(item.Cells[2].Value) - 1);
                    entry.Name = item.Cells[3].Value.ToString();
                    labConfig.EnemyPriorityList.Add(entry);
                }
            }

            // Save Lab to .json
            await labConfig.Save(ConfigFile.FromObject(comboBoxLab.SelectedItem).Path);

            // Save Timings
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                var t = LabTimings.Timings[item.Cells[0].Value.ToString()];
                t.Delay = int.Parse(item.Cells[1].Value.ToString());
                t.Jitter = int.Parse(item.Cells[2].Value.ToString());
            }
            await LabTimings.Save();

            // Save Schedule
            scheduler.Schedules.Clear();
            foreach (ListViewItem item in listViewSchedule.Items)
            {
                var schedule = (Scheduler.Schedule)item.Tag;
                scheduler.Schedules.Add(schedule);
            }
            _ = scheduler.Save(); // No await here or it causes a deadlock on the UI thread

            // Save Notifications
            ComboBoxNotificationEvents_SelectedIndexChanged(sender, e);
            Notifications.Default.Events = notificationEvents;
            await Notifications.Default.Save();

            // Message
            ColorConsole.WriteLine("Done!");

            // Update machine
            if (controller.Machine != null)
            {
                controller.Machine.Config = labConfig;
                controller.Adb.CaptureRate = trackBarCaptureRate.Value * 10;
                controller.Adb.FindPrecision = ((double)(10 - trackBarFindPrecision.Value)) / 10;
                controller.Adb.FindAccuracy = trackBarFindAccuracy.Value;
                controller.Adb.TapDelay = trackBarTapDelay.Value * 10;
                controller.Adb.TapDuration = trackBarTapDuration.Value * 10;
                controller.Adb.TapPressure = (int)numericUpDownTapPressure.Value;
                controller.Adb.Capture = (Adb.CaptureType)comboBoxCapture.SelectedIndex;
                controller.Adb.Input = (Adb.InputType)comboBoxInput.SelectedIndex;
                controller.Adb.ScreenshotFolder = textBoxScreenshotFolder.Text;
            }

            // Watchdog
            var watchdogConfig = new LabWatchdog.Configuration()
            {
                CrashSeconds = (int)numericUpDownWatchdogCrash.Value,
                HangSeconds = (int)numericUpDownWatchdogHang.Value,
                HangWarningSeconds = (int)numericUpDownWatchdogHangWarning.Value,
                HangWarningLoopThreshold = (int)numericUpDownHangLoopThreshold.Value,
                HangScreenshot = checkBoxWatchdogScreenshot.Checked,
                BattleMinutes = (int)numericUpDownWatchdogBattle.Value,
                RestartLoopThreshold = (int)numericUpDownRestartLoopThreshold.Value,
                RestartLoopWindowMinutes = (int)numericUpDownRestartLoopWindow.Value,
                MaxRetries = (int)numericUpDownRestartMaxRetries.Value,
                BattleMaxRetries = (int)numericUpDownBattleMaxRetries.Value
            };
            controller.Machine?.Watchdog.Update(watchdogConfig);

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
                    controller.Stop();
                    Application.Restart();
                    Environment.Exit(0);
                }
                lblRestart.Visible = false;
            }

            // Close
            if (sender == buttonOk) this.Close();
        }

        private async void ComboBoxLab_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Options
            labConfig = await LabConfiguration.Load<LabConfiguration>(ConfigFile.FromObject(comboBoxLab.SelectedItem).Path);
            comboBoxLabParty.SelectedIndex = (int)labConfig.PartyIndex;
            checkBoxLabDoors.Checked = labConfig.OpenDoors;
            checkBoxLabAvoidExplore.Checked = labConfig.AvoidExploreIfTreasure;
            checkBoxLabAvoidPortal.Checked = labConfig.AvoidPortal;
            checkBoxLabAvoidPortalExplore.Checked = labConfig.AvoidPortalIfExplore;
            checkBoxLabAvoidPortalMore.Checked = labConfig.AvoidPortalIfMore;
            checkBoxLabAvoidMaster.Checked = labConfig.AvoidMasterIfTreasure;
            checkBoxLabAvoidMasterExplore.Checked = labConfig.AvoidMasterIfExplore;
            checkBoxLabAvoidMasterMore.Checked = labConfig.AvoidMasterIfMore;
            checkBoxLabRestartFailedBattle.Checked = labConfig.RestartFailedBattle;
            checkBoxLabStopOnMasterPainting.Checked = labConfig.StopOnMasterPainting;
            CheckBoxLabStopOnMasterPainting_CheckedChanged(sender, e);
            checkBoxLabRestart.Checked = labConfig.RestartLab;
            CheckBoxLabRestart_CheckedChanged(sender, e);
            checkBoxLabUsePotions.Checked = labConfig.UsePotions;
            checkBoxLabWaitForStamina.Checked = labConfig.WaitForStamina;
            checkBoxLabUseLetheTears.Checked = labConfig.UseLetheTears;
            CheckBoxLabUseLetheTears_CheckedChanged(sender, e);
            numericUpDownFatigue.Value = labConfig.LetheTearsFatigue;
            checkBoxSlot1.Checked = ((labConfig.LetheTearsSlots[0] >> 4) & 1) != 0;
            checkBoxSlot2.Checked = ((labConfig.LetheTearsSlots[0] >> 3) & 1) != 0;
            checkBoxSlot3.Checked = ((labConfig.LetheTearsSlots[0] >> 2) & 1) != 0;
            checkBoxSlot4.Checked = ((labConfig.LetheTearsSlots[0] >> 1) & 1) != 0;
            checkBoxSlot5.Checked = ((labConfig.LetheTearsSlots[0] >> 0) & 1) != 0;
            checkBoxSlot6.Checked = ((labConfig.LetheTearsSlots[1] >> 4) & 1) != 0;
            checkBoxSlot7.Checked = ((labConfig.LetheTearsSlots[1] >> 3) & 1) != 0;
            checkBoxSlot8.Checked = ((labConfig.LetheTearsSlots[1] >> 2) & 1) != 0;
            checkBoxSlot9.Checked = ((labConfig.LetheTearsSlots[1] >> 1) & 1) != 0;
            checkBoxSlot10.Checked = ((labConfig.LetheTearsSlots[1] >> 0) & 1) != 0;
            checkBoxSlot11.Checked = ((labConfig.LetheTearsSlots[2] >> 4) & 1) != 0;
            checkBoxSlot12.Checked = ((labConfig.LetheTearsSlots[2] >> 3) & 1) != 0;
            checkBoxSlot13.Checked = ((labConfig.LetheTearsSlots[2] >> 2) & 1) != 0;
            checkBoxSlot14.Checked = ((labConfig.LetheTearsSlots[2] >> 1) & 1) != 0;
            checkBoxSlot15.Checked = ((labConfig.LetheTearsSlots[2] >> 0) & 1) != 0;
            checkBoxLetheTearsMasterOnly.Checked = labConfig.LetheTearsMasterOnly;
            checkBoxLabUseTeleport.Checked = labConfig.UseTeleportStoneOnMasterPainting;
            comboBoxLabMission.SelectedIndex = (int)labConfig.CompleteDailyMission;
            checkBoxLabScreenshotRadiant.Checked = labConfig.ScreenshotRadiantPainting;
            checkBoxLabBlockListOverride.Checked = labConfig.EnemyBlocklistAvoidOptionOverride;
            checkBoxLabAutoStart.Checked = labConfig.AutoStart;
            checkBoxBoostRestore.Checked = labConfig.BoostRestoreEnabled;
            numericUpDownRestoreFatigue.Value = labConfig.BoostRestoreFatigue;

            // Painting priorities
            listViewPaintings.Items.Clear();
            foreach (var item in labConfig.PaintingPriorityMap)
            {
                var newItem = new ListViewItem(item.Value.ToString());
                newItem.SubItems.Add(Lookups.Paintings[item.Key]);
                newItem.Tag = item.Key;
                newItem.ImageIndex = 1;
                listViewPaintings.Items.Add(newItem);
            }

            // Treasure priorities
            treasuresLoaded = false;
            listViewTreasures.Items.Clear();
            foreach (var item in labConfig.TreasureFilterMap)
            {
                var newItem = new ListViewItem();
                newItem.Text = "";
                newItem.SubItems.Add(item.Value.Priority.ToString());
                newItem.SubItems.Add(item.Value.MaxKeys.ToString());
                newItem.SubItems.Add(Lookups.Treasures[item.Key]);
                newItem.Checked = item.Value.Priority > 0;
                newItem.Tag = item.Key;
                newItem.ImageIndex = 0;
                listViewTreasures.Items.Add(newItem);
            }
            treasuresLoaded = true;

            // Enemy blocklist
            dataGridViewEnemies.Rows.Clear();
            foreach (LabConfiguration.EnemyPriority entry in labConfig.EnemyPriorityList)
            {
                var newItem = dataGridViewEnemies.Rows[dataGridViewEnemies.Rows.Add()];
                var priorityItems = ((DataGridViewComboBoxCell)newItem.Cells[1]).Items;
                var partyItems = ((DataGridViewComboBoxCell)newItem.Cells[2]).Items;
                newItem.Cells[0].Value = entry.Enabled;
                newItem.Cells[1].Value = priorityItems[entry.PriorityAdjust + 3];
                newItem.Cells[2].Value = entry.Parties.Count == 0 ? partyItems[0] : partyItems[entry.Parties[0] + 1];
                newItem.Cells[3].Value = entry.Name;
                if (Lookups.Blocklist.ContainsKey(entry.Name)) newItem.Cells[3].ToolTipText = Lookups.Blocklist[entry.Name];
            }

        }

        private void ListViewTreasures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewTreasures.SelectedItems.Count == 0) return;
            comboBoxKeys.SelectedIndex = comboBoxKeys.FindString(listViewTreasures.SelectedItems[0].SubItems[2].Text);
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
            var p = int.Parse(listViewTreasures.SelectedItems[0].SubItems[1].Text) - 1;
            if (p < 0) return;
            if (p <= 1) p = 1;
            listViewTreasures.SelectedItems[0].SubItems[1].Text = p.ToString();
            listViewTreasures.Sort();
            listViewTreasures.Focus();
        }

        private void ButtonTreasureDown_Click(object sender, EventArgs e)
        {
            if (listViewTreasures.SelectedItems.Count == 0) return;
            var p = int.Parse(listViewTreasures.SelectedItems[0].SubItems[1].Text) + 1;
            if (p >= 255) p = 255;
            listViewTreasures.SelectedItems[0].SubItems[1].Text = p.ToString();
            listViewTreasures.SelectedItems[0].Checked = p > 0;
            listViewTreasures.Sort();
            listViewTreasures.Focus();
        }

        private void ListViewTreasures_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!treasuresLoaded || !treasuresTabLoaded) return;
            if (e.Item.Checked)
            {
                e.Item.SubItems[1].Text = "1";
            }
            else
            {
                e.Item.SubItems[1].Text = "0";
            }
            buttonTreasureUp.Enabled = e.Item.Checked;
            buttonTreasureDown.Enabled = e.Item.Checked;
            listViewTreasures.Sort();
        }

        private void ComboBoxKeys_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (listViewTreasures.SelectedItems.Count == 0) return;
            listViewTreasures.SelectedItems[0].SubItems[2].Text = comboBoxKeys.Text;
            listViewTreasures.Sort();
            listViewTreasures.Focus();

        }

        private void NeedsRestart_Changed(object sender, EventArgs e)
        {

            var changed = (
                (short)buttonDebug.Tag != configHelper.GetShort("console.debugCategories", 0) |
                checkBoxDatalog.Checked != configHelper.GetBool("datalogger.enabled", false) |
                numericUpDownProxyPort.Value != configHelper.GetInt("proxy.port", 8081) |
                checkBoxProxySecure.Checked != configHelper.GetBool("proxy.secure", true) |
                textBoxProxyBlocklist.Text != configHelper.GetString("proxy.blocklist", "") |
                checkBoxProxyAutoConfig.Checked != configHelper.GetBool("proxy.autoconfig", false) |
                checkBoxProxyConnectionPool.Checked != configHelper.GetBool("proxy.connectionPooling", false) |
                textBoxAdbPath.Text != configHelper.GetString("adb.path", "adb.exe") |
                ((comboBoxAdbHost.SelectedItem != null) ? ((AdbHostItem)comboBoxAdbHost.SelectedItem).Value : comboBoxAdbHost.Text) != configHelper.GetString("adb.host", "127.0.0.1:7555") |
                comboBoxCapture.SelectedIndex != configHelper.GetInt("adb.capture", 1) |
                comboBoxInput.SelectedIndex != configHelper.GetInt("adb.input", 1)
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

        private void CheckBoxLabStopOnMasterPainting_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxLabUseTeleport.Enabled = !checkBoxLabStopOnMasterPainting.Checked;
            checkBoxLabRestart.Enabled = !checkBoxLabStopOnMasterPainting.Checked;
            CheckBoxLabRestart_CheckedChanged(sender, e);
            CheckBoxLabUseTeleport_CheckedChanged(sender, e);
        }

        private void CheckBoxLabUseTeleport_CheckedChanged(object sender, EventArgs e)
        {
            labelLabMission.Enabled = checkBoxLabUseTeleport.Checked && !checkBoxLabStopOnMasterPainting.Checked;
            comboBoxLabMission.Enabled = checkBoxLabUseTeleport.Checked && !checkBoxLabStopOnMasterPainting.Checked;
        }

        private void CheckBoxLabRestart_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxLabUsePotions.Enabled = (checkBoxLabRestart.Checked && !checkBoxLabStopOnMasterPainting.Checked);
            checkBoxLabWaitForStamina.Enabled = checkBoxLabUsePotions.Enabled;
        }

        private void CheckBoxLabUseLetheTears_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownFatigue.Enabled = checkBoxLabUseLetheTears.Checked;
            flowLayoutPanelTeam1.Enabled = checkBoxLabUseLetheTears.Checked;
            flowLayoutPanelTeam2.Enabled = checkBoxLabUseLetheTears.Checked;
            flowLayoutPanelTeam3.Enabled = checkBoxLabUseLetheTears.Checked;
            checkBoxLetheTearsMasterOnly.Enabled = checkBoxLabUseLetheTears.Checked;
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
            var DefaultTimings = LabTimings.DefaultTimings;
            if (e.ColumnIndex == 1 && int.TryParse(row.Cells[1].Value.ToString(), out i))
            {
                if (DefaultTimings.ContainsKey(key) && DefaultTimings[key].Delay != i) e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
            }
            if (e.ColumnIndex == 2 && int.TryParse(row.Cells[2].Value.ToString(), out i))
            {
                if (DefaultTimings.ContainsKey(key) && DefaultTimings[key].Jitter != i) e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
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

        private async void ButtonCheckForUpdates_Click(object sender, EventArgs e)
        {

            if (await Updates.Check(checkBoxPrerelease.Checked))
            {
                var ret = MessageBox.Show(this, "There is a new version available, download and install it now?", "Check For Updates", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            
                if (ret == DialogResult.Yes)
                {
                    this.WindowState = FormWindowState.Minimized;
                    if (await Updates.DownloadInstallerAndRun(checkBoxPrerelease.Checked, false))
                    {
                        controller.Stop();
                        Environment.Exit(0);
                    }
                }
            
            } else
            {
                MessageBox.Show(this, "This version is up-to-date!", "Check For Updates", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void ButtonLabConfigurations_Click(object sender, EventArgs e)
        {
            ConfigListForm.CreateAndShow(configHelper.GetString("lab.configFile", "config/lab.balanced.json").ToLower());
            LoadConfigs();
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

        private void DebugToolStripMenuItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            ColorConsole.DebugCategory t = (ColorConsole.DebugCategory)buttonDebug.Tag;
            ColorConsole.DebugCategory target = (ColorConsole.DebugCategory)item.Tag;
            t ^= target;
            buttonDebug.Tag = t;
            buttonDebug.Text = String.Format("{0}", ColorConsole.GetSelectedCategories(t));
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", toolTip1.GetToolTip(linkLabel1));
        }

        private void buttonShowCounters_Click(object sender, EventArgs e)
        {
            CountersForm.CreateAndShow(controller);
        }

        private void ButtonDebug_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            contextMenuStrip1.Tag = button;
            contextMenuStrip1.Show(button, new Point(0, button.Height));
        }

        private void CheckBoxNotificationSound_CheckedChanged(object sender, EventArgs e)
        {
            textBoxNotificationSound.Enabled = checkBoxNotificationSound.Checked;
            buttonNotificationSoundBrowse.Enabled = checkBoxNotificationSound.Checked;
        }

        private void ComboBoxNotificationEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Persist values to local copy
            if (selectedNotificationEvent.HasValue)
            {
                var n = notificationEvents[selectedNotificationEvent.Value];
                var s = n.OfType<SoundNotification>().FirstOrDefault();
                if (s == null)
                {
                    n.Add(new SoundNotification() { 
                        FilePath = textBoxNotificationSound.Text, 
                        Enabled = checkBoxNotificationSound.Checked 
                    });
                }
                else
                {
                    s.FilePath = textBoxNotificationSound.Text;
                    s.Enabled = checkBoxNotificationSound.Checked;
                }
                var c = n.OfType<ConsoleNotification>().FirstOrDefault();
                if (c == null)
                {
                    n.Add(new ConsoleNotification() { Enabled = checkBoxNotificationConsole.Checked });
                }
                else
                {
                    c.Enabled = checkBoxNotificationConsole.Checked;
                }
                var f = n.OfType<FlashTaskbarNotification>().FirstOrDefault();
                if (f == null)
                {
                    n.Add(new FlashTaskbarNotification() { Enabled = checkBoxNotificationFlashTaskbar.Checked });
                }
                else
                {
                    f.Enabled = checkBoxNotificationFlashTaskbar.Checked;
                }
                var m = n.OfType<EmailNotification>().FirstOrDefault();
                if (m == null)
                {
                    n.Add(new EmailNotification() { 
                        Enabled = checkBoxNotifcationEmail.Checked,
                        SMTPHost = textBoxSMTPServer.Text,
                        Port = (int)numericUpDownSMTPPort.Value,
                        EnableSsl = checkBoxSMTPSSL.Checked,
                        UserName = textBoxSMTPUser.Text,
                        Password = textBoxSMTPPassword.Text,
                        From = textBoxSMTPFrom.Text,
                        To = textBoxSMTPTo.Text
                    });;
                }
                else
                {
                    m.Enabled = checkBoxNotifcationEmail.Checked;
                    m.SMTPHost = textBoxSMTPServer.Text;
                    m.Port = (int)numericUpDownSMTPPort.Value;
                    m.EnableSsl = checkBoxSMTPSSL.Checked;
                    m.UserName = textBoxSMTPUser.Text;
                    m.Password = textBoxSMTPPassword.Text;
                    m.From = textBoxSMTPFrom.Text;
                    m.To = textBoxSMTPTo.Text;
                }
            }


            selectedNotificationEvent = Lookups.NotificationEventsInverse[comboBoxNotificationEvents.SelectedItem.ToString()];
            var notfications = notificationEvents[selectedNotificationEvent.Value];
            var soundNotification = notfications.OfType<SoundNotification>().FirstOrDefault();
            if (soundNotification == null)
            {
                textBoxNotificationSound.Text = "";
                checkBoxNotificationSound.Checked = false;
            } else
            {
                textBoxNotificationSound.Text = soundNotification.FilePath;
                checkBoxNotificationSound.Checked = soundNotification.Enabled;
            }
            var consoleNotification = notfications.OfType<ConsoleNotification>().FirstOrDefault();
            if (consoleNotification == null)
            {
                checkBoxNotificationConsole.Checked = false;
            }
            else
            {
                checkBoxNotificationConsole.Checked = consoleNotification.Enabled;
            }
            var flashTaskbar = notfications.OfType<FlashTaskbarNotification>().FirstOrDefault();
            if (flashTaskbar == null)
            {
                checkBoxNotificationFlashTaskbar.Checked = false;
            }
            else
            {
                checkBoxNotificationFlashTaskbar.Checked = flashTaskbar.Enabled;
            }
            var email = notfications.OfType<EmailNotification>().FirstOrDefault();
            if (email == null)
            {
                checkBoxNotifcationEmail.Checked = false;
            }
            else
            {
                checkBoxNotifcationEmail.Checked = email.Enabled;
                textBoxSMTPServer.Text = email.SMTPHost;
                numericUpDownSMTPPort.Value = email.Port;
                checkBoxSMTPSSL.Checked = email.EnableSsl;
                textBoxSMTPUser.Text = email.UserName;
                textBoxSMTPPassword.Text = email.Password;
                textBoxSMTPFrom.Text = email.From;
                textBoxSMTPTo.Text = email.To;
            }
        }

        private async void ButtonNotificationTest_Click(object sender, EventArgs e)
        {
            ComboBoxNotificationEvents_SelectedIndexChanged(sender, e);
            var testEvent = Lookups.NotificationEventsInverse[comboBoxNotificationEvents.SelectedItem.ToString()];
            await Notifications.Default.ProcessEvent(testEvent, "Testing Notification", notificationEvents);
        }

        private void ButtonNotificationSoundBrowse_Click(object sender, EventArgs e)
        {
            var file = new FileInfo(Path.GetFullPath(textBoxNotificationSound.Text));

            openFileDialogSound.InitialDirectory = file.Directory.FullName;
            openFileDialogSound.FileName = textBoxNotificationSound.Text;
            var result = openFileDialogSound.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                textBoxNotificationSound.Text = openFileDialogSound.FileName;
            }
        }

        private void CheckBoxNotifcationEmail_CheckedChanged(object sender, EventArgs e)
        {
            panelSMTP.Enabled = checkBoxNotifcationEmail.Checked;
        }

        private void TextBoxSMTPUser_TextChanged(object sender, EventArgs e)
        {
            textBoxSMTPFrom.Text = textBoxSMTPUser.Text;
        }

        private void NumericUpDownRestartLoopThreshold_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownRestartLoopWindow.Enabled = numericUpDownRestartLoopThreshold.Value > 0;
            label27.Enabled = numericUpDownRestartLoopWindow.Enabled;
        }

        private void TrackBarCaptureRate_ValueChanged(object sender, EventArgs e)
        {
            labelCaptureRate.Text = $"{trackBarCaptureRate.Value * 10}ms";
        }

        private void TrackBarTapDelay_ValueChanged(object sender, EventArgs e)
        {
            labelTapDelay.Text = $"{trackBarTapDelay.Value * 10}ms";
        }

        private void TrackBarTapDuration_ValueChanged(object sender, EventArgs e)
        {
            labelTapDuration.Text = $"{trackBarTapDuration.Value * 10}ms";
        }

        private void TrackBarFindPrecision_ValueChanged(object sender, EventArgs e)
        {
            labelFindPrecision.Text = $"{trackBarFindPrecision.Value*10}%";
        }

        private void TrackBarFindAccuracy_ValueChanged(object sender, EventArgs e)
        {
            labelFindAccuracy.Text = $"{trackBarFindAccuracy.Value+1}";
        }

        private void NumericUpDownWatchdogHangWarning_ValueChanged(object sender, EventArgs e)
        {
            checkBoxWatchdogScreenshot.Enabled = numericUpDownWatchdogHangWarning.Value > 0;
        }

        private void NumericUpDownWatchdogHang_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownWatchdogHangWarning.Maximum = (numericUpDownWatchdogHang.Value == 0)? 6000 : numericUpDownWatchdogHang.Value * 0.75M;
        }

        private void DataGridViewEnemies_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!e.Row.IsNewRow)
            {
                DialogResult response = MessageBox.Show($"Are you sure you wish to delete {e.Row.Cells[3].Value}?", "Confirm Delete",
                                  MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Question,
                                  MessageBoxDefaultButton.Button2);

                if (response == DialogResult.No)
                    e.Cancel = true;
            }
        }

        private void CheckBoxBoostRestore_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownRestoreFatigue.Enabled = checkBoxBoostRestore.Checked;
        }

        private async void ComboBoxAdbHost_DropDown(object sender, EventArgs e)
        {
            // Get all non-tcp (usb) devices
            var devices = (await controller.Adb.GetDevices()).Where(d => !d.Contains("."));

            foreach (var item in devices)
            {
                if (!Lookups.AdbHosts.Any(h => h.Value.Equals(item)))
                {
                    Lookups.AdbHosts.Add(new AdbHostItem() { Name = $"USB", Value = item });
                }
            }

        }

        private void ButtonLogFolder_Click(object sender, EventArgs e)
        {
            var folder = new DirectoryInfo(Path.GetFullPath(ColorConsole.LogBuffer.LogDirectory));
            folderBrowserDialog1.SelectedPath = folder.FullName;
            var result = folderBrowserDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                textBoxLogFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void ButtonScreenshotFolder_Click(object sender, EventArgs e)
        {
            var folder = new DirectoryInfo(Path.GetFullPath(String.IsNullOrEmpty(textBoxScreenshotFolder.Text)? Application.StartupPath : textBoxScreenshotFolder.Text));
            folderBrowserDialog1.SelectedPath = folder.FullName;
            var result = folderBrowserDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                textBoxScreenshotFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        #region DragDropRegion
        // https://stackoverflow.com/a/1623968/383761
        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;
        private void DataGridViewEnemies_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty &&
                    !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {

                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = dataGridViewEnemies.DoDragDrop(
                    dataGridViewEnemies.Rows[rowIndexFromMouseDown],
                    DragDropEffects.Move);
                }
            }
        }

        private void DataGridViewEnemies_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            rowIndexFromMouseDown = dataGridViewEnemies.HitTest(e.X, e.Y).RowIndex;
            if (rowIndexFromMouseDown != -1 && rowIndexFromMouseDown < dataGridViewEnemies.Rows.Count - 1)
            {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                                               e.Y - (dragSize.Height / 2)),
                                    dragSize);
            }
            else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void DataGridViewEnemies_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void DataGridViewEnemies_DragDrop(object sender, DragEventArgs e)
        {
            // The mouse locations are relative to the screen, so they must be 
            // converted to client coordinates.
            Point clientPoint = dataGridViewEnemies.PointToClient(new Point(e.X, e.Y));

            // Get the row index of the item the mouse is below. 
            rowIndexOfItemUnderMouseToDrop = dataGridViewEnemies.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            // If the drag operation was a move then remove and insert the row.
            if (e.Effect == DragDropEffects.Move)
            {
                DataGridViewRow rowToMove = e.Data.GetData(typeof(DataGridViewRow)) as DataGridViewRow;

                dataGridViewEnemies.Rows.RemoveAt(rowIndexFromMouseDown);
                if (rowIndexOfItemUnderMouseToDrop >= dataGridViewEnemies.Rows.Count) { rowIndexOfItemUnderMouseToDrop--; }
                dataGridViewEnemies.Rows.Insert(rowIndexOfItemUnderMouseToDrop, rowToMove);
            }
        }

        #endregion

    }

}
