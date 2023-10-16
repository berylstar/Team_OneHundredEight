using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStat
{
    public Define.StatsChangeType statsChangeType;
    [field: SerializeField] public float HP { get;  set; }
    [field: SerializeField] public float MaxHp { get;  set; }
    [field: SerializeField] public float MoveSpeed { get;  set; }
    [field: SerializeField] public float JumpForce { get;  set; }
}