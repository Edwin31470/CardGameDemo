using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Extensions;

namespace Assets.Scripts
{
    public class TargetConditions
    {
        public PlayerType PlayerType { get; set; } = PlayerType.Either;
        public Area Area { get; set; } = Area.Field;
        public CardType CardType { get; set; } = CardType.Any;
        public Colour Colour { get; set; } = Colour.Any;
        public SubType SubType { get; set; } = SubType.Any;

        public int MinAttack { get; set; } = -100;
        public int MaxAttack { get; set; } = 100;
        public int MinDefence { get; set; } = -100;
        public int MaxDefence { get; set; } = 100;
        public int MinCost { get; set; } = 0;
        public int MaxCost { get; set; } = 3;

        public bool IsMatch(BaseCard card)
        {
            var owner = card.Owner;

            if (!PlayerType.HasFlag(card.Owner.PlayerType))
                return false;

            if (!IsAreaMatch(card, Area))
                return false;

            if (!CardType.HasFlag(card.Type))
                return false;

            if (!Colour.HasFlag(card.Colour))
                return false;

            if ((card.SubTypes == SubType.None && SubType != SubType.Any) || !SubType.HasFlag(card.SubTypes))
                return false;

            if (!card.Cost.IsBetween(MinCost, MaxCost))
                return false;

            if (card is CreatureCard creatureCard)
            {
                if (!creatureCard.Attack.IsBetween(MinAttack, MaxAttack))
                    return false;

                if (!creatureCard.Defence.IsBetween(MinDefence, MaxDefence))
                    return false;
            }

            return true;
        }

        private bool IsAreaMatch(BaseCard card, Area area)
        {
            if (card.Owner.IsInDeck(card) && !area.HasFlag(Area.Deck))
                return false;

            if (card.Owner.IsInHand(card) && !area.HasFlag(Area.Hand))
                return false;

            if (card.Owner.IsOnField(card) && !area.HasFlag(Area.Field))
                return false;

            if (card.Owner.IsInDestroyed(card) && !area.HasFlag(Area.Destroyed))
                return false;

            if (card.Owner.IsInEliminated(card) && !area.HasFlag(Area.Eliminated))
                return false;

            return true;
        }
    }
}
