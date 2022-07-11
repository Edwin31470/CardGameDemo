using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Extensions;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class CardObject : BaseUIObject
    {
        public BaseCard CardReference => (BaseCard)SourceReference;

        private const int MoveSpeed = 5;
        private Vector2 TargetPosition { get; set; }

        // Small Card
        private GameObject SmallCard { get; set; }
        private SpriteRenderer Background { get; set; }
        private SpriteRenderer CardSymbol { get; set; }
        private SpriteRenderer CostSymbol { get; set; }
        private Text AttackLabel { get; set; }
        private Text DefenceLabel { get; set; }


        // Collider
        private Collider2D Collider { get; set; }
        public bool IsInPosition => Vector2.Distance(transform.position, TargetPosition) < 0.1;
        public bool DestroyWhenInPosition { get; set; }

        public void Initialize(BaseCard cardReference)
        {
            SourceReference = cardReference;

            var colour = cardReference.Colour.ToString();
            var cost = cardReference.Cost;

            // Small Card
            SmallCard = transform.Find("SmallCard").gameObject;

            AttackLabel = SmallCard.transform.Find("Canvas/Attack").GetComponent<Text>();
            DefenceLabel = SmallCard.transform.Find("Canvas/Defence").GetComponent<Text>();

            AttackLabel.text = string.Empty;
            DefenceLabel.text = string.Empty;

            Background = SmallCard.GetComponent<SpriteRenderer>();
            CardSymbol = SmallCard.transform.Find("CardSymbol").GetComponent<SpriteRenderer>();
            CostSymbol = SmallCard.transform.Find("CostSymbol").GetComponent<SpriteRenderer>();

            Background.sprite = Resources.Load<Sprite>($"Sprites/{colour}CardSmall{CardReference.Type}");

            CostSymbol.sprite = cost != 0 ? Resources.Load<Sprite>($"Sprites/Symbols/Cost{cost}") : null;

            CardSymbol.sprite = CardGraphicsManager.GetSymbolSprite(CardReference);
            CardSymbol.color = CardGraphicsManager.GetSymbolColor(CardReference).Normalise();

            // Find Collider
            Collider = GetComponent<Collider2D>();
        }

        public void SetSortingLayer(string layer)
        {
            SmallCard.GetComponent<SortingGroup>().sortingLayerName = layer;
            SmallCard.transform.Find("Canvas").GetComponent<Canvas>().sortingLayerName = layer;
            Background.sortingLayerName = layer;
            CardSymbol.sortingLayerName = layer;
            CostSymbol.sortingLayerName = layer;
        }

        public void SetSortingOrder(int order)
        {
            SmallCard.GetComponent<SortingGroup>().sortingOrder = order;
        }

        // Movement
        public void SetTargetPosition(Vector2 position)
        {
            TargetPosition = position;
        }

        public void MoveToPosition(Vector2 position)
        {
            transform.position = position;
            TargetPosition = position;
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        void Update()
        {
            if (DestroyWhenInPosition && IsInPosition)
            {
                Destroy();
                return;
            }

            // Update Labels
            if (SourceReference is CreatureCard creatureCard)
            {
                AttackLabel.text = creatureCard.Attack.Clamp(0, 99).ToString();
                DefenceLabel.text = creatureCard.Defence.Clamp(0, 99).ToString();
            }

            // Move
            transform.position = Vector2.Lerp(transform.position, TargetPosition, Time.deltaTime * MoveSpeed);
        }
    }
}