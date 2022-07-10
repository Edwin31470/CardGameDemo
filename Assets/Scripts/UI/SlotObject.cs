using Assets.Scripts.Bases;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class SlotObject : BaseUIObject
    {
        // Source Reference is null for mana slot
        // TODO: does this need changing?
        public FieldSlot SlotReference => (FieldSlot)SourceReference;

        public SlotType SlotType;
        public PlayerType Owner;
        public int Index;
    }
}
