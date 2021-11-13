using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.Managers
{
    public class PileManager : MonoBehaviour
    {
        public bool IsActive { get; set; }

        private static CardObject CardObjectPrefab { get; set; }
        private static GameObject ContainerPrefab { get; set; }


        // Runtime Variables
        private static GameObject Container { get; set; }
        private static Collider2D[] Colliders { get; set; }
        private static Vector2 FirstCardSpace { get; set; }
        private static HashSet<CardObject> CardObjects { get; set; }

        public void Start()
        {
            // Load Resources
            CardObjectPrefab = Resources.Load<CardObject>("Prefabs/CardObject");
            ContainerPrefab = Resources.Load<GameObject>("Prefabs/ViewCardPile");
        }

        //Space Between Cards
        private static readonly int _xOffset = (64 + 6) / 10;
        private static readonly int _yOffset = (64 + 6) / 10;

        public void ShowPile(IEnumerable<BaseCard> cards)
        {
            IsActive = true;

            // Load Variables
            Container = Instantiate(ContainerPrefab, Vector2.zero, Quaternion.identity);
            Colliders = Container.GetComponents<Collider2D>();
            FirstCardSpace = ContainerPrefab.transform.Find("FirstCardSpace").transform.position;
            CardObjects = new HashSet<CardObject>();


            // Starting Grid Position
            var x = 0;
            var y = 0;

            foreach (var card in cards)
            {
                var pos = new Vector2(FirstCardSpace.x + x * _xOffset, FirstCardSpace.y + y * -_yOffset);

                var cardObject = CreateCard(card, pos);
                CardObjects.Add(cardObject);

                x++;

                if (x == 7)
                {
                    x = 0;
                    y++;
                }
            }
        }

        public void ClosePile()
        {
            foreach (var cardObject in CardObjects)
            {
                cardObject.Destroy();
            }

            CardObjects.Clear();
            Destroy(Container);

            IsActive = false;
        }

        private CardObject CreateCard(BaseCard card, Vector2 position)
        {
            var newCard = Instantiate(CardObjectPrefab, position, Quaternion.identity);
            newCard.Initialize(card);
            newCard.SetTargetPosition(position);
            newCard.SetSortingLayer("Card Overlay");
            return newCard;
        }

        private void Update()
        {
            if (!IsActive)
                return;

            // On click
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("Overlay"));

                if (hit.collider != null && Colliders.Contains(hit.collider))
                {
                    ClosePile();
                }
            }
        }
    }
}
