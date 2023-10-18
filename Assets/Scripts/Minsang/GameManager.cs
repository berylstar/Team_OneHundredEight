using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
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
    [SerializeField] private List<Transform> spawnList;
    // 기초 스탯 (플레이어 스탯, 무기 정보, 공격 스탯, 맵 정보...)

    // 증강 선택
    private EnhancementManager _enhancementManager;

    public EnhancementManager EnhancementManager
    {
        get
        {
            if (_enhancementManager == null)
            {
                _enhancementManager = gameObject.AddComponent<EnhancementManager>();
                SubscribeEnhancementEvents();
            }

            return _enhancementManager;
        }
    }

    // PvP
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
        _photonView = GetComponent<PhotonView>();
        Pooler = GetComponent<ObjectPooling>();

        KnockoutPlayers = new List<int>(10);

    }

    private void Start()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { keyLoadScene, true } });
        StartCoroutine(CoLoading());

        EnhancementIntegrationTest();
    }

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
        _enhancementManager.OnEnhancementEvent += SelectEnhancement;
        _enhancementManager.OnReadyToFight += NextRound;
    }

    private void NextRound()
    {
        Debug.Log("------------------------");
        Debug.Log("Create NextRound");
        Debug.Log("------------------------");
        ClearKnockoutPlayers();
    }

    private void SelectEnhancement(int playerIndex, EnhancementData data)
    {
        Debug.Log($"Player{playerIndex} select {data}");
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