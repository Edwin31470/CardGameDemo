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
    public class AllOutAttack : BaseSourceEffect<PermanentCard>
    {
        public override int Id => 11;

        public override IEnumerable<BaseEvent> GetEffect(PermanentCard source, BoardState board)
        {
            yield return new CustomPassiveAllEvent<PermanentCard, CreatureCard>(
                source,
                new TargetConditions()
                {
                    PlayerType = board.GetSourceOwner(source).PlayerType
                },
                (source, board, creatures) =>
                {
                    foreach (var creature in creatures)
                    {
                        creature.BonusAttack.Add(5);
                        creature.BonusDefence.Remove(1);
                    }
                });
            yield return new CustomTriggerEvent<PermanentCard>(
                source,
                (source, board, triggeringEvent) => triggeringEvent is NewPhaseEvent phaseEvent && phaseEvent.Phase == Phase.Mana,
                (source, board, triggeringEvent) =>
                {
                    return new[] { new GameEndEvent($"{board.GetSourceOwner(source).PlayerType} Player loses due to All Out Attack!" ) };
                });
        }
    }
}
