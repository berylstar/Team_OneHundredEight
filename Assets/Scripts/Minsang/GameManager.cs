using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using Weapon;
using Weapon.Components;
using Weapon.Model;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public ObjectPooling Pooler { get; private set; }

    // 기본 정보
    [SerializeField] private GameObject panelLoading;
    [SerializeField] private TextMeshProUGUI textWinner;

    // 적용된 증강 정보


    // 증강
    public EnhancementManager EnhancementManager { get; private set; }

    // 참가자 정보 
    public int PlayerCount { get; private set; }
    public ParticipantsManager ParticipantsManager { get; private set; }

    // PvP
    private StageManager _stageManager;

    [field: SerializeField] public HashSet<int> KnockoutPlayers { get; private set; }

    //[field: SerializeField] public List<int> Winners { get; private set; }
    public Dictionary<int, int> Winners { get; private set; }

    private PhotonView _photonView;
    public GameObject myPlayer;
    private AttackHandler _attackHandler;

    private readonly string player = "Player";
    private readonly string keyLoadScene = "LOAD_SCENE";
    private readonly string keyLoadPlayer = "LOAD_PLAYER";

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != null) { Destroy(gameObject); }

        //DontDestroyOnLoad(gameObject);
        ParticipantsManager = ParticipantsManager.Instance;
        EnhancementManager = gameObject.AddComponent<EnhancementManager>();
        _photonView = GetComponent<PhotonView>();
        Pooler = GetComponent<ObjectPooling>();
        _stageManager = GetComponentInChildren<StageManager>();

        KnockoutPlayers = new HashSet<int>();
        //Winners = new List<int>();
        Winners = new Dictionary<int, int>();
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

        PlayerCount = PhotonNetwork.PlayerList.Length;
        panelLoading.SetActive(false);
        CreatePhotonPlayer();
    }

    private void CreatePhotonPlayer()
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber;
        myPlayer = PhotonNetwork.Instantiate(player, Vector3.zero, Quaternion.identity);
        _attackHandler = myPlayer.GetComponentInChildren<AttackHandler>();
        WeaponData weaponData = ParticipantsManager.PlayerInfos[PhotonNetwork.LocalPlayer.ActorNumber].WeaponData;
        _attackHandler.SetWeaponData(weaponData);
        myPlayer.GetComponent<PhotonView>().RPC("RPCSetActive", RpcTarget.All, false);
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
        int index = 0;
        foreach(int a in KnockoutPlayers)
        {
            ranking[index++] = a;
        }

        EnhancementManager.Init(ranking);
    }

    // 증강
    private void UnsubscribeEnhancementEvents()
    {
        EnhancementManager.OnReadyToFight -= NextRound;
        EnhancementManager.OnEnhancementEvent -= EnhancePlayer;
    }


    private void SubscribeEnhancementEvents()
    {
        EnhancementManager.OnReadyToFight += NextRound;
        EnhancementManager.OnEnhancementEvent += EnhancePlayer;
    }


    private void NextRound()
    {
        Debug.Log("------------------------");
        Debug.Log("Create NextRound");
        Debug.Log("------------------------");

        _stageManager.StageSelect();

        ClearKnockoutPlayers();
        SetPlayerSpawn();
        myPlayer.GetComponent<PhotonView>().RPC("RPCSetActive", RpcTarget.All, true);
    }

    private void SetPlayerSpawn()
    {
        List<Vector2> poses = _stageManager.SetSpawn();
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
                myPlayer.GetComponent<PhotonView>().RPC("RPCSetTransform", RpcTarget.All,
                    new Vector3(poses[i].x, poses[i].y, 0), Quaternion.identity);
            }
        }
    }

    // PvP
    public void ClearKnockoutPlayers()
    {
        KnockoutPlayers.Clear();
    }

    // PvP 중 플레이어 탈락시 호출
    public void AddKnockoutPlayer(int actNum)
    {
        _photonView.RPC(nameof(RPCAddKnockoutPlayer), RpcTarget.All, actNum);
    }

    [PunRPC]
    private void RPCAddKnockoutPlayer(int actNum)
    {
        KnockoutPlayers.Add(actNum);
        CheckEndBattle();
    }

    private void CheckEndBattle()
    {
        string winnerNickname = "";

        if (KnockoutPlayers.Count == PlayerCount - 1)
        {
            foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (p.activeInHierarchy)
                {
                    PhotonView pv = p.GetComponent<PhotonView>();
                    AddKnockoutPlayer(pv.Controller.ActorNumber);

                    if (Winners.ContainsKey(pv.Controller.ActorNumber))
                    {
                        Winners[pv.Controller.ActorNumber] += 1;
                    }
                    else
                    {
                        Winners[pv.Controller.ActorNumber] = 1;
                    }

                    winnerNickname = pv.Controller.NickName;
                }
            }

            // pvp종료 후 증강 선택으로 넘어감
            StartCoroutine(WinnerDelay(winnerNickname));

            if (myPlayer.activeInHierarchy)
                myPlayer.GetComponent<PhotonView>().RPC("RPCSetActive", RpcTarget.All, false);

            foreach (int v in Winners.Values)
            {
                if (v >= 2)
                {
                    // 게임 종료
                    ParticipantsManager.transform.SetParent(Camera.main.transform);
                    PhotonNetwork.LeaveRoom();
                    PhotonNetwork.LoadLevel("LobbyScene");
                }
            }

            if (Winners.Count == 3)
            {
                // 게임 종료
                ParticipantsManager.transform.SetParent(Camera.main.transform);
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LoadLevel("LobbyScene");
            }

            // 증강 다시 선택
            UnsubscribeEnhancementEvents();
            Destroy(EnhancementManager);
            EnhancementManager = gameObject.AddComponent<EnhancementManager>();
            SubscribeEnhancementEvents();
        }
    }

    private IEnumerator WinnerDelay(string name)
    {
        textWinner.text = $" WINNER IS {name} !";
        textWinner.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        textWinner.gameObject.SetActive(false);
        _stageManager.StageDelete();
        ShowEnhancementUI();
    }

    private void EnhancePlayer(int actorNumber, EnhancementData data)
    {
        if (actorNumber != PhotonNetwork.LocalPlayer.ActorNumber) { return; }

        _attackHandler.Enhance(data);
    }
}