using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Weapon.Model;

namespace Weapon.Components
{
    ////todo synchronize ??
    //public class AttackHandler : MonoBehaviour
    //{
    //    public AttackData CurrentAttackData { get; private set; }
    //    private List<AttackData> _attackDataModifiers = new List<AttackData>();
    //    private WeaponData _weapon;

    //    private void Start()
    //    {
    //        EnhanceTester.Instance.OnEnhancementEvent += OnEnhancement;
    //    }

    //    private void OnEnhancement(int playerIndex, EnhancementData data)
    //    {
    //        if (playerIndex == PhotonNetwork.LocalPlayer.ActorNumber)
    //        {
    //            AddAttackModifier(data.AttackData);
    //        }
    //    }

    //    public void SetWeaponData(WeaponData weaponData)
    //    {
    //        _weapon = weaponData;
    //        UpdateStats();
    //    }

    //    private void UpdateStats()
    //    {
    //        if (CurrentAttackData == null)
    //        {
    //            CurrentAttackData = new AttackData()
    //            {
    //                bulletDamage = _weapon.baseAttackData.bulletDamage,
    //                bulletSpeed = _weapon.baseAttackData.bulletSpeed,
    //                maxMagazine = _weapon.baseAttackData.maxMagazine,
    //                shotInterval = _weapon.baseAttackData.shotInterval,
    //                reloadTime = _weapon.baseAttackData.reloadTime
    //            };
    //        }

    //        foreach (var attackDataModifier in _attackDataModifiers)
    //        {
    //            CurrentAttackData += attackDataModifier;
    //        }
    //    }

    //    public void AddAttackModifier(AttackData attackData)
    //    {
    //        _attackDataModifiers.Add(attackData);
    //        UpdateStats();
    //    }
    //}
}