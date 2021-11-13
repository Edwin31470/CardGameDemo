using Assets.Scripts.Enums;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Cards
{
    public class PermanentCard : BaseCard
    {
        public PermanentCard(PlayerType player, CardInfo cardInfo) : base(player, cardInfo)
        {
            CardEvents = CardEffectsManager.GetCardEvents(this).ToList();
        }

        public override CardType Type => CardType.Permanent;
    }
}
