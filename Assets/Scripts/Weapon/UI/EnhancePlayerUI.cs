using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

namespace Weapon.UI
{
    public class EnhancePlayerUI : MonoBehaviour
    {
        public record UiState
        {
            public string Nickname;
            public Color Color;
            public Sprite Image;
            public bool IsPlayerTurn;
            public bool IsSelected;
        }

        [SerializeField] private Image playerImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image checkImage;
        [SerializeField] private Image orderArrowImage;
        [SerializeField] private Text nicknameText;

        private UiState _state;
        public UiState CurrentState => _state;

        public void ChangeState(UiState newState)
        {
            _state = newState;
            UpdateUI();
        }

        private void UpdateUI()
        {
            nicknameText.text = CurrentState.Nickname;
            backgroundImage.color = CurrentState.Color;
            checkImage.gameObject.SetActive(CurrentState.IsSelected);
            orderArrowImage.gameObject.SetActive(CurrentState.IsPlayerTurn);
        }
    }
}