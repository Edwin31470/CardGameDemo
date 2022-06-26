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
        private BoardState Board { get; set; }

        public PlayCardEvent(Player source, bool hasOtherPlayerPassed) : base(source)
        {
            HasOtherPlayerPassed = hasOtherPlayerPassed;
        }

        public override IEnumerable<BaseEvent> Process(UIManager uIManager, BoardState board)
        {
            var player = Source;

            // If no cards in hand, automatically pass
            if (!player.Hand.Any()) {
                return PassTurn();
            }

            uIManager.SetTurn(player.PlayerType);
            Board = board;

            var conditions = new TargetConditions {
                PlayerType = player.PlayerType,
                Area = Area.Hand
            };

            uIManager.BeginDragAndDrop(player.Hand, player.Field.Where(x => x.Card == null), player.PlayerType, PlayCard, SacrificeCard, PassTurn);

            // resulting events of PlayCard and PassTurn are handled in DragAndDropManager.Finish()
            return Enumerable.Empty<BaseEvent>();
        }

        protected IEnumerable<BaseEvent> PlayCard(BaseCard droppedCard, FieldSlot fieldSlot)
        {
            var player = Board.GetSourceOwner(droppedCard);

            // Don't play if unable to pay for card
            if (player.GetManaAmount(droppedCard.Colour) < droppedCard.Cost) {
                yield break;
            }

            // Pay and play card
            player.RemoveMana(droppedCard.Colour, droppedCard.Cost);
            yield return new EnterFieldEvent(droppedCard, fieldSlot);

            yield return new NewTurnEvent(Source.PlayerType.Opposite(), false);
        }

        private IEnumerable<BaseEvent> SacrificeCard(BaseCard card)
        {
            yield return new ManaSacrificeEvent(card);
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
