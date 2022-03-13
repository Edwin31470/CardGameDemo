using Assets.Scripts.Enums;

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
            BaseAttack = new Stat(-99, 99, cardInfo.CardData.Attack);
            BaseDefence = new Stat(-99, 99, cardInfo.CardData.Defence);

            BonusAttack = new Stat(-99, 99, 0);
            BonusDefence = new Stat(-99, 99, 0);
        }

        public override CardType Type => CardType.Creature;
    }
}
