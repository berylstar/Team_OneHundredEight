using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    public Player curPlayer;

    [SerializeField] private TextMeshProUGUI palyerNameText;
    [SerializeField] private TextMeshProUGUI ReadyStartText;

    public void Ready()
    {
        ReadyStartText.text = "Ready!";
    }

    private void OnEnable()
    {
        palyerNameText.text = curPlayer.NickName;
        ReadyStartText.text = string.Empty;
    }

    private void OnDisable()
    {
        curPlayer = null;
    }
}
