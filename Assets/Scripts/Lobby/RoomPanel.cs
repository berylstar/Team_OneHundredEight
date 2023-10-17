using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject mainName;
    [SerializeField] private GameObject roomPanel;
    
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private RectTransform playerPanelCase;
    [SerializeField] private GameObject playerPanel;

    [SerializeField] private TextMeshProUGUI readyAndStartTxt;

    private List<PlayerPanel> playerPanelList = new List<PlayerPanel>();

    private void Awake()
    {
        for(int i = 0; i < 5; i++ )
        {
            GameObject go = Instantiate(playerPanel);
            go.transform.SetParent(playerPanelCase);
            playerPanelList.Add( go.GetComponent<PlayerPanel>() );
        }
    }

    public override void OnEnable()
    {

        base.OnEnable();
        print(PhotonNetwork.CurrentRoom.Players.Count + "명.");
        roomName.text = PhotonNetwork.CurrentRoom.Name;

        int index = 0;
        foreach (var (idx,player) in PhotonNetwork.CurrentRoom.Players)
        {
            playerPanelList[index].curPlayer = player;
            playerPanelList[index].gameObject.SetActive(true);
            print(playerPanelList[index].curPlayer.NickName + "님 프로필 추가.");
            index++;
        }
        IsMaster();
        //for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        //{
        //    playerPanelList[i].curPlayer = PhotonNetwork.CurrentRoom.Players[i + 1];
        //    playerPanelList[i].gameObject.SetActive(true);
        //    print(playerPanelList[i].curPlayer.NickName + "님 프로필 추가.");
        //}
    }

    public void LeaveRoom()//방 나가기
    {
        PhotonNetwork.LeaveRoom();

        for (int i = 0; i < 5; i++)
        {
            playerPanelList[i].gameObject.SetActive(false);
        }

        roomPanel.SetActive(false);
        mainName.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)//방에 플레이어가 들어오면 호출 되는 콜백
    {
        base.OnPlayerEnteredRoom(newPlayer);

        playerPanelList[PhotonNetwork.CurrentRoom.PlayerCount - 1].curPlayer = newPlayer;
        playerPanelList[PhotonNetwork.CurrentRoom.PlayerCount - 1].gameObject.SetActive(true);

        print(newPlayer.NickName + "님이 방에 들어왔습니다.");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)//방에서 플레이어가 나가면 호출 되는 콜백
    {
        base.OnPlayerEnteredRoom(otherPlayer);

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount + 1; i++)
        {
            if (playerPanelList[i].curPlayer == otherPlayer)
            {
                playerPanelList[i].gameObject.SetActive(false);
                print(otherPlayer.NickName + "님의 정보를 지웠습니다.");
            }
        }

        print(otherPlayer.NickName + "님이 방에 나가셨습니다.");

        IsMaster();
    }

    public void IsMaster()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            readyAndStartTxt.text = "Start!";
        }
        else
        {
            readyAndStartTxt.text = "Ready!";

        }
    }
}
