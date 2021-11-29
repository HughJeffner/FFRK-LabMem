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

        public static void Init()
        {
            player = new SoundPlayer(Properties.Resources.Silent);
            player.Play();
        }

        public static void Play(string pathToWav)
        {
            player.SoundLocation = pathToWav;
            player.Play();
        }

    }
}
