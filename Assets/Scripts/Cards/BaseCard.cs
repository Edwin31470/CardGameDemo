using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using System.Collections.Generic;

namespace Assets.Scripts.Cards
{
    public abstract class BaseCard
    {
        public string Id { get; set; }
        public Colour Colour { get; set; }
        public int Cost { get; set; }
        public abstract CardType Type { get; }
        public string Name { get; set; }
        public SubType SubTypes { get; set; }
        public string Flavour { get; set; }

        public List<BaseEvent> CardEvents { get; set; }
        public string EffectText { get; set; }

        public bool IsSummoned { get; set; }
        public bool HasPersistence { get; set; }

        protected BaseCard(CardInfo cardInfo)
        {
            Colour = cardInfo.Colour;
            Cost = cardInfo.Cost;
            Name = cardInfo.Name;
            SubTypes = cardInfo.SubTypes;
            EffectText = cardInfo.EffectText;
            Flavour = cardInfo.Flavour;
            IsSummoned = cardInfo.IsSummoned;
            HasPersistence = cardInfo.HasPersistence;
        }

        public CardInfo ToCardInfo()
        {
            var cardInfo = new CardInfo
            {
                Colour = Colour,
                Cost = Cost,
                CardType = Type,
                Name = Name,
                SubTypes = SubTypes,
                EffectText = EffectText,
                Flavour = Flavour,
                IsSummoned = IsSummoned,
                HasPersistence = HasPersistence
            };

            if (this is CreatureCard creatureCard)
            {
                cardInfo.Attack = creatureCard.BaseAttack.Get();
                cardInfo.Defence = creatureCard.BaseDefence.Get();
            }

            return cardInfo;
        }

        // Factory method to create
        public static BaseCard Create(CardInfo cardInfo)
        {
            switch (cardInfo.CardType)
            {
                case CardType.Creature:
                    return new CreatureCard(cardInfo);
                case CardType.Action:
                    return new ActionCard(cardInfo);
                case CardType.Permanent:
                    return new PermanentCard(cardInfo);
                default:
                    throw new ArgumentOutOfRangeException(nameof(cardInfo.CardType), $"Type must be {CardType.Creature}, {CardType.Action} or {CardType.Permanent}");
            }
        }
    }
}
