using System;
using UnityEngine;
using Weapon.Model;

namespace Weapon.Controller
{
    public class ProjectileController : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;
        public AttackData AttackData { get; private set; }
        public Vector2 Direction { private set; get; }
        private bool _isReady = false;

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        protected virtual void Update()
        {
            if (!_isReady)
            {
                return;
            }

            _rigidbody.velocity = Direction * AttackData.bulletSpd;
        }

        protected void OnDisable()
        {
            _isReady = false;
        }

        public void Initialize(AttackData attackData, Vector2 direction)
        {
            AttackData = attackData;
            Direction = direction;
            _isReady = true;
        }
    }
}