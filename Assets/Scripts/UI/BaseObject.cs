using Assets.Scripts.Bases;
using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    // A UI object is something that can be targeted by effects
    public class BaseUIObject : MonoBehaviour
    {
        public ITargetable SourceReference { get; set; }
    }
}
