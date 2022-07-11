using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Extensions;
using System.Collections.Generic;
using System.Linq;


namespace Assets.Scripts.Effects.CardEffects
{

    public class BirdOfParadise : BaseSourceEffect<CreatureCard>
    {
        public override int Id => 12;

        public override IEnumerable<BaseEvent> GetEffect(CreatureCard source, BoardState board)
        {
            var player = board.GetSourceOwner(source).PlayerType;

            yield return new AddManaEvent(player, Colour.Green, 1);
        }
    }

}
