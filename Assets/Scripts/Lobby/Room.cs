using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Room : MonoBehaviourPunCallbacks
{

    public TextMeshProUGUI roomName;
    public TextMeshProUGUI personnel;

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName.text);
    }

    public override void OnJoinedRoom()
    {
        LobbySettingManger.I.lobbyPanel.SetActive(false);
        LobbySettingManger.I.roomPanel.SetActive(true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        gameObject.SetActive(false);
    }
}

