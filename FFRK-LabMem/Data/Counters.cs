using FFRK_LabMem.Config;
using FFRK_LabMem.Machines;
using FFRK_Machines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FFRK_LabMem.Data
{
    public class Counters
    {

        // Singleton instance
        private static Counters _instance = null;
        // Constants
        private const string CONFIG_PATH = "./Data/counters.json";
        public static readonly ReadOnlyDictionary<string, CounterSet> DefaultCounterSets = new ReadOnlyDictionary<string, CounterSet>(
            new Dictionary<string, CounterSet>
            {
                {"Session", new CounterSet() { Name = "Current Session" } },
                {"CurrentLab", new CounterSet() { Name = "Current Lab" } },
                {"Total", new CounterSet() { Name = "All Time" } },
            }
        );

        // Events
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

        // Public properties
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public Dictionary<string, CounterSet> CounterSets { get; set; } 
        public DropCategory DropCategories { get; set; } = DropCategory.EQUIPMENT | 
            DropCategory.LABYRINTH_ITEM | 
            DropCategory.COMMON | 
            DropCategory.SPHERE_MATERIAL;
        public bool LogDropsToTotalCounters { get; set; } = false;
        public int MaterialsRarityFilter { get; set; } = 6;
        public string CurrentLabId = null;

        // Private fields
        private CounterSet currentLabBufferSet { get; set; } = new CounterSet();
        private readonly LabController controller;
        private readonly Stopwatch runtimeStopwatch = new Stopwatch();

        private Counters(LabController controller)
        {
            this.CounterSets = DefaultCounterSets.Select(dict => dict).ToDictionary(pair => pair.Key, pair => pair.Value);
            this.controller = controller;
            controller.OnEnabled += Controller_OnEnabled;
            controller.OnDisabled += Controller_OnDisabled;
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
            CurrentLabId = null;
            currentLabBufferSet.Reset(CounterSet.DataType.All);
        }
        private void Controller_OnEnabled(object sender, EventArgs e)
        {
            runtimeStopwatch.Restart();
        }
        public static async Task LabRunCompleted()
        {
            // Increment counters
            await _instance.IncrementCounter("LabRunsCompleted", 1, false);
            
            // Reset the current lab counter set
            _instance.CounterSets["CurrentLab"].Reset(CounterSet.DataType.All);
            
            // Reset the current lab id and buffer since it is now unkown
            _instance.currentLabBufferSet.Reset(CounterSet.DataType.All);
            _instance.CurrentLabId = null;

            // Save to file
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
                    if (rarity == 0) rarity = InferRarity(category, name);
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
            foreach (var set in GetTargetCounterSets())
            {
                set.Value.Counters[key] += amt;
            }
            if (save) await _instance.Save();
        }
        private void IncrementRuntime(string key, TimeSpan amt)
        {
            if (amt.TotalMilliseconds <= 0) return;
            foreach (var set in GetTargetCounterSets())
            {
                set.Value.Runtime[key] += amt;
            }
        }
        private void IncrementHE(string name)
        {
            foreach (var set in GetTargetCounterSets())
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
            foreach (var set in GetTargetCounterSets())
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
        private List<KeyValuePair<string,CounterSet>> GetTargetCounterSets(){
            var ret = CounterSets.Where(s => DefaultCounterSets.ContainsKey(s.Key) || s.Key.Equals(CurrentLabId)).ToList();
            if (CurrentLabId == null) ret.Add(new KeyValuePair<string, CounterSet>("_Buffer", currentLabBufferSet));
            return ret;
        }
        private static int InferRarity(DropCategory category, string name)
        {

            // Motes - First character is a digit (star in name)
            if (category == DropCategory.SPHERE_MATERIAL && char.IsDigit(name[0]))
            {
                return int.Parse(name[0].ToString());
            }

            // Crystals/Orbs
            if (category == DropCategory.ABILITY_MATERIAL)
            {
                // Crystals are 6*
                if (name.EndsWith("Crystal")) return 6;

                // Orbs
                if (name.EndsWith("Orb"))
                {
                    if (name.StartsWith("Major")) return 5;
                    if (name.StartsWith("Greater")) return 4;
                    if (name.StartsWith("Lesser")) return 2;
                    if (name.StartsWith("Minor")) return 1;
                    return 3;
                }
            }

            // Upgrade materials
            if (category == DropCategory.EQUIPMENT_SP_MATERIAL)
            {
                if (name.EndsWith("Crystal")) return 6;
                if (name.StartsWith("Giant")) return 5;
                if (name.StartsWith("Large")) return 4;
                if (name.StartsWith("Small")) return 2;
                if (name.StartsWith("Tiny")) return 1;
                return 3;

            }

            // Tails
            if (category == DropCategory.HISTORIA_CRYSTAL_ENHANCEMENT_MATERIAL)
            {
                if (name.StartsWith("Huge")) return 5;
                if (name.StartsWith("Large")) return 4;
                if (name.StartsWith("Medium")) return 3;
                if (name.StartsWith("Small")) return 2;
                return 1; // Does not exist?
            }

            // Eggs
            if (category == DropCategory.GROW_EGG)
            {
                if (name.StartsWith("Major")) return 5;
                if (name.StartsWith("Greater")) return 4;
                if (name.StartsWith("Lesser")) return 2;
                if (name.StartsWith("Minor")) return 1;
                return 3;
            }

            // Arcana
            if (category == DropCategory.BEAST_FOOD)
            {
                if (name.StartsWith("Major")) return 5;
                if (name.StartsWith("Greater")) return 4;
                if (name.StartsWith("Lesser")) return 2;
                if (name.StartsWith("Minor")) return 1;  // Does not exist?
                return 3;
            }

            return 0;

        }
        private async void SetLab(string id, string name)
        {
            if (CurrentLabId == null || !CurrentLabId.Equals(id))
            {
                // Update or create entry here
                if (CounterSets.ContainsKey(id))
                {
                    CounterSets[id].AddCounters(currentLabBufferSet);
                }
                else
                {
                    // Create a new entry and add counters in the buffer to it
                    CounterSet newEntry = new CounterSet();
                    newEntry.Name = name;
                    newEntry.AddCounters(currentLabBufferSet);
                    CounterSets.Add(id, newEntry);
                }
                await Save();
                ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Current lab set to {0}", name);
            }
            // Reset counters in buffer
            currentLabBufferSet.Reset(CounterSet.DataType.All);
            CurrentLabId = id;
        }
        public static void SetCurrentLab(string id, string name)
        {
            _instance.SetLab(id, name);
        }
        public async Task Load(string path = CONFIG_PATH)
        {
            try
            {
                JsonConvert.PopulateObject(File.ReadAllText(path), CounterSets);
            }
            catch (Exception ex)
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Error loading counters file: {0}", ex);
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
        
    }
}
