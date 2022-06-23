using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Effects.CardEffects
{
    public class AllOutAttack : BaseCardEffect<PermanentCard>
    {
        public override int Id => 11;

        public override IEnumerable<BaseEvent> GetEffect(PermanentCard source, BoardState board)
        {
            yield return new CustomPassiveAllCreaturesEvent<PermanentCard>(
                source,
                new() { PlayerType = board.GetCardOwner(source).PlayerType},
                (source, board, creatures) =>
                {
                    foreach (var creature in creatures)
                    {
                        creature.BonusAttack.Add(5);
                        creature.BonusDefence.Remove(1);
                    }
                });
            yield return new CustomTriggerEvent(
                source,
                (triggeringEvent) => triggeringEvent is NewPhaseEvent phaseEvent && phaseEvent.Phase == Phase.Mana,
                (board, triggeringEvent) =>
                {
                    return new[] { new GameEndEvent($"{board.GetCardOwner(source).PlayerType} Player loses due to All Out Attack!" ) };
                });
        }
    }
}
