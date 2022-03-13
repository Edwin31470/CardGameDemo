using Assets.Scripts.Enums;
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
        private SpriteRenderer Glow { get; set; }

        public void Start()
        {
            if (SlotType == SlotType.Mana)
                return;

            Glow = transform.Find("Glow").gameObject.GetComponent<SpriteRenderer>();
        }

        // TODO: Update colour based on effect type
        public void SetGlow(EffectType effectType)
        {
            if (FieldSlot.EffectType != EffectType.None) {
                Glow.enabled = true;
                return;
            }

            Glow.enabled = false;
        }
    }
}
