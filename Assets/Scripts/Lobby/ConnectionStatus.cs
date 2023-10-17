using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionStatus : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI StatusText;

    void Awake()
    {
        StatusText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
    }
}
