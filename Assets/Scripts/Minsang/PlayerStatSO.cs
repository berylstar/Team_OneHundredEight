using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStat", menuName = "Scriptable Object/Player Stat")]
public class PlayerStatSO : ScriptableObject
{
    [field: SerializeField] public int MaxHp { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }

    [field: SerializeField] public GameObject Bullet { get; private set; }
    [field: SerializeField] public int MaxMagazine { get; private set; }
    [field: SerializeField] public float Shootingdelay { get; private set; }
    [field: SerializeField] public float ReloadSpeed { get; private set; }
}
