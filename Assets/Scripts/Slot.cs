using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Slot : MonoBehaviour
    {
        public SlotType SlotType;
        public PlayerType Owner;

        public bool IsOccupied()
        {
            // Get any touching cards
            var contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(LayerMask.GetMask("Card"));
            var touchingCards = new List<Collider2D>();

            int colliderCount = GetComponent<Collider2D>().OverlapCollider(contactFilter, touchingCards);

            // If any, is occupied
            return colliderCount > 0;
        }
    }
}
