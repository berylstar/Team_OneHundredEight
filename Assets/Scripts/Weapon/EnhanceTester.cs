using System;
using System.Text;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine;
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
            public bool[] Selection;
            public EnhancementData[] SelectedData;
        }

        private static EnhanceTester _instance;
        private EnhancementManager _enhancementManager;
        private bool _isUpdated;
        public event Action<int, EnhancementData> OnEnhancementEvent;
        [SerializeField] private Text stateText;
        [SerializeField] private EnhanceUI enhanceUi;

        private EnhancementState _state;

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
        }

        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
            enhanceUi.Init(_enhancementManager);
            _enhancementManager.OnEnhancementEvent += OnEnhance;
            _state = new EnhancementState()
            {
                Headcount = 2, Selection = new bool[2], SelectedData = new EnhancementData[2]
            };

            _enhancementManager.SetRanking(new[] { 1, 2 });
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

        public override void OnJoinedLobby() =>
            PhotonNetwork.JoinOrCreateRoom(
                roomName: "EnhanceTest",
                roomOptions: new RoomOptions { MaxPlayers = 5 },
                null);

        public override void OnJoinedRoom()
        {
            Debug.Log($"on joined room {PhotonNetwork.CurrentRoom.Name}");
            Debug.Log($"actor-number is {PhotonNetwork.LocalPlayer.ActorNumber}");
            Debug.Log($"player-number is {PhotonNetwork.LocalPlayer.GetPlayerNumber()}");
        }

        private void OnEnhance(int player, EnhancementData data)
        {
            _state.Selection[player - 1] = true;
            _state.SelectedData[player - 1] = data;
            _isUpdated = true;
            OnEnhancementEvent?.Invoke(player, data);
        }

        private void UpdateUi()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _state.Headcount; i++)
            {
                sb.Append($"player({i + 1})");
                sb.Append($"selection[{(_state.Selection[i] ? "V" : "X")}]");
                sb.Append($"selectedData [{(_state.SelectedData[i] == null ? "" : _state.SelectedData[i].Name)}]");
                sb.Append("\n");
            }

            stateText.text = sb.ToString();
        }
    }
}