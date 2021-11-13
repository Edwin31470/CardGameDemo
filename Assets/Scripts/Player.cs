using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Extensions;
using Assets.Scripts.Items;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private HashSet<BaseCard> Field { get; set; }
        private List<BaseCard> DestroyedPile { get; set; }
        private List<BaseCard> EliminatedPile { get; set; }

        public IEnumerable<CreatureCard> FieldCreatures => Field.OfType<CreatureCard>();


        public List<TokenType> Tokens { get; set; }
        private int TokenAttack => Tokens.Count(x => x == TokenType.Claw) - Tokens.Count(x => x == TokenType.Blunt);
        private int TokenDefence => Tokens.Count(x => x == TokenType.Shell) - Tokens.Count(x => x == TokenType.Cracked);


        public List<BaseItem> Items { get; set; }


        public Player(PlayerType playerType, IEnumerable<CardInfo> cardInfos)
        {
            PlayerType = playerType;

            Health = new Stat(-99, 99, 50);
            RedMana = new Stat(0, 99, 0);
            BlueMana = new Stat(0, 99, 0);
            GreenMana = new Stat(0, 99, 0);
            PurpleMana = new Stat(0, 99, 0);
            //RedMana = new Stat(0, 99, 10);
            //BlueMana = new Stat(0, 99, 10);
            //GreenMana = new Stat(0, 99, 10);
            //PurpleMana = new Stat(0, 99, 10);

            Deck = new Queue<BaseCard>();
            Hand = new HashSet<BaseCard>();
            Field = new HashSet<BaseCard>();
            DestroyedPile = new List<BaseCard>();
            EliminatedPile = new List<BaseCard>();
            Tokens = new List<TokenType>();

            foreach (var cardInfo in cardInfos.OrderBy(x => Guid.NewGuid()))
            {
                switch (cardInfo.CardType)
                {
                    case CardType.Creature:
                        Deck.Enqueue(new CreatureCard(PlayerType, cardInfo));
                        break;
                    case CardType.Action:
                        Deck.Enqueue(new ActionCard(PlayerType, cardInfo));
                        break;
                    case CardType.Permanent:
                        Deck.Enqueue(new PermanentCard(PlayerType, cardInfo));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("cardInfo.Type", $"Type must be {CardType.Creature}, {CardType.Action} or {CardType.Permanent}");
                }
            }
        }

        // Counts
        public int DeckCount()
        {
            return Deck.Count();
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

        public HashSet<BaseCard> GetField()
        {
            return Field;
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

        // Adding
        public void AddToDeck(BaseCard card)
        {
            card.Area = Area.Deck;
            Deck.Enqueue(card);
        }

        public void AddToHand(BaseCard card)
        {
            card.Area = Area.Hand;
            Hand.Add(card);
        }

        public void AddToField(BaseCard card)
        {
            card.Area = Area.Field;
            Field.Add(card);
        }

        public void AddToDestroyed(BaseCard card)
        {
            card.Area = Area.Destroyed;
            DestroyedPile.Add(card);
        }

        public void AddToEliminated(BaseCard card, bool gainMana = true)
        {
            if(gainMana)
                AddMana(card.Colour, 1);

            card.Area = Area.Eliminated;
            EliminatedPile.Add(card);
        }


        // Removing
        public BaseCard TakeFromDeck()
        {
            var card = Deck.Dequeue();
            card.Area = Area.None;
            return card;
        }

        public void RemoveFromHand(BaseCard card)
        {
            Hand.Remove(card);
            card.Area = Area.None;
        }

        public void RemoveFromField(BaseCard card)
        {
            Field.Remove(card);
            card.Area = Area.None;
        }

        public void RemoveFromDeck(BaseCard card)
        {
            Deck = new Queue<BaseCard>(Deck.Where(x => x != card));
            card.Area = Area.None;
        }

        public void RemoveFromDestroyed(BaseCard card)
        {
            DestroyedPile.Remove(card);
            card.Area = Area.None;
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
                    throw new ArgumentOutOfRangeException("colour", $"Colour must be either {Colour.Red}, {Colour.Green}, {Colour.Blue} or {Colour.Purple}");
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
                    throw new ArgumentOutOfRangeException("colour", $"Colour must be either {Colour.Red}, {Colour.Green}, {Colour.Blue} or {Colour.Purple}");
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
                    throw new ArgumentOutOfRangeException("colour", $"Colour must be either {Colour.Red}, {Colour.Green}, {Colour.Blue} or {Colour.Purple}");
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
