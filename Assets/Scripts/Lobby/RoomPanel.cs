using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI roomName;

    public override void OnEnable()
    {
        base.OnEnable();
        roomName.text = PhotonNetwork.CurrentRoom.Name;
    }
}
