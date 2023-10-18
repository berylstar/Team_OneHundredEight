using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using UnityEngine;

namespace Weapon
{
    public class EnhanceIntegrationTester : MonoBehaviourPunCallbacks
    {
        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
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
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
            {
                InitTestState();
            }
        }

        private void InitTestState()
        {
            GameManager.Instance.gameObject.SetActive(true);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"player enter the room:{newPlayer.ActorNumber}");
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
            {
                Debug.Log($"on joined room {PhotonNetwork.CurrentRoom.Name}");
                Debug.Log($"actor-number is {PhotonNetwork.LocalPlayer.ActorNumber}");
                Debug.Log($"player-number is {PhotonNetwork.LocalPlayer.GetPlayerNumber()}");
                InitTestState();
            }
        }
    }
}