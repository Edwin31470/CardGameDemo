using Assets.Scripts.Enums;
using Assets.Scripts.Extensions;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Cards;

namespace Assets.Scripts.Events
{
    public class PlayCardEvent : BaseUIInteractionEvent<Player>
    {
        public override float Delay => 1f;
        public override string EventTitle => $"{Source.PlayerType} Player's Turn";

        private bool HasOtherPlayerPassed { get; set; }

        public PlayCardEvent(Player source, bool hasOtherPlayerPassed) : base(source)
        {
            HasOtherPlayerPassed = hasOtherPlayerPassed;
        }

        public override IEnumerable<BaseEvent> Process(UIManager uIManager, BoardState board)
        {
            var availableCards = Source.Hand;
            var availableSlots = Source.Field.Where(x => x.Occupier == null);

            // If no cards in hand, automatically pass
            if (!availableCards.Any()) {
                foreach (var @event in PassTurn()) {
                    yield return @event;
                }
            }

            uIManager.SetTurn(Source.PlayerType);

            var conditions = new TargetConditions {
                PlayerType = Source.PlayerType,
                Area = Area.Hand
            };

            uIManager.BeginDragAndDrop(
                availableCards,
                availableSlots,
                Source.PlayerType,
                PlayCard,
                SacrificeCard,
                PassTurn);

            // resulting events of PlayCard and PassTurn are handled in DragAndDropManager.Finish()
            yield break;
        }

        protected IEnumerable<BaseEvent> PlayCard(BaseCard droppedCard, FieldSlot fieldSlot)
        {
            // Play again if unable to pay for card
            if (Source.GetManaAmount(droppedCard.Colour) < droppedCard.Cost) {
                yield return new MessageEvent("Unable to pay for card cost");
                yield return new PlayCardEvent(Source, false);
                yield break;
            }

            // Pay and play card
            Source.RemoveMana(droppedCard.Colour, droppedCard.Cost);

            if (droppedCard is FieldCard fieldCard) {
                yield return new EnterFieldEvent<FieldCard>(fieldCard, fieldSlot);
            } else if (droppedCard is ActionCard actionCard) {
                yield return new PlayActionCardEvent(actionCard);
            }

            yield return new NewTurnEvent(Source.PlayerType.Opposite(), false);
        }

        private IEnumerable<BaseEvent> SacrificeCard(BaseCard card)
        {
            yield return new ManaSacrificeEvent(Source, card);
            yield return new NewTurnEvent(Source.PlayerType.Opposite(), false);
        }

        protected IEnumerable<BaseEvent> PassTurn()
        {
            // If both players have now passed, go to the next phase
            if (HasOtherPlayerPassed)
            {
                yield return new NewPhaseEvent(Phase.Damage);
                yield break;
            }

            yield return new NewTurnEvent(Source.PlayerType.Opposite(), true);
        }
    }
}
