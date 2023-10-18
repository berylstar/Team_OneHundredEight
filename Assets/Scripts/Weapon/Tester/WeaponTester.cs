using System;
using UnityEngine;
using Weapon.Components;
using Weapon.Model;
using Text = TMPro.TextMeshProUGUI;

namespace Weapon
{
    public class WeaponTester : MonoBehaviour
    {
        [SerializeField] private Text stateText;
        private AttackHandler _handler;
        private FireProjectile _fire;

        private void Awake()
        {
            _handler = GameObject.Find("Raptor").GetComponent<AttackHandler>();

            _fire = _handler.GetComponent<FireProjectile>();
        }

        public void Start()
        {
            _handler.SetWeaponData(new WeaponData()
            {
                baseAttackData = new AttackData()
                {
                    bulletDamage = 1,
                    bulletSpeed = 3,
                    maxMagazine = 3,
                    reloadTime = 5f,
                    shotInterval = 0.5f
                }
            });
            _fire.OnFireDuringReloading += UpdateUiDuringReload;
            _fire.OnReloadCompleted += UpdateUiReloadCompletion;
            _fire.OnReloadStarted += UpdateUiReloadStart;
            _fire.OnMagazineConsumed += UpdateUiConsumeMagazine;
        }

        private void UpdateUiConsumeMagazine(int obj)
        {
            stateText.text = $"{obj} / {_handler.CurrentAttackData.maxMagazine}";
        }

        private void UpdateUiReloadStart()
        {
            stateText.text = "장전 시작";
        }

        private void UpdateUiReloadCompletion()
        {
            stateText.text = $"{_fire.RestMagazine} / {_handler.CurrentAttackData.maxMagazine}";
        }


        private void UpdateUiDuringReload()
        {
            stateText.text = "장전중 발사!";
        }
    }
}