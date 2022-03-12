using Assets.Scripts.Enums;
using Assets.Scripts.Managers;
using System.Linq;

namespace Assets.Scripts.Cards
{
    public class CreatureCard : FieldCard
    {
        public Stat BaseAttack { get; set; }
        public Stat BaseDefence { get; set; }

        // From passive effects etc.
        public Stat BonusAttack { get; set; }
        public Stat BonusDefence { get; set; }

        public int Attack => BaseAttack.Get() + BonusAttack.Get();
        public int Defence => BaseDefence.Get() + BonusDefence.Get();

        public CreatureCard(CardInfo cardInfo) : base(cardInfo)
        {
            BaseAttack = new Stat(-99, 99, cardInfo.Attack);
            BaseDefence = new Stat(-99, 99, cardInfo.Defence);

            BonusAttack = new Stat(-99, 99, 0);
            BonusDefence = new Stat(-99, 99, 0);

            CardEvents = CardEffectsManager.GetCardEvents(this).ToList();
        }

        public override CardType Type => CardType.Creature;
    }
}
