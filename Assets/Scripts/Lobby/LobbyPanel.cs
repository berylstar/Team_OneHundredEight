using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject roomPanel;

    [SerializeField] private RectTransform roomPanelCase;

    [SerializeField] private GameObject room;

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // �� ����Ʈ �ݹ��� �κ� ���������� �ڵ����� ȣ��ȴ�.
        // �κ񿡼��� ȣ���� �� ����...
        Debug.Log($"�� ����Ʈ ������Ʈ ::::::: ���� �� ���� : {roomList.Count}");
        if (PhotonNetwork.CountOfRooms > roomPanelCase.childCount)
        {
            for (int i = 0; i < PhotonNetwork.CountOfRooms - roomPanelCase.childCount; i++)
            {
                GameObject go = Instantiate(room);
                go.transform.SetParent(roomPanelCase);
            }
        }

        for (int i = 0; i < PhotonNetwork.CountOfRooms; i++)
        {
            roomPanelCase.GetChild(i).GetComponent<Room>().roomName.text = roomList[i].Name;
            roomPanelCase.GetChild(i).GetComponent<Room>().personnel.text = roomList[i].PlayerCount.ToString()+ "/"+ roomList[i].MaxPlayers.ToString();
            roomPanelCase.GetChild(i).gameObject.SetActive(true);
        }
        Debug.Log("asd");
    }
    public void CancelRoom()
    {
        lobbyPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void RoomIn()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
    }
}
