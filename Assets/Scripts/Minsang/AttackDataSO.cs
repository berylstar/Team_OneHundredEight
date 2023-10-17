using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackInitialData", menuName = "Scriptable Object/Attack Initial Data")]
public class AttackDataSO : ScriptableObject
{
    [field: SerializeField] public int BulletDamage { get; private set; }
    [field: SerializeField] public float BulletSpeed { get; private set; }
    [field: SerializeField] public int MaxMagazine { get; private set; }
    [field: SerializeField] public float ShotInterval { get; private set; }
    [field: SerializeField] public float ReloadTime { get; private set; }
}
