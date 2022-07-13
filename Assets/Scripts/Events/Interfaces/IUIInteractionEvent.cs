using Assets.Scripts.UI;
using System.Collections.Generic;

namespace Assets.Scripts.Events.Interfaces
{
    internal interface IUIInteractionEvent : IOnceEvent
    {
        IEnumerable<BaseEvent> Process(UIManager uIManager, BoardState boardState);
    }
}
