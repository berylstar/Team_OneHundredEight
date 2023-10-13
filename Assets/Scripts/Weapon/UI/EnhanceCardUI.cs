using UnityEngine;
using UnityEngine.UI;
using Weapon.Model;
using Text = TMPro.TextMeshProUGUI;

public class EnhanceCardUI : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text descText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image selectImage;

    private bool _isSelected = false;
    private EnhancementData _enhancementData;

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
        selectImage.gameObject.SetActive(false);
    }

    public void SelectEnhancement(int playerIndex)
    {
        if (_isSelected)
        {
            return;
        }

        selectImage.gameObject.SetActive(true);
        //todo change select image color
    }
}