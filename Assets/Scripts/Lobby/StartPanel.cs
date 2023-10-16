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
        // ó���� Photon Online Server�� �����ϴ� �� ���� �߿���!!
        // Photon Online Server�� �����ϱ�.
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // Photon Online Server�� �����ϸ� �Ҹ��� �ݹ� �Լ�.
        print("���� ���� �Ϸ�.");
        NextScene();
    }

    private void NextScene()
    {
        startPanel.SetActive(false);
        nicknameSettingPanel.SetActive(true);
    }
}
