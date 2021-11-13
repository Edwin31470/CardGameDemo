using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Extensions;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Events
{
    public class PlayCardEvent : BaseUIInteractionEvent
    {
        public override float Delay => 1f;
        public override string EventTitle => $"{PlayingPlayer} Player's Turn";

        private PlayerType PlayingPlayer { get; set; }
        private bool OtherPlayerPassed { get; set; }

        public PlayCardEvent(PlayerType playingPlayer, bool otherPlayerPassed)
        {
            PlayingPlayer = playingPlayer;
            OtherPlayerPassed = otherPlayerPassed;
        }

        public override void Process(UIManager uIManager)
        {
            if (MainController.GetPlayer(PlayingPlayer).GetHand().Count() == 0)
            {
                PassTurn();
                return;
            }

            var conditions = new TargetConditions
            {
                PlayerType = PlayingPlayer,
                Area = Area.Hand
            };

            uIManager.SetTurn(PlayingPlayer);
            uIManager.BeginDragAndDrop(conditions, PlayCard, PassTurn);
        }

        protected bool PlayCard(CardObject selectedCard, Slot slot)
        {
            var player = MainController.GetPlayer(PlayingPlayer);

            // Eliminate card if dropping in the mana slot
            if (slot.SlotType == SlotType.Mana)
            {
                MainController.AddEvent(new EliminateCardEvent(selectedCard.CardReference, false));
                selectedCard.Destroy();
            }
            // Don't play if unable to pay for card
            else if (player.GetManaAmount(selectedCard.CardReference.Colour) < selectedCard.CardReference.Cost)
            {
                return false;

            }
            else
            {
                player.RemoveMana(selectedCard.CardReference.Colour, selectedCard.CardReference.Cost);
                MainController.AddEvent(new EnterFieldEvent(selectedCard.CardReference));
            }

            MainController.AddEvent(new NewTurnEvent(PlayingPlayer.GetOpposite(), false));
            return true;
        }

        protected void PassTurn()
        {
            // If both players have now passed, go to the next phase
            if (OtherPlayerPassed)
            {
                MainController.AddEvent(new NewPhaseEvent(Phase.Damage));
                return;
            }

            MainController.AddEvent(new NewTurnEvent(PlayingPlayer.GetOpposite(), true));
        }
    }
}
