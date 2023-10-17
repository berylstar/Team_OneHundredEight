using UnityEngine;
using UnityEngine.UI;
using Weapon.Model;
using Text = TMPro.TextMeshProUGUI;

public class RoundPlayerInfoUI : MonoBehaviour
{
    [SerializeField] private EnhanceCardUI selectedEnhancementUI;
    [SerializeField] private Image playerIcon;
    [SerializeField] private Text nameText;

    private RoundPlayerInfoState _state;

    public void Init(RoundPlayerInfoState state)
    {
        _state = state;
        UpdateUI();
    }

    private void UpdateUI()
    {
        Sprite spriteObj = Resources.Load<Sprite>(_state.IconUrl);
        Sprite sprite = Instantiate(spriteObj);
        nameText.text = _state.Name;
        playerIcon.sprite = sprite;
        selectedEnhancementUI.SetEnhancementData(data: _state.SelectedEnhancement);
    }
}