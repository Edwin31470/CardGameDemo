using Assets.Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Unity object to 
    /// </summary>
    public class SlotObject : MonoBehaviour
    {
        public SlotType SlotType;
        public PlayerType Owner;
        public int Index;
        public FieldSlot FieldSlot { get; set; } // null for mana slot

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
