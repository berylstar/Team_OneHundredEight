using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponInitialData", menuName = "Scriptable Object/Weapon Initial Data")]
public class WeaponDataSO : ScriptableObject
{
    [field: SerializeField] public string WeaponName { get; private set; }
    [field: SerializeField] public string Tooltip { get; private set; }
    [field: SerializeField] public string SpriteName { get; private set; }
}
