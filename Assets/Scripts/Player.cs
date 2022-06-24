﻿using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Extensions;
using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Events;

namespace Assets.Scripts
{
    public class Player
    {
        public PlayerType PlayerType { get; set; }

        public Stat Health { get; set; } = new Stat(-99, 99, 50);
        public int TotalAttack => (FieldCreatures.Aggregate(0, (i, x) => i += x.Attack) + TokenAttack).Clamp(0, 99);
        public int TotalDefence => (FieldCreatures.Aggregate(0, (i, x) => i += x.Defence) + TokenDefence).Clamp(0, 99);

        public Stat RedMana { get; set; } = new Stat(0, 99, 10);
        public Stat GreenMana { get; set; } = new Stat(0, 99, 10);
        public Stat BlueMana { get; set; } = new Stat(0, 99, 10);
        public Stat PurpleMana { get; set; } = new Stat(0, 99, 10);

        public Queue<BaseCard> Deck { get; set; } = new();
        public HashSet<BaseCard> Hand { get; set; } = new();
        public FieldSlot[] Field { get; set; } = Enumerable.Range(1, 5).Select(x => new FieldSlot()).ToArray();
        public List<BaseCard> Destroyed { get; set; } = new();
        public List<BaseCard> Eliminated { get; set; } = new();
        public IEnumerable<FieldCard> FieldCards => Field.Select(x => x.Card).Where(x => x != null);
        public IEnumerable<CreatureCard> FieldCreatures => FieldCards.OfType<CreatureCard>();

        public List<TokenType> Tokens { get; set; } = new();
        private int TokenAttack => Tokens.Count(x => x == TokenType.Claw) - Tokens.Count(x => x == TokenType.Blunt);
        private int TokenDefence => Tokens.Count(x => x == TokenType.Shell) - Tokens.Count(x => x == TokenType.Cracked);

        public List<BaseItem> Items { get; set; } = new();

        public Player(PlayerType playerType, IEnumerable<CardInfo> cardInfos, IEnumerable<ItemInfo> itemInfos)
        {
            PlayerType = playerType;

            foreach (var cardInfo in cardInfos.OrderBy(x => Guid.NewGuid()))
            {
                Deck.Enqueue(BaseCard.Create(cardInfo));
            }

            foreach (var itemInfo in itemInfos)
            {
                Items.Add(BaseItem.Create(itemInfo));
            }
        }

        // Getting
        public FieldSlot GetRandomEmptySlot()
        {
            return Field.Where(x => x.Card == null)
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefault();
        }

        public List<TokenType> GetTokens(StatType statType)
        {
            if (statType == StatType.Attack)
                return Tokens.Where(x => x == TokenType.Claw || x == TokenType.Blunt).ToList();
            else if (statType == StatType.Defence)
                return Tokens.Where(x => x == TokenType.Shell || x == TokenType.Cracked).ToList();

            return Tokens.ToList();
        }

        // Exists
        public bool OwnsCard(BaseCard card)
        {
            return Hand
                .Concat(FieldCards)
                .Concat(Deck)
                .Concat(Destroyed)
                .Concat(Eliminated)
                .Contains(card);
        }

        public bool IsInDeck(BaseCard card)
        {
            return Deck.Contains(card);
        }

        public bool IsInHand(BaseCard card)
        {
            return Hand.Contains(card);
        }

        public bool IsOnField(BaseCard card)
        {
            return Field.Select(x => x.Card).Contains(card);
        }

        public bool IsInDestroyed(BaseCard card)
        {
            return Destroyed.Contains(card);
        }

        public bool IsInEliminated(BaseCard card)
        {
            return Eliminated.Contains(card);
        }

        public bool OwnsItem(BaseItem item)
        {
            return Items.Contains(item);
        }


        // Adding
        public void AddToDeck(BaseCard card)
        {
            Deck.Enqueue(card);
        }

        public void AddToHand(BaseCard card)
        {
            Hand.Add(card);
        }

        // Should never try to add to an occupied slot
        public IEnumerable<BaseEvent> AddToField(FieldCard card, int index)
        {
            return Field[index].Add(card);
        }

        public void AddToDestroyed(BaseCard card)
        {
            Destroyed.Add(card);
        }

        public void AddToEliminated(BaseCard card, bool gainMana = true)
        {
            if(gainMana)
                AddMana(card.Colour, 1);

            Eliminated.Add(card);
        }


        // Removing
        public BaseCard TakeFromDeck()
        {
            var card = Deck.Dequeue();
            return card;
        }

        public void RemoveFromHand(BaseCard card)
        {
            Hand.Remove(card);
        }

        public IEnumerable<BaseEvent> RemoveFromField(FieldCard card)
        {
            return Field.Single(x => x.Card == card).Remove();
        }

        public void RemoveFromDeck(BaseCard card)
        {
            Deck = new Queue<BaseCard>(Deck.Where(x => x != card));
        }

        public void RemoveFromDestroyed(BaseCard card)
        {
            Destroyed.Remove(card);
        }


        // Mana
        public int GetManaAmount(Colour colour)
        {
            switch (colour)
            {
                case Colour.Red:
                    return RedMana.Get();
                case Colour.Green:
                    return GreenMana.Get();
                case Colour.Blue:
                    return BlueMana.Get();
                case Colour.Purple:
                    return PurpleMana.Get();
                default:
                    throw new ArgumentOutOfRangeException(nameof(colour), $"Colour must be either {Colour.Red}, {Colour.Green}, {Colour.Blue} or {Colour.Purple}");
            }
        }

        public void AddMana(Colour colour, int amount)
        {
            switch (colour)
            {
                case Colour.Red:
                    RedMana.Add(amount);
                    break;
                case Colour.Green:
                    GreenMana.Add(amount);
                    break;
                case Colour.Blue:
                    BlueMana.Add(amount);
                    break;
                case Colour.Purple:
                    PurpleMana.Add(amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(colour), $"Colour must be either {Colour.Red}, {Colour.Green}, {Colour.Blue} or {Colour.Purple}");
            }
        }

        public void RemoveMana(Colour colour, int amount)
        {
            switch (colour)
            {
                case Colour.Red:
                    RedMana.Remove(amount);
                    break;
                case Colour.Green:
                    GreenMana.Remove(amount);
                    break;
                case Colour.Blue:
                    BlueMana.Remove(amount);
                    break;
                case Colour.Purple:
                    PurpleMana.Remove(amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(colour), $"Colour must be either {Colour.Red}, {Colour.Green}, {Colour.Blue} or {Colour.Purple}");
            }
        }

        // Tokens
        public void AddTokens(TokenType tokenType, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Tokens.Add(tokenType);
            }
        }

        // Removes as many as possible
        public void RemoveTokens(TokenType tokenType, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Tokens.Remove(tokenType);
            }
        }
    }
}
