using System;
using UnityEngine;
using Weapon.Model;
using Photon.Pun;

namespace Weapon.Controller
{
    public class ProjectileController : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;
        public AttackData AttackData { get; private set; }
        public Vector2 Direction { private set; get; }

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        protected void OnDisable()
        {
            GameManager.Instance.Pooler.PoolDestroy(gameObject);
        }

        public void Initialize(AttackData attackData, Vector2 direction)
        {
            AttackData = attackData;
            Direction = direction;

            _rigidbody.velocity = Direction * AttackData.bulletSpeed;
        }

        [PunRPC]
        public void RPCSetActive(bool flag)     // 오브젝트 풀링시 RPC로 활성화
        {
            gameObject.SetActive(flag);
        }
    }
}