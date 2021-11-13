using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Cards
{
    public abstract class BaseCard
    {
        public string Id { get; set; }
        public PlayerType Owner { get; set; }
        public Area Area { get; set; }
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

        public BaseCard(PlayerType player, CardInfo cardInfo)
        {
            Owner = player;
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
    }
}
