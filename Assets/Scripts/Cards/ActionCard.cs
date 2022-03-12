using Assets.Scripts.Enums;
using Assets.Scripts.Managers;
using System.Linq;

namespace Assets.Scripts.Cards
{
    public class ActionCard : BaseCard
    {
        public ActionCard(CardInfo cardInfo) : base(cardInfo)
        {
            CardEvents = CardEffectsManager.GetCardEvents(this).ToList();
        }

        public override CardType Type => CardType.Action;
    }
}
