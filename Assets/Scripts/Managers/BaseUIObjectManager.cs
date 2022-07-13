using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    // Something that interacts with BaseUIObjects (Cards, Slots, Terrains)
    public abstract class BaseUIObjectManager : MonoBehaviour
    {
        private Func<bool> _isOverlayUp;
        public bool IsOverlayUp => _isOverlayUp.Invoke();

        public virtual void Initialize(Func<bool> isOverlayUp)
        {
            _isOverlayUp = isOverlayUp;
        }
    }
}
