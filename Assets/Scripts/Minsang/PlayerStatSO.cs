using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInitialStat", menuName = "Scriptable Object/Player Initial Stat")]
public class PlayerStatSO : ScriptableObject
{
    [field: SerializeField] public float MaxHp { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public int WeaponIndex { get; private set; }
}
