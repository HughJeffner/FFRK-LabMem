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
            LoadCounters();
        }

        private void CountersForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Counters.OnUpdated -= LoadCounters;
            _isLoaded = false;
        }

        private void Counters_OnUpdated()
        {
            if (listViewCounters.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(LoadCounters));
            }
            else
            {
                LoadCounters();
            }
        }

        private void LoadCounters()
        {
            listViewCounters.Items.Clear();

            // Counters
            var sessionCounters = Counters.Default.CounterSets["Session"].Counters.ToList();
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
                newItem.SubItems.Add(Counters.Default.CounterSets["CurrentLab"].Counters[item.Key].ToString());
                newItem.SubItems.Add(Counters.Default.CounterSets["Total"].Counters[item.Key].ToString());
                listViewCounters.Items.Add(newItem);
            }

            // Runtime
            string runtimeFormat = @"d\.hh\:mm\:ss";
            var sessionRuntime = Counters.Default.CounterSets["Session"].Runtime.ToList();
            foreach (var item in sessionRuntime)
            {
                var newItem = new ListViewItem();
                newItem.Group = listViewCounters.Groups["Runtime"];
                newItem.Text = item.Key;
                newItem.SubItems.Add(item.Value.ToString(runtimeFormat));
                newItem.SubItems.Add(Counters.Default.CounterSets["CurrentLab"].Runtime[item.Key].ToString(runtimeFormat));
                newItem.SubItems.Add(Counters.Default.CounterSets["Total"].Runtime[item.Key].ToString(runtimeFormat));
                listViewCounters.Items.Add(newItem);
            }

            // Merge HE Keys
            var heKeys = Counters.Default.CounterSets.Values.SelectMany(s => s.HeroEquipment.Keys).Distinct().OrderBy(s => s);
            foreach (var item in heKeys)
            {
                var newItem = new ListViewItem();
                newItem.Group = listViewCounters.Groups["HE"];
                newItem.Text = item;
                if (Counters.Default.CounterSets["Session"].HeroEquipment.ContainsKey(item))
                {
                    newItem.SubItems.Add(Counters.Default.CounterSets["Session"].HeroEquipment[item].ToString());
                }
                else
                {
                    newItem.SubItems.Add("-");
                }
                if (Counters.Default.CounterSets["CurrentLab"].HeroEquipment.ContainsKey(item))
                {
                    newItem.SubItems.Add(Counters.Default.CounterSets["CurrentLab"].HeroEquipment[item].ToString());
                }
                else
                {
                    newItem.SubItems.Add("-");
                }
                newItem.SubItems.Add("-");
                listViewCounters.Items.Add(newItem);
            }

            // Merge Drops
            var dropKeys = Counters.Default.CounterSets.Values.SelectMany(s => s.Drops.Keys).Distinct().OrderBy(s => s);
            foreach (var item in dropKeys)
            {
                var newItem = new ListViewItem();
                newItem.Group = listViewCounters.Groups["Drops"];
                newItem.Text = item;
                if (Counters.Default.CounterSets["Session"].Drops.ContainsKey(item))
                {
                    newItem.SubItems.Add(Counters.Default.CounterSets["Session"].Drops[item].ToString());
                }
                else
                {
                    newItem.SubItems.Add("-");
                }
                if (Counters.Default.CounterSets["CurrentLab"].Drops.ContainsKey(item))
                {
                    newItem.SubItems.Add(Counters.Default.CounterSets["CurrentLab"].Drops[item].ToString());
                }
                else
                {
                    newItem.SubItems.Add("-");
                }
                newItem.SubItems.Add("-");
                listViewCounters.Items.Add(newItem);
            }

        }

        private async void ButtonCountersReset_Click(object sender, EventArgs e)
        {
            String tag = (string)((Button)sender).Tag;
            var result = MessageBox.Show(this, $"Are you sure you want to reset {tag} counters?", "Reset Counters", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                var target = tag.Equals("All") ? null : tag;
                await Counters.Reset(target);
            }
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            ConfigForm.CreateAndShow(new Config.ConfigHelper(), controller, 6);
        }
    }
}
