using Managers;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Weapon.Model;
using Text = TMPro.TextMeshProUGUI;

namespace Lobby
{
    public class ProfileDetailUI : MonoBehaviour
    {
        [SerializeField] private Text weaponNameText;
        [SerializeField] private Text weaponDescText;
        [SerializeField] private List<Button> weaponButtons;
        [SerializeField] private Image profileImage;
        [SerializeField] private Image weaponImage;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button closeButton;

        private ParticipantsManager _participantsManager;
        private IReadOnlyList<WeaponData> _weapons;

        private void OnDestroy()
        {
            _participantsManager.OnPlayerStatusChangedEvent -= UpdateUI;
        }

        public void Init(ParticipantsManager manager, IReadOnlyList<WeaponData> weapons)
        {
            _participantsManager = manager;
            _participantsManager.OnPlayerStatusChangedEvent += UpdateUI;
            _weapons = weapons;

            closeButton.onClick.AddListener(() => { Destroy(gameObject); });
            int idx = 0;
            foreach (WeaponData weaponData in _weapons)
            {
                Sprite spriteAsset = Resources.Load<Sprite>(weaponData.spriteName);
                Sprite sprite = Instantiate(spriteAsset);
                weaponButtons[idx].GetComponentInChildren<Image>().sprite = sprite;
                weaponButtons[idx].onClick.AddListener(() => { OnWeaponChanged(weaponData); });
                idx++;
            }

            leftButton.onClick.AddListener(ChangeLeftProfile);
            rightButton.onClick.AddListener(ChangeRightProfile);
            UpdateUI(PhotonNetwork.LocalPlayer.ActorNumber);
        }

        private void OnWeaponChanged(WeaponData weaponData)
        {
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            PlayerInfo info = _participantsManager.PlayerInfos[actorNumber];
            PlayerInfo newInfo = new PlayerInfo(info.Nickname, info.CharacterImage, weaponData);
            _participantsManager.ChangePlayerInfo(actorNumber, newInfo);
        }

        private void UpdateUI(int actorNumber)
        {
            if (actorNumber != PhotonNetwork.LocalPlayer.ActorNumber) { return; }

            PlayerInfo info = _participantsManager.PlayerInfos[actorNumber];
            Sprite profileSpriteAsset =
                Resources.Load<Sprite>(info.CharacterImage);
            Sprite profileSprite = Instantiate(profileSpriteAsset);
            profileImage.sprite = profileSprite;

            Sprite weaponSpriteAsset =
                Resources.Load<Sprite>(info.WeaponData.spriteName);
            Sprite weaponSprite = Instantiate(weaponSpriteAsset);
            weaponImage.sprite = weaponSprite;
            weaponNameText.text = info.WeaponData.weaponName;
            weaponDescText.text = info.WeaponData.tooltip;
        }

        private void ChangeRightProfile()
        {
            _participantsManager.ChangeProfile(PhotonNetwork.LocalPlayer.ActorNumber, 1);
        }

        private void ChangeLeftProfile()
        {
            _participantsManager.ChangeProfile(PhotonNetwork.LocalPlayer.ActorNumber, -1);
        }
    }
}