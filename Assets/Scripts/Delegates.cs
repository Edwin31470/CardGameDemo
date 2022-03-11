using System.Collections.Generic;
using Assets.Scripts.Cards;
using Assets.Scripts.Events;

namespace Assets.Scripts
{
    // Shorthand for Func<IEnumerable<BaseCard>,IEnumerable<BaseEvent>>
    public delegate IEnumerable<BaseEvent> FinishSelection(IEnumerable<BaseCard> cards);

    public delegate IEnumerable<BaseEvent> FinishAction(BaseCard owner);
}
