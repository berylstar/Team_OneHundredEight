using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Weapon.Data;
using Weapon.Model;
using Weapon.UI;
using Random = UnityEngine.Random;

namespace Weapon
{
    public class EnhancementManager : MonoBehaviourPunCallbacks
    {
        private GameManager _gameManager;

        //todo migrate to constants
        public const int MaxCardCount = 6;
        public const float SelectionLimitTime = Constants.SelectionTime;

        //todo migrate to data manager
        [SerializeField] private List<Color> playerColors;

        public Dictionary<int, Color> PlayerColors = new Dictionary<int, Color>();
        private List<EnhancementDataEntry> _dataEntries;

        public List<EnhancementData> DataList { get; private set; }

        private List<KeyValuePair<EnhancementData, bool>> _remainEnhancements;

        //todo get from gameManager
        private int _headcount = 3;
        private Dictionary<int, string> _userIds = new Dictionary<int, string>();
        public int Headcount => _headcount;
        private int CardCount => _headcount + 1 > MaxCardCount ? MaxCardCount : _headcount + 1;

        private EnhanceUI _enhanceUI;
        private HashSet<int> _enhancedPlayerIndexSet;

        private readonly Dictionary<int, EnhancementData> _cachedEnhancements =
            new Dictionary<int, EnhancementData>();

        public IReadOnlyDictionary<int, EnhancementData> CachedEnhancements => _cachedEnhancements;

        private readonly Dictionary<int, bool> _canSelectEnhance = new Dictionary<int, bool>();
        private float _currentTime;
        private bool _isInit = false;
        private bool _isAllPlayerSelected;
        private bool _isFightStarted = false;
        private int _currentEnhanceOrder = -1;
        private int _selectedPlayerCount = 0;
        public event Action<int> OnPlayerSelectEnhancement;

        /// key is playerIndex, value is selected enhancementData
        public event Action<int, EnhancementData> OnEnhancementEvent;

        public event Action<int, Color> OnUpdateEnhanceUIEvent;
        public event Action<float> OnTimeElapsed;
        public event Action OnAllPlayerEnhanced;
        public event Action OnReadyToFight;


        private void Awake()
        {
            LoadDataSet();
            _gameManager = GameManager.Instance;
            _enhancedPlayerIndexSet = new HashSet<int>();

            //todo get player image from manager
            playerColors = new List<Color>()
            {
                Color.blue,
                Color.green,
                Color.red,
                Color.magenta,
                Color.cyan
            };
        }

        private void Start()
        {
            OnAllPlayerEnhanced += ShowNextRoundUI;
            OnEnhancementEvent += AddPlayerEnhanceCached;
        }

        private void AddPlayerEnhanceCached(int player, EnhancementData data)
        {
            _cachedEnhancements[player] = data;
        }


