
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Effects;
using Assets.Scripts.Effects.ItemEffects;
using Assets.Scripts.Enums;
using Assets.Scripts.Events.Interfaces;
using Assets.Scripts.Terrains;
using Assets.Scripts.UI;

namespace Assets.Scripts.Events
{
    public class BaseSlotEffectEvent : BaseGameplayEvent
    {
    }

    public abstract class BaseUITargetSlotsEvent<T> : BaseUIInteractionEvent<T> where T : BaseSource
    {
        public int Count { get; set; }
        protected SelectionType SelectionType { get; set; }

        public IEnumerable<FieldSlot> OverrideTargets { get; set; } // For use with interupt events
        protected BoardState BoardState { get; set; } // Save the board state to be used in TriggerEffect

        protected BaseUITargetSlotsEvent(T source, int count, SelectionType selectionType) : base(source)
        {
            Count = count;
            SelectionType = selectionType;
        }

        // Begin card selection in UI
        public override IEnumerable<BaseEvent> Process(UIManager uIManager, BoardState board)
        {
            BoardState = BoardState;
            var allowableTargets = board.GetPlayerSlots(board.GetSourceOwner(Source).PlayerType);
            //uIManager.BeginTargeting(allowableTargets, OverrideTargets, Count, FinishSelection, SelectionType);

            // Events added to queue when targeting is complete
            yield break;
        }

        // Return new events
        public abstract IEnumerable<BaseEvent> FinishSelection(IEnumerable<FieldSlot> targets);
    }

    public abstract class BaseTargetSlotsEvent<T> : BaseUITargetSlotsEvent<T> where T : BaseSource
    {
        public BaseTargetSlotsEvent(T source, int count, SelectionType selectionType) : base(source, count, selectionType)
        {
        }
    }

    public class AddTerrainToSlotEvent<T> : BaseSourceEvent<T> where T : BaseSource
    {
        private BaseTerrain Terrain { get; set; }
        private FieldSlot Slot { get; set; }


        public AddTerrainToSlotEvent(T source, BaseTerrain terrain, FieldSlot slot) : base(source)
        {
            Terrain = terrain;
            Slot = slot;
        }

        public override IEnumerable<BaseEvent> Process(BoardState board)
        {
            Slot.AddTerrain(Terrain);
            yield return new AddTerrainUIEvent(Terrain, Slot);

            foreach (var terrainEvent in Terrain.GetEvents(board))
            {
                yield return terrainEvent;
            }
        }
    }

    public class CustomTriggerTerrainEnterEvent<T> : BaseTriggerEvent<BaseTerrain> where T : FieldCard
    {
        private Func<BaseTerrain, BoardState, T, IEnumerable<BaseEvent>> OnTrigger { get; set; }

        public CustomTriggerTerrainEnterEvent(BaseTerrain source, Func<BaseTerrain, BoardState, T, IEnumerable<BaseEvent>> onTrigger, bool triggerOnce = false) : base(source, triggerOnce)
        {
            OnTrigger = onTrigger;
        }

        public override bool Conditions(BoardState boardState, BaseEvent triggeringEvent)
        {
            if (triggeringEvent is IEnterFieldEvent enterFieldEvent &&
                enterFieldEvent.Slot.Terrain == Source &&
                enterFieldEvent.BaseSource is T) {
                return true;
            }

            return false;
        }

        public override IEnumerable<BaseEvent> Process(BoardState boardState, BaseEvent triggeringEvent)
        {
            var enterFieldEvent = (IEnterFieldEvent)triggeringEvent;
            var enteringCard = (T)enterFieldEvent.BaseSource;

            return OnTrigger.Invoke(Source, boardState, enteringCard);
        }
    }
}
