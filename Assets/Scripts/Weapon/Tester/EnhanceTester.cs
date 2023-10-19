using Managers;
using System;
using System.Text;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Weapon.Components;
using Weapon.Model;
using Weapon.UI;
using Text = TMPro.TextMeshProUGUI;

namespace Weapon
{
    public class EnhanceTester : MonoBehaviourPunCallbacks
    {
        public record EnhancementState
        {
            public int Headcount;
            public float CurrentTime;
            public Dictionary<int, bool> Selection;
            public Dictionary<int, EnhancementData> SelectedData;
        }

        private int _playerIndex = 0;
        private static EnhanceTester _instance;
        private EnhancementManager _enhancementManager;
        private bool _isUpdated;
        public event Action<int, EnhancementData> OnEnhancementEvent;
        [SerializeField] private Text stateText;

        private Dictionary<int, List<EnhancementData>> _enhancementDataSet =
            new Dictionary<int, List<EnhancementData>>();

        private EnhancementState _state;
        private int[] _ranking;

        public static EnhanceTester Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindObjectOfType<EnhanceTester>();
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = new GameObject().AddComponent<EnhanceTester>();
                return _instance;
            }
        }

        private void Awake()
        {
            _enhancementManager = FindObjectOfType<EnhancementManager>();
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
            _enhancementManager.OnEnhancementEvent += OnEnhance;
            _enhancementManager.OnAllPlayerEnhanced += OnEnhancementEnd;
        }


        private void Update()
        {
            if (!_isUpdated)
            {
                return;
            }

            _isUpdated = false;
            UpdateUi();
        }


        public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

        public override void OnJoinedLobby()
        {
            PhotonNetwork.JoinOrCreateRoom(roomName: "EnhanceTest",
                roomOptions: new RoomOptions { MaxPlayers = 5 },
                TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"on joined room {PhotonNetwork.CurrentRoom.Name}");
            Debug.Log($"actor-number is {PhotonNetwork.LocalPlayer.ActorNumber}");
            Debug.Log($"player-number is {PhotonNetwork.LocalPlayer.GetPlayerNumber()}");
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 1)
            {
                InitTestState();
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"player enter the room:{newPlayer.ActorNumber}");
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 1)
            {
                Debug.Log($"on joined room {PhotonNetwork.CurrentRoom.Name}");
                Debug.Log($"actor-number is {PhotonNetwork.LocalPlayer.ActorNumber}");
                Debug.Log($"player-number is {PhotonNetwork.LocalPlayer.GetPlayerNumber()}");
                InitTestState();
            }
        }

        private void OnEnhance(int player, EnhancementData data)
        {
            _state.Selection[player] = true;
            _state.SelectedData[player] = data;
            if (!_enhancementDataSet.TryGetValue(player, out List<EnhancementData> dataSet))
            {
                dataSet = new List<EnhancementData>();
                _enhancementDataSet.Add(player, dataSet);
            }

            dataSet.Add(data);
            _isUpdated = true;
            OnEnhancementEvent?.Invoke(player, data);
        }

        private void UpdateUi()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var player in _state.Selection)
            {
                sb.Append($"player({player.Key})");
                sb.Append($"selection[{(player.Value ? "V" : "X")}]");
                sb.Append(
                    $"selectedData [{(_state.SelectedData[player.Key] == null ? "" : _state.SelectedData[player.Key].Name)}]");
                sb.Append("\n");
            }

            stateText.text = sb.ToString();
        }

        private void InitTestState()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 1)
            {
                PhotonNetwork.LocalPlayer.NickName = "KimDaeYeol";
                ParticipantsManager.Instance.Init();
                _state = new EnhancementState()
                {
                    Headcount = PhotonNetwork.CurrentRoom.PlayerCount,
                    Selection = new Dictionary<int, bool>(),
                    SelectedData = new Dictionary<int, EnhancementData>()
                };

                foreach (var player in PhotonNetwork.CurrentRoom.Players)
                {
                    _state.Selection.Add(player.Value.ActorNumber, false);
                    _state.SelectedData.Add(player.Value.ActorNumber, null);
                }

                _ranking = PhotonNetwork.CurrentRoom
                    .Players
                    .Values
                    .OrderBy(it => it.ActorNumber)
                    .Select(it => it.ActorNumber)
                    .ToArray();
                _enhancementManager.Init(_ranking);
            }
        }

        private void OnEnhancementEnd()
        {
            GameObject raptor = Instantiate(
                Resources.Load<GameObject>("Raptor"),
                new Vector3(0, 0, 0),
                Quaternion.identity);

            AttackHandler attackHandler = raptor.GetComponentInChildren<AttackHandler>();
            WeaponData weaponData = new WeaponData();
            weaponData.baseAttackData = new AttackData() { bulletSpeed = 1f };
            attackHandler.SetWeaponData(weaponData);

            //todo call manager to save state 
            foreach (var enhancementData in _enhancementDataSet[PhotonNetwork.LocalPlayer.ActorNumber])
            {
                Debug.Log($"AddAttackModifier:{enhancementData.AttackData.ToString()}");
                attackHandler.AddAttackModifier(enhancementData.AttackData);
            }
        }

        private void TestPlayerInfoUI()
        {
            RoundPlayerInfoState state = new RoundPlayerInfoState(
                name: "김대열",
                playerIndex: 1,
                iconUrl: "Sprite/Blood",
                new EnhancementData() { Desc = "선택한 증강 옵션", IconUrl = "Sprite/Enhancement/TRG", Name = "증강" }
            );

            GameObject go = GameObject.Find("EnhancedPayerInfo");
            if (go == null)
            {
                Debug.LogWarning("Cannot find EnhancedPayerInfo");
                return;
            }

            RoundPlayerInfoUI infoUI = go.GetComponentInChildren<RoundPlayerInfoUI>();
            infoUI.Init(state);
        }
    }
}