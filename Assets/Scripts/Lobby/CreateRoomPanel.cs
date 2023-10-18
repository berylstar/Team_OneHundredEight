using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomPanel : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject roomPanel;

    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TextMeshProUGUI roomNameText;
    
    [SerializeField] private Slider personnelSlider;
    [SerializeField] private TextMeshProUGUI personnelText;

    public void ValueChanged()
    {
        personnelText.text = personnelSlider.value.ToString();
    }
    public void CreateRoom()
    {
        if (roomNameInput.text != "")
        {
            PhotonNetwork.CreateRoom(roomNameInput.text, new RoomOptions { MaxPlayers = (int)personnelSlider.value });
        }
        else
        {
            roomNameText.text = "방 이름이 없습니다!";
        }
    }

    public void CancelRoom()
    {
        createRoomPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        roomNameInput.text = string.Empty;
        roomNameText.text = "이미 사용중인 방 이름 입니다";
    }

    public override void OnJoinedRoom()
    {
        roomNameInput.text = string.Empty;
        createRoomPanel.SetActive(false);
        roomPanel.SetActive(true);
    }
}
