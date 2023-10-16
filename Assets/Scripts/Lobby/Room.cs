using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Room : LobbyPanel
{
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI personnel;

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName.ToString());
    }
    public override void OnJoinedRoom()
    {
        RoomIn();
    }
}
