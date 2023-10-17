using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySettingManger : MonoBehaviour
{
    public static LobbySettingManger I;

    public GameObject startPanel;
    public GameObject nicknameSettingPanel;
    public GameObject mainPanel;
    public GameObject createRoomPanel;
    public GameObject lobbyPanel;
    public GameObject roomPanel;

    private void Awake()
    {
        I = this;
    }
}
