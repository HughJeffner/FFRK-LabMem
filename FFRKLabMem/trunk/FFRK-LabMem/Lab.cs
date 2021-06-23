using System;
using System.Collections.Generic;
using System.Drawing;
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
            Start,
            PaintingPicked,
            PaintingConfirmed,
            BattleCompleted,
            BattleFailed
        }

        public enum State
        {
            Starting,
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

            this.Device = device;
            this.StateMachine = new StateMachine<State, Trigger>(State.Starting);

            this.StateMachine.Configure(State.Starting)
                .Permit(Trigger.Start, State.Unknown);

            this.StateMachine.Configure(State.Unknown)
                .OnEntry(t => DetermineState());

            this.StateMachine.Configure(State.PickPainting)
                .Permit(Trigger.PaintingPicked, State.PickConfirm);

            this.StateMachine.Configure(State.PickConfirm)
                .SubstateOf(State.PickPainting)
                .Permit(Trigger.PaintingConfirmed, State.ExploreItem)
                .Permit(Trigger.PaintingConfirmed, State.Treasure);


            this.StateMachine.Fire(Trigger.Start);

        }

        private void DetermineState()
        {

            using (var framebuffer = AdbClient.Instance.GetFrameBufferAsync(this.Device, System.Threading.CancellationToken.None).Result)
            {
                using (Bitmap b = new Bitmap(framebuffer))
                {
                    Console.WriteLine(b.GetPixel(100, 200).ToString());
                }

            }

        }

    }
}
