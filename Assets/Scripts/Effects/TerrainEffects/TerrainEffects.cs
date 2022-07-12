using Assets.Scripts.Cards;
using Assets.Scripts.Events;
using Assets.Scripts.Terrains;
using System.Collections.Generic;

namespace Assets.Scripts.Effects.TerrainEffects
{
    public class BlessedSoil : BaseSourceEffect<BaseTerrain>
    {
        public override int Id => 0;

        public override IEnumerable<BaseEvent> GetEffect(BaseTerrain source, BoardState board)
        {
            yield return new CustomTriggerTerrainEnterEvent<CreatureCard>(source, GetStatEvents);
        }

        public IEnumerable<BaseEvent> GetStatEvents(BaseTerrain source, BoardState boardState, CreatureCard enteringCard)
        {
            yield return new StrengthenCreatureEvent<BaseTerrain>(source, enteringCard, 1);
            yield return new FortifyCreatureEvent<BaseTerrain>(source, enteringCard, 1);
        }
    }
}
