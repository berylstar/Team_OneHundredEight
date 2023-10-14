using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Weapon.Data;
using Weapon.Model;
using Random = UnityEngine.Random;

namespace Weapon
{
    public class EnhancementManager : MonoBehaviourPunCallbacks
    {
        //todo migrate to data manager
        private const string ENHANCEMENT_CSV_FILE = "enhancement_dataset";
        private Camera _camera;

        [field: SerializeField] public Color[] PlayerColors { get; private set; }
        [SerializeField] private Canvas cards;
        [SerializeField] private EnhanceCardUI cardPrefab;

        private List<EnhancementDataEntry> _dataEntries;

        //todo get from gameManager
        private int _headcount = 2;
        private int CardCount => _headcount + 2;

        private HashSet<int> _isPlayerSelectCard;
        private EnhanceCardUI[] _cards;
        public event Action<int, EnhancementData> OnEnhancementEvent;


        private void Awake()
        {
            LoadDataSet();
            _camera = Camera.main;
            _isPlayerSelectCard = new HashSet<int>();
            _cards = new EnhanceCardUI[CardCount];
        }

        private void Start()
        {
            CreateCards();
        }

        private void LoadDataSet()
        {
            //Inversion of control -> dataManager
            _dataEntries = CsvReader.ReadCsvFromResources<EnhancementDataEntry>(ENHANCEMENT_CSV_FILE, 1);
        }

        //game manager 가 다음 라운드로 넘어간 경우 호출해야됨 그러나 씬을 넘기는 구조라면 private 으로 해도 됨
        private void CreateCards()
        {
            int repeat = 0;
            int cnt = 0;
            bool[] isCreated = new bool[_dataEntries.Count];
            while (repeat < 10000 && cnt < CardCount)
            {
                repeat++;
                int idx = Random.Range(0, _dataEntries.Count);
                if (isCreated[idx])
                {
                    continue;
                }

                cnt++;
                isCreated[idx] = true;
            }

            cnt = 0;
            float width = 0f;
            for (int i = 0; i < _dataEntries.Count; i++)
            {
                if (isCreated[i])
                {
                    EnhanceCardUI card = Instantiate(cardPrefab, cards.transform, false);
                    _cards[cnt] = card;

                    card.SetEnhancementData(_dataEntries[i].ToEnhancementData());
                    if (width == 0)
                    {
                        width = card.GetComponent<RectTransform>().rect.width;
                    }

                    ArrangeCard(card, width, cnt++);
                }
            }
        }

        private void ArrangeCard(EnhanceCardUI card, float cardWidth, int index)
        {
            //todo animate card arrangement
            Vector3 spacing = new Vector3(cardWidth + 50f, 0f);
            Vector3 centerPosition = _camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            Vector3 startPosition = centerPosition - (spacing) * ((_headcount + 2) / 2f);
            startPosition.x += spacing.x / 2f;
            card.Arrange(this, startPosition + spacing * index, index);
        }

        public void SelectCard(int playerIndex, int cardIndex, EnhancementData data)
        {
            //on click event callback 
            Debug.Log($"select Card-P :{playerIndex} ,C : {cardIndex}, data:{data.Name} ");


            //todo synchronize state
            if (_isPlayerSelectCard.Contains(playerIndex))
            {
                return;
            }

            _isPlayerSelectCard.Add(playerIndex);
            OnEnhancementEvent?.Invoke(playerIndex, data);
            _cards[cardIndex].SelectEnhancement(PlayerColors[playerIndex - 1]);
        }
    }
}