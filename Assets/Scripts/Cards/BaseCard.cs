using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Cards
{
    public abstract class BaseCard
    {
        public string Name { get; set; }
        public Colour Colour { get; set; }
        public int Cost { get; set; }
        public abstract CardType Type { get; }
        public SubType SubTypes { get; set; }
        public string Symbol { get; set; }
        public string EffectText { get; set; }
        public string FlavourText { get; set; }

        public bool HasPersistence { get; set; }
        public bool IsUnique { get; set; }

        public Func<IEnumerable<BaseEvent>> EffectEvents { get; set; }
        public bool IsSummoned { get; set; }


        protected BaseCard(CardInfo cardInfo)
        {
            Name = cardInfo.CardData.Name;
            Colour = cardInfo.CardData.Colour;
            Cost = cardInfo.CardData.Cost;
            SubTypes = cardInfo.CardData.SubTypes;
            Symbol = cardInfo.CardData.Symbol;
            EffectText = cardInfo.CardData.EffectText;
            FlavourText = cardInfo.CardData.FlavourText;
            HasPersistence = cardInfo.CardData.HasPersistence;
            IsUnique = cardInfo.CardData.IsUnique;

            EffectEvents = cardInfo.EffectEvents;
            IsSummoned = cardInfo.IsSummoned;
        }

        public CardInfo ToCardInfo()
        {
            var cardInfo = new CardInfo
            {
                CardData = new CardData
                {
                    Name = Name,
                    Colour = Colour,
                    Cost = Cost,
                    CardType = Type,
                    SubTypes = SubTypes,
                    EffectText = EffectText,
                    FlavourText = FlavourText,
                    HasPersistence = HasPersistence
                },
                EffectEvents = EffectEvents,
                IsSummoned = IsSummoned,
            };

            if (this is CreatureCard creatureCard)
            {
                cardInfo.CardData.Attack = creatureCard.BaseAttack.Get();
                cardInfo.CardData.Defence = creatureCard.BaseDefence.Get();
            }

            return cardInfo;
        }

        // Factory method to create
        public static BaseCard Create(CardInfo cardInfo)
        {
            switch (cardInfo.CardData.CardType)
            {
                case CardType.Creature:
                    return new CreatureCard(cardInfo);
                case CardType.Action:
                    return new ActionCard(cardInfo);
                case CardType.Permanent:
                    return new PermanentCard(cardInfo);
                default:
                    throw new ArgumentOutOfRangeException(nameof(cardInfo.CardData.CardType), $"Type must be {CardType.Creature}, {CardType.Action} or {CardType.Permanent}");
            }
        }
    }
}
