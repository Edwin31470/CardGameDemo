using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Items;
using Assets.Scripts.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.UI
{
    // Show and close piles
    public class PileManager : MonoBehaviour
    {
        // Cards come in from the left and leave to the right
        private Vector2 LeftOrigin { get; } = new Vector2(-32.2f, 0);
        private Vector2 RightDestination { get; } = new Vector2(40, 0);

        private static OverlayObject OverlayPrefab { get; set; }
        private static CardObject CardObjectPrefab { get; set; }
        private static ItemObject ItemObjectPrefab { get; set; }
        private static TokenObject TokenObjectPrefab { get; set; }


        private static OverlayObject Overlay { get; set; }

        public bool IsShowing { get; set; }

        public void Start()
        {
            // Load Resources
            OverlayPrefab = Resources.Load<OverlayObject>("Prefabs/ViewCardPile");
            CardObjectPrefab = Resources.Load<CardObject>("Prefabs/CardObject");
            ItemObjectPrefab = Resources.Load<ItemObject>("Prefabs/ItemObject");
            TokenObjectPrefab = Resources.Load<TokenObject>("Prefabs/TokenObject");
        }

        public void ShowPile(IEnumerable<BaseSource> sources)
        {
            // If there is already a pile showing, do nothing
            if (IsShowing)
                return;

            IsShowing = true;

            // Load Variables
            Overlay = Instantiate(OverlayPrefab, Vector2.zero, Quaternion.identity);
            Overlay.Initialize(sources, CreateUIObject);

            MoveableUIObject CreateUIObject(BaseSource source, Vector2 position, Transform parent, float movementDelay)
            {
                MoveableUIObject newUIObject;

                switch (source)
                {
                    case BaseCard baseCard:
                        var cardObject = Instantiate(CardObjectPrefab, LeftOrigin, Quaternion.identity, parent);
                        cardObject.Initialize(baseCard);
                        cardObject.SetTargetPosition(position, false, movementDelay);
                        newUIObject = cardObject;
                        break;
                    case Item baseItem:
                        var itemObject = Instantiate(ItemObjectPrefab, LeftOrigin, Quaternion.identity, parent);
                        itemObject.Initialize(baseItem);
                        itemObject.SetTargetPosition(position, false, movementDelay);
                        newUIObject = itemObject;
                        break;
                    case BaseToken baseToken:
                        var tokenObject = Instantiate(TokenObjectPrefab, LeftOrigin, Quaternion.identity, parent);
                        tokenObject.Initialize(baseToken);
                        tokenObject.SetTargetPosition(position, false, movementDelay);
                        newUIObject = tokenObject;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(source), $"Must be {typeof(BaseCard)}, {typeof(Item)} or {typeof(BaseToken)}");
                }

                newUIObject.SetSortingLayer("Card Overlay");
                newUIObject.gameObject.layer = LayerMask.NameToLayer("Card Overlay");
                return newUIObject;
            }
        }

        public void ClosePile()
        {
            Destroy(Overlay.gameObject);

            IsShowing = false;
        }

        private void Update()
        {
            if (!IsShowing)
                return;

            // When the pile is empty, close it
            if (Overlay.IsEmpty)
                ClosePile();

            // On escape, begin closing the pile
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
            {
                Overlay.BeginClosing(RightDestination);
            }
        }
    }
}
