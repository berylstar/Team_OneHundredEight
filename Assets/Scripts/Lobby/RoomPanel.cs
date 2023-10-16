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
        print(PhotonNetwork.CurrentRoom.Players.Count + "��.");
        roomName.text = PhotonNetwork.CurrentRoom.Name;

        int index = 0;
        foreach (var (idx,player) in PhotonNetwork.CurrentRoom.Players)
        {
            playerPanelList[index].curPlayer = player;
            playerPanelList[index].gameObject.SetActive(true);
            print(playerPanelList[index].curPlayer.NickName + "�� ������ �߰�.");
            index++;
        }

        //for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        //{
        //    playerPanelList[i].curPlayer = PhotonNetwork.CurrentRoom.Players[i + 1];
        //    playerPanelList[i].gameObject.SetActive(true);
        //    print(playerPanelList[i].curPlayer.NickName + "�� ������ �߰�.");
        //}
    }

    public void LeaveRoom()//�� ������
    {
        PhotonNetwork.LeaveRoom();

        for (int i = 0; i < 5; i++)
        {
            playerPanelList[i].gameObject.SetActive(false);
        }

        roomPanel.SetActive(false);
        mainName.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)//�濡 �÷��̾ ������ ȣ�� �Ǵ� �ݹ�
    {
        base.OnPlayerEnteredRoom(newPlayer);

        playerPanelList[PhotonNetwork.CurrentRoom.PlayerCount - 1].curPlayer = newPlayer;
        playerPanelList[PhotonNetwork.CurrentRoom.PlayerCount - 1].gameObject.SetActive(true);

        print(newPlayer.NickName + "���� �濡 ���Խ��ϴ�.");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)//�濡�� �÷��̾ ������ ȣ�� �Ǵ� �ݹ�
    {
        base.OnPlayerEnteredRoom(otherPlayer);

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount + 1; i++)
        {
            if (playerPanelList[i].curPlayer == otherPlayer)
            {
                playerPanelList[i].gameObject.SetActive(false);
                print(otherPlayer.NickName + "���� ������ �������ϴ�.");
            }
        }

        print(otherPlayer.NickName + "���� �濡 �����̽��ϴ�.");
    }
}
