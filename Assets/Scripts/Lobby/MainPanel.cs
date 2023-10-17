using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private GameObject roomPanel;
    

    public void OnJoinRandomRoomButtonClicked()//랜덤 방을 찾는 함수
    {
        PhotonNetwork.JoinRandomRoom();//랜덤 룸 입장
    }

    public override void OnJoinRandomFailed(short returnCode, string message)//랜덤 방이 없을 떄 호출 되는 함수 
    {
        string roomName = PhotonNetwork.LocalPlayer.NickName + "님 방";

        RoomOptions options = new RoomOptions { MaxPlayers = 5 };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public override void OnJoinedRoom()
    {
        mainPanel.SetActive(false);
        roomPanel.SetActive(true);
    }

    public void CreateRoomPanelOpen()
    {
        mainPanel.SetActive(false);
        createRoomPanel.SetActive(true);
    }

    public void LobbyPanelOpen()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        print("로비 접속 완료.");
        mainPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }
}
