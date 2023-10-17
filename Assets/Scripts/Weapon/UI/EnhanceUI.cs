using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using Weapon.Model;
using Random = UnityEngine.Random;
using Text = TMPro.TextMeshProUGUI;

namespace Weapon.UI
{
    public class EnhanceUI : MonoBehaviourPun
    {
        private enum EnhanceUiState
        {
            Select,
            AllPlayerSelected
        }

        [SerializeField] private Text timeText;
        [SerializeField] private RectTransform playerContainer;
        private Camera _camera;

        private EnhanceUiState _uiState = EnhanceUiState.Select;
        private bool _isUpdated = false;
        private EnhancementManager _enhancementManager;
        private List<EnhanceCardUI> _enhanceCards = new List<EnhanceCardUI>();
        private List<EnhancePlayerUI> _enhancePlayers = new List<EnhancePlayerUI>();

        private void Update()
        {
            if (!_isUpdated)
            {
                return;
            }

            _isUpdated = false;
            switch (_uiState)
            {
                case EnhanceUiState.Select:
                    break;
                case EnhanceUiState.AllPlayerSelected:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Init(EnhancementManager enhancementManager, int maxCardCount, int cardCount)
        {
            _camera = Camera.main;
            _enhancementManager = enhancementManager;
            _enhancementManager.OnTimeElapsed += UpdateTime;
            _enhancementManager.OnAllPlayerEnhanced += AllPlayerEnhanced;
            _enhancementManager.OnUpdateEnhanceUIEvent += UpdateEnhanceCardUI;
            CreatePlayers();
            CreateCards(enhancementManager.DataList, maxCardCount, cardCount);
        }


        private void UpdateEnhanceCardUI(int cardIndex, Color color)
        {
            _enhanceCards[cardIndex].SelectEnhancement(color);
        }

        private void UpdateTime(float time)
        {
            timeText.text = $"{time}";
        }

        private void CreateCards(List<EnhancementData> dataList, int maxCardCount, int cardCount)
        {
            EnhanceCardUI cardPrefab = Resources.Load<EnhanceCardUI>("EnhanceCardUI");
            int repeat = 0;
            int cnt = 0;
            bool[] isCreated = new bool[dataList.Count];
            while (repeat < 10000 && cnt < cardCount)
            {
                repeat++;
                int idx = Random.Range(0, dataList.Count);
                if (isCreated[idx])
                {
                    continue;
                }

                cnt++;
                isCreated[idx] = true;
            }

            cnt = 0;
            float width = 0f;

            for (int i = 0; i < dataList.Count; i++)
            {
                if (isCreated[i])
                {
                    EnhanceCardUI card = Instantiate(cardPrefab, transform, false);
                    _enhanceCards.Add(card);
                    card.SetEnhancementData(dataList[i]);
                    ArrangeCard(card, cnt++, maxCardCount, cardCount);
                }
            }
        }

        private void ArrangeCard(EnhanceCardUI card, int index, int maxCardCount, int cardCount)
        {
            float startX = (maxCardCount - cardCount) / (float)maxCardCount * 0.5f;
            float paddingX = 1 / (float)maxCardCount;
            Vector3 startPosition = _camera.ViewportToScreenPoint(new Vector3(startX + paddingX * index, 0.5f));
            card.Arrange(_enhancementManager, startPosition, index);
        }

        public void SetPlayerOrder()
        {
            //todo
        }

        private void CreatePlayers()
        {
            foreach (var player in _enhancementManager.PlayerColors)
            {
                EnhancePlayerUI go = Resources.Load<EnhancePlayerUI>("EnhancePlayerUi");
                EnhancePlayerUI playerUi = Instantiate(go, playerContainer, false);
                _enhancePlayers.Add(playerUi);
                playerUi.PlayerImage.color = _enhancementManager.PlayerColors[player.Key];
            }
        }

        public void SetPlayerChecked(int playerNumber)
        {
            //todo dictionary??
        }

        private void AllPlayerEnhanced()
        {
            _isUpdated = true;
        }
    }
}