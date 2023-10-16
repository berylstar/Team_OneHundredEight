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
        //todo migrate to constants
        private const int MaxCardCount = 6;
        private const float SelectionLimitTime = 3f;

        //todo migrate to data manager
        private const string ENHANCEMENT_CSV_FILE = "enhancement_dataset";
        private Camera _camera;

        //todo make dictionary
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
        private int _currentEnhanceOrder = -1;
        public event Action<int, EnhancementData> OnEnhancementEvent;
        public event Action<int, Color> OnUpdateEnhanceUIEvent;
        public event Action<float> OnTimeElapsed;
        public event Action OnAllPlayerEnhanced;

        private void Awake()
        {
            LoadDataSet();
            _camera = Camera.main;
            _enhancedPlayerIndexSet = new HashSet<int>();
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
                    _currentTime = 0f;
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
            var data = _dataEntries[cardIndex].ToEnhancementData();
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
            OnUpdateEnhanceUIEvent?.Invoke(cardIndex, PlayerColors[playerIndex]);
            _enhanceUI.SetPlayerChecked(playerIndex);
            //todo 모두다 고른 경우 이벤트 호출 
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
            _enhanceUI = Instantiate(go);
            _enhanceUI.Init(this, MaxCardCount, CardCount);
            _currentTime = SelectionLimitTime;
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