using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;
using Weapon;
using Weapon.Model;
using Text = TMPro.TextMeshProUGUI;

public class EnhanceCardUI : MonoBehaviourPun
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text descText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image selectImage;
    [SerializeField] Button cardButton;

    private EnhancementManager _manager;
    private int _index = -1;
    private bool _isSelected = false;
    private EnhancementData _enhancementData;


    private void Start()
    {
        cardButton.onClick.AddListener(ClickCard);
    }

    public void SetEnhancementData(EnhancementData data)
    {
        _enhancementData = data;
        UpdateUi();
    }

    private void UpdateUi()
    {
        nameText.text = _enhancementData.Name;
        descText.text = _enhancementData.Desc;
        iconImage.sprite = Resources.Load<Sprite>(_enhancementData.IconUrl);
        selectImage.gameObject.SetActive(_isSelected);
    }

    private void ClickCard()
    {
        if (_isSelected)
        {
            return;
        }

        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber;
        _manager.EnhanceWeapon(playerIndex, _index);
    }

    public void SelectEnhancement(Color selectedColor)
    {
        _isSelected = true;
        selectImage.color = selectedColor;
        UpdateUi();
    }

    public void Arrange(EnhancementManager enhancementManager, Vector2 position, int index)
    {
        _manager = enhancementManager;
        _index = index;
        AnimateArrange(position);
    }

    private void AnimateArrange(Vector2 position)
    {
        //todo animate 
        transform.position = position;
    }
}