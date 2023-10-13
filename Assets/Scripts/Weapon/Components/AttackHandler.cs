using System.Collections.Generic;
using UnityEngine;
using Weapon.Model;

namespace Weapon.Components
{
    public class AttackHandler : MonoBehaviour
    {
        public AttackData CurrentAttackData { get; private set; }
        private List<AttackData> _attackDataModifiers = new List<AttackData>();
        private WeaponData _weapon;

        public void SetWeaponData(WeaponData weaponData)
        {
            _weapon = weaponData;
            UpdateStats();
        }

        private void UpdateStats()
        {
            if (CurrentAttackData == null)
            {
                CurrentAttackData = new AttackData()
                {
                    bulletSpd = _weapon.baseAttackData.bulletSpd,
                    damage = _weapon.baseAttackData.damage,
                    delay = _weapon.baseAttackData.delay,
                    magazine = _weapon.baseAttackData.magazine
                };
            }

            foreach (var attackDataModifier in _attackDataModifiers)
            {
                CurrentAttackData += attackDataModifier;
            }
        }

        public void AddAttackModifier(AttackData attackData)
        {
            _attackDataModifiers.Add(attackData);
            UpdateStats();
        }
    }
}