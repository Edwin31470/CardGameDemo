using Assets.Scripts.Cards;
using Assets.Scripts.Extensions;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class CardObject : MonoBehaviour
    {
        private const int MoveSpeed = 5;
        public BaseCard CardReference { get; private set; }
        private Vector2 TargetPosition { get; set; }

        // Small Card
        private GameObject SmallCard { get; set; }
        private SpriteRenderer Background { get; set; }
        private SpriteRenderer CardSymbol { get; set; }
        private SpriteRenderer CostSymbol { get; set; }
        private Text AttackLabel { get; set; }
        private Text DefenceLabel { get; set; }
        private SpriteRenderer HighlightSprite { get; set; }

        // Large Card
        private GameObject LargeCard { get; set; }
        private Text NameLabel { get; set; }
        private Text EffectTextLabel { get; set; }
        private Text TypesLabel { get; set; }

        // Collider
        private Collider2D Collider { get; set; }
        public bool IsInPosition => Vector2.Distance(transform.position, TargetPosition) < 0.1;
        public bool DestroyWhenInPosition { get; set; }

        public void Initialize(BaseCard cardReference)
        {
            CardReference = cardReference;

            var colour = CardReference.Colour.ToString();
            var cost = CardReference.Cost;

            // Small Card
            SmallCard = transform.Find("SmallCard").gameObject;

            AttackLabel = SmallCard.transform.Find("Canvas/Attack").GetComponent<Text>();
            DefenceLabel = SmallCard.transform.Find("Canvas/Defence").GetComponent<Text>();
            HighlightSprite = SmallCard.transform.Find("Highlight").GetComponent<SpriteRenderer>();

            AttackLabel.text = string.Empty;
            DefenceLabel.text = string.Empty;

            Background = SmallCard.GetComponent<SpriteRenderer>();
            CardSymbol = SmallCard.transform.Find("CardSymbol").GetComponent<SpriteRenderer>();
            CostSymbol = SmallCard.transform.Find("CostSymbol").GetComponent<SpriteRenderer>();

            Background.sprite = Resources.Load<Sprite>($"Sprites/{colour}CardSmall{CardReference.Type}");

            CostSymbol.sprite = cost != 0 ? Resources.Load<Sprite>($"Sprites/Symbols/Cost{cost}") : null;

            CardSymbol.sprite = CardGraphicsManager.GetSymbolSprite(CardReference);
            CardSymbol.color = CardGraphicsManager.GetSymbolColor(CardReference).Normalise();


            // Large Card
            LargeCard = transform.Find("LargeCard").gameObject;

            LargeCard.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Sprites/{colour}CardLarge");

            // Set Large Card Position
            LargeCard.SetActive(false);
            if (true) //cardReference.Owner.PlayerType == PlayerType.Back) // TODO: show based on current card position
                LargeCard.transform.localPosition = new Vector3(LargeCard.transform.localPosition.x, -LargeCard.transform.localPosition.y, LargeCard.transform.localPosition.z);

            NameLabel = LargeCard.transform.Find("Canvas/Name").GetComponent<Text>();
            EffectTextLabel = LargeCard.transform.Find("Canvas/EffectText").GetComponent<Text>();
            TypesLabel = LargeCard.transform.Find("Canvas/Types").GetComponent<Text>();

            NameLabel.text = cardReference.Name;
            EffectTextLabel.text = cardReference.EffectText;
            TypesLabel.text = string.Join(",", cardReference.SubTypes);

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

        public void Highlight(Color colour)
        {
            HighlightSprite.enabled = true;
            HighlightSprite.color = colour;
        }

        public void Dehighlight()
        {
            HighlightSprite.enabled = false;
            HighlightSprite.color = Color.white;
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
            LargeCard.SetActive(false); // Stops flashing when dropping
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
            if (CardReference is CreatureCard creatureCard)
            {
                AttackLabel.text = creatureCard.Attack.Clamp(0, 99).ToString();
                DefenceLabel.text = creatureCard.Defence.Clamp(0, 99).ToString();
            }

            // Toggle Large Card visibility
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Collider.bounds.Contains(new Vector2(mousePos.x, mousePos.y)))
            {
                if (IsInPosition) // Stops drag and drop flashing
                    LargeCard.SetActive(true);
            }
            else if (LargeCard.activeSelf)
            {
                LargeCard.SetActive(false);
            }

            // Move
            transform.position = Vector2.Lerp(transform.position, TargetPosition, Time.deltaTime * MoveSpeed);
        }
    }
}