using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
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
    private const float AnimationTime = 0.5f;
    private float _time = 0f;
    private Vector2 _startPosition;
    private Vector2 _controlPosition;
    private Vector2 _destPosition;

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
        Sprite iconObj = Resources.Load<Sprite>(_enhancementData.IconUrl);
        Sprite icon = Instantiate(iconObj);
        iconImage.sprite = icon;

        if (selectImage != null)
        {
            selectImage.gameObject.SetActive(_isSelected);
        }
    }

    private void ClickCard()
    {
        if (_isSelected || _manager == null)
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

    public void Arrange(EnhancementManager enhancementManager, Vector2 centerPosition, Vector2 position, int index)
    {
        Rect rect = GetComponent<RectTransform>().rect;
        _startPosition = centerPosition;
        _manager = enhancementManager;
        _index = index;
        _destPosition = position;
        _controlPosition = _startPosition + (position - _startPosition) / 2 + (Vector2.up * rect.height * 1.25f);
        AnimateArrange();
    }

    private void AnimateArrange()
    {
        StartCoroutine(AnimateArrangeEnum());
    }

    private IEnumerator AnimateArrangeEnum()
    {
        while (_time <= AnimationTime)
        {
            _time += Time.deltaTime;
            Vector2 pos1 = Vector2.Lerp(_startPosition, _controlPosition, _time / AnimationTime);
            Vector2 pos2 = Vector2.Lerp(_controlPosition, _destPosition, _time / AnimationTime);
            transform.position = Vector2.Lerp(pos1, pos2, _time / AnimationTime);
            yield return null;
        }
    }
}