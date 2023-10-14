using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class ItemStats
{
    [field: SerializeField] public float Duration { get; set; }
    [field: HideInInspector] public bool isTimed { get; private set; }
    public ItemStatSO statSO;
}
