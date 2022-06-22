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
    }
}
