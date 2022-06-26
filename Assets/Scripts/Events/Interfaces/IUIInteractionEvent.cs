using Assets.Scripts.UI;
using System.Collections.Generic;

namespace Assets.Scripts.Events.Interfaces
{
    internal interface IUIInteractionEvent
    {
        IEnumerable<BaseEvent> Process(UIManager uIManager, BoardState boardState);
    }
}