        /// 카드를 시간 마다 고른다. 시간내에 고르지 못하면 다음 순서의 인원도 카드를 고를 수 있다.
        /// 마지막 까지 고르지 않으면 앞번호부터 순차대로 가져간다. 강화를 다 골랐다면 준비 후 다음 전투를 개시한다.
        private void Update()
        {
            if (!_isInit)
            {
                return;
            }

            if (_isAllPlayerSelected)
            {
                if (_currentTime > 0f)
                {
                    _currentTime -= Time.deltaTime;
                    OnTimeElapsed?.Invoke(_currentTime);
                }
                else if (!_isFightStarted)
                {
                    OnReadyToFight?.Invoke();
                    _isFightStarted = true;
                }

                return;
            }

            if (_currentTime <= 0f)
            {
                //todo rpc call 
                PhotonView pv = PhotonView.Get(this);
                pv.RPC(nameof(SetNextSelectionOrderRPC), RpcTarget.AllBuffered);
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
            _dataEntries =
                CsvReader.ReadCsvFromResources<EnhancementDataEntry>(Constants.FilePath.ENHANCEMENT_CSV_FILE, 1);

            DataList = _dataEntries.Select(it => it.ToEnhancementData()).ToList();

            _remainEnhancements = _dataEntries
                .Select(data => new KeyValuePair<EnhancementData, bool>(data.ToEnhancementData(), false))
                .ToList();
        }


        public void EnhanceWeapon(int playerIndex, int cardIndex)
        {
            PhotonView pv = PhotonView.Get(this);
            pv.RPC("EnhanceWeaponRPC", RpcTarget.AllBuffered, playerIndex, cardIndex);
        }

        [PunRPC]
        public void EnhanceWeaponRPC(int playerIndex, int cardIndex)
        {
            EnhancementData data = _dataEntries[cardIndex].ToEnhancementData();
            Debug.Log($"select Card P :{playerIndex} ,C : {cardIndex} ");

            if (!_canSelectEnhance.ContainsKey(playerIndex))
            {
                Debug.LogWarning("not contain player index");
                return;
            }

            if (!_canSelectEnhance[playerIndex])
            {
                return;
            }

            if (_enhancedPlayerIndexSet.Contains(playerIndex))
            {
                return;
            }

            if (_currentEnhanceOrder == playerIndex)
            {
                _currentTime = 0f;
            }

            //todo make event function
            OnPlayerSelectEnhancement?.Invoke(playerIndex);

            _selectedPlayerCount++;
            _enhancedPlayerIndexSet.Add(playerIndex);
            OnEnhancementEvent?.Invoke(playerIndex, data);
            OnUpdateEnhanceUIEvent?.Invoke(cardIndex, PlayerColors[playerIndex]);
            KeyValuePair<EnhancementData, bool> currentEnhancement = _remainEnhancements[cardIndex];
            _remainEnhancements[cardIndex] = new KeyValuePair<EnhancementData, bool>(currentEnhancement.Key, true);

            if (_selectedPlayerCount == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                OnAllPlayerEnhanced?.Invoke();
            }
        }

        public void Init(int[] ranking)
        {
            int colorIndex = 0;
            foreach (var playerIndex in ranking)
            {
                _canSelectEnhance[playerIndex] = false;
                PlayerColors[playerIndex] = playerColors[colorIndex++];
            }

            _headcount = PhotonNetwork.CurrentRoom.PlayerCount;
            _canSelectEnhance[ranking[0]] = true;
            _currentEnhanceOrder = ranking[0];
            
            EnhanceUI go = Resources.Load<EnhanceUI>("EnhanceUI");
            EnhanceUI ui = Instantiate(go);
            ui.Init(this, MaxCardCount, CardCount);
            _currentTime = SelectionLimitTime;
            _isInit = true;
        }

        private void EnhanceNotSelectedPlayer()
        {
            List<KeyValuePair<int, int>> playerToEnhanceList = new List<KeyValuePair<int, int>>();
            bool[] isCardSelected = new bool[CardCount];
            foreach (var playerSelectState in _canSelectEnhance)
            {
                if (_enhancedPlayerIndexSet.Contains(playerSelectState.Key) == false)
                {
                    for (int i = 0; i < _remainEnhancements.Count; i++)
                    {
                        KeyValuePair<EnhancementData, bool> enhancementState = _remainEnhancements[i];
                        if (enhancementState.Value) { continue; }

                        if (isCardSelected[i]) { continue; }

                        KeyValuePair<int, int> playerToEnhance = new KeyValuePair<int, int>(
                            key: playerSelectState.Key,
                            value: i
                        );

                        isCardSelected[i] = true;
                        playerToEnhanceList.Add(playerToEnhance);
                        break;
                    }
                }
            }

            foreach (KeyValuePair<int, int> playerCard in playerToEnhanceList)
            {
                int playerIndex = playerCard.Key;
                int cardIndex = playerCard.Value;
                EnhanceWeapon(playerIndex, cardIndex);
            }
        }

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

        /// <returns>if true : all player selection order is done</returns>
        [PunRPC]
        private void SetNextSelectionOrderRPC()
        {
            if (SetNextSelectionOrder())
            {
                _isAllPlayerSelected = true;
                _currentTime = 0f;
                EnhanceNotSelectedPlayer();
            }
        }

        private void ShowNextRoundUI()
        {
            _currentTime = Constants.TimeToNextRound;
            NextRoundUI nextRoundObj = Resources.Load<NextRoundUI>("UI/ReadyBattleUI");
            NextRoundUI nextRoundUI = Instantiate(nextRoundObj);
            nextRoundUI.Init(this);
        }
    }
}