using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_LabMem.Data
{
    public class CounterSet
    {
        [Flags]
        public enum DataType
        {
            All = ~0,
            Counters = 1 << 1,
            Runtime = 1 << 2,
            HeroEquipment = 1 << 3,
            Drops = 1 << 4,
            QEDrops = 1 << 5
        }

        public enum FilterType
        {
            ExcludeQE = 0,
            IncludeQE = 1,
            OnlyQE = 2
        }
        public Dictionary<string, int> Counters { get; set; }
        public Dictionary<string, TimeSpan> Runtime { get; set; }
        public SortedDictionary<string, int> HeroEquipment { get; set; }
        public SortedDictionary<string, int> HeroEquipmentQE { get; set; } = new SortedDictionary<string, int>();
        public SortedDictionary<string, int> Drops { get; set; }
        public SortedDictionary<string, int> DropsQE { get; set; } = new SortedDictionary<string, int>();
        public string Name { get; set; }
        public DateTime LastCompleted { get; set; } = DateTime.MinValue;
        [JsonIgnore]
        public SortedDictionary<string, int> HeroEquipmentCombined
        {
            get
            {
                var ret = new SortedDictionary<string, int>(HeroEquipment);
                foreach (var item in HeroEquipmentQE.Keys)
                {
                    if (ret.ContainsKey(item))
                    {
                        ret[item] += HeroEquipmentQE[item];
                    }
                    else
                    {
                        ret.Add(item, HeroEquipmentQE[item]);
                    }
                }
                return ret;
            }
        }
        [JsonIgnore]
        public SortedDictionary<string, int> DropsCombined
        {
            get
            {
                var ret = new SortedDictionary<string, int>(Drops);
                foreach (var item in DropsQE.Keys)
                {
                    if (ret.ContainsKey(item))
                    {
                        ret[item] += DropsQE[item];
                    }
                    else
                    {
                        ret.Add(item, DropsQE[item]);
                    }
                }
                return ret;
            }
        }

        public SortedDictionary<string, int> GetHEFiltered(FilterType filter)
        {
            switch (filter)
            {
                case FilterType.IncludeQE:
                    return HeroEquipmentCombined;
                case FilterType.OnlyQE:
                    return HeroEquipmentQE;
                default:
                    return HeroEquipment;
            }
        }

        public SortedDictionary<string, int> GetDropsFiltered(FilterType filter)
        {
            switch (filter)
            {
                case FilterType.IncludeQE:
                    return DropsCombined;
                case FilterType.OnlyQE:
                    return DropsQE;
                default:
                    return Drops;
            }
        }

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
                    {"QuickExplores",0},
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

        /// <summary>
        /// Resets the counters in this set
        /// </summary>
        /// <param name="types">A flags enum of which types to reset</param>
        public void Reset(DataType types)
        {
            if (types.HasFlag(DataType.Counters)) this.Counters = GetDefaultCounters();
            if (types.HasFlag(DataType.Runtime)) this.Runtime = GetDefaultRuntimes();
            if (types.HasFlag(DataType.HeroEquipment)) this.HeroEquipment = new SortedDictionary<string, int>();
            if (types.HasFlag(DataType.Drops)) this.Drops = new SortedDictionary<string, int>();
            if (types.HasFlag(DataType.QEDrops))
            {
                this.DropsQE = new SortedDictionary<string, int>();
                this.HeroEquipmentQE = new SortedDictionary<string, int>();
            }
        }

        /// <summary>
        /// Adds all the values of the specified counter set to this one
        /// </summary>
        /// <param name="from">The counter set to add values from</param>
        public void AddCounters(CounterSet from)
        {
            foreach (var item in from.Counters.Keys)
            {
                if (this.Counters.ContainsKey(item))
                {
                    this.Counters[item] += from.Counters[item];
                }
                else
                {
                    this.Counters.Add(item, from.Counters[item]);
                }
            }
            foreach (var item in from.Runtime.Keys)
            {
                if (this.Runtime.ContainsKey(item))
                {
                    this.Runtime[item] = this.Runtime[item].Add(from.Runtime[item]);
                }
                else
                {
                    this.Runtime.Add(item, from.Runtime[item]);
                }
            }
            foreach (var item in from.HeroEquipment.Keys)
            {
                if (this.HeroEquipment.ContainsKey(item))
                {
                    this.HeroEquipment[item] += from.HeroEquipment[item];
                }
                else
                {
                    this.HeroEquipment.Add(item, from.HeroEquipment[item]);
                }
            }
            foreach (var item in from.Drops.Keys)
            {
                if (this.Drops.ContainsKey(item))
                {
                    this.Drops[item] += from.Drops[item];
                } 
                else
                {
                    this.Drops.Add(item, from.Drops[item]);
                }
                
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

    }
}
