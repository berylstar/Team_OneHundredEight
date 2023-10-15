using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "Scriptable Object/Player Stat")]
public class PlayerStatSO : ScriptableObject
{
    [field: SerializeField] public int MaxHp { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }

    //[field: SerializeField] public Sprite WeaponSprite { get; private set; }      => 스프라이트는 직렬화 불가능한 타입
    //[field: SerializeField] public Sprite BulletSprite { get; private set; }         이름 string으로 받고 Resource.Load 써야할듯
    [field: SerializeField] public int MaxMagazine { get; private set; }
    [field: SerializeField] public float Shootingdelay { get; private set; }
    [field: SerializeField] public float ReloadSpeed { get; private set; }

}
