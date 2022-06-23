﻿using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events
{
    public class NewPhaseEvent : BasePhaseEvent
    {
        public Phase Phase { get; set; }

        public NewPhaseEvent(Phase phase)
        {
            Phase = phase;
        }

        public override void Process(MainController controller)
        {
            controller.NewPhase(Phase);
        }
    }

    public class NewTurnEvent : BasePhaseEvent
    {
        private PlayerType PlayingPlayer { get; set; }
        private bool OtherPlayerPassed { get; set; }

        public NewTurnEvent(PlayerType playingPlayer, bool otherPlayerPassed)
        {
            PlayingPlayer = playingPlayer;
            OtherPlayerPassed = otherPlayerPassed;
        }

        public override void Process(MainController controller)
        { 
            controller.EnqueueEvent(new PlayCardEvent(PlayingPlayer, OtherPlayerPassed));
        }
    }
}
