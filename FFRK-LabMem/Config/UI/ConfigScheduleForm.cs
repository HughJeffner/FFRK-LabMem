using FFRK_LabMem.Services;
using Quartz;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFRK_LabMem.Config.UI
{
    public partial class ConfigScheduleForm : Form
    {

        protected Scheduler.Schedule schedule;

        public static Scheduler.Schedule EditSchedule(Scheduler.Schedule schedule)
        {

            if (schedule == null)
            {
                schedule = new Scheduler.Schedule();
                schedule.Name = "New Schedule";
                schedule.EnableDate = DateTime.Now;
                schedule.DisableDate = DateTime.Now;
            }

            // Show form
            var form = new ConfigScheduleForm
            {
                schedule = schedule
            };
            var ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                return form.schedule;
            } else
            {
                return null;
            }
        }

        public ConfigScheduleForm()
        {
            InitializeComponent();
        }

        private void ConfigScheduleForm_Load(object sender, EventArgs e)
        {
            textBoxName.Text = schedule.Name;
            dateTimePickerEnable.Value = schedule.EnableDate;
            dateTimePickerEnable.Checked = schedule.EnableEnabled;
            comboBoxEnableRepeat.SelectedIndex = GetRepeatType(schedule.EnableCronTab);
            SetCheckBoxes(flowLayoutPanelEnable, schedule.EnableCronTab);
            checkBoxHardStart.Checked = schedule.EnableHardStart;
            checkBoxForceStart.Checked = schedule.EnableForceStart;

            dateTimePickerDisable.Value = schedule.DisableDate;
            dateTimePickerDisable.Checked = schedule.DisableEnabled;
            comboBoxDisableRepeat.SelectedIndex = GetRepeatType(schedule.DisableCronTab);
            SetCheckBoxes(flowLayoutPanelDisable, schedule.DisableCronTab);
            checkBoxHardStop.Checked = schedule.DisableCloseApp;
        }

        private void SetControlState(object sender, EventArgs e)
        {
            comboBoxEnableRepeat.Enabled = dateTimePickerEnable.Checked;
            flowLayoutPanelEnable.Enabled = dateTimePickerEnable.Checked && comboBoxEnableRepeat.SelectedIndex == 2;
            checkBoxHardStart.Enabled = dateTimePickerEnable.Checked;
            checkBoxForceStart.Enabled = dateTimePickerEnable.Checked;

            comboBoxDisableRepeat.Enabled = dateTimePickerDisable.Checked;
            flowLayoutPanelDisable.Enabled = dateTimePickerDisable.Checked && comboBoxDisableRepeat.SelectedIndex == 2;
            checkBoxHardStop.Enabled = dateTimePickerDisable.Checked;
        }

        private void SetCheckBoxes(Panel panel, string cron)
        {
            String[] days = new string[7];
            if (!String.IsNullOrEmpty(cron))
                days = cron.Split(' ')[5].Split(',');

            foreach (CheckBox item in panel.Controls)
            {
                item.Checked = days.Contains((int.Parse(item.Tag.ToString())+1).ToString());

            }

        }

        private int GetRepeatType(string cron)
        {
            if (!String.IsNullOrEmpty(cron))
            {
                return cron.EndsWith("? * *") ? 1 : 2;
            }
            else
            {
                return 0;
            }
        }

        private string GetCron(int repeatType, Panel panel, DateTime date)
        {
            CronScheduleBuilder builderEnable = null;
            if (repeatType == 1)
            {
                builderEnable = CronScheduleBuilder.DailyAtHourAndMinute(date.Hour, date.Minute);
            }
            else if (repeatType == 2)
            {
                builderEnable = CronScheduleBuilder.AtHourAndMinuteOnGivenDaysOfWeek(date.Hour, date.Minute, GetDays(panel));
            }
            if (builderEnable != null) return ((CronTriggerImpl)builderEnable.Build()).CronExpressionString;
            return "";
        }

        private DayOfWeek[] GetDays(Panel panel)
        {
            var ret = new List<DayOfWeek>();
            foreach (CheckBox item in panel.Controls)
            {
                if (item.Checked) {
                    DayOfWeek day;
                    if (Enum.TryParse<DayOfWeek>(item.Tag.ToString(), out day))
                        ret.Add(day); 
                }

            }
            return ret.ToArray();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            schedule.Name = textBoxName.Text;

            schedule.EnableEnabled = dateTimePickerEnable.Checked;
            schedule.EnableDate = dateTimePickerEnable.Value.AddSeconds(-dateTimePickerEnable.Value.Second);
            schedule.EnableHardStart = checkBoxHardStart.Checked;
            schedule.EnableForceStart = checkBoxForceStart.Checked;
            schedule.EnableCronTab = GetCron(comboBoxEnableRepeat.SelectedIndex, flowLayoutPanelEnable, dateTimePickerEnable.Value);

            schedule.DisableEnabled = dateTimePickerDisable.Checked;
            schedule.DisableDate = dateTimePickerDisable.Value.AddSeconds(-dateTimePickerDisable.Value.Second);
            schedule.DisableCloseApp = checkBoxHardStop.Checked;
            schedule.DisableCronTab = GetCron(comboBoxDisableRepeat.SelectedIndex, flowLayoutPanelDisable, dateTimePickerDisable.Value);

        }
    }
}
