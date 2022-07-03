﻿using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.IO;
using Assets.Scripts.Items;
using System.Collections.Generic;

namespace Assets.Scripts.Managers
{
    public static class PlayerManager
    {
        public static PlayerInfo GetPlayer(PlayerType playerType)
        {
            var playerData = PlayerIO.ReadPlayer(playerType);
            var deck = DeckManager.GetDeck(playerData);
            var items = ItemManager.GetItems(playerData);

            return new PlayerInfo
            {
                PlayerData = playerData,
                Deck = deck,
                Items = items
            };
        }
    }

    public class PlayerInfo
    {
        public PlayerData PlayerData { get; set; }
        public IEnumerable<CardInfo> Deck { get; set; }
        public IEnumerable<ItemInfo> Items { get; set; }
    }
}
