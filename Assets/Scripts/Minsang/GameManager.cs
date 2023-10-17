using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
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
    }

    private void Start()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { keyLoadScene, true } });

        StartCoroutine(CoLoading());
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

    // 증강 선택
    public void SetEnhancement()
    {

    }

    // PvP

    public void NewGame()
    {
        KnockoutPlayers = new List<int>();
    }

    // PvP 중 플레이어 탈락시 호출
    public void PlayerKnockout(int actNum)
    {
        KnockoutPlayers.Add(actNum);
    }
}
