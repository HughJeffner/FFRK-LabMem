using System;

namespace FFRK_LabMem.Machines
{
    public class LabStaminaInfo
    {
        private const int STAMINA_SECONDS = 180;
        public int Cost { get; set; }
        public DateTime FullTime { get; private set; } = DateTime.MaxValue;
        public int MaxStamina { get; private set; }
        public int? Potions { get; set; } = null;

        public double CurrentStamina
        {
            get
            {
                var dff = (FullTime - DateTime.Now).TotalSeconds / STAMINA_SECONDS;
                return Math.Min(this.MaxStamina - dff, this.MaxStamina);
            }
        }

        public void SetStamina(int secondsUntilFull, int maxStamina)
        {
            this.FullTime = DateTime.Now.AddSeconds(secondsUntilFull);
            this.MaxStamina = maxStamina;
        }

        public DateTime GetTargetTime()
        {
            if (this.Cost == 0 || this.CurrentStamina < 0) return DateTime.MinValue;
            var remaining = this.Cost - this.CurrentStamina;
            return DateTime.Now.AddSeconds(remaining * STAMINA_SECONDS);
        }

        public void Clear()
        {
            this.Cost = 0;
            this.FullTime = DateTime.MaxValue;
            this.MaxStamina = 0;
            this.Potions = null;
        }
    }
}
