using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFRK_LabMem.Machines
{
    public class LabTimings
    {
        private const string CONFIG_PATH = "./Config/timings.json";
        private static LabTimings _instance = null;
        private static readonly Random rng = new Random();
        private TimingDictionary timings { get; set; } = new TimingDictionary();
        public LabTimings()
        {
            this.timings = GetDefaultTimings();
        }
        private static int DelayWithJitter(Timing timing)
        {
            return rng.Next(timing.Delay, timing.Delay + timing.Jitter);
        }
        private static async Task<LabTimings> GetInstance()
        {
            if (_instance == null)
            {
                _instance = new LabTimings();
                await Load();
            }
            return _instance;
        }
        /// <summary>
        /// The current timings dictionary
        /// </summary>
        public static TimingDictionary Timings
        {
            get
            {
                return GetInstance().Result.timings;
            }
        }
        /// <summary>
        /// Loads the timings dictionary from disk
        /// </summary>
        /// <param name="path">The path of the .json file to load</param>
        /// <returns></returns>
        public static async Task Load(string path = CONFIG_PATH)
        {
            try
            {
                var t = (await GetInstance()).timings;
                JsonConvert.PopulateObject(File.ReadAllText(path), t);
            }
            catch (Exception)
            {
                await ResetToDefaults();
            }
            await Task.CompletedTask;
        }
        /// <summary>
        /// Persists the timings dictionary to disk
        /// </summary>
        /// <param name="path">The path of the file to save to in .json format</param>
        /// <returns></returns>
        public static async Task Save(string path = CONFIG_PATH)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(LabTimings.Timings, Formatting.Indented));
            await Task.CompletedTask;
        }
        /// <summary>
        /// Creates a cancellable task that completes after the delay specified in the Timing
        /// </summary>
        /// <param name="key">The key of the Timing in the Timings dictionary</param>
        /// <param name="cancellationToken">The cancellation token that will be checked</param>
        /// <returns></returns>
        public static async Task Delay(string key, CancellationToken cancellationToken)
        {
            await Task.Delay(DelayWithJitter((await GetInstance()).timings[key]), cancellationToken);
        }
        /// <summary>
        /// Gets a timespan for the specified timing
        /// </summary>
        /// <param name="key">The key of the Timing in the Timings dictionary</param>
        /// <returns>A timespan that represents the Timing </returns>
        public static async Task<TimeSpan> GetTimeSpan(string key)
        {
            return TimeSpan.FromMilliseconds(DelayWithJitter((await GetInstance()).timings[key]));
        }
        /// <summary>
        /// Resets the Timings dictionary to the default values
        /// </summary>
        /// <returns></returns>
        public static async Task ResetToDefaults()
        {
            (await GetInstance()).timings = GetDefaultTimings();
        }

        public static TimingDictionary GetDefaultTimings()
        {
            return new TimingDictionary()
            {
                { "Pre-AutoStart", new Timing() { Delay=10} },
                { "Inter-AutoStart", new Timing() { Delay=1000} },
                { "Post-AutoStart", new Timing() { Delay=0} },
                { "Pre-SelectPainting", new Timing()},
                { "Inter-SelectPainting", new Timing(){ Delay=1000 } },
                { "Post-SelectPainting", new Timing(){ Delay=0 } },
                { "Pre-RadiantPaintingScreenshot", new Timing(){ Delay=4000 } },
                { "Pre-SelectTreasure", new Timing() },
                { "Inter-SelectTreasure", new Timing() {Delay=2000 } },
                { "Post-SelectTreasure", new Timing() {Delay=0 } },
                { "Pre-Door", new Timing() {Delay=1000} },
                { "Post-Door", new Timing(){ Delay=1000} },
                { "Pre-MoveOn", new Timing() },
                { "Post-MoveOn", new Timing() { Delay=1000 } },
                { "Post-MoveOn-Portal", new Timing() { Delay=5000} },
                { "Pre-StartBattle", new Timing() { Delay=0} },
                { "Pre-StartBattle-Fatigue", new Timing() { Delay=20000} },
                { "Inter-StartBattle", new Timing() { Delay=500} },
                { "Post-StartBattle", new Timing() { Delay=0} },
                { "Post-Battle", new Timing(){ Delay=1000 } },
                { "Pre-ConfirmPortal", new Timing(){ Delay=5000 } },
                { "Post-ConfirmPortal", new Timing(){ Delay=2000 } },
                { "Pre-LetheTears", new Timing(){ Delay=4000 } },
                { "Inter-LetheTears", new Timing(){ Delay=2000 } },
                { "Inter-LetheTears-Unit", new Timing(){ Delay=500 } },
                { "Post-LetheTears", new Timing(){ Delay=0 } },
                { "Pre-TeleportStone", new Timing(){ Delay=2000 } },
                { "Inter-TeleportStone", new Timing(){ Delay=2000 } },
                { "Post-TeleportStone", new Timing(){ Delay=0 } },
                { "Pre-StaminaPotion", new Timing(){ Delay=2000 } },
                { "Inter-StaminaPotion", new Timing(){ Delay=2000 } },
                { "Post-StaminaPotion", new Timing(){ Delay=0 } },
                { "Pre-RestartLab", new Timing(){ Delay=60000 } },
                { "Inter-RestartLab", new Timing(){ Delay=5000 } },
                { "Post-RestartLab", new Timing(){ Delay=4000 } },
                { "Pre-RestartFFRK", new Timing(){ Delay=5000 } },
                { "Inter-RestartFFRK", new Timing(){ Delay=4000 } },
                { "Inter-RestartFFRK-Timeout", new Timing(){ Delay=180000 } },
                { "Post-RestartFFRK", new Timing(){ Delay=0 } },
                { "Pre-RestartBattle", new Timing(){ Delay=5000 } },
                { "Inter-RestartBattle", new Timing(){ Delay=2000 } },
                { "Post-RestartBattle", new Timing(){ Delay=0 } },
                { "Pre-QuickExplore", new Timing(){ Delay=5000 } },
                { "Inter-QuickExplore", new Timing(){ Delay=2000 } },
                { "Inter-QuickExplore-Timeout", new Timing(){ Delay=20000 } },
                { "Post-QuickExplore", new Timing(){ Delay=0 } },
                { "Pre-SelectParty", new Timing(){ Delay=4000 } },
                { "Post-SelectParty", new Timing(){ Delay=0 } },
                { "Pre-CheckAutoBattle", new Timing(){ Delay=10000 } },
                { "Inter-CheckAutoBattle", new Timing(){ Delay=1000 } },
                { "Post-CheckAutoBattle", new Timing(){ Delay=0 } },
            };
        }

        public class TimingDictionary : Dictionary<string, Timing>
        {
            public TimingDictionary() { }
        }

        public class Timing
        {
            public int Delay { get; set; } = 5000;
            public int Jitter { get; set; } = 0;
        }

    }
}
