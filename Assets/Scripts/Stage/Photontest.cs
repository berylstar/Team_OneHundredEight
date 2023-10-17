using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photontest : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();
    public override void OnJoinedLobby() => PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 5 }, null);
    public override void OnJoinedRoom()
    {
        
    }

}
