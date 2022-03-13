using Assets.Scripts.Enums;

namespace Assets.Scripts.Cards
{
    public class ActionCard : BaseCard
    {
        public ActionCard(CardInfo cardInfo) : base(cardInfo)
        {

        }

        public override CardType Type => CardType.Action;
    }
}
