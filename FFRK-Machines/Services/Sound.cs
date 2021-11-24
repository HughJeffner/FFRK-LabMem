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
        private static SoundPlayer player;

        public static void Init()
        {
            player = new SoundPlayer(Properties.Resources.Silent);
            player.Play();
        }

        public static void PlayFanfaire()
        {
            player.Stream = Properties.Resources.FF1_Fanfaire;
            player.Play();
        }

        public static void PlayEvent()
        {
            player.Stream = Properties.Resources.FF1_Event;
            player.Play();
        }

    }
}
