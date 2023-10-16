using Photon.Pun;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

namespace Weapon.UI
{
    public class EnhanceUI : MonoBehaviourPun
    {
        [SerializeField] private Text timeText;
        [SerializeField] private RectTransform playerContainer;
        private EnhanceCardUI[] _cardUISet;
        private EnhancementManager _enhancementManager;
        
        public void Init(EnhancementManager enhancementManager)
        {
            _enhancementManager = enhancementManager;
            _enhancementManager.OnTimeElapsed += UpdateTime;
            
        }

        private void UpdateTime(float time)
        {
            timeText.text = $"{time:N2}";
        }

        public void CreateCards()
        {
            //todo 
        }

        public void SetPlayerOrder()
        {
        }
    }
}