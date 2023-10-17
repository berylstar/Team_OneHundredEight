using System;
using UnityEngine;
using Weapon.Model;
using Photon.Pun;

namespace Weapon.Controller
{
    public class ProjectileController : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;
        [field: SerializeField] public AttackData AttackData { get; private set; }
        [field: SerializeField] public Vector2 Direction { private set; get; }

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        //private void OnEnable()
        //{
        //    Invoke(nameof(TESTdisapear), 1f);
        //}

        //private void TESTdisapear()
        //{
        //    GameManager.Instance.Pooler.PoolDestroy(gameObject);
        //}

        private void FixedUpdate()
        {
            if (AttackData == null)
                return;

            _rigidbody.velocity = Direction * AttackData.bulletSpeed;
        }

        public void Initialize(AttackData attackData, Vector2 direction)
        {
            AttackData = attackData;
            Direction = direction;
        }

        [PunRPC]
        public void RPCSetActive(bool flag)     // 오브젝트 풀링시 RPC로 활성화
        {
            gameObject.SetActive(flag);
        }
    }
}