using System;

namespace Assets.Scripts.Enums
{
    [Flags]
    public enum SelectionType
    {
        None = 0,
        Neutral = 1,
        Positive = 2,
        Negative = 4,
        Highlight = 8,
        Target = 16,

        NeutralHighlight = Neutral | Highlight,
        PositiveHighlight = Positive | Highlight,
        NegativeHighlight = Negative | Highlight,

        NeutralTarget = Neutral | Target,
        PositiveTarget = Positive | Target,
        NegativeTarget = Negative | Target,

    }
}
