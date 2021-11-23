using FFRK_LabMem.Config.UI;
using FFRK_LabMem.Machines;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFRK_LabMem.Data.UI
{
    public partial class CountersForm : Form
    {

        private static bool _isLoaded = false;
        private LabController controller = null;

        public CountersForm()
        {
            InitializeComponent();
        }

        public static void CreateAndShow(LabController controller)
        {
            // Show form
            if (_isLoaded == false)
            {
                _isLoaded = true;
                var form = new CountersForm();
                form.controller = controller;
                form.Show();
            }
        }

        private void CountersForm_Load(object sender, EventArgs e)
        {
            Counters.OnUpdated += Counters_OnUpdated;
            LoadAll();
        }

        private void CountersForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Counters.OnUpdated -= Counters_OnUpdated;
            _isLoaded = false;
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
            LoadDrops("HE", Counters.Default.CounterSets.Values.SelectMany(s => s.HeroEquipment.Keys).Distinct().OrderBy(s => s), true);
            LoadDrops("Drops", Counters.Default.CounterSets.Values.SelectMany(s => s.Drops.Keys).Distinct().OrderBy(s => s), false);
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
                newItem.SubItems[2].Text = Counters.Default.CounterSets["CurrentLab"].Counters[item.Key].ToString();
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
                newItem.SubItems[2].Text = Counters.Default.CounterSets["CurrentLab"].Runtime[item.Key].ToString(runtimeFormat);
                newItem.SubItems[3].Text = Counters.Default.CounterSets["Total"].Runtime[item.Key].ToString(runtimeFormat);

            }

        }

        private void LoadDrops(string group, IOrderedEnumerable<string> keySet, bool isHE)
        {

            if (keySet.Count() == 0) RemoveItemsForGroup(group);

            foreach (var item in keySet)
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

                SetSubItemText(newItem.SubItems[1], item, isHE, "Session");
                SetSubItemText(newItem.SubItems[2], item, isHE, "CurrentLab");
                SetSubItemText(newItem.SubItems[3], item, isHE, "Total");
                
            }

        }

        private void SetSubItemText(ListViewItem.ListViewSubItem subItem, string item, bool isHE, string counterSetKey)
        {
            var target = (isHE) ? Counters.Default.CounterSets[counterSetKey].HeroEquipment : Counters.Default.CounterSets[counterSetKey].Drops;
            if (target.ContainsKey(item))
            {
                subItem.Text = target[item].ToString();
            }
            else
            {
                subItem.Text = "-";
            }
        }

        private void RemoveItemsForGroup(string group)
        {
            List<ListViewItem> remove = new List<ListViewItem>();
            foreach (ListViewItem item in listViewCounters.Groups[group].Items)
            {
                remove.Add(item);
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
                Counters.CounterSet.DataType types = (Counters.CounterSet.DataType)Enum.Parse(typeof(Counters.CounterSet.DataType), menuItemTag);
                await Counters.Reset(target, types);
            }

        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            ConfigForm.CreateAndShow(new Config.ConfigHelper(), controller, 6);
        }
       
    }
}
