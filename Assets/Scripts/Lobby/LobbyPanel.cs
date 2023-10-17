using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        // 룸 리스트 콜백은 로비에 접속했을때 자동으로 호출된다.
        // 로비에서만 호출할 수 있음...
        Debug.Log($"룸 리스트 업데이트 ::::::: 현재 방 갯수 : {roomList.Count}");

        if (roomList.Count > roomPanelCase.childCount)
        {
            for (int i = 0; i < PhotonNetwork.CountOfRooms - roomPanelCase.childCount; i++)
            {
                GameObject go = Instantiate(room);
                go.transform.SetParent(roomPanelCase);
            }
        }
        else
        {
            int CloseRoom = roomPanelCase.childCount - roomList.Count;
            for (int i = CloseRoom; i > 0; i--)
            {
                roomPanelCase.GetChild(i - 1).gameObject.SetActive(false);
                Debug.Log("방 닫음");
            }
        }


        for (int i = 0; i < roomList.Count ; i++)
        {
            roomPanelCase.GetChild(i).GetComponent<Room>().roomName.text = roomList[i].Name;
            roomPanelCase.GetChild(i).GetComponent<Room>().personnel.text = roomList[i].PlayerCount.ToString()+ "/"+ roomList[i].MaxPlayers.ToString();
            roomPanelCase.GetChild(i).gameObject.SetActive(true);
            Debug.Log(roomList[i].Name + "생성");
        }
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

    public void RoomListReset()
    {
        PhotonNetwork.JoinLobby();
    }
}
