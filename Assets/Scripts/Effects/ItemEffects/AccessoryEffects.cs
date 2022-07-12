using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Effects.TerrainEffects;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Events.Interfaces;
using Assets.Scripts.Items;
using Assets.Scripts.Managers;
using Assets.Scripts.Terrains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Effects.ItemEffects
{
    public class BrandingIron : CustomOnRoundStartEffect<Item>
    {
        public override int Id => 5;

        public override IEnumerable<BaseEvent> OnRoundStart(Item source, BoardState boardState, BaseEvent triggeringEvent)
        {
            yield return new CustomTriggerEvent<Item>(source, Conditions, OnTrigger, true);
        }

        private bool Conditions(Item source, BoardState boardState, BaseEvent triggeringEvent)
        {
            return triggeringEvent is IDamageEvent damageEvent &&
                boardState.GetSourceOwner(damageEvent.Target) == boardState.GetSourceOwner(source) &&
                damageEvent.Target.Colour == Colour.Red;
        }

        private IEnumerable<BaseEvent> OnTrigger(Item source, BoardState boardState, BaseEvent triggeringEvent)
        {
            var target = ((IDamageEvent)triggeringEvent).Target;

            yield return new MessageEvent($"Branding Iron strengthens {target.Name} by 3");
            yield return new StrengthenCreatureEvent<Item>(source, target, 3);
        }
    }


    public class BlessedWord : CustomSingleTargetEffect<Item, FieldSlot>
    {
        public override int Id => 6;

        protected override SelectionType SelectionType => SelectionType.PositiveTarget;

        protected override string Message => "Choose a slot to add Blessed Soil to";

        protected override IEnumerable<BaseEvent> Effect(Item source, BoardState boardState, FieldSlot target)
        {
            yield return new AddTerrainToSlotEvent<Item>(source, BaseTerrain.Create(TerrainManager.GetTerrain("Blessed Soil")), target);
        }

        protected override TargetConditions GetTargetConditions(Item source, BoardState board)
        {
            return new();
        }
    }

    public class GoodSoup : CustomTriggerEffect<Item>
    {
        public override int Id => 7;

        public override bool Conditions(Item source, BoardState boardState, BaseEvent triggeringEvent)
        {
            return triggeringEvent is IFortifyEvent fortifyEvent &&
                boardState.GetSourceOwner(fortifyEvent.BaseSource) == boardState.GetSourceOwner(source) &&
                fortifyEvent.Value >= 3;
        }

        public override IEnumerable<BaseEvent> OnTrigger(Item source, BoardState boardState, BaseEvent triggeringEvent)
        {
            var target = ((IFortifyEvent)triggeringEvent).Target;

            yield return new MessageEvent($"Good Soup strengthens {target.Name} by 1");
            yield return new StrengthenCreatureEvent<Item>(source, target, 1);
        }
    }

    //public class LuckyCoin : CustomInteruptEffect<Item>
    //{
    //    public override int Id => 8;
    //}
}
