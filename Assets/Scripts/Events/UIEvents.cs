using Assets.Scripts.Cards;
﻿using System.Collections.Generic;
using Assets.Scripts.Enums;
using Assets.Scripts.UI;
using UnityEngine.XR;

namespace Assets.Scripts.Events
{
    public class DrawCardUIEvent : BaseUIEvent
    {
        private PlayerType PlayerType { get; }
        private BaseCard Card { get; }
        private IEnumerable<BaseCard> HandCards { get; }

        public DrawCardUIEvent(PlayerType playerType, BaseCard card, IEnumerable<BaseCard> handCards)
        {
            PlayerType = playerType;
            Card = card;
            HandCards = handCards;
        }

        public override void Process(UIManager uIManager)
        {
            uIManager.CreateInHand(PlayerType, Card);
            uIManager.UpdateHand(PlayerType, HandCards);
        }
    }

    public class CreateCardInSlotUIEvent : BaseUIEvent
    {
        private PlayerType PlayerType { get; set; }
        private FieldCard Card { get; set; }
        private FieldSlot Slot { get; set; }

        public CreateCardInSlotUIEvent(PlayerType playerType, FieldCard card, FieldSlot slot)
        {
            PlayerType = playerType;
            Card = card;
            Slot = slot;
        }

        public override void Process(UIManager uIManager)
        {
            uIManager.CreateInSlot(PlayerType, Card, Slot);
        }
    }

    public class MoveCardToFieldUIEvent : BaseUIEvent
    {
        private FieldCard Card { get; set; }
        private FieldSlot Slot { get; set; }

        public MoveCardToFieldUIEvent(FieldCard card, FieldSlot slot)
        {
            Card = card;
            Slot = slot;
        }

        public override void Process(UIManager uIManager)
        {
            // TODO: Move to index instead
            uIManager.MoveToSlot(Card, Slot);
        }
    }

    public class ReturnToDeckUIEvent : BaseUIEvent
    {
        private PlayerType PlayerType { get; set; }
        private BaseCard Card { get; set; }

        public ReturnToDeckUIEvent(PlayerType playerType, BaseCard card)
        {
            PlayerType = playerType;
            Card = card;
        }

        public override void Process(UIManager uIManager)
        {
            uIManager.ReturnToDeck(PlayerType, Card);
        }
    }

    public class UpdateHandUIEvent : BaseUIEvent
    {
        private PlayerType PlayerType { get; set; }
        private IEnumerable<BaseCard> HandCards { get; set; }

        public UpdateHandUIEvent(PlayerType playerType, IEnumerable<BaseCard> handCards)
        {
            PlayerType = playerType;
            HandCards = handCards;
        }

        public override void Process(UIManager uIManager)
        {
            uIManager.UpdateHand(PlayerType, HandCards);
        }
    }

    public class DestroyCardUIEvent : BaseUIEvent
    {
        private PlayerType PlayerType { get; set; }
        private BaseCard Card { get; set; }

        public DestroyCardUIEvent(PlayerType playerType, BaseCard card)
        {
            PlayerType = playerType;
            Card = card;
        }

        public override void Process(UIManager uIManager)
        {
            uIManager.DestroyCard(PlayerType, Card);
        }
    }

    public class SacrificeCardUIEvent : BaseUIEvent
    {
        private BaseCard Card { get; set; }

        public SacrificeCardUIEvent(BaseCard card)
        {
            Card = card;
        }

        public override void Process(UIManager uIManager)
        {
            uIManager.SacrificeCard(Card);
        }
    }


    public class UpdateSlotGlowUIEvent : BaseUIEvent
    {
        private FieldSlot Slot { get; }
        private EffectType Type { get; }

        public UpdateSlotGlowUIEvent(FieldSlot slot, EffectType type)
        {
            Slot = slot;
            Type = type;
        }

        public override void Process(UIManager uIManager)
        {
            uIManager.UpdateSlotGlow(Slot, Type);
        }
    }
}
