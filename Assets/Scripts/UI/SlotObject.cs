using Assets.Scripts.Bases;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Unity object to 
    /// </summary>
    public class SlotObject : BaseUIObject
    {
        // Source Reference is null for mana slot
        // TODO: does this need changing?
        public FieldSlot SlotReference => (FieldSlot)SourceReference;

        public SlotType SlotType;
        public PlayerType Owner;
        public int Index;
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
            if (SlotReference.EffectType != EffectType.None) {
                Glow.enabled = true;
                return;
            }

            Glow.enabled = false;
        }
    }
}
