using Common;
using Lobby;
using Managers;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Weapon.Model;
using Text = TMPro.TextMeshProUGUI;

public class RoomPanel : MonoBehaviour
{
    public class RoomUIState
    {
        public Dictionary<int, PlayerInfo> PlayerInfos;
        public Dictionary<int, bool> ReadyPlayers;
        public string RoomName;
        public int MaxPlayerCount;
        public bool IsMaster;

        public RoomUIState(
            Dictionary<int, PlayerInfo> playerInfos,
            string roomName,
            int maxPlayerCount,
            bool isMaster,
            Dictionary<int, bool> readyPlayers)
        {
            PlayerInfos = playerInfos;
            RoomName = roomName;
            MaxPlayerCount = maxPlayerCount;
            IsMaster = isMaster;
            ReadyPlayers = readyPlayers;
        }
    }

    private ParticipantsManager _participantsManager;

    [SerializeField] private ScrollRect chatScrollView;
    [SerializeField] private RectTransform chatContent;
    [SerializeField] private Text roomNameText;

    [SerializeField] private TMP_InputField chatTextInput;

    [SerializeField] private Button readyButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button sendButton;

    [SerializeField] private List<ParticipantUI> participants;

    [SerializeField] private Text chatTextPrefab;

    private RoomUIState _uiState;

    private void Awake()
    {
        _participantsManager = ParticipantsManager.Instance;
        _uiState = new RoomUIState(
            playerInfos: new Dictionary<int, PlayerInfo>(),
            roomName: string.Empty,
            maxPlayerCount: 0,
            isMaster: false,
            readyPlayers: new Dictionary<int, bool>());
    }

    private void Start()
    {
        _participantsManager.OnEnterRoomEvent += InitRoomUIState;
        _participantsManager.OnExitRoomEvent += CloseRoomUI;
        _participantsManager.OnStartConditionChanged += ChangeStartButtonVisibility;
        _participantsManager.OnPlayerStatusChangedEvent += AddParticipantInfo;
        _participantsManager.OnPlayerLeftRoomEvent += RemoveParticipantInfo;
        _participantsManager.OnReceivedMessage += AddMessage;

        chatTextInput.onValueChanged.AddListener(ChangeMessage);
        chatTextInput.onSubmit.AddListener(_ => { SendChat(); });
        startButton.onClick.AddListener(StartGame);
        readyButton.onClick.AddListener(Ready);
        exitButton.onClick.AddListener(LeaveRoom);
        sendButton.onClick.AddListener(SendChat);

        foreach (ParticipantUI participantUI in participants)
        {
            participantUI.OnProfileClicked += OnProfileClicked;
        }

        gameObject.SetActive(false);
    }

    private void AddMessage(string newMessage)
    {
        Text chat = Instantiate(chatTextPrefab, chatContent.transform, false);
        chat.text = newMessage;
        chatScrollView.verticalNormalizedPosition = 0f;
    }


    private void OnDestroy()
    {
        //todo unsubscribe all events 
        _participantsManager.OnEnterRoomEvent -= InitRoomUIState;
        _participantsManager.OnExitRoomEvent -= CloseRoomUI;
        _participantsManager.OnStartConditionChanged -= ChangeStartButtonVisibility;
        _participantsManager.OnPlayerStatusChangedEvent -= AddParticipantInfo;
        _participantsManager.OnPlayerLeftRoomEvent -= RemoveParticipantInfo;
        _participantsManager.OnReceivedMessage -= AddMessage;
    }

    private void InitRoomUIState()
    {
        gameObject.SetActive(true);
        _uiState.RoomName = _participantsManager.RoomName;
        _uiState.IsMaster = _participantsManager.IsMaster;
        _uiState.MaxPlayerCount = _participantsManager.MaxPlayerCount;
        _uiState.PlayerInfos.Clear();

        foreach (var playerInfoInCurrentRoom in _participantsManager.PlayerInfos)
        {
            int actorNumber = playerInfoInCurrentRoom.Key;
            PlayerInfo info = playerInfoInCurrentRoom.Value;
            _uiState.PlayerInfos.Add(actorNumber, info);

            //todo get from manager
            _uiState.ReadyPlayers.Add(actorNumber, false);
        }

        UpdateUI();
    }

    private void RemoveParticipantInfo(int actorNumber)
    {
        _uiState.PlayerInfos.Remove(actorNumber);
    }

    private void CloseRoomUI()
    {
        _uiState.PlayerInfos.Clear();
        _uiState.ReadyPlayers.Clear();
        gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        roomNameText.text = _uiState.RoomName;
        int headcount = _uiState.PlayerInfos.Count;

        int index = 0;

        foreach (var playerInfo in _uiState.PlayerInfos)
        {
            int actorNumber = playerInfo.Key;
            UpdateParticipantUI(index, actorNumber, playerInfo);
            index++;
        }

        while (index < 5)
        {
            participants[index].UpdateVisibility(false);
            index++;
        }
    }

    private void UpdateParticipantUI(int index, int actorNumber, KeyValuePair<int, PlayerInfo> playerInfo)
    {
        ParticipantUI participantUI = participants[index];
        if (!participantUI.IsInit)
        {
            participantUI.Init(actorNumber);
        }

        participantUI.UpdateVisibility(true);
        participantUI.UpdateBackgroundImage(Constants.PlayerColors[index]);
        participantUI.UpdateNicknameText(playerInfo.Value.Nickname);
        participantUI.UpdatePlayerImage(playerInfo.Value.CharacterImage);
        participantUI.UpdateWeaponImage(playerInfo.Value.WeaponData.spriteName);
    }

    private void AddParticipantInfo(int actorNumber)
    {
        Debug.Log($"actorNumber is {actorNumber}");
        PlayerInfo info = _participantsManager.PlayerInfos[actorNumber];
        bool isSuccess = _uiState.PlayerInfos.TryAdd(actorNumber, info);
        if (!isSuccess) { Debug.LogWarning($"Failed to add {actorNumber}"); }

        int index = (actorNumber - 1) % 5;
        UpdateParticipantUI(index, actorNumber, new KeyValuePair<int, PlayerInfo>(actorNumber, info));
    }

    public void ChangeStartButtonVisibility(bool isVisible)
    {
        startButton.gameObject.SetActive(isVisible);
    }

    public void LeaveRoom()
    {
        _uiState.PlayerInfos.Clear();
        _uiState.ReadyPlayers.Clear();
        _participantsManager.LeaveRoom();
    }

    public void StartGame()
    {
        _participantsManager.StartGame();
    }

    public void OnProfileClicked(int actorNumber)
    {
        if (actorNumber != PhotonNetwork.LocalPlayer.ActorNumber) { return; }

        ShowProfileDetailPopup();
    }


    private void SendChat()
    {
        _participantsManager.SendMessage();
        chatTextInput.text = string.Empty;
    }

    private void Ready()
    {
    }

    private void ShowProfileDetailPopup()
    {
        ProfileDetailUI detailUIAsset = Resources.Load<ProfileDetailUI>($"{nameof(ProfileDetailUI)}");
        ProfileDetailUI detailUI = Instantiate(detailUIAsset);
        detailUI.Init(_participantsManager, _participantsManager.Weapons);
    }

    private void ChangeMessage(string msg)
    {
        _participantsManager.OnMessageInputChanged(msg);
    }
}