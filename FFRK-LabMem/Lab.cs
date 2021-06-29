using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SharpAdbClient;
using Stateless;
using Stateless.Graph;

namespace FFRK_LabMem
{
    public class Lab
    {
        public enum Trigger
        {
            Started,
            ResetState,
            PickPainting,
            PickPaintingConfirm,
            OpenDoor,
            DontOpenDoor,
            MoveOn,
            BattleConfirm,
            PartySelect,
            BattleSuccess,
            BattleFailed
        }

        public enum State
        {
            Starting,
            Unknown,
            Ready,
            PickConfirm,
            FoundItem,
            FoundTreasure,
            FoundBuffs,
            FoundSealedDoor,
            FoundBattle,
            PreBattle,
            Battle,
            Failed
        }

        public DeviceData Device { get; set; }
        public StateMachine<State, Trigger> StateMachine { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public Lab(DeviceData device)
        {

            this.Device = device;
            this.StateMachine = new StateMachine<State, Trigger>(State.Starting);

            this.StateMachine.Configure(State.Starting)
                .Permit(Trigger.Started, State.Unknown);

            this.StateMachine.Configure(State.Unknown)
                .OnEntry(t => DetermineState())
                .Permit(Trigger.ResetState, State.Ready);

            this.StateMachine.Configure(State.Ready)
                .Permit(Trigger.PickPainting, State.PickConfirm);

            this.StateMachine.Configure(State.PickConfirm)
                .SubstateOf(State.Ready)
                .Permit(Trigger.PickPaintingConfirm, State.FoundItem)
                .Permit(Trigger.PickPaintingConfirm, State.FoundBuffs)
                .Permit(Trigger.PickPaintingConfirm, State.FoundTreasure)
                .Permit(Trigger.PickPaintingConfirm, State.FoundBattle)
                .Permit(Trigger.PickPaintingConfirm, State.FoundSealedDoor);

            this.StateMachine.Configure(State.FoundItem)
                .Permit(Trigger.MoveOn, State.Ready);

            this.StateMachine.Configure(State.FoundBuffs)
                .Permit(Trigger.MoveOn, State.Ready);

            this.StateMachine.Configure(State.FoundTreasure)
                .Permit(Trigger.MoveOn, State.Ready);

            this.StateMachine.Configure(State.FoundSealedDoor)
                .Permit(Trigger.DontOpenDoor, State.Ready)
                .Permit(Trigger.OpenDoor, State.PreBattle)
                .Permit(Trigger.OpenDoor, State.FoundItem)
                .Permit(Trigger.OpenDoor, State.FoundTreasure);

            this.StateMachine.Configure(State.FoundBattle)
               .Permit(Trigger.BattleConfirm, State.PreBattle);
            
            this.StateMachine.Configure(State.PreBattle)
                .Permit(Trigger.PartySelect, State.Battle);

            this.StateMachine.Configure(State.Battle)
                .Permit(Trigger.BattleSuccess, State.Ready)
                .Permit(Trigger.BattleFailed, State.Failed);

            this.StateMachine.Fire(Trigger.Started);
            string graph = UmlDotGraph.Format(this.StateMachine.GetInfo());

        }

        private void DetermineState()
        {

            // Get screen dimensions
            using (var framebuffer = AdbClient.Instance.GetFrameBufferAsync(this.Device, System.Threading.CancellationToken.None).Result)
            {
                using (Bitmap b = new Bitmap(framebuffer))
                {
                    this.ScreenWidth = b.Width;
                    this.ScreenHeight = b.Height;
                    Console.WriteLine("Detected display: {0}x{1}", this.ScreenWidth, this.ScreenHeight);
                }

            }

        }

        public void OnPaintingsLoaded(JArray paintings)
        {

            this.StateMachine = new StateMachine<State, Trigger>(State.Ready);


        }

    }
}
