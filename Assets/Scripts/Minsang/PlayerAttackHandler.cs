using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

using Photon.Pun;

public class PlayerAttackHandler : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private WeaponDataSO _initialWeapon;                 // 게임매니저에 저장 ?
    [field: SerializeField] public WeaponData CurrentWeapon { get; private set; }

    [SerializeField] private AttackDataSO _initialAttack;                 // 게임매니저에 저장 ?
    [field: SerializeField] public AttackData CurrentAttack { get; private set; }
    public LinkedList<AttackData> attackModifiers = new LinkedList<AttackData>();

    public event Action OnShoot;

    private void InitialWeapon()
    {
        CurrentWeapon.weaponName = _initialWeapon.WeaponName;
        CurrentWeapon.tooltip = _initialWeapon.Tooltip;
        CurrentWeapon.spriteName = _initialWeapon.SpriteName;
    }

    public void SetWeapon(WeaponData weaponModifier)
    {
        CurrentWeapon.weaponName = weaponModifier.weaponName;
        CurrentWeapon.tooltip = weaponModifier.tooltip;
        CurrentWeapon.spriteName = weaponModifier.spriteName;
    }

    public void AddAttackModifier(AttackData attackModifier)
    {
        attackModifiers.AddLast(attackModifier);
        UpdateAttack();
    }

    public void RemoveAttackModifier(AttackData attackModifier)
    {
        if (!attackModifiers.Contains(attackModifier))
            return;

        attackModifiers.Remove(attackModifier);
        UpdateAttack();
    }

    private void SetBaseAttack()
    {
        CurrentAttack.bulletDamage = _initialAttack.BulletDamage;
        CurrentAttack.bulletSpeed = _initialAttack.BulletSpeed;
        CurrentAttack.maxMagazine = _initialAttack.MaxMagazine;
        CurrentAttack.shotInterval = _initialAttack.ShotInterval;
        CurrentAttack.reloadTime = _initialAttack.ReloadTime;
    }

    private void UpdateAttack()
    {
        SetBaseAttack();

        foreach (AttackData modifier in attackModifiers)
        {
            if (modifier.statsChangeType == Define.StatsChangeType.Override)
            {
                UpdateStat((o, o1) => o1, modifier);
            }
            else if (modifier.statsChangeType == Define.StatsChangeType.Add)
            {
                UpdateStat((o, o1) => o + o1, modifier);
            }
            else if (modifier.statsChangeType == Define.StatsChangeType.Multiple)
            {
                UpdateStat((o, o1) => o * o1, modifier);
            }
        }
    }

    private void UpdateStat(Func<float, float, float> operation, AttackData newModifier)
    {
        CurrentAttack.bulletDamage = (int)operation(CurrentAttack.bulletDamage, newModifier.bulletDamage);
        CurrentAttack.bulletSpeed = operation(CurrentAttack.bulletSpeed, newModifier.bulletSpeed);
        CurrentAttack.maxMagazine = (int)operation(CurrentAttack.maxMagazine, newModifier.maxMagazine);
        CurrentAttack.shotInterval = operation(CurrentAttack.shotInterval, newModifier.shotInterval);
        CurrentAttack.reloadTime = operation(CurrentAttack.reloadTime, newModifier.reloadTime);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(CurrentWeapon.spriteName);
        }
        else
        {
            CurrentWeapon.spriteName = (string)stream.ReceiveNext();
        }
    }
}
