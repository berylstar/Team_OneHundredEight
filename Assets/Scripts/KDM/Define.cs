using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum ItemType
    {
        Random,
        HpUp,
        HpDown,
        SpeedUp,
        SpeedDown,
        ReverseKey,
        Invincible,
        End
    }

    public enum BuffType
    {
        Hp,
        Speed,
        ReverseKey,
        Invincible
    }

    public enum StatsChangeType
    {
        Add,
        Multiple,
        Override,
    }
}
