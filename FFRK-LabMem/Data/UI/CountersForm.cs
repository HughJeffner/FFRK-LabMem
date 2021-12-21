using FFRK_LabMem.Config.UI;
using FFRK_LabMem.Machines;
using FFRK_Machines;
using FFRK_Machines.Threading;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFRK_LabMem.Data.UI
{
    public partial class CountersForm : Form
    {

        public static bool IsLoaded { get; set; } = false;
        private LabController controller = null;
        private HashSet<string> PerfectPassives { get; set; } = new HashSet<string>();
        private const string CONFIG_PATH = "./Data/counters_passives.json";


        public CountersForm()
        {
            InitializeComponent();
        }

        public static void CreateAndShow(LabController controller)
        {
            // Show form
            if (IsLoaded == false)
            {
                IsLoaded = true;
                Task mytask = Utility.StartSTATask(() =>
                {
                    var form = new CountersForm
                    {
                        controller = controller
                    };
                    form.ShowDialog();
                });
            }
        }

        private void CountersForm_Load(object sender, EventArgs e)
        {
            Counters.OnUpdated += Counters_OnUpdated;
            comboBoxQE.SelectedIndex = 0;
            LoadPassives();
            LoadLabs();
            
        }

        private void CountersForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Counters.OnUpdated -= Counters_OnUpdated;
            SavePassives();
            IsLoaded = false;
        }

        private void Counters_OnUpdated()
        {
            if (listViewCounters.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(LoadAll));
            }
            else
            {
                LoadAll();
            }
        }

        private void LoadAll()
        {
            LoadCounters();
            LoadRuntimes();
            LoadDrops("HE");
            LoadDrops("Drops");
        }

        private void LoadLabs()
        {
            comboBoxLab.Items.Clear();

            // Current lab
            var currentLab = Counters.Default.CounterSets["CurrentLab"];
            if (currentLab.Name == null) currentLab.Name = "Current Lab";
            comboBoxLab.Items.Add(currentLab);

            // Others
            var sets = Counters.Default.CounterSets.Where(s => !Counters.DefaultCounterSets.ContainsKey(s.Key)).OrderBy(s => s.Key).ToList();
            foreach (var item in sets)
            {
                comboBoxLab.Items.Add(item.Value);
            }
            comboBoxLab.SelectedIndex = 0;
        }

        private void LoadPassives()
        {
            try
            {
                JsonConvert.PopulateObject(File.ReadAllText(CONFIG_PATH), PerfectPassives);
            }
            catch (FileNotFoundException) { }
            catch (DirectoryNotFoundException) { }
            catch (Exception ex)
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Error loading passives file: {0}", ex);
            }
        }

        private void SavePassives()
        {
            try
            {
                File.WriteAllText(CONFIG_PATH, JsonConvert.SerializeObject(this.PerfectPassives, Formatting.Indented));
            }
            catch (Exception)
            {
            }
        }

        private void LoadCounters()
        {

            // Counters
            var sessionCounters = Counters.Default.CounterSets["Session"].Counters.ToList();
            foreach (var item in sessionCounters)
            {

                ListViewItem newItem;
                if (listViewCounters.Items.ContainsKey(item.Key))
                {
                    newItem = listViewCounters.Items[item.Key];
                } else
                {
                    // Add
                    newItem = new ListViewItem();
                    newItem.Name = item.Key;
                    newItem.Tag = item.Key;
                    newItem.Group = listViewCounters.Groups["Counters"];
                    if (Lookups.Counters.ContainsKey(item.Key))
                    {
                        newItem.Text = Lookups.Counters[item.Key];
                    }
                    else
                    {
                        newItem.Text = item.Key;
                    }
                    newItem.SubItems.Add("");
                    newItem.SubItems.Add("");
                    newItem.SubItems.Add("");
                    newItem.SubItems.Add("");
                    listViewCounters.Items.Add(newItem);
                }
                newItem.SubItems[1].Text = item.Value.ToString();
                newItem.SubItems[2].Text = GetSelectedLab().Counters[item.Key].ToString();
                newItem.SubItems[3].Text = Counters.Default.CounterSets["Group"].Counters[item.Key].ToString();
                newItem.SubItems[4].Text = Counters.Default.CounterSets["Total"].Counters[item.Key].ToString();

            }

        }

        private void LoadRuntimes()
        {

            string runtimeFormat = @"d\.hh\:mm\:ss";
            var sessionRuntime = Counters.Default.CounterSets["Session"].Runtime.ToList();
            foreach (var item in sessionRuntime)
            {
                ListViewItem newItem;
                if (listViewCounters.Items.ContainsKey(item.Key))
                {
                    newItem = listViewCounters.Items[item.Key];
                }
                else
                {
                    newItem = new ListViewItem();
                    newItem.Text = item.Key;
                    newItem.Name = item.Key;
                    newItem.Tag = item.Key;
                    newItem.Group = listViewCounters.Groups["Runtime"];
                    newItem.SubItems.Add("");
                    newItem.SubItems.Add("");
                    newItem.SubItems.Add("");
                    newItem.SubItems.Add("");
                    listViewCounters.Items.Add(newItem);
                }

                newItem.SubItems[1].Text = item.Value.ToString(runtimeFormat);
                newItem.SubItems[2].Text = GetSelectedLab().Runtime[item.Key].ToString(runtimeFormat);
                newItem.SubItems[3].Text = Counters.Default.CounterSets["Group"].Runtime[item.Key].ToString(runtimeFormat);
                newItem.SubItems[4].Text = Counters.Default.CounterSets["Total"].Runtime[item.Key].ToString(runtimeFormat);

            }

        }

        private void LoadDrops(string group)
        {
            bool isHE = group.Equals("HE");

            // Create sorting object
            IComparer<string> sorter;
            if (isHE)
            {
                sorter = new CounterComparers.HEComparer();
            }
            else 
            { 
                sorter = new CounterComparers.DropComparer();
            }

            // Set of all items common to all counter sets
            IEnumerable<string> keySet;
            if (isHE)
            {
                keySet = keySet = Counters.Default.CounterSets.Values.SelectMany(s => s.GetHEFiltered((CounterSet.FilterType)comboBoxQE.SelectedIndex).Keys);
            } 
            else
            {
                keySet = Counters.Default.CounterSets.Values.SelectMany(s => s.GetDropsFiltered((CounterSet.FilterType)comboBoxQE.SelectedIndex).Keys);
            }

            // Only distinct values
            keySet = keySet.Distinct();

            // If stage selected then only show its HE
            if (isHE && comboBoxLab.SelectedIndex > 0)
            {
                keySet = keySet.Where(i => GetSelectedLab().GetHEFiltered((CounterSet.FilterType)comboBoxQE.SelectedIndex).ContainsKey(i));
            }

            // Remove any items present in the list that do not match
            CleanGroup(group, keySet);

            // Iterate
            foreach (var item in keySet.OrderBy(s => s, sorter))
            {
                ListViewItem newItem;
                if (listViewCounters.Items.ContainsKey(item))
                {
                    newItem = listViewCounters.Items[item];
                }
                else
                {
                    newItem = new ListViewItem();
                    newItem.Group = listViewCounters.Groups[group];
                    newItem.Name = item;
                    newItem.Text = item;
                    if (isHE && PerfectPassives.Contains(item)) newItem.BackColor = Color.LightGreen;
                    newItem.SubItems.Add("");
                    newItem.SubItems.Add("");
                    newItem.SubItems.Add("");
                    newItem.SubItems.Add("");
                    listViewCounters.Items.Add(newItem);
                }

                SetSubItemText(newItem.SubItems[1], item, isHE, Counters.Default.CounterSets["Session"]);
                SetSubItemText(newItem.SubItems[2], item, isHE, GetSelectedLab());
                SetSubItemText(newItem.SubItems[3], item, isHE, Counters.Default.CounterSets["Group"]);
                SetSubItemText(newItem.SubItems[4], item, isHE, Counters.Default.CounterSets["Total"]);
                
            }

        }

        private CounterSet GetSelectedLab()
        {
            return (CounterSet)comboBoxLab.SelectedItem;
        }

        private void SetSubItemText(ListViewItem.ListViewSubItem subItem, string item, bool isHE, CounterSet counterSet)
        {
            SortedDictionary<string, int> target;
            target = (isHE) ? counterSet.GetHEFiltered((CounterSet.FilterType)comboBoxQE.SelectedIndex) : 
                counterSet.GetDropsFiltered((CounterSet.FilterType)comboBoxQE.SelectedIndex);
            if (target.ContainsKey(item))
            {
                subItem.Text = target[item].ToString();
            }
            else
            {
                subItem.Text = "-";
            }
        }

        private void CleanGroup(string group, IEnumerable<string> keys)
        {
            List<ListViewItem> remove = new List<ListViewItem>();
            foreach (ListViewItem item in listViewCounters.Groups[group].Items)
            {
                if (!keys.Contains(item.Text)) remove.Add(item);
            }

            foreach (ListViewItem item in remove)
            {
                listViewCounters.Items.Remove(item);
            }
        }

        private void ButtonCountersReset_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            contextMenuStrip1.Tag = button;
            contextMenuStrip1.Show(button, new Point(0, button.Height));
        }

        private async void ResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var button = (Button)contextMenuStrip1.Tag;
            string buttonTag = button.Tag.ToString();
            string buttonText = Char.ToLower(button.Text[0]) + button.Text.Substring(1);
            var menuItem = (ToolStripMenuItem)sender;
            string menuItemTag = menuItem.Tag.ToString();
            string menuItemText = menuItem.Text;
            if (button == buttonCountersResetLab) buttonText = "reset " + comboBoxLab.Text;

            var result = MessageBox.Show(this, $"Are you sure you want to {buttonText} {menuItemText}?", "Reset Counters", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                var target = buttonTag.Equals("All") ? null : buttonTag;
                CounterSet.DataType types = (CounterSet.DataType)Enum.Parse(typeof(CounterSet.DataType), menuItemTag);
                await Counters.Reset(target, types);
            }
            listViewCounters.Items.Clear();
            LoadAll();
        }

        private async void ButtonExport_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = "./data/";
            saveFileDialog1.FileName = String.Format("counters_{0:yyyyMMdd}.csv", DateTime.Now);
            var result = saveFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                await SaveListViewToCSV(listViewCounters, saveFileDialog1.FileName);
            }
        }

        private void comboBoxLab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLab.SelectedIndex >= 0)
            {
                var selectedLab = GetSelectedLab();
                listViewCounters.Columns[2].Text = selectedLab.Name;
                buttonCountersResetLab.Tag = Counters.Default.CounterSets.FirstOrDefault(x => x.Value == selectedLab).Key;
                if (sender == comboBoxQE) CleanGroup("HE", new List<string>());
                LoadAll();
                
            }
        }

        private async void listViewCounters_KeyUp(object sender, KeyEventArgs e)
        {
            if (listViewCounters.SelectedItems.Count == 0 || e.KeyCode != Keys.Delete) return;
            var target = listViewCounters.SelectedItems[0];
            var response = MessageBox.Show($"Are you sure you wish to delete {target.Text} in all counters?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (response == DialogResult.Yes) await Counters.ResetItem(target.Name);
        }

        private async Task SaveListViewToCSV(ListView listView, string fileName)
        {

            List<string> row = new List<string>();
            using (var writer = new StringWriter())
            {
                // Header row
                foreach (ColumnHeader item in listView.Columns)
                {
                    row.Add(item.Text);
                }
                row[0] = "Item";
                DataLogger.WriteCSVLine(writer, row.ToArray(), row.Count);

                // Items
                foreach (ListViewItem item in listView.Items)
                {
                    for (int i = 0; i < item.SubItems.Count; i++)
                    {
                        row[i] = item.SubItems[i].Text;
                        if (row[i].Equals("-")) row[i] = "0";
                    }
                    DataLogger.WriteCSVLine(writer, row.ToArray(), row.Count);
                }
                try
                {
                    using (var stream = new StreamWriter(fileName, false))
                    {
                        await stream.WriteAsync(writer.ToString());
                    }
                }
                catch (Exception ex)
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Error writing to file: {0}", ex.Message);
                }
            }

        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            if (listViewCounters.SelectedItems.Count == 0 || listViewCounters.SelectedItems[0].Group.Name != "HE") return;
            var item = listViewCounters.SelectedItems[0].Name;
            if (PerfectPassives.Contains(item))
            {
                PerfectPassives.Remove(item);
            } else
            {
                PerfectPassives.Add(item);
            }
            listViewCounters.SelectedItems[0].BackColor = toolStripMenuItem8.Checked ? Color.LightGreen : listViewCounters.BackColor;
        }

        private void listViewCounters_MouseUp(object sender, MouseEventArgs e)
        {
            if (listViewCounters.SelectedItems.Count == 0 || listViewCounters.SelectedItems[0].Group.Name != "HE") return;
            toolStripMenuItem8.Checked = listViewCounters.SelectedItems[0].BackColor.Equals(Color.LightGreen);
            if (e.Button == MouseButtons.Right) contextMenuStrip2.Show(listViewCounters, e.X, e.Y);
        }
    }
}
