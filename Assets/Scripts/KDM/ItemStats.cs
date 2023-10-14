using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
internal class ItemStats
{
    [field: SerializeField] public float Duration { get; set; }
    public bool isTimed { get; set; }
    public ItemStatSO statSO;
}
