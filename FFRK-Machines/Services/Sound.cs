using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_Machines.Services
{
    
    public class Sound
    {

        public static string FF1_Victory = "./Sounds/FF1-Victory.wav";
        public static string FF1_Event = "./Sounds/FF1-Event.wav";
        public static string FF1_Inn = "./Sounds/FF1-Inn.wav";
        public static string FF1_Treasure = "./Sounds/FF1-Treasure.wav";

        private static SoundPlayer player;

        public static async Task Initalize()
        {
            if (!OperatingSystem.IsWindows()) return;
            player = new SoundPlayer(Properties.Resources.Silent);
            player.Play();
            await Task.CompletedTask;
        }

        public static void Play(string pathToWav)
        {
            if (OperatingSystem.IsWindows()){
                player.SoundLocation = pathToWav;
                player.Play();
            }
            
        }

    }
}
