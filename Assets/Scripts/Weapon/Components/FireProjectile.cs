using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Weapon.Controller;

using Photon.Pun;

namespace Weapon.Components
{
    public class FireProjectile : MonoBehaviour
    {
        [SerializeField] private PlayerController _controller;

        private AttackHandler _handler;
        [SerializeField] private Transform bulletSpawnPosition;
        [SerializeField] private Transform emptySpawnPosition;

        //todo migrate to manager
        [SerializeField] private ProjectileController bullet;

        private bool _isInit = false;
        private float _timeSinceLastAttack = float.MaxValue;
        private float _timeSinceReload = 0f;

        public int RestMagazine { get; private set; } = 0;
        private bool _isAttacking = false;
        private bool _isReloading = false;
        private Vector2 _attackDirection;

        public event Action OnReloadStarted;
        public event Action OnReloadCompleted;
        public event Action OnFireDuringReloading;
        public event Action<int> OnMagazineConsumed;

        private void Awake()
        {
            _handler = GetComponent<AttackHandler>();
        }

        private void Start()
        {
            _controller.OnFire += Attack;
            _handler.OnReadyEvent += Init;
        }

        //todo synchronize
        private void Update()
        {
            if (!_isInit)
            {
                Init();
                return;
            }


            if (_isReloading)
            {
                if (_timeSinceReload >= _handler.CurrentAttackData.reloadTime)
                {
                    CompleteReload();
                }
                else
                {
                    Reload();
                }
            }

            if (_timeSinceLastAttack >= _handler.CurrentAttackData.shotInterval)
            {
                if (_isAttacking)
                {
                    HandleAttack();
                }
            }
            else
            {
                _timeSinceLastAttack += Time.deltaTime;
                _isAttacking = false;
            }
        }

        private void Attack(Vector2 dir)
        {
            _isAttacking = true;
            _attackDirection = dir.normalized;
        }

        private void CreateProjectile()
        {
            ////todo Migrate to manager class
            //ProjectileController projectile = Instantiate(bullet, spawnPosition.position, quaternion.identity);
            //projectile.Initialize(_handler.CurrentAttackData, _attackDirection);

            GameObject obj = GameManager.Instance.Pooler.PoolInstantiate("Bullet", bulletSpawnPosition.position, Quaternion.identity);
            obj.GetComponent<ProjectileController>().Initialize(_handler.CurrentAttackData, _attackDirection);
            obj.GetComponent<PhotonView>().RPC("RPCSetActive", RpcTarget.All, true);

            Vector3 euler = emptySpawnPosition.rotation.eulerAngles;
            euler.z %= 360f;
            if (euler.z > 90f && euler.z < 270f)
            {
                euler.z = 180f - euler.z;
                euler.y = 180f;
            }

            obj = PhotonNetwork.Instantiate("Effects/EmptyCartridge", emptySpawnPosition.position, Quaternion.Euler(euler));
            obj = PhotonNetwork.Instantiate("Effects/Muzzle", bulletSpawnPosition.position, bulletSpawnPosition.rotation);
            //projectile.Initialize(_handler.CurrentAttackData, _attackDirection);
        }

        /// <summary>
        /// 공격 처리 수행 장전중인 경우에도 수행 되며 장전 중인 경우에는 소리 이펙트를 넣기 위해서 이벤트를 전달한다.
        /// 해당 함수는 공격 딜레이만 지났으면 호출되며 해당 함수에서 현재 탄창에 맞는 이벤트들을 호출하며 탄창 생성을 요청한다.
        /// </summary>
        private void HandleAttack()
        {
            _isAttacking = false;
            _timeSinceLastAttack = 0f;
            if (RestMagazine > 0)
            {
                CreateProjectile();
                OnMagazineConsumed?.Invoke(--RestMagazine);
                _isReloading = RestMagazine <= 0;

                if (_isReloading)
                {
                    OnReloadStarted?.Invoke();
                }
            }
            else
            {
                OnFireDuringReloading?.Invoke();
            }
        }

        private void Reload()
        {
            _timeSinceLastAttack = float.MaxValue;
            _timeSinceReload += Time.deltaTime;
            RestMagazine = 0;
        }

        private void CompleteReload()
        {
            _timeSinceReload = 0f;
            RestMagazine = _handler.CurrentAttackData.maxMagazine;
            _isReloading = false;
            OnReloadCompleted?.Invoke();
        }

        private void Init()
        {
            _isInit = true;
            RestMagazine = _handler.CurrentAttackData.maxMagazine;
        }
    }
}