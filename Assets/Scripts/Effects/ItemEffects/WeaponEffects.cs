using Assets.Scripts.Bases;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Effects.ItemEffects
{
    public class VitalCore : BaseItemEffect
    {
        public override int Id => 0;

        public override IEnumerable<BaseEvent> GenerateEffects(BaseItem source, BoardState board)
        {
            var player = board.GetItemOwner(source).PlayerType;

            yield return new MessageEvent("Adding mana from Vital Core", 1);
            yield return new AddManaEvent(player, Colour.Red, 1);
            yield return new AddManaEvent(player, Colour.Green, 1);
            yield return new AddManaEvent(player, Colour.Blue, 1);
            yield return new AddManaEvent(player, Colour.Purple, 1);

        }
    }
}
