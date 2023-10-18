using Common;
using Managers;
using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Weapon.Model;

namespace Weapon.Components
{
    //todo synchronize ??
    public class AttackHandler : MonoBehaviour
    {
        [SerializeField] private WeaponData _weapon;
        public AttackData CurrentAttackData { get; private set; }
        private List<AttackData> _attackDataModifiers = new List<AttackData>();
        private bool _isReady = false;
        private FireProjectile _fireProjectile;

        private void Awake()
        {
            _fireProjectile = GetComponent<FireProjectile>();
        }

        public void Enhance(EnhancementData data)
        {
            Debug.Log($"Enhance:{data}");
            AddAttackModifier(data.AttackData);
        }

        public void SetWeaponData(WeaponData data)
        {
            _weapon = data;
            UpdateStats();
            _fireProjectile.Init();
        }

        private void UpdateStats()
        {
            if (CurrentAttackData == null)
            {
                CurrentAttackData = new AttackData()
                {
                    bulletDamage = _weapon.baseAttackData.bulletDamage,
                    bulletSpeed = _weapon.baseAttackData.bulletSpeed,
                    maxMagazine = _weapon.baseAttackData.maxMagazine,
                    shotInterval = _weapon.baseAttackData.shotInterval,
                    reloadTime = _weapon.baseAttackData.reloadTime
                };
            }

            foreach (var attackDataModifier in _attackDataModifiers)
            {
                CurrentAttackData += attackDataModifier;
            }

            LimitStats();
        }

        public void AddAttackModifier(AttackData attackData)
        {
            _attackDataModifiers.Add(attackData);
            UpdateStats();
        }

        private void LimitStats()
        {
            if (CurrentAttackData.bulletDamage <= Constants.Min.MinDamage)
            {
                CurrentAttackData.bulletDamage = Constants.Min.MinDamage;
            }

            if (CurrentAttackData.bulletSpeed <= Constants.Min.MinBulletSpd)
            {
                CurrentAttackData.bulletSpeed = Constants.Min.MinBulletSpd;
            }

            if (CurrentAttackData.maxMagazine <= Constants.Min.MinMagazine)
            {
                CurrentAttackData.maxMagazine = Constants.Min.MinMagazine;
            }

            if (CurrentAttackData.reloadTime <= Constants.Min.MinReloadTime)
            {
                CurrentAttackData.reloadTime = Constants.Min.MinReloadTime;
            }

            if (CurrentAttackData.shotInterval <= Constants.Min.MinAttackDelay)
            {
                CurrentAttackData.shotInterval = Constants.Min.MinAttackDelay;
            }
        }
    }
}