using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using Weapon.Model;

namespace Weapon
{
    public class EnhanceTester : MonoBehaviourPunCallbacks
    {
        private static EnhanceTester _instance;
        private EnhancementManager _enhancementManager;
        public event Action<int, EnhancementData> OnEnhancementEvent;

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
            _enhancementManager.OnEnhancementEvent += OnEnhancementEvent;
        }

        public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

        public override void OnJoinedLobby() =>
            PhotonNetwork.JoinOrCreateRoom("EnhanceTest", new RoomOptions { MaxPlayers = 5 }, null);

        public override void OnJoinedRoom()
        {
            Debug.Log($"on joined room {PhotonNetwork.CurrentRoom.Name}");
            Debug.Log($"actor-number is {PhotonNetwork.LocalPlayer.ActorNumber}");
            Debug.Log($"player-number is {PhotonNetwork.LocalPlayer.GetPlayerNumber()}");
        }
    }
}