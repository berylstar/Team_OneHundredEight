using UnityEngine;
using Weapon.Model;
using Photon.Pun;

namespace Weapon.Controller
{
    public class ProjectileController : MonoBehaviour
    {
        [field: SerializeField] public int Damage { get; private set; } = 0;
        [field: SerializeField] public float Speed { get; private set; } = 0;
        [field: SerializeField] public Vector2 Direction { get; private set; } = Vector2.zero;

        private Rigidbody2D _rigidbody;
        private PhotonView _photonView;

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _photonView = GetComponent<PhotonView>();
        }

        private void FixedUpdate()
        {
            _rigidbody.velocity = Direction * Speed;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Stage"))
                Disapear();

            if (!_photonView.IsMine && collision.CompareTag("Player") && collision.GetComponent<PhotonView>().IsMine)
            {
                collision.GetComponent<PlayerController>().Hit(10);
                Disapear();
            }
        }

        public void Initialize(AttackData data, Vector2 direction)
        {
            Damage = data.bulletDamage;
            Speed = data.bulletSpeed;
            Direction = direction;
        }

        private void Disapear()
        {
            _photonView.RPC(nameof(RPCSetActive), RpcTarget.All, false);
        }

        [PunRPC]
        public void RPCSetActive(bool flag)     // 오브젝트 풀링시 RPC로 활성화
        {
            gameObject.SetActive(flag);
        }
    }
}