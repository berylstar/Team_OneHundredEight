using System;
using System.Text;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
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
            public Dictionary<int, bool> Selection;
            public Dictionary<int, EnhancementData> SelectedData;
        }

        private static EnhanceTester _instance;
        private EnhancementManager _enhancementManager;
        private bool _isUpdated;
        public event Action<int, EnhancementData> OnEnhancementEvent;
        [SerializeField] private Text stateText;

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
            _enhancementManager.OnEnhancementEvent += OnEnhance;
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

            if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
            {
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

                int[] ranking = PhotonNetwork.CurrentRoom.Players
                    .Select(player => player.Value.ActorNumber)
                    .ToArray();

                _enhancementManager.Init(ranking);
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
            {
                _enhancementManager.Init(new[] { 1, 2 });
            }
        }

        private void OnEnhance(int player, EnhancementData data)
        {
            _state.Selection[player] = true;
            _state.SelectedData[player] = data;
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
    }
}