using FFRK_LabMem.Machines;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FFRK_LabMem.Data
{
    public class Counters
    {

        private static Counters _instance = null;
        private const string CONFIG_PATH = "./DataLog/counters.json";

        public static event EventHandler OnUpdated;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public Dictionary<string, CounterSet> CounterSets { get; set; }

        private readonly LabController controller;
        private readonly Stopwatch runtimeStopwatch = new Stopwatch();

        private Counters(LabController controller)
        {
            this.CounterSets = GetDefaultCounterSets();
            this.controller = controller;
            controller.OnEnabled += Controller_OnEnabled;
            controller.OnDisabled += Controller_OnDisabled;
        }
        private Dictionary<string, CounterSet> GetDefaultCounterSets()
        {
            return new Dictionary<string, CounterSet>
            {
                {"Total", new CounterSet() },
                {"Session", new CounterSet() },
                {"CurrentLab", new CounterSet() },
            };
        }
        public static Counters Default
        {
            get
            {
                if (_instance == null) throw new InvalidOperationException();
                return _instance;
            }
        }
        public static async Task Initalize(LabController controller)
        {
            if (_instance == null)
            {
                _instance = new Counters(controller);
                await _instance.Load();
            }

        }
        public static void Uninitalize()
        {
            _instance.controller.OnEnabled -= _instance.Controller_OnEnabled;
            _instance.controller.OnDisabled -= _instance.Controller_OnDisabled;
        }
        private async void Controller_OnDisabled(object sender, EventArgs e)
        {
            await Save();
            runtimeStopwatch.Stop();
        }
        private void Controller_OnEnabled(object sender, EventArgs e)
        {
            runtimeStopwatch.Restart();
        }
        public static async Task LabRunCompleted()
        {
            await _instance.IncrementCounter("LabRunsCompleted", 1, false);
            _instance.CounterSets["CurrentLab"].Reset();
            await _instance.Save();
        }
        public static async Task PaintingSelected()
        {
            await _instance.IncrementCounter("PaintingsSelected");
        }
        public static async Task BattleWon(TimeSpan runtime)
        {
            _instance.IncrementRuntime("Battle", runtime);
            await _instance.IncrementCounter("BattlesWon");
        }
        public static async Task TreausreOpened()
        {
            await _instance.IncrementCounter("TreasuresOpened");
        }
        public static async Task FoundRadiantPainting()
        {
            await _instance.IncrementCounter("RadiantPaintings");
        }
        public static async Task FoundMagicPot()
        {
            await _instance.IncrementCounter("MagicPots");
        }
        public static async Task UsedTears(int amt)
        {
            await _instance.IncrementCounter("UsedTears", amt);
        }
        public static async Task UsedKeys(int amt)
        {
            await _instance.IncrementCounter("UsedKeys", amt);
        }
        public static async Task UsedTeleportStone()
        {
            await _instance.IncrementCounter("UsedTeleportStones");
        }
        public static async Task UsedStaminaPot()
        {
            await _instance.IncrementCounter("UsedStaminaPots");
        }
        public static async Task PulledInPortal()
        {
            await _instance.IncrementCounter("PulledInPortal");
        }
        public static async Task FFRKRestarted()
        {
            await _instance.IncrementCounter("FFRKRestarts");
        }
        public static async Task FoundHE(string name)
        {
            _instance.IncrementHE(name);
            await _instance.IncrementCounter("HeroEquipmentGot");
        }
        private async Task IncrementCounter(string key, int amt = 1, bool save = true)
        {
            if (amt == 0) return;
            foreach (var set in CounterSets)
            {
                set.Value.Counters[key] += amt;
            }
            if (save) await _instance.Save();
        }
        private void IncrementRuntime(string key, TimeSpan amt)
        {
            if (amt.TotalMilliseconds <= 0) return;
            foreach (var set in CounterSets)
            {
                set.Value.Runtime[key] += amt;
            }
        }
        private void IncrementHE(string name)
        {
            foreach (var set in CounterSets)
            {
                if (!set.Key.Equals("Total"))
                {
                    if (set.Value.HeroEquipment.ContainsKey(name))
                    {
                        set.Value.HeroEquipment[name] += 1;
                    } else
                    {
                        set.Value.HeroEquipment.Add(name, 1);
                    }
                }
            }
        }
        public async Task Load(string path = CONFIG_PATH)
        {
            try
            {
                JsonConvert.PopulateObject(File.ReadAllText(path), CounterSets);
            }
            catch (Exception)
            {
                CounterSets = GetDefaultCounterSets();
            }
            await Task.CompletedTask;
        }
        public async Task Save(string path = CONFIG_PATH)
        {
            if (runtimeStopwatch.IsRunning)
            {
                IncrementRuntime("Total", runtimeStopwatch.Elapsed);
                runtimeStopwatch.Restart();
            }
            if (OnUpdated != null) OnUpdated.Invoke(this, new EventArgs());
            try
            {
                File.WriteAllText(path, 
                    JsonConvert.SerializeObject(this.CounterSets, 
                    Formatting.Indented, 
                    new ExcludeSessionDictionaryItemConverter<IDictionary<string, CounterSet>, CounterSet>()));
            }
            catch (Exception)
            {
            }
            await Task.CompletedTask;
        }
        public static async Task Reset(string key)
        {
            if (key == null)
            {
                foreach (var item in _instance.CounterSets)
                {
                    item.Value.Reset();
                }
            } else
            {
                _instance.CounterSets[key].Reset();
            }
            await _instance.Save();

        }
        public class CounterSet
        {
            public Dictionary<string, int> Counters { get; set; }
            public Dictionary<string, TimeSpan> Runtime { get; set; }
            public Dictionary<string, int> HeroEquipment { get; set; }

            public CounterSet()
            {
                this.Counters = GetDefaultCounters();
                this.Runtime = GetDefaultRuntimes();
                this.HeroEquipment = new Dictionary<string, int>();
            }

            private Dictionary<string, int> GetDefaultCounters()
            {
                return new Dictionary<string, int>()
                {
                    {"LabRunsCompleted",0},
                    {"PaintingsSelected",0},
                    {"BattlesWon",0},
                    {"TreasuresOpened",0},
                    {"RadiantPaintings",0},
                    {"MagicPots",0},
                    {"UsedTears",0},
                    {"UsedKeys",0},
                    {"UsedTeleportStones",0},
                    {"UsedStaminaPots",0},
                    {"PulledInPortal",0},
                    {"FFRKRestarts",0},
                    {"HeroEquipmentGot",0},
                };
            }

            private Dictionary<string, TimeSpan> GetDefaultRuntimes()
            {
                return new Dictionary<string, TimeSpan>()
                {
                    {"Total", new TimeSpan()},
                    {"Battle", new TimeSpan()},
                };
            }

            public void Reset()
            {
                this.Counters = GetDefaultCounters();
                this.Runtime = GetDefaultRuntimes();
                this.HeroEquipment = new Dictionary<string, int>();
            }

        }

        public class ExcludeSessionDictionaryItemConverter<TDictionary, TValue> : JsonConverter where TDictionary : IDictionary<string, TValue>
        {
            public override bool CanConvert(Type objectType)
            {
                return typeof(TDictionary).IsAssignableFrom(objectType);
            }

            public override bool CanRead => false;
            public override bool CanWrite => true;

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                JToken t = JToken.FromObject(value);
                if (t.Type != JTokenType.Object)
                {
                    t.WriteTo(writer);
                }
                else
                {
                    JObject o = (JObject)t;
                    o.Remove("Session");
                    o.WriteTo(writer);
                }
            }
        }
    }
}
