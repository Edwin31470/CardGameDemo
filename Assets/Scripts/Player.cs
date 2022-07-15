using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Extensions;
using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Events;
using Assets.Scripts.Bases;
using Assets.Scripts.Managers;
using Assets.Scripts.Tokens;
using Assets.Scripts.Terrains;

namespace Assets.Scripts
{
    public class Player : BaseSource
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
        public IEnumerable<FieldCard> FieldCards => Field.Select(x => x.Occupier).Where(x => x != null);
        public IEnumerable<CreatureCard> FieldCreatures => FieldCards.OfType<CreatureCard>();
        public IEnumerable<BaseTerrain> Terrains => Field.Select(x => x.Terrain).Where(x => x != null);

        public List<BaseToken> Tokens { get; set; } = new();
        private int TokenCount(TokenType tokenType) => Tokens.Count(x => (TokenType)x.Id == tokenType);
        private int TokenAttack => TokenCount(TokenType.Claw) - TokenCount(TokenType.Blunt);
        private int TokenDefence => TokenCount(TokenType.Shell) - TokenCount(TokenType.Cracked);

        public List<Item> Items { get; set; } = new();

        private IEnumerable<IEnumerable<BaseSource>> SourceCollections => new IEnumerable<BaseSource>[]
        {
            Deck, Hand, FieldCards, Destroyed, Eliminated, Terrains, Tokens, Items
        };

        public Player(PlayerInfo playerInfo)
        {
            PlayerType = playerInfo.PlayerData.PlayerType;

            foreach (var cardInfo in playerInfo.Deck.Shuffle())
            {
                Deck.Enqueue(BaseCard.Create(cardInfo));
            }

            foreach (var itemInfo in playerInfo.Items)
            {
                Items.Add(Item.Create(itemInfo));
            }
        }

        // Getting
        public FieldSlot GetRandomEmptySlot()
        {
            return Field.Where(x => x.Occupier == null)
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefault();
        }

        // Exists
        public bool IsSourceOwner(BaseSource source)
        {
            foreach (var collection in SourceCollections)
            {
                if(collection.Contains(source))
                    return true;
            }

            return false;
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
            return Field.Select(x => x.Occupier).Contains(card);
        }

        public bool IsInDestroyed(BaseCard card)
        {
            return Destroyed.Contains(card);
        }

        public bool IsInEliminated(BaseCard card)
        {
            return Eliminated.Contains(card);
        }

        public bool OwnsItem(Item item)
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
        public void AddToField(FieldCard card, int index)
        {
            Field[index].AddCard(card);
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

        public void RemoveFromField(FieldCard card)
        {
            Field.Single(x => x.Occupier == card).RemoveCard();
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

        public BaseToken AddToken(TokenType tokenType)
        {
            var newToken = TokenManager.GetToken(tokenType);
            Tokens.Add(newToken);
            return newToken;
        }

        public void RemoveToken(TokenType tokenType)
        {
            var token = Tokens.FirstOrDefault(x => x.TokenType == tokenType);
            if (token != null)
                Tokens.Remove(token);
        }
    }
}
