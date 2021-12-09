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
            Drops = 1 << 4
        }
        public Dictionary<string, int> Counters { get; set; }
        public Dictionary<string, TimeSpan> Runtime { get; set; }
        public SortedDictionary<string, int> HeroEquipment { get; set; }
        public SortedDictionary<string, int> Drops { get; set; }
        public string Name { get; set; }

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
        }

        /// <summary>
        /// Adds all the values of the specified counter set to this one
        /// </summary>
        /// <param name="from">The counter set to add values from</param>
        public void AddCounters(CounterSet from)
        {
            foreach (var item in this.Counters.Keys)
            {
                this.Counters[item] += from.Counters[item];
            }
            foreach (var item in this.Runtime.Keys)
            {
                this.Runtime[item] = this.Runtime[item].Add(from.Runtime[item]);
            }
            foreach (var item in this.HeroEquipment.Keys)
            {
                this.HeroEquipment[item] += from.HeroEquipment[item];
            }
            foreach (var item in this.Drops.Keys)
            {
                this.Drops[item] += from.Drops[item];
            }
        }

    }
}
