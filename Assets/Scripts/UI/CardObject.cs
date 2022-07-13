using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Extensions;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class CardObject : MoveableUIObject
    {
        public BaseCard CardReference { get; set; }
        public override ITargetable SourceReference => CardReference;

        private SpriteRenderer Background { get; set; }
        private SpriteRenderer CardSymbol { get; set; }
        private SpriteRenderer CostSymbol { get; set; }
        private Text AttackLabel { get; set; }
        private Text DefenceLabel { get; set; }

        public void Initialize(BaseCard cardReference)
        {
            CardReference = cardReference;

            var colour = cardReference.Colour.ToString();
            var cost = cardReference.Cost;


            AttackLabel = transform.Find("Canvas/Attack").GetComponent<Text>();
            DefenceLabel = transform.Find("Canvas/Defence").GetComponent<Text>();

            AttackLabel.text = string.Empty;
            DefenceLabel.text = string.Empty;

            Background = GetComponent<SpriteRenderer>();
            CardSymbol = transform.Find("CardSymbol").GetComponent<SpriteRenderer>();
            CostSymbol = transform.Find("CostSymbol").GetComponent<SpriteRenderer>();

            Background.sprite = Resources.Load<Sprite>($"Sprites/{colour}CardSmall{CardReference.Type}");

            CostSymbol.sprite = cost != 0 ? Resources.Load<Sprite>($"Sprites/Symbols/Cost{cost}") : null;

            CardSymbol.sprite = CardGraphicsManager.GetSymbolSprite(CardReference);
            CardSymbol.color = CardGraphicsManager.GetSymbolColor(CardReference).Normalise();

            base.Initialize();
        }

        protected override void Update()
        {
            // Update Labels
            if (SourceReference is CreatureCard creatureCard)
            {
                AttackLabel.text = creatureCard.Attack.Clamp(0, 99).ToString();
                DefenceLabel.text = creatureCard.Defence.Clamp(0, 99).ToString();
            }

            base.Update();
        }
    }
}