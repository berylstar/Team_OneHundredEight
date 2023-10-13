using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Weapon.Controller;

namespace Weapon.Components
{
    public class FireProjectile : MonoBehaviour
    {
        private WeaponController _controller;
        private AttackHandler _handler;
        [SerializeField] private Transform spawnPosition;

        //todo migrate to manager
        [SerializeField] private ProjectileController bullet;

        private void Awake()
        {
            _controller = GetComponent<WeaponController>();
            _handler = GetComponent<AttackHandler>();
        }

        private void Start()
        {
            _controller.OnFireEvent += CreateProjectile;
        }

        private void CreateProjectile(Vector2 dir)
        {
            //todo Migrate to manager class 
            ProjectileController projectile = Instantiate(bullet, spawnPosition.position, quaternion.identity);
            projectile.Initialize(_handler.CurrentAttackData, dir);
        }
    }
}