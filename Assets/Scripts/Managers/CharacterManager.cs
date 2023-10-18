using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using Weapon.Model;
using Object = UnityEngine.Object;

namespace Managers
{
    public class CharacterManager : MonoBehaviourPun
    {
        private static CharacterManager _instance;

        public static CharacterManager Instance
        {
            get
            {
                if (_instance != null) { return _instance; }

                _instance = FindObjectOfType<CharacterManager>();
                if (_instance != null) { return _instance; }

                GameObject go = new GameObject() { name = nameof(CharacterManager) };
                _instance = go.AddComponent<CharacterManager>();

                return _instance;
            }
        }

        private Dictionary<int, PlayerInfo> _playerInfos;
        public IReadOnlyDictionary<int, PlayerInfo> PlayerInfos => _playerInfos;
        [SerializeField] private WeaponData defaultWeaponData;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            if (defaultWeaponData == null)
            {
                //todo get from data manager
                defaultWeaponData = new WeaponData() { };
            }

            _playerInfos = new Dictionary<int, PlayerInfo>();
            Init();
            DontDestroyOnLoad(gameObject);
        }

        public void Init()
        {
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                PlayerInfo info = new PlayerInfo(player.Value.NickName, "PlayerImage/Human", defaultWeaponData);
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