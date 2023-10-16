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
        PhotonNetwork.CreateRoom(roomNameInput.text, new RoomOptions { MaxPlayers = (int)personnelSlider.value });
    }

    public void CancelRoom()
    {
        createRoomPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        roomNameInput.text = string.Empty;
        roomNameText.text = "�̹� ������� �� �̸� �Դϴ�";
    }

    public override void OnJoinedRoom()
    {
        createRoomPanel.SetActive(false);
        roomPanel.SetActive(true);
    }
}
