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
    public class CreatureCard : BaseCard
    {
        public Stat BaseAttack { get; set; }
        public Stat BaseDefence { get; set; }

        // From passive effects etc.
        public Stat BonusAttack { get; set; }
        public Stat BonusDefence { get; set; }

        public int Attack => BaseAttack.Get() + BonusAttack.Get();
        public int Defence => BaseDefence.Get() + BonusDefence.Get();

        public CreatureCard(PlayerType player, CardInfo cardInfo) : base(player, cardInfo)
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
