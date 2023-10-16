using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Object/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    [System.Serializable]
    public struct AttackData
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Tooltip { get; private set; }
        [field: SerializeField] public Sprite WeaponSprite { get; private set; }

        [field: SerializeField] public int MaxMagazine { get; private set; }
        [field: SerializeField] public float ShotInterval { get; private set; }
        [field: SerializeField] public float ReloadSpeed { get; private set; }
        [field: SerializeField] public int Damage { get; private set; }
    }

    [field: SerializeField] public List<AttackData> AttackDatas { get; private set; }
}
