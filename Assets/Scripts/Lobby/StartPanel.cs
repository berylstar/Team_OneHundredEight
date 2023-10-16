using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class StartPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject nicknameSettingPanel;

    public void Connect()
    {
        // 처음에 Photon Online Server에 접속하는 게 가장 중요함!!
        // Photon Online Server에 접속하기.
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // Photon Online Server에 접속하면 불리는 콜백 함수.
        print("서버 접속 완료.");
        NextScene();
    }

    private void NextScene()
    {
        startPanel.SetActive(false);
        nicknameSettingPanel.SetActive(true);
    }
}
