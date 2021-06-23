using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpAdbClient;
using Stateless;

namespace FFRK_LabMem
{
    public class Lab
    {
        public enum Trigger
        {
            PaintingPicked,
            PaintingConfirmed,
            BattleCompleted,
            BattleFailed
        }

        public enum State
        {
            Unknown,
            PickPainting,
            PickConfirm,
            ExploreItem,
            Treasure,
            Buffed,
            CombatConfirm,
            Party,
            FatigueWarning,
            Battle,
            BattleSuccess,
            BattleSuccessLoot,
            BattleFailed
        }

        public DeviceData Device;
        public StateMachine<State, Trigger> StateMachine;

        public Lab(DeviceData device)
        {
           
            this.StateMachine = new StateMachine<State, Trigger>(State.Unknown);
            this.StateMachine.Configure(State.Unknown)
                .OnEntry(t => DetermineState());

            this.StateMachine.Configure(State.PickPainting)
                .Permit(Trigger.PaintingPicked, State.PickConfirm);

            this.StateMachine.Configure(State.PickConfirm)
                .SubstateOf(State.PickPainting)
                .Permit(Trigger.PaintingConfirmed, State.ExploreItem)
                .Permit(Trigger.PaintingConfirmed, State.Treasure);


        }

        private void DetermineState()
        {

        }

    }
}
