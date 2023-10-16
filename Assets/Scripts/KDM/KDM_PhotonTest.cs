using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class KDM_PhotonTest : MonoBehaviourPunCallbacks
{
    private readonly string player = "Player";

    void Start()
    {
        //PhotonNetwork.SendRate = 60;
        //PhotonNetwork.SerializationRate = 30;

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();
    public override void OnJoinedLobby() => PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 5 }, null);
    public override void OnJoinedRoom()
    {
        //PhotonNetwork.Instantiate(player, Vector3.zero, Quaternion.identity);
        PhotonNetwork.Instantiate("Blood", Vector3.zero, Quaternion.identity);
        Pickup.Create(new Vector3(0, 5, 0), Define.ItemType.Random, 10);
    }
}