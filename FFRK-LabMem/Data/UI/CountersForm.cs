using FFRK_LabMem.Config.UI;
using FFRK_LabMem.Machines;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
                Task mytask = Task.Run(() =>
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
            LoadLabs();
            
        }

        private void CountersForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Counters.OnUpdated -= Counters_OnUpdated;
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
            var sets = Counters.Default.CounterSets.Where(s => !Counters.DefaultCounterSets.ContainsKey(s.Key)).ToList();
            foreach (var item in sets)
            {
                comboBoxLab.Items.Add(item.Value);
            }
            comboBoxLab.SelectedIndex = 0;
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
                    listViewCounters.Items.Add(newItem);
                }
                newItem.SubItems[1].Text = item.Value.ToString();
                newItem.SubItems[2].Text = GetSelectedLab().Counters[item.Key].ToString();
                newItem.SubItems[3].Text = Counters.Default.CounterSets["Total"].Counters[item.Key].ToString();

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
                    listViewCounters.Items.Add(newItem);
                }

                newItem.SubItems[1].Text = item.Value.ToString(runtimeFormat);
                newItem.SubItems[2].Text = GetSelectedLab().Runtime[item.Key].ToString(runtimeFormat);
                newItem.SubItems[3].Text = Counters.Default.CounterSets["Total"].Runtime[item.Key].ToString(runtimeFormat);

            }

        }

        private void LoadDrops(string group)
        {

            bool isHE = group.Equals("HE");
            IEnumerable<string> keySet;
            if (isHE)
            {
                switch (comboBoxQE.SelectedIndex)
                {
                    case 1:
                        keySet = Counters.Default.CounterSets.Values.SelectMany(s => s.HeroEquipmentCombined.Keys);
                        break;
                    case 2:
                        keySet = Counters.Default.CounterSets.Values.SelectMany(s => s.HeroEquipmentQE.Keys);
                        break;
                    default:
                        keySet = Counters.Default.CounterSets.Values.SelectMany(s => s.HeroEquipment.Keys);
                        break;
                }
            } 
            else
            {
                switch (comboBoxQE.SelectedIndex)
                {
                    case 1:
                        keySet = Counters.Default.CounterSets.Values.SelectMany(s => s.DropsCombined.Keys);
                        break;
                    case 2:
                        keySet = Counters.Default.CounterSets.Values.SelectMany(s => s.DropsQE.Keys);
                        break;
                    default:
                        keySet = Counters.Default.CounterSets.Values.SelectMany(s => s.Drops.Keys);
                        break;
                }
            }
            keySet = keySet.Distinct();

            CleanGroup(group, keySet);

            foreach (var item in keySet.Distinct().OrderBy(s => s))
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
                    newItem.SubItems.Add("");
                    newItem.SubItems.Add("");
                    newItem.SubItems.Add("");
                    listViewCounters.Items.Add(newItem);
                }

                SetSubItemText(newItem.SubItems[1], item, isHE, Counters.Default.CounterSets["Session"]);
                SetSubItemText(newItem.SubItems[2], item, isHE, GetSelectedLab());
                SetSubItemText(newItem.SubItems[3], item, isHE, Counters.Default.CounterSets["Total"]);
                
            }

        }

        private CounterSet GetSelectedLab()
        {
            return (CounterSet)comboBoxLab.SelectedItem;
        }

        private void SetSubItemText(ListViewItem.ListViewSubItem subItem, string item, bool isHE, CounterSet counterSet)
        {
            SortedDictionary<string, int> target;
            switch (comboBoxQE.SelectedIndex)
            {
                case 1:
                    target = (isHE) ? counterSet.HeroEquipmentCombined : counterSet.DropsCombined;
                    break;
                case 2:
                    target = (isHE) ? counterSet.HeroEquipmentQE : counterSet.DropsQE;
                    break;
                default:
                    target = (isHE) ? counterSet.HeroEquipment : counterSet.Drops;
                    break;
            }
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
            var menuItem = (ToolStripMenuItem)sender;
            string menuItemTag = menuItem.Tag.ToString();

            var result = MessageBox.Show(this, $"Are you sure you want to reset {buttonTag} {menuItemTag} counters?", "Reset Counters", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                var target = buttonTag.Equals("All") ? null : buttonTag;
                CounterSet.DataType types = (CounterSet.DataType)Enum.Parse(typeof(CounterSet.DataType), menuItemTag);
                await Counters.Reset(target, types);
            }
            listViewCounters.Items.Clear();
            LoadAll();

        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            ConfigForm.CreateAndShow(new Config.ConfigHelper(), controller, 6);
        }

        private void comboBoxLab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLab.SelectedIndex >= 0)
            {
                var selectedLab = GetSelectedLab();
                listViewCounters.Columns[2].Text = selectedLab.Name;
                buttonCountersResetLab.Tag = Counters.Default.CounterSets.FirstOrDefault(x => x.Value == selectedLab).Key;
                LoadAll();
            }
        }
    }
}
