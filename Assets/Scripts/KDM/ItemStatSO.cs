using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemStat" , menuName = "Scriptable Object/Item Stat")]
public class ItemStatSO : ScriptableObject
{ 
    [field: SerializeField] public Define.BuffType BuffType { get; private set; }
    [field: SerializeField] public Define.StatsChangeType StatsChangeType { get; private set; }
    [field: SerializeField] public float  Value { get; private set; }
}