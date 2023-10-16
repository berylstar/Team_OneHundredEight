using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Weapon.Data;
using Weapon.Model;
using Random = UnityEngine.Random;

namespace Weapon
{
    public class EnhancementManager : MonoBehaviourPunCallbacks
    {
        //todo migrate to constants
        private const int MaxCardCount = 6;
        private const float SelectionLimitTime = 3f;

        //todo migrate to data manager
        private const string ENHANCEMENT_CSV_FILE = "enhancement_dataset";
        private Camera _camera;

        [field: SerializeField] public Color[] PlayerColors { get; private set; }
        [SerializeField] private Canvas cards;
        [SerializeField] private EnhanceCardUI cardPrefab;

        private List<EnhancementDataEntry> _dataEntries;
        private List<KeyValuePair<EnhancementData, bool>> _remainEnhancements;

        //todo get from gameManager
        private int _headcount = 3;
        private int CardCount => _headcount + 1 > MaxCardCount ? MaxCardCount : _headcount + 1;

        private HashSet<int> _enhancedPlayerIndexSet;
        private EnhanceCardUI[] _cards;
        private readonly Dictionary<int, bool> _canSelectEnhance = new Dictionary<int, bool>();
        private float _currentTime;
        private bool _isInit = false;
        private int _currentEnhanceOrder = -1;
        public event Action<int, EnhancementData> OnEnhancementEvent;
        public event Action<float> OnTimeElapsed;
        public event Action OnAllPlayerEnhanced;

        private void Awake()
        {
            LoadDataSet();
            _camera = Camera.main;
            _enhancedPlayerIndexSet = new HashSet<int>();
            _cards = new EnhanceCardUI[CardCount];
        }

        private void Start()
        {
            //todo remove
            CreateCards();
        }

        private void Update()
        {
            if (!_isInit)
            {
                return;
            }

            if (_currentTime <= 0f)
            {
                if (SetNextSelectionOrder())
                {
                    EnhanceNotSelectedPlayer();
                    OnAllPlayerEnhanced?.Invoke();
                }
            }
            else
            {
                _currentTime -= Time.deltaTime;
            }

            OnTimeElapsed?.Invoke(_currentTime);
        }


        private void LoadDataSet()
        {
            //TODO Inversion of control -> dataManager
            _dataEntries = CsvReader.ReadCsvFromResources<EnhancementDataEntry>(ENHANCEMENT_CSV_FILE, 1);
            _remainEnhancements = _dataEntries
                .Select(data => new KeyValuePair<EnhancementData, bool>(data.ToEnhancementData(), false))
                .ToList();
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
                    ArrangeCard(card, cnt++);
                }
            }

            _currentTime = SelectionLimitTime;
        }

        //todo migrate to cardsUi
        private void ArrangeCard(EnhanceCardUI card, int index)
        {
            float startX = (MaxCardCount - CardCount) / (float)MaxCardCount * 0.5f;
            float paddingX = 1 / (float)MaxCardCount;
            Vector3 startPosition = _camera.ViewportToScreenPoint(new Vector3(startX + paddingX * index, 0.5f));
            card.Arrange(this, startPosition, index);
        }

        public void EnhanceWeapon(int playerIndex, int cardIndex)
        {
            var data = _dataEntries[cardIndex].ToEnhancementData();
            Debug.Log($"select Card-P :{playerIndex} ,C : {cardIndex} ");

            if (!_canSelectEnhance.ContainsKey(playerIndex))
            {
                Debug.LogWarning("not contain player index");
                return;
            }

            if (!_canSelectEnhance[playerIndex])
            {
                return;
            }

            //todo synchronize state
            if (_enhancedPlayerIndexSet.Contains(playerIndex))
            {
                return;
            }

            if (_currentEnhanceOrder == playerIndex)
            {
                _currentTime = 0f;
            }

            _enhancedPlayerIndexSet.Add(playerIndex);
            OnEnhancementEvent?.Invoke(playerIndex, data);
            _cards[cardIndex].SelectEnhancement(PlayerColors[playerIndex - 1]);
            //todo 모두다 고른 경우 이벤트 호출 
        }

        public void SetRanking(int[] ranking)
        {
            foreach (var playerIndex in ranking)
            {
                _canSelectEnhance[playerIndex] = false;
            }

            _canSelectEnhance[ranking[0]] = true;
            _currentEnhanceOrder = ranking[0];
            _isInit = true;
        }

        private void EnhanceNotSelectedPlayer()
        {
            foreach (var keyValue in _canSelectEnhance)
            {
                if (_enhancedPlayerIndexSet.Contains(keyValue.Key))
                {
                    _enhancedPlayerIndexSet.Add(keyValue.Key);
                }
            }
        }

        /// <returns>if true : all player selection order is done</returns>
        private bool SetNextSelectionOrder()
        {
            bool isEnd = true;
            foreach (var selectPair in _canSelectEnhance)
            {
                if (selectPair.Value)
                {
                    continue;
                }

                _currentTime = SelectionLimitTime;
                _canSelectEnhance[selectPair.Key] = true;
                _currentEnhanceOrder = selectPair.Key;
                isEnd = false;
                break;
            }

            return isEnd;
        }
    }
}