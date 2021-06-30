using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFRK_LabMem.Services;
using Newtonsoft.Json.Linq;
using SharpAdbClient;
using Stateless;
using Stateless.Graph;

namespace FFRK_LabMem.Machines
{
    public class Lab : Machine
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

        public Adb Adb { get; set; }
        public StateMachine<State, Trigger> StateMachine { get; set; }
        public JArray Paintings { get; set; }
        public JObject CurrentPainting { get; set; }

        public Lab(Adb adb)
        {

            this.Adb = adb;
            this.StateMachine = new StateMachine<State, Trigger>(State.Starting);

            this.StateMachine.Configure(State.Starting)
                .Permit(Trigger.Started, State.Unknown);

            this.StateMachine.Configure(State.Unknown)
                .OnEntry(t => DetermineState())
                .Permit(Trigger.ResetState, State.Ready);

            this.StateMachine.Configure(State.Ready)
                .OnEntryAsync(t => PickPainting())
                .Permit(Trigger.PickPainting, State.PickConfirm)
                .PermitReentry(Trigger.ResetState);

            this.StateMachine.Configure(State.PickConfirm)
                .SubstateOf(State.Ready)
                .OnEntryAsync(t => PickPaintingConfirm())
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

        public override void RegisterWithProxy(Proxy Proxy)
        {
            Proxy.AddRegistration("get_display_paintings", this);
        }

        public override void PassFromProxy(string UrlContained, JObject data)
        {
            switch (UrlContained)
            {
                case "get_display_paintings":
                    this.Paintings = (JArray)data["labyrinth_dungeon_session"]["display_paintings"];
                    this.StateMachine.Fire(Trigger.ResetState);
                    break;

            }
        }

        private void DetermineState()
        {

            // Get screen dimensions
            Console.WriteLine("Detected display size: {0}x{1}", this.Adb.ScreenSize.Width, this.Adb.ScreenSize.Height);

           
        }

        private async Task PickPainting()
        {

            // Logic to determine painting here
            
            await Task.Delay(1000);
            Console.WriteLine("Picking painting 2");
            this.CurrentPainting = (JObject)this.Paintings[2];
            await this.Adb.TapPct(50, 50);

        }

        private async Task PickPaintingConfirm()
        {
            await Task.Delay(1000);
            int num = (int)this.CurrentPainting["num"];
            Console.WriteLine("Confirm painting {0}", num);
            await this.Adb.TapPct(17 + (33 * (num-1)), 50);
        }
                
    }
}
