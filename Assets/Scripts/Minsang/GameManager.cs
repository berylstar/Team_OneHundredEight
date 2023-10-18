using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using Weapon;
using Weapon.Model;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public ObjectPooling Pooler { get; private set; }

    // 기본 정보
    [SerializeField] private GameObject panelLoading;


    // 기초 스탯 (플레이어 정보 목록)
    private Dictionary<int, PlayerStatus> _playerStatusMap;
    public IReadOnlyDictionary<int, PlayerStatus> PlayerStatusMap => _playerStatusMap;

    // 공격 정보 


    // 증강
    public EnhancementManager EnhancementManager { get; private set; }

    // PvP
    private StageManager _stageManager;
    public List<int> KnockoutPlayers { get; private set; }

    private PhotonView _photonView;

    private readonly string player = "Player";
    private readonly string keyLoadScene = "LOAD_SCENE";
    private readonly string keyLoadPlayer = "LOAD_PLAYER";

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != null) { Destroy(gameObject); }

        DontDestroyOnLoad(gameObject);
        EnhancementManager = gameObject.AddComponent<EnhancementManager>();
        _photonView = GetComponent<PhotonView>();
        Pooler = GetComponent<ObjectPooling>();
        _stageManager = GetComponentInChildren<StageManager>();

        _playerStatusMap = new Dictionary<int, PlayerStatus>(5);
        KnockoutPlayers = new List<int>(5);
    }

    private void Start()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { keyLoadScene, true } });
        StartCoroutine(CoLoading());

        SubscribeEnhancementEvents();
        EnhancementIntegrationTest();
    }

    [Obsolete("For testing")]
    private void EnhancementIntegrationTest()
    {
        ClearKnockoutPlayers();
        foreach (var currentPlayer in PhotonNetwork.CurrentRoom
                     .Players
                     .OrderBy(it => it.Value.ActorNumber)
                )
        {
            AddKnockoutPlayer(currentPlayer.Value.ActorNumber);
        }

        ShowEnhancementUI();
    }

    private IEnumerator CoLoading()
    {
        while (!AllHasTag(keyLoadScene)) { yield return null; }

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { keyLoadPlayer, true } });

        while (!AllHasTag(keyLoadPlayer)) { yield return null; }

        panelLoading.SetActive(false);
        PhotonNetwork.Instantiate(player, Vector3.zero, Quaternion.identity);
    }

    private void AddPlayerStatus()
    {
    }

    private bool AllHasTag(string key)
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties[key] == null)
                return false;
        }

        return true;
    }

    public void ShowEnhancementUI()
    {
        int cnt = KnockoutPlayers.Count;
        int[] ranking = new int[cnt];
        for (int i = 0; i < ranking.Length; i++)
        {
            ranking[i] = KnockoutPlayers[cnt - i - 1];
        }

        EnhancementManager.Init(ranking);
    }

    // 증강
    private void SubscribeEnhancementEvents()
    {
        EnhancementManager.OnEnhancementEvent += SelectEnhancement;
        EnhancementManager.OnReadyToFight += NextRound;
    }

    private void NextRound()
    {
        Debug.Log("------------------------");
        Debug.Log("Create NextRound");
        Debug.Log("------------------------");
        ClearKnockoutPlayers();
        SetPlayerSpawn();
    }

    private void SetPlayerSpawn()
    {
        List<Vector2> poses = _stageManager.SetSpawn();
        int i = 0;
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            p.transform.position = poses[i];
            i += 1;
        }
    }

    private void SelectEnhancement(int playerIndex, EnhancementData data)
    {
        Debug.Log($"({playerIndex}) select {data}");

        //todo get attackHandler to enhance
    }

    // PvP
    public void ClearKnockoutPlayers()
    {
        KnockoutPlayers.Clear();
    }

    // PvP 중 플레이어 탈락시 호출
    public void AddKnockoutPlayer(int actNum)
    {
        KnockoutPlayers.Add(actNum);
    }
}