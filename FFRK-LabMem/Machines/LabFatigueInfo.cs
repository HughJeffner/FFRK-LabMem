using FFRK_Machines;
using FFRK_Machines.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FFRK_LabMem.Machines
{
    public class LabFatigueInfo : List<LabFatigueInfo.BuddyInfoList>
    {

        readonly AsyncAutoResetEvent AutoResetEventFatigue = new AsyncAutoResetEvent(false);

        public LabFatigueInfo(int parties = 0, int members = 0)
        {
            if (parties <= 0) return;
            Enumerable.Range(0, parties).ToList().ForEach(arg => this.Add(new BuddyInfoList(members)));
        }

        public class BuddyInfo
        {
            public int BuddyId { get; set; }
            public int Fatigue { get; set; } = 3;
            public override string ToString()
            {
                return Fatigue.ToString();
            }
        }

        public class BuddyInfoList : List<BuddyInfo>
        {
            public BuddyInfoList(int members = 5)
            {
                if (members <= 0) return;
                Enumerable.Range(0, members).ToList().ForEach(arg => this.Add(new BuddyInfo()));
            }
            public override string ToString()
            {
                return $"[{String.Join(",", this)}]";
            }
        }

        public new void Clear()
        {
            Reset("CLEAR");
            base.Clear();
        }

        public Task<bool> Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var ret = AutoResetEventFatigue.WaitAsync(timeout, cancellationToken);
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, $"Fatigue values reset READ: {AutoResetEventFatigue}");
            return ret;
        }

        public void Reset(string description)
        {
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, $"Fatigue values reset {description}: {AutoResetEventFatigue}");
            AutoResetEventFatigue.Reset();
        }

        public void Set(string description)
        {
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, $"Fatigue values set {description}: {AutoResetEventFatigue}");
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, $"{this}");
            AutoResetEventFatigue.Set();
        }

        public void UpdateBattle(int selectedPartyIndex)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (i == selectedPartyIndex)
                {
                    this[i].ForEach(f => f.Fatigue = Math.Min(f.Fatigue + 2, 10));
                }
                else
                {
                    this[i].ForEach(f => f.Fatigue = Math.Max(f.Fatigue - 1, Math.Min(f.Fatigue, 3)));
                }
            }
            Set("BATTLE");
        }

        public void UpdateTears(int selectedPartyIndex, List<int> selectedUnits)
        {
            selectedUnits.ForEach(i => this[selectedPartyIndex][i].Fatigue = 0);
            Set("TEARS");
        }

        public List<int> UnitsOverThreshold(int selectedPartyIndex, byte selectedUnits, int fatigueThreshold)
        {

            // No data
            if (selectedPartyIndex >= this.Count) return new List<int>();

            // Any of the specified units for the party above threshold?
            return this[selectedPartyIndex].Where(buddy =>
            (selectedUnits & (1 << 4 - this[selectedPartyIndex].IndexOf(buddy))) != 0 &&
                buddy.Fatigue >= fatigueThreshold
            ).Select(r => this[selectedPartyIndex].IndexOf(r)).ToList();

        }

        public bool IsOverThreshold(int selectedPartyIndex, List<byte> selectedUnits, int fatigueThreshold)
        {
            // No data
            if (selectedPartyIndex >= this.Count) return false;

            // Any of the specified units for the party above threshold?
            return this[selectedPartyIndex].Any(buddy =>
                (selectedUnits[selectedPartyIndex] & (1 << 4 - this[selectedPartyIndex].IndexOf(buddy))) != 0 &&
                buddy.Fatigue >= fatigueThreshold
            );
        }

        public bool IsOverThreshold(int selectedPartyIndex, int fatigueThreshold)
        {
            // No data
            if (selectedPartyIndex >= this.Count) return false;

            // Average of the party above threshold?
            return this[selectedPartyIndex].Average(buddy => buddy.Fatigue) >= fatigueThreshold;

        }

        public bool IsOverThreshold(int fatigueThreshold)
        {
            // Average of all units above threshold?
            return this.SelectMany(p => p).Average(buddy => buddy.Fatigue) >= fatigueThreshold;
        }

        public override string ToString()
        {
            return this.Count == 0 ? "[no fatigue values]" : String.Join(" ", this);
        }

    }
}
