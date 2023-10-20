using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class ParticipantUI : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image playerImage;
        [SerializeField] private Image weaponImage;
        [SerializeField] private TextMeshProUGUI nicknameText;
        [SerializeField] private Button profileButton;

        public event Action<int> OnProfileClicked;
        private int _actorNumber;
        public bool IsInit => _actorNumber >= 1;

        private void Awake()
        {
            _actorNumber = -1;
        }

        private void Start()
        {
            if (_actorNumber != -1)
            {
                OnProfileClicked?.Invoke(_actorNumber);
            }
            else
            {
                Debug.LogWarning("ProfileUI is not initialized.");
            }

            profileButton.onClick.AddListener(ShowProfileChangeUI);
        }

        private void ShowProfileChangeUI()
        {
            if (_actorNumber == -1)
            {
                Debug.LogWarning("ActorNumber is not initialized.");
                return;
            }

            OnProfileClicked?.Invoke(_actorNumber);
        }

        public void Init(int actorNumber)
        {
            _actorNumber = actorNumber;
        }

        public void UpdatePlayerImage(string imageUrl)
        {
            Sprite spriteAsset = Resources.Load<Sprite>(imageUrl);
            Sprite sprite = Instantiate(spriteAsset);
            playerImage.sprite = sprite;
        }

        public void UpdateWeaponImage(string weaponUrl)
        {
            Sprite spriteAsset = Resources.Load<Sprite>(weaponUrl);
            Sprite sprite = Instantiate(spriteAsset);
            weaponImage.sprite = sprite;
        }

        public void UpdateNicknameText(string nickname)
        {
            nicknameText.text = nickname;
        }

        public void UpdateBackgroundImage(Color color)
        {
            backgroundImage.color = color;
        }

        public void UpdateVisibility(bool isVisible)
        {
            if (!isVisible)
            {
                _actorNumber = -1;
            }

            backgroundImage.gameObject.SetActive(isVisible);
            playerImage.gameObject.SetActive(isVisible);
            weaponImage.gameObject.SetActive(isVisible);
            nicknameText.gameObject.SetActive(isVisible);
        }
    }
}