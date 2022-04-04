using System;

namespace FFRK_Machines.Machines
{
    public class InvalidStateException<T, S> : InvalidOperationException
    {
        public T Trigger { get; set; }
        public S State { get; set; }
        public InvalidStateException(T trigger, S state) :
            base($"Trigger {trigger} not permitted for state {state}")
        {
            this.Trigger = trigger;
            this.State = state;
        }
    }
}
