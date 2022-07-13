using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class PileManager : MonoBehaviour
    {
        // Cards come in from the left and leave to the right
        private Vector2 LeftOrigin { get; } = new Vector2(-32.2f, 0);
        private Vector2 RightDestination { get; } = new Vector2(40, 0);

        private static CardObject CardObjectPrefab { get; set; }
        private static OverlayObject OverlayPrefab { get; set; }

        private static OverlayObject Overlay { get; set; }

        public bool IsShowing { get; set; }

        public void Start()
        {
            // Load Resources
            CardObjectPrefab = Resources.Load<CardObject>("Prefabs/CardObject");
            OverlayPrefab = Resources.Load<OverlayObject>("Prefabs/ViewCardPile");
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
                    default:
                        throw new ArgumentOutOfRangeException(nameof(source), $"Must be {typeof(BaseCard)} or");
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
