using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Effects;

namespace Assets.Scripts.Managers
{
    public static class DeckManager
    {
        private static readonly Dictionary<int, CardData> CardLibrary = CardIO.ReadAll().ToDictionary(x => x.Id, x => x);

        private static readonly Dictionary<int, BaseEffect> EffectLibrary = Assembly.GetAssembly(typeof(BaseEffect))
            .GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(BaseEffect)))
            .Select(x => (BaseEffect)Activator.CreateInstance(x))
            .ToDictionary(x => x.Id, x => x);

        public static List<CardInfo> GetDeck(string deckName)
        {
            var cardIds = DeckIO.ReadDeck(deckName);

            return cardIds.Select(x => new CardInfo
            {
                CardData = CardLibrary[x],
                GenerateEvents = EffectLibrary.TryGetValue(x, out var effect) ? effect.GenerateEffects : (x, y) => Enumerable.Empty<BaseEvent>()
            }).ToList();
        }
    }
}
