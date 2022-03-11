using Assets.Scripts.Enums;
using Assets.Scripts.Extensions;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Cards;

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

        public override IEnumerable<BaseEvent> Process(UIManager uIManager, Func<PlayerType, Player> getPlayer)
        {
            // If no cards in hand, automatically pass
            if (!getPlayer(PlayingPlayer).GetHand().Any())
            {
                return PassTurn();
            }

            var conditions = new TargetConditions
            {
                PlayerType = PlayingPlayer,
                Area = Area.Hand
            };

            uIManager.SetTurn(PlayingPlayer);
            uIManager.BeginDragAndDrop(conditions, PlayCard, PassTurn);

            // resulting events of PlayCard and PassTurn are handled in DragAndDropManager.Finish()
            return Enumerable.Empty<BaseEvent>();
        }

        protected IEnumerable<BaseEvent> PlayCard(CardObject droppedCard, SlotObject slot)
        {
            var player = droppedCard.CardReference.Owner;

            // Eliminate card if dropping in the mana slot
            if (slot.SlotType == SlotType.Mana)
            {
                yield return new EliminateCardEvent(droppedCard.CardReference, false);
                droppedCard.Destroy();
            }
            // Don't play if unable to pay for card
            else if (player.GetManaAmount(droppedCard.CardReference.Colour) < droppedCard.CardReference.Cost)
            {
                yield break;
            }
            else
            {
                // Pay and play card
                player.RemoveMana(droppedCard.CardReference.Colour, droppedCard.CardReference.Cost);
                var fieldIndex = Array.IndexOf(player.GetField(), slot.FieldSlot);
                yield return new EnterFieldEvent(droppedCard.CardReference, fieldIndex);
            }

            yield return new NewTurnEvent(PlayingPlayer.GetOpposite(), false);
        }

        protected IEnumerable<BaseEvent> PassTurn()
        {
            // If both players have now passed, go to the next phase
            if (OtherPlayerPassed)
            {
                yield return new NewPhaseEvent(Phase.Damage);
                yield break;
            }

            yield return new NewTurnEvent(PlayingPlayer.GetOpposite(), true);
        }
    }
}
