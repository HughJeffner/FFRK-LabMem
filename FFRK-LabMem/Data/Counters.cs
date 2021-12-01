using FFRK_LabMem.Config;
using FFRK_LabMem.Machines;
using FFRK_Machines;
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
        private const string CONFIG_PATH = "./Data/counters.json";

        public static event Action OnUpdated;

        [Flags]
        public enum DropCategory
        {
            EQUIPMENT = 1 << 0,
            LABYRINTH_ITEM = 1 << 1,
            COMMON = 1 << 2,
            SPHERE_MATERIAL = 1 << 3,
            ABILITY_MATERIAL = 1 << 4,
            EQUIPMENT_SP_MATERIAL = 1 << 5,
            HISTORIA_CRYSTAL_ENHANCEMENT_MATERIAL = 1 << 6,
            GROW_EGG = 1 << 7,
            BEAST_FOOD = 1 << 8
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public Dictionary<string, CounterSet> CounterSets { get; set; } 

        public DropCategory DropCategories { get; set; } = DropCategory.EQUIPMENT | 
            DropCategory.LABYRINTH_ITEM | 
            DropCategory.COMMON | 
            DropCategory.SPHERE_MATERIAL;

        public bool LogDropsToTotalCounters { get; set; } = false;
        public int MaterialsRarityFilter { get; set; } = 6;

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
                {"Session", new CounterSet() },
                {"CurrentLab", new CounterSet() },
                {"Total", new CounterSet() },
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
        public static async Task Initalize(ConfigHelper config, LabController controller)
        {
            if (_instance == null)
            {
                _instance = new Counters(controller);
                await _instance.Load();
                _instance.DropCategories = (Counters.DropCategory)config.GetInt("counters.dropCategories", 15);
                _instance.LogDropsToTotalCounters = config.GetBool("counters.logDropsToTotal", false);
                _instance.MaterialsRarityFilter = config.GetInt("counters.materialsRarityFilter", 6);
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
            _instance.CounterSets["CurrentLab"].Reset(CounterSet.DataType.All);
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
        public static async Task EnemyIsUponYou()
        {
            await _instance.IncrementCounter("EnemyIsUponYou");
        }
        public static async Task FoundDrop(DropCategory category, string name, int rarity, int qty)
        {
            if (_instance.DropCategories.HasFlag(category)){

                if (category.Equals(DropCategory.EQUIPMENT))
                {
                    _instance.IncrementHE(name);
                    await _instance.IncrementCounter("HeroEquipmentGot");
                } else
                {
                    // Filter materials drops
                    if (!(DropCategory.LABYRINTH_ITEM | DropCategory.COMMON).HasFlag(category) && rarity > 0 && rarity < _instance.MaterialsRarityFilter) return;
                    _instance.IncrementDrop(name, qty);
                    await _instance.Save();
                }
            }
           
        }
        public static async Task FoundDrop(string dropType, string name, int rarity, int qty)
        {
            if (Enum.TryParse(dropType, out DropCategory category))
            {
                await FoundDrop(category, name, rarity, qty);
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Unknown drop type: {0}", dropType);
            }

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
                if (!set.Key.Equals("Total") || LogDropsToTotalCounters)
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
        private void IncrementDrop(string name, int amt = 1)
        {
            foreach (var set in CounterSets)
            {
                if (!set.Key.Equals("Total") || LogDropsToTotalCounters)
                {
                    if (set.Value.Drops.ContainsKey(name))
                    {
                        set.Value.Drops[name] += amt;
                    }
                    else
                    {
                        set.Value.Drops.Add(name, amt);
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
            OnUpdated?.Invoke();
            try
            {
                
                new FileInfo(path).Directory.Create();
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
        public static async Task Reset(string key, CounterSet.DataType types)
        {
            // Ugh I don't like doing this but it works.  Need to dump the current stopwatch and save it before resetting
            await _instance.Save();
            if (key == null)
            {
                foreach (var item in _instance.CounterSets)
                {
                    item.Value.Reset(types);
                }
            } else
            {
                _instance.CounterSets[key].Reset(types);
            }
            // Now save the reset values
            await _instance.Save();

        }

        public class CounterSet
        {
            [Flags]
            public enum DataType
            {
                All = ~0,
                Counters = 1 << 1,
                Runtime = 1 << 2,
                HeroEquipment = 1 << 3,
                Drops = 1 << 4
            }

            public Dictionary<string, int> Counters { get; set; }
            public Dictionary<string, TimeSpan> Runtime { get; set; }
            public SortedDictionary<string, int> HeroEquipment { get; set; }
            public SortedDictionary<string, int> Drops { get; set; }

            public CounterSet()
            {
                this.Counters = GetDefaultCounters();
                this.Runtime = GetDefaultRuntimes();
                this.HeroEquipment = new SortedDictionary<string, int>();
                this.Drops = new SortedDictionary<string, int>();
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
                    {"EnemyIsUponYou",0},
                };
            }

            private Dictionary<string, TimeSpan> GetDefaultRuntimes()
            {
                return new Dictionary<string, TimeSpan>()
                {
                    {"Battle", new TimeSpan()},
                    {"Total", new TimeSpan()},
                };
            }

            public void Reset(DataType types)
            {
                if (types.HasFlag(DataType.Counters)) this.Counters = GetDefaultCounters();
                if (types.HasFlag(DataType.Runtime)) this.Runtime = GetDefaultRuntimes();
                if (types.HasFlag(DataType.HeroEquipment)) this.HeroEquipment = new SortedDictionary<string, int>();
                if (types.HasFlag(DataType.Drops)) this.Drops = new SortedDictionary<string, int>();
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
