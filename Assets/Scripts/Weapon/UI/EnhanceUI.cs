using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Weapon.Model;
using Random = UnityEngine.Random;
using Text = TMPro.TextMeshProUGUI;

namespace Weapon.UI
{
    public class EnhanceUI : MonoBehaviourPun
    {
        [SerializeField] private Text timeText;
        [SerializeField] private RectTransform playerContainer;

        private Camera _camera;

        private bool _isAllPlayerReady = false;
        private EnhancementManager _enhancementManager;

        private readonly List<EnhanceCardUI> _enhanceCards = new List<EnhanceCardUI>();

        private readonly Dictionary<int, EnhancePlayerUI> _enhancePlayerUis = new Dictionary<int, EnhancePlayerUI>();

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            _enhancementManager.OnTimeElapsed -= UpdateTime;
            _enhancementManager.OnAllPlayerEnhanced -= AllPlayerEnhanced;
            _enhancementManager.OnPlayerSelectEnhancement -= SetPlayerChecked;
            _enhancementManager.OnUpdateEnhanceUIEvent -= UpdateEnhanceCardUI;
            _enhancementManager.OnReadyToFight -= Disappear;
        }

        public void Init(EnhancementManager enhancementManager, int maxCardCount, int cardCount)
        {
            _camera = Camera.main;
            _enhancementManager = enhancementManager;
            _enhancementManager.OnTimeElapsed += UpdateTime;
            _enhancementManager.OnAllPlayerEnhanced += AllPlayerEnhanced;
            _enhancementManager.OnPlayerSelectEnhancement += SetPlayerChecked;
            _enhancementManager.OnUpdateEnhanceUIEvent += UpdateEnhanceCardUI;
            _enhancementManager.OnReadyToFight += Disappear;
            CreatePlayers();
            CreateCards(enhancementManager.DataList, maxCardCount, cardCount);
        }

        private void Disappear()
        {
            Destroy(gameObject);
        }

        private void UpdateEnhanceCardUI(int cardIndex, Color color)
        {
            _enhanceCards[cardIndex].SelectEnhancement(color);
        }

        private void UpdateTime(float time)
        {
            timeText.text = $"{time:N0}";
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

            if (repeat >= 10000)
            {
                Debug.LogError("repeat is larger or equal to 10,000... check count of card data set");
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
            Vector3 startPosition = _camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            Vector3 destPosition = _camera.ViewportToScreenPoint(new Vector3(startX + paddingX * index, 0.5f));
            card.Arrange(_enhancementManager, startPosition, destPosition, index);
        }

        private void CreatePlayers()
        {
            foreach (var player in _enhancementManager.PlayerColors)
            {
                EnhancePlayerUI go = Resources.Load<EnhancePlayerUI>("EnhancePlayerUi");
                EnhancePlayerUI playerUi = Instantiate(go, playerContainer, false);
                _enhancePlayerUis.Add(player.Key, playerUi);
                playerUi.PlayerImage.color = _enhancementManager.PlayerColors[player.Key];
            }
        }

        public void SetPlayerChecked(int playerNumber)
        {
            _enhancePlayerUis[playerNumber].Check();
        }

        private void AllPlayerEnhanced()
        {
            Disappear();
        }
    }
}