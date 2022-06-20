using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.LegendkeeperIntegration;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public static class DeckManager
    {
        private static readonly Dictionary<int, CardData> CardLibrary = CardIO.ReadAll().ToDictionary(x => x.Id, x => x);

        private static readonly Dictionary<int, BaseCardEffect> EffectLibrary = Assembly.GetAssembly(typeof(BaseCardEffect))
            .GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(BaseCardEffect)))
            .Select(x => (BaseCardEffect)Activator.CreateInstance(x))
            .ToDictionary(x => x.CardId, x => x);

        public static List<CardInfo> GetDeck(string deckName)
        {
            var cardIds = DeckIO.ReadDeck(deckName);

            return cardIds.Select(x => new CardInfo
            {
                CardData = CardLibrary[x],
                GenerateEvents = EffectLibrary[x].GetEffect
            }).ToList();
        }

        // Card effects, ability effects, item effects, slot effects
        private abstract class BaseEffect
        {

        }

        private abstract class BaseCardEffect
        {
            public abstract int CardId { get; }
            public abstract IEnumerable<BaseEvent> GetEffect(BaseCard source);

            // Conditional methods
            protected static Func<BaseEvent, bool> IsDestroyed(BaseCard target) {
                return triggeringEvent => triggeringEvent is DestroyCardEvent destroyCardEvent && destroyCardEvent.Card == target;
            }
        }

        private abstract class CustomPassiveCreatureSourceAllCreaturesEffect : BaseCardEffect
        {
            protected abstract TargetConditions TargetConditions { get; }

            public override IEnumerable<BaseEvent> GetEffect(BaseCard source)
            {
                yield return new CustomPassiveCreatureSourceAllCreaturesEvent(source, TargetConditions, Effect);
            }

            protected abstract void Effect(BoardState boardState, CreatureCard source, IEnumerable<CreatureCard> targets);
        }

        // Card effects that require no custom events
        private abstract class SimpleTargetEffect : BaseCardEffect
        {
            protected abstract TargetConditions TargetConditions { get; }
        }

        private abstract class CustomSingleTargetEffect : SimpleTargetEffect
        {
            protected abstract SelectionType SelectionType { get; }
            protected abstract string Message { get; }


            public override IEnumerable<BaseEvent> GetEffect(BaseCard source)
            {
                yield return new CustomSingleTargetEvent(source, TargetConditions, Effect, SelectionType, Message);
            }

            protected abstract IEnumerable<BaseEvent> Effect(BoardState boardState, BaseCard source, BaseCard target);
        }

        private class RoilingElementalEffect : CustomPassiveCreatureSourceAllCreaturesEffect
        {
            public override int CardId => 0;
            protected override TargetConditions TargetConditions => new TargetConditions {
                CardType = CardType.Creature,
                SubType = SubType.Elemental
            };

            protected override void Effect(BoardState boardState, CreatureCard source, IEnumerable<CreatureCard> targets)
            {
                var count = targets.Count(x => x != source);

                source.BonusAttack.Add(count);
                source.BonusDefence.Add(count);
            }
        }

        // TODO: simple trigger effect
        private class SmoulderingDraug : SimpleTargetEffect
        {
            // Destroy target creature
            public override int CardId => 1;
            protected override TargetConditions TargetConditions => new TargetConditions
            {
                CardType = CardType.Creature
            };

            public override IEnumerable<BaseEvent> GetEffect(BaseCard source)
            {
                yield return new DestroyTargetsEvent(TargetConditions, 1);
            }
        }
    }
}
