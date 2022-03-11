﻿using System;

namespace Assets.Scripts.Enums
{
    [Flags]
    public enum Area
    {
        None = 0,
        Hand = 1,
        Field = 2,
        Deck = 4,
        Destroyed = 8,
        Eliminated = 16,

        PlayArea = Hand | Field,
        Pile = Destroyed | Eliminated,
        Any = Hand | Field | Deck | Destroyed | Eliminated
    }
}
