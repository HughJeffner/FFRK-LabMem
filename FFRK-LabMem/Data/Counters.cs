using FFRK_LabMem.Machines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FFRK_LabMem.Data
{
    public class Counters
    {

        private static Counters _instance = null;
        private const string CONFIG_PATH = "./DataLog/counters.json";

        public static event EventHandler OnUpdated;

        public CounterSet Total { get; set; } = new CounterSet();
        public CounterSet Session { get; set; } = new CounterSet();

        private LabController controller;
        private Stopwatch runtimeStopwatch = new Stopwatch();

        private Counters(LabController controller)
        {
            this.controller = controller;
            controller.OnEnabled += Controller_OnEnabled;
            controller.OnDisabled += Controller_OnDisabled;
        }

        public static Counters Default()
        {
            if (_instance == null) throw new InvalidOperationException();
            return _instance;
        }

        public static async Task Initalize(LabController controller)
        {
            if (_instance == null)
            {
                _instance = new Counters(controller);
                await _instance.Load();
            }

         }

        private async void Controller_OnDisabled(object sender, EventArgs e)
        {
            await Save();
            runtimeStopwatch.Stop();
            this.Total.Runtime += runtimeStopwatch.Elapsed;
            this.Session.Runtime += runtimeStopwatch.Elapsed;
        }

        private void Controller_OnEnabled(object sender, EventArgs e)
        {
            runtimeStopwatch.Start();
        }

        public static async Task LabRunCompleted()
        {
            await _instance.IncrementCounters("LabRunsCompleted");
        }

        public static async Task PaintingSelected()
        {
            await _instance.IncrementCounters("PaintingsSelected");
        }

        public static async Task BattleWon()
        {
            await _instance.IncrementCounters("BattlesWon");
        }

        public static async Task TreausreOpened()
        {
            await _instance.IncrementCounters("TreasuresOpened");
        }

        private async Task IncrementCounters(string key)
        {
            Session.Counters[key] += 1;
            Total.Counters[key] += 1;
            await _instance.Save();
        }

        private async Task Load()
        {
            try
            {
                Total = JsonConvert.DeserializeObject<CounterSet>(File.ReadAllText(CONFIG_PATH));
            }
            catch (Exception)
            {
            }
            await Task.CompletedTask;
        }

        public async Task Save()
        {
            if (runtimeStopwatch.IsRunning)
            {
                this.Total.Runtime += runtimeStopwatch.Elapsed;
                this.Session.Runtime += runtimeStopwatch.Elapsed;
                runtimeStopwatch.Restart();
            }
            if (OnUpdated != null) OnUpdated.Invoke(this, new EventArgs());
            try
            {
                File.WriteAllText(CONFIG_PATH, JsonConvert.SerializeObject(this.Total, Formatting.Indented));
            } 
            catch (Exception)
            { 
            }
            await Task.CompletedTask;
        }

        public static async Task Reset(bool sessionOnly)
        {
            _instance.Session.Reset();
            if (!sessionOnly) _instance.Total.Reset();
            await Data.Counters.Default().Save();

        }

        public class CounterSet
        {

            public Dictionary<string, int> Counters { get; set; }
            public TimeSpan Runtime { get; set; }

            public CounterSet()
            {
                this.Counters = GetDefaults();
            }

            private Dictionary<string, int> GetDefaults()
            {
                return new Dictionary<string, int>()
                {
                    {"LabRunsCompleted",0},
                    {"PaintingsSelected",0},
                    {"BattlesWon",0},
                    {"TreasuresOpened",0}
                };
            }

            public void Reset()
            {
                this.Counters = GetDefaults();
                this.Runtime = new TimeSpan();
            }

        }

    }
}
