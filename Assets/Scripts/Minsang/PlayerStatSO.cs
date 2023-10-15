using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "Scriptable Object/Player Stat")]
public class PlayerStatSO : ScriptableObject
{
    [field: SerializeField] public int MaxHp { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }

    //[field: SerializeField] public Sprite WeaponSprite { get; private set; }      => ��������Ʈ�� ����ȭ �Ұ����� Ÿ��
    //[field: SerializeField] public Sprite BulletSprite { get; private set; }         �̸� string���� �ް� Resource.Load ����ҵ�
    [field: SerializeField] public int MaxMagazine { get; private set; }
    [field: SerializeField] public float Shootingdelay { get; private set; }
    [field: SerializeField] public float ReloadSpeed { get; private set; }

}
