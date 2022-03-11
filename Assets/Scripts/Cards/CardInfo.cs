using Assets.Scripts.Enums;

namespace Assets.Scripts.Cards
{
    public class CardInfo
    {
        public string Id { get; set; }
        public Colour Colour { get; set; }
        public int Cost { get; set; }
        public CardType CardType { get; set; }
        public string Name { get; set; }
        public SubType SubTypes { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }
        public string EffectText { get; set; }
        public string Flavour { get; set; }
        public bool IsSummoned { get; set; }
        public bool HasPersistence { get; set; }
    }
}
