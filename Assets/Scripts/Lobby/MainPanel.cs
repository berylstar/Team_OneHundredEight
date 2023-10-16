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
    

    public void OnJoinRandomRoomButtonClicked()//���� ���� ã�� �Լ�
    {
        PhotonNetwork.JoinRandomRoom();//���� �� ����
    }

    public override void OnJoinRandomFailed(short returnCode, string message)//���� ���� ���� �� ȣ�� �Ǵ� �Լ� 
    {
        string roomName = PhotonNetwork.LocalPlayer.NickName + "�� ��";

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
        print("�κ� ���� �Ϸ�.");
        mainPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }
}
