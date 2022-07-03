using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Effects;
using Assets.Scripts.IO;

namespace Assets.Scripts.Managers
{
    public static class DeckManager
    {
        private static readonly Dictionary<int, CardData> CardLibrary = DataIO.ReadAll<CardData>().ToDictionary(x => x.Id, x => x);

        private static readonly Dictionary<int, BaseEffect> EffectLibrary = Assembly.GetAssembly(typeof(BaseEffect))
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(BaseSourceEffect<CreatureCard>)) ||
                        x.IsSubclassOf(typeof(BaseSourceEffect<ActionCard>)) ||
                        x.IsSubclassOf(typeof(BaseSourceEffect<PermanentCard>)))
            .Select(x => (BaseEffect)Activator.CreateInstance(x))
            .ToDictionary(x => x.Id, x => x);

        public static List<CardInfo> GetDeck(string deckName)
        {
            var cardIds = PlayerIO.ReadDeck(deckName);

            return cardIds.Select(x => new CardInfo
            {
                CardData = CardLibrary[x],
                Effect = EffectLibrary.GetValueOrDefault(x),
            }).ToList();
        }
    }
}
