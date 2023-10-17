using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NicknameSettingPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject nicknameSettingPanel;
    [SerializeField] private GameObject mainPanel;

    [SerializeField] private TMP_InputField inputNickName;
    [SerializeField] private TextMeshProUGUI inputText;

    public void NicknameCheck()
    {
        if (inputNickName.text != "")
        {
            print("올바른 닉네임");
            PhotonNetwork.LocalPlayer.NickName = inputNickName.text;
            inputNickName.text = string.Empty;
            NextScene();
        }
        else
        {
            inputText.text = "올바른 닉네임이 아닙니다";
            inputNickName.text = string.Empty;
        }
    }

    private void NextScene()
    {
        nicknameSettingPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}