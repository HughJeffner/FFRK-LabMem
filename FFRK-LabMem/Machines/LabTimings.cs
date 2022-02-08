using FFRK_Machines;
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
        private TimingDictionary timings { get; set; } = new TimingDictionary(DefaultTimings);
        private TimingTuningParameters tuningParams { get; set; } = new TimingTuningParameters();
        public LabTimings()
        {
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

        public static TimingTuningParameters TuningParams
        {
            get
            {
                return GetInstance().Result.tuningParams;
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
            int delay = DelayWithJitter((await GetInstance()).timings[key]);
            ColorConsole.Debug(ColorConsole.DebugCategory.Timings, $"Delay {key} : {delay}ms");
            await Task.Delay(delay, cancellationToken);
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
            (await GetInstance()).timings = new TimingDictionary(DefaultTimings);
        }

        public static void TuneTiming(string key, bool found, int tries)
        {
            // Main switch
            if (!LabTimings.TuningParams.Enabled) return;
            
            // Get specivied timing and tuning data
            var timing = LabTimings.Timings[key];
            if (timing.Tuning == null) timing.Tuning = new LabTimings.TimingTuning();
            var tuning = timing.Tuning;

            // State check
            if (tuning.State != LabTimings.TimingTuning.TuningState.Learned || tuning.State != LabTimings.TimingTuning.TuningState.Ignore)
            {
                // Global tuning parameters
                var parameters = LabTimings.TuningParams;
                
                // Set retry counter
                tuning.RetryCounter = tries;
                
                // Was the button found?
                if (found)
                {
                    // On the first try then decrement
                    if (tries == 0)
                    {
                        tuning.SuccessCounter += 1;
                        if (tuning.SuccessCounter >= parameters.DecrementThreshold)
                        {
                            ColorConsole.Debug(ColorConsole.DebugCategory.Timings, $"Decrementing timing: {key} by {parameters.DecrementAmount}ms after {parameters.DecrementThreshold} successes");
                            timing.Delay -= LabTimings.TuningParams.DecrementAmount;
                            tuning.State = LabTimings.TimingTuning.TuningState.Learning;
                            tuning.SuccessCounter = 0;
                        }
                    }
                    // With 1 retry and in learning state then freeze timing
                    if (tries == 1 && tuning.State == LabTimings.TimingTuning.TuningState.Learning)
                    {
                        ColorConsole.Debug(ColorConsole.DebugCategory.Timings, $"Freezing timing: {key} at {parameters.DecrementAmount}ms");
                        timing.Delay += LabTimings.TuningParams.DecrementAmount;
                        tuning.State = LabTimings.TimingTuning.TuningState.Learned;
                    }
                    // More than 1 retry then incrment
                    if (tries > 1)
                    {
                        tuning.RetryCounter += 1;
                        if (tuning.RetryCounter >= parameters.IncrementThreshold)
                        {
                            ColorConsole.Debug(ColorConsole.DebugCategory.Timings, $"Incrementing timing: {key} by {parameters.IncrementAmount}ms after {parameters.IncrementThreshold} retries");
                            timing.Delay += LabTimings.TuningParams.IncrementAmount;
                            tuning.State = LabTimings.TimingTuning.TuningState.Normal;
                            tuning.RetryCounter = 0;
                            tuning.SuccessCounter = 0;
                        }
                    }
                }
            }

        }

        public static readonly TimingDictionary DefaultTimings = new TimingDictionary()
        {
            { "Pre-AutoStart", new Timing() { Delay=10} },
            { "Inter-AutoStart", new Timing() { Delay=1000} },
            { "Post-AutoStart", new Timing() { Delay=0} },
            { "Pre-SelectPainting", new Timing(){ Delay=2000 } },
            { "Inter-SelectPainting", new Timing(){ Delay=1000 } },
            { "Post-SelectPainting", new Timing(){ Delay=0 } },
            { "Pre-RadiantPaintingScreenshot", new Timing(){ Delay=4000 } },
            { "Pre-SelectTreasure", new Timing() {Delay=2000 } },
            { "Inter-SelectTreasure", new Timing() {Delay=1000 } },
            { "Post-SelectTreasure", new Timing() {Delay=0 } },
            { "Pre-Door", new Timing() {Delay=1000} },
            { "Post-Door", new Timing(){ Delay=1000} },
            { "Pre-MoveOn", new Timing() { Delay=1000 } },
            { "Post-MoveOn", new Timing() { Delay=1000 } },
            { "Post-MoveOn-Portal", new Timing() { Delay=5000} },
            { "Pre-StartBattle", new Timing() { Delay=0} },
            { "Pre-StartBattle-Fatigue", new Timing() { Delay=20000} },
            { "Inter-StartBattle", new Timing() { Delay=500} },
            { "Post-StartBattle", new Timing() { Delay=0} },
            { "Post-Battle", new Timing(){ Delay=1000 } },
            { "Post-BattleButton", new Timing(){ Delay=2000 } },
            { "Pre-ConfirmPortal", new Timing(){ Delay=0 } },
            { "Post-ConfirmPortal", new Timing(){ Delay=2000 } },
            { "Pre-LetheTears", new Timing(){ Delay=1000 } },
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
            { "Inter-RestartLab", new Timing(){ Delay=1000 } },
            { "Post-RestartLab", new Timing(){ Delay=4000 } },
            { "Pre-RestartFFRK", new Timing(){ Delay=1000 } },
            { "Inter-RestartFFRK", new Timing(){ Delay=1000 } },
            { "Inter-RestartFFRK-Timeout", new Timing(){ Delay=180000 } },
            { "Post-RestartFFRK", new Timing(){ Delay=0 } },
            { "Pre-RestartBattle", new Timing(){ Delay=5000 } },
            { "Inter-RestartBattle", new Timing(){ Delay=2000 } },
            { "Post-RestartBattle", new Timing(){ Delay=0 } },
            { "Pre-QuickExplore", new Timing(){ Delay=5000 } },
            { "Inter-QuickExplore", new Timing(){ Delay=2000 } },
            { "Inter-QuickExplore-Timeout", new Timing(){ Delay=20000 } },
            { "Post-QuickExplore", new Timing(){ Delay=0 } },
            { "Pre-SelectParty", new Timing(){ Delay=1000 } },
            { "Post-SelectParty", new Timing(){ Delay=0 } },
            { "Pre-CheckAutoBattle", new Timing(){ Delay=10000 } },
            { "Post-CheckAutoBattle", new Timing(){ Delay=0 } },
            { "Pre-BattleInfo", new Timing(){ Delay=0} },
            { "Pre-EnterOutpost", new Timing(){ Delay=4000 } },
            { "Pre-HandleError", new Timing(){ Delay=2000 } },
            { "Post-HandleError", new Timing(){ Delay=0 } },
        };

        public class TimingDictionary : Dictionary<string, Timing>
        {
            public TimingDictionary() { }
            public TimingDictionary(Dictionary<string, Timing> from) 
            {
                foreach (KeyValuePair<string, Timing> entry in from)
                {
                    this.Add(entry.Key, (Timing)entry.Value.Clone());
                }
            }
        }

        public class TimingTuningParameters
        {
            public int IncrementThreshold { get; set; } = 1;
            public int IncrementAmount { get; set; } = 100;
            public int DecrementThreshold { get; set; } = 3;
            public int DecrementAmount { get; set; } = 10;
            public bool Enabled { get; set; } = false;
        }

        public class TimingTuning : ICloneable
        {
            public enum TuningState
            {
                Normal,
                Learning,
                Learned,
                Ignore
            }
            public TuningState State { get; set; } = TuningState.Normal;
            public int SuccessCounter { get; set; } = 0;
            public int RetryCounter { get; set; } = 0;

            public object Clone()
            {
                return new TimingTuning()
                {
                    State = this.State,
                    SuccessCounter = this.SuccessCounter,
                    RetryCounter = this.RetryCounter
                };
            }
        }

        public class Timing : ICloneable
        {

            public int Delay { get; set; } = 5000;
            public int Jitter { get; set; } = 0;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public TimingTuning Tuning { get; set; } = null;

            public object Clone()
            {
                return new Timing()
                {
                    Delay = this.Delay,
                    Jitter = this.Jitter,
                    Tuning = (TimingTuning)this.Tuning?.Clone()
                };
            }
        }

    }
}
