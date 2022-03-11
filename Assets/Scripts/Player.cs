﻿using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Extensions;
using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public class Player
    {
        public PlayerType PlayerType { get; set; }

        public Stat Health { get; set; }
        public int TotalAttack => (FieldCreatures.Aggregate(0, (i, x) => i += x.Attack) + TokenAttack).Clamp(0, 99);
        public int TotalDefence => (FieldCreatures.Aggregate(0, (i, x) => i += x.Defence) + TokenDefence).Clamp(0, 99);

        public Stat RedMana { get; set; }
        public Stat GreenMana { get; set; }
        public Stat BlueMana { get; set; }

        public Stat PurpleMana { get; set; }

        private Queue<BaseCard> Deck { get; set; }
        private HashSet<BaseCard> Hand { get; set; }
        private FieldSlot[] Field { get; set; }
        private List<BaseCard> DestroyedPile { get; set; }
        private List<BaseCard> EliminatedPile { get; set; }

        public IEnumerable<CreatureCard> FieldCreatures => Field
            .Select(x => x.Card)
            .OfType<CreatureCard>();


        public List<TokenType> Tokens { get; set; }
        private int TokenAttack => Tokens.Count(x => x == TokenType.Claw) - Tokens.Count(x => x == TokenType.Blunt);
        private int TokenDefence => Tokens.Count(x => x == TokenType.Shell) - Tokens.Count(x => x == TokenType.Cracked);


        public List<BaseItem> Items { get; set; }


        public Player(PlayerType playerType, IEnumerable<CardInfo> cardInfos)
        {
            PlayerType = playerType;

            Health = new Stat(-99, 99, 50);
            //RedMana = new Stat(0, 99, 0);
            //BlueMana = new Stat(0, 99, 0);
            //GreenMana = new Stat(0, 99, 0);
            //PurpleMana = new Stat(0, 99, 0);
            RedMana = new Stat(0, 99, 10);
            BlueMana = new Stat(0, 99, 10);
            GreenMana = new Stat(0, 99, 10);
            PurpleMana = new Stat(0, 99, 10);

            Deck = new Queue<BaseCard>();
            Hand = new HashSet<BaseCard>();
            Field = new FieldSlot[5];
            for (var i = 0; i < Field.Length; i++) {
                Field[i] = new FieldSlot();
            }
            DestroyedPile = new List<BaseCard>();
            EliminatedPile = new List<BaseCard>();
            Tokens = new List<TokenType>();

            foreach (var cardInfo in cardInfos.OrderBy(x => Guid.NewGuid()))
            {
                Deck.Enqueue(BaseCard.Create(this, cardInfo));
            }
        }

        // Counts
        public int DeckCount()
        {
            return Deck.Count;
        }


        // Getting
        public Queue<BaseCard> GetDeck()
        {
            return Deck;
        }

        public HashSet<BaseCard> GetHand()
        {
            return Hand;
        }

        public FieldSlot[] GetField()
        {
            return Field;
        }

        public int GetRandomEmptySlot()
        {
            var randomSlot = Field.Where(x => x.Card == null)
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefault();

            return randomSlot != null ? Array.IndexOf(Field, randomSlot) : -1;
        }

        public List<BaseCard> GetDestroyed()
        {
            return DestroyedPile;
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
            return DestroyedPile.Contains(card);
        }

        public bool IsInEliminated(BaseCard card)
        {
            return EliminatedPile.Contains(card);
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
        public void AddToField(FieldCard card, int index)
        {
            Field[index].Add(card);
        }

        public void AddToDestroyed(BaseCard card)
        {
            DestroyedPile.Add(card);
        }

        public void AddToEliminated(BaseCard card, bool gainMana = true)
        {
            if(gainMana)
                AddMana(card.Colour, 1);

            EliminatedPile.Add(card);
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

        public FieldCard RemoveFromField(int index)
        {
            return Field[index].Take();
        }

        public FieldCard RemoveFromField(FieldCard card)
        {
            return Field.Single(x => x.Card == card).Take();
        }

        public void RemoveFromDeck(BaseCard card)
        {
            Deck = new Queue<BaseCard>(Deck.Where(x => x != card));
        }

        public void RemoveFromDestroyed(BaseCard card)
        {
            DestroyedPile.Remove(card);
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
