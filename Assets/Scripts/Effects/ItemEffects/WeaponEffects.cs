using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Events.Interfaces;
using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Effects.ItemEffects
{
    public class VitalCore : BaseSourceEffect<Item>
    {
        public override int Id => 0;

        public override IEnumerable<BaseEvent> GetEffect(Item source, BoardState board)
        {
            var player = board.GetSourceOwner(source).PlayerType;

            yield return new MessageEvent("Adding mana from Vital Core", 1);
            yield return new AddManaEvent(player, Colour.Red, 1);
            yield return new AddManaEvent(player, Colour.Green, 1);
            yield return new AddManaEvent(player, Colour.Blue, 1);
            yield return new AddManaEvent(player, Colour.Purple, 1);

        }
    }

    public class WickedBlade : CustomInteruptEffect<Item>
    {
        public override int Id => 1;

        public override bool TryInterupt(Item source, BoardState boardState, BaseEvent triggeringEvent)
        {
            if (triggeringEvent is not IDamageEvent damageEvent)
                return false;

            if(boardState.GetSourceOwner(damageEvent.BaseSource) == boardState.GetSourceOwner(source))
                return false;

            damageEvent.Value += 1;

            return true;
        }
    }

    public class MoltenGauntlets : BaseSourceEffect<Item>
    {
        public override int Id => 2;

        public override IEnumerable<BaseEvent> GetEffect(Item source, BoardState board)
        {
            yield return new CustomOnGameStartEvent<Item>(source, OnGameStart);
            yield return new CustomOnRoundStartEvent<Item>(source, OnRoundStart);
        }

        private IEnumerable<BaseEvent> OnGameStart(Item source, BoardState board, BaseEvent triggeringEvent)
        {
            yield return new MessageEvent("Molten Gauntlets");
            yield return new AddTokensEvent(board.GetSourceOwner(source).PlayerType, TokenType.Claw, 6);
        }

        private IEnumerable<BaseEvent> OnRoundStart(Item source, BoardState board, BaseEvent triggeringEvent)
        {
            yield return new MessageEvent("Molten Gauntlets");
            yield return new AddTokensEvent(board.GetSourceOwner(source).PlayerType, TokenType.Cracked, 2);
        }
    }
}
