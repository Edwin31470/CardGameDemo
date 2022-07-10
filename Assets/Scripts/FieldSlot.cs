using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Effects;
using Assets.Scripts.Effects.ItemEffects;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Events.Interfaces;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Terrains;

namespace Assets.Scripts
{
    public class FieldSlot : ITargetable
    {
        public FieldCard Occupier { get; set; }
        public BaseTerrain Terrain { get; set; }

        public BaseCard RemoveCard()
        {
            var card = Occupier;
            Occupier = null;
            return card;
        }

        public BaseTerrain RemoveTerrain()
        {
            var terrain = Terrain;
            Terrain = null;
            return terrain;
        }

        public void AddCard(FieldCard card)
        {
            Occupier = card;
        }

        public void AddTerrain(BaseTerrain terrain)
        {
            Terrain = terrain;
        }
    }

    //public abstract class BaseTriggerSlotEffect : BaseSlotEffect
    //{
    //    public override TriggerType TriggerType => TriggerType.Trigger;

    //    public abstract bool Conditions(BaseEvent triggeringEvent, BoardState board);
    //    public abstract IEnumerable<BaseEvent> GetEvents( BaseEvent triggeringEvent, BoardState board);
    //}

    //public class BlessedGround : BaseSlotEffect
    //{
    //    public override int Id => 0;

    //    public override TriggerType TriggerType => TriggerType.Trigger;

    //    public override TerrainEffectType EffectType => TerrainEffectType.Positive;


    //    public override IEnumerable<BaseEvent> GetEffect(FieldSlot source, BoardState board)
    //    {
    //        yield return new CustomInteruptEvent<FieldSlot>(source, PreventDamage);
    //    }

    //    public bool PreventDamage(FieldSlot source, BoardState board, BaseEvent triggeringEvent)
    //    {
    //        if (triggeringEvent is IDamageEvent damageEvent && damageEvent.BaseSource == source.Occupier)
    //        {
    //            damageEvent.Value =- 1;
    //            return true;
    //        }

    //        return false;
    //    }
    //}
}
