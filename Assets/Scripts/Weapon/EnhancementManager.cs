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
        public const float SelectionLimitTime = 3f;

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
        private readonly Dictionary<int, bool> _canSelectEnhance = new Dictionary<int, bool>();
        private float _currentTime;
        private bool _isInit = false;
        private bool _isAllPlayerSelected;
        private int _currentEnhanceOrder = -1;
        private int _selectedPlayerCount = 0;
        public event Action<int> OnPlayerSelectEnhancement;
        public event Action<int, EnhancementData> OnEnhancementEvent;
        public event Action<int, Color> OnUpdateEnhanceUIEvent;
        public event Action<float> OnTimeElapsed;
        public event Action OnAllPlayerEnhanced;

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
            //todo get ranking from gameManager
            OnAllPlayerEnhanced += LoadNextRound;
        }


        private void Update()
        {
            if (!_isInit)
            {
                return;
            }

            if (_isAllPlayerSelected)
            {
                return;
            }

            if (_currentTime <= 0f)
            {
                if (SetNextSelectionOrder())
                {
                    _isAllPlayerSelected = true;
                    _currentTime = 0f;
                    EnhanceNotSelectedPlayer();
                    _isInit = false;
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
                    _enhancedPlayerIndexSet.Add(playerSelectState.Key);
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

        private void LoadNextRound()
        {
            //todo photonNetwork new scene load
        }
    }
}