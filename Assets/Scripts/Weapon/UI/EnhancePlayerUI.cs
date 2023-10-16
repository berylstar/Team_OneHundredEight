using UnityEngine;
using UnityEngine.UI;

namespace Weapon.UI
{
    public class EnhancePlayerUI : MonoBehaviour
    {
        [field: SerializeField] public Image PlayerImage { get; private set; }
        [field: SerializeField] public Image CheckImage { get; private set; }

        public void SetPlayerImage(Sprite image)
        {
            PlayerImage.sprite = image;
        }

        public void Check()
        {
            CheckImage.gameObject.SetActive(true);
        }
    }
}