using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Weapon.Model;
using Object = UnityEngine.Object;

namespace Managers
{
    public class ParticipantsManager : MonoBehaviourPunCallbacks
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
        private string _defaultCharacterImageUrl;

        public bool IsMaster { get; private set; } = true;

        public bool CanStart => IsMaster && PhotonNetwork.CurrentRoom.PlayerCount >= 2;
        public string RoomName { get; private set; } = "RoomName";
        public int MaxPlayerCount { get; set; } = 2;
        public int Headcount => PhotonNetwork.CurrentRoom.PlayerCount;

        private string _message = string.Empty;

        public event Action OnEnterRoomEvent;

        public event Action OnExitRoomEvent;
        public event Action<int> OnPlayerStatusChangedEvent;

        public event Action<int> OnPlayerLeftRoomEvent;
        public event Action<bool> OnStartConditionChanged;


        // public event Action 

        #region Unity Event

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            LoadDefaultData();
            _playerInfos = new Dictionary<int, PlayerInfo>();
            DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region Photon Callbacks

        public override void OnJoinedRoom()
        {
            LoadRoomInfo();
            OnEnterRoomEvent?.Invoke();
            OnStartConditionChanged?.Invoke(CanStart);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            PlayerInfo info = new PlayerInfo(newPlayer.NickName, _defaultCharacterImageUrl, _defaultWeaponData);
            if (_playerInfos.TryAdd(newPlayer.ActorNumber, info) == false)
            {
                Debug.LogWarning($"Already exists player info:{newPlayer.NickName}");
            }

            OnPlayerStatusChangedEvent?.Invoke(newPlayer.ActorNumber);
            OnStartConditionChanged?.Invoke(CanStart);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (_playerInfos.Remove(otherPlayer.ActorNumber) == false)
            {
                Debug.LogWarning($"RemoveFailed :{otherPlayer.NickName}");
            }
        }

        public override void OnLeftRoom()
        {
            ClearParticipantsInfo();
        }

        #endregion


        private void LoadDefaultData()
        {
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
                    spriteName = "Sprite/Weapon/M1A1",
                    tooltip = "균형잡힌 성능을 자랑하는 권총이다.",
                    weaponName = "권총"
                };
            }

            if (_defaultCharacterImageUrl == null)
            {
                _defaultCharacterImageUrl = "PlayerImage/Human";
            }
        }

        public void Init()
        {
            var playersInCurrentRoom = PhotonNetwork
                .CurrentRoom
                .Players
                .OrderBy(it => it.Value.ActorNumber);

            foreach (KeyValuePair<int, Player> player in playersInCurrentRoom)
            {
                PlayerInfo info = new PlayerInfo(player.Value.NickName, "PlayerImage/Human", _defaultWeaponData);
                _playerInfos.Add(
                    key: player.Key,
                    value: info
                );
            }

            RoomName = PhotonNetwork.CurrentRoom.Name;
            IsMaster = PhotonNetwork.CurrentRoom.MasterClientId == PhotonNetwork.LocalPlayer.ActorNumber;
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

        public void ClearParticipantsInfo()
        {
            _playerInfos.Clear();
        }


        private void LoadRoomInfo()
        {
            //todo get all data from ...
            Init();
        }


        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            OnExitRoomEvent?.Invoke();
            ClearParticipantsInfo();
        }

        public void StartGame()
        {
            PhotonView.Get(this).RPC(nameof(StartGameRPC), RpcTarget.All);
        }

        [PunRPC]
        private void StartGameRPC()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            PhotonNetwork.CurrentRoom.IsOpen = false; // 게임이 시작했기 때문에 방을 닫는것 안그러면 게임중에 로비로 다른 플레이어가 들어오는 문제가 생김
            PhotonNetwork.CurrentRoom.IsVisible = false; //위와 마찬가지로 방을 비공개로(확실치 않음)

            PhotonNetwork.LoadLevel("NewMinsangScene");
        }
    }
}