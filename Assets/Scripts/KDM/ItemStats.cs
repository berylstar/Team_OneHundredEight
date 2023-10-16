using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemStats
{
    [field: SerializeField] public float Duration { get; set; }
    public bool isTimed { get; set; }

    public ItemStatSO statSO;

    [field : HideInInspector] public PlayerStat ApplyStat { get; set; }
}
