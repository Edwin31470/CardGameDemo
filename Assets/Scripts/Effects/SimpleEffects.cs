using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Effects
{
    // Effects that generate pre-existing events (in other words no custom effects)

    public abstract class SimpleTargetEffect : BaseCardEffect
    {
        protected abstract TargetConditions TargetConditions { get; }
    }
}
