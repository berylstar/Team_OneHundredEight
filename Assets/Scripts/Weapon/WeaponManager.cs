using System;
using UnityEngine;
using Weapon.Components;

namespace Weapon
{
    public class WeaponManager : MonoBehaviour
    {
        private AttackHandler _handler;

        //todo get from source
        [SerializeField] private WeaponData weaponData;

        private void Awake()
        {
            _handler = GetComponent<AttackHandler>();
        }

        private void Start()
        {
            _handler.SetWeaponData(weaponData);
        }
    }
}