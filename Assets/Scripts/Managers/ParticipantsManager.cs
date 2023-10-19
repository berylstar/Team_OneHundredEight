using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using Weapon.Model;
using Object = UnityEngine.Object;

namespace Managers
{
    public class ParticipantsManager : MonoBehaviourPun
    {
        private static ParticipantsManager _instance;

        public static ParticipantsManager Instance
        {
            get
            {
                if (_instance != null) { return _instance; }

                _instance = FindObjectOfType<ParticipantsManager>();
                if (_instance != null) { return _instance; }

                GameObject go = new GameObject() { name = nameof(ParticipantsManager) };
                _instance = go.AddComponent<ParticipantsManager>();

                return _instance;
            }
        }

        private Dictionary<int, PlayerInfo> _playerInfos;
        public IReadOnlyDictionary<int, PlayerInfo> PlayerInfos => _playerInfos;
        private WeaponData _defaultWeaponData;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            //todo get from data manager
            if (_defaultWeaponData == null)
            {
                _defaultWeaponData = new WeaponData()
                {
                    baseAttackData = new AttackData()
                    {
                        bulletDamage = 3,
                        bulletSpeed = 3,
                        maxMagazine = 10,
                        reloadTime = 10,
                        shotInterval = 0.5f,
                        statsChangeType = Define.StatsChangeType.Override
                    },
                    bulletName = "Bullet",
                    spriteName = "Raptor",
                    tooltip = "균형잡힌 성능을 자랑하는 권총이다.",
                    weaponName = "권총"
                };
            }

            _playerInfos = new Dictionary<int, PlayerInfo>();
            Init();
            DontDestroyOnLoad(gameObject);
        }

        public void Init()
        {
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                PlayerInfo info = new PlayerInfo(player.Value.NickName, "PlayerImage/Human", _defaultWeaponData);
                _playerInfos.Add(
                    key: player.Key,
                    value: info
                );
            }
        }

        public void ChangePlayerInfo(int actorNumber, PlayerInfo info)
        {
            PhotonView
                .Get(this)
                .RPC(methodName: nameof(ChangePlayerInfo),
                    target: RpcTarget.All,
                    actorNumber, info);
        }

        public void RemovePlayerInfo(int actorNumber)
        {
            if (_playerInfos.ContainsKey(actorNumber))
            {
                _playerInfos.Remove(actorNumber);
            }
        }

        [PunRPC]
        private void ChangePlayerInfoRPC(int actorNumber, PlayerInfo info)
        {
            _playerInfos[actorNumber] = info;
        }
    }
}