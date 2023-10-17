using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: SerializeField] public List<PlayerStatHandler> PlayerStats { get; private set; }
    [field: SerializeField] public List<WeaponData> Weapons { get; private set; }

    private PhotonView _photonView;
    public ObjectPooling Pooler { get; private set; }

    [SerializeField] private GameObject panelLoading;

    private readonly string player = "Player";

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != null) { Destroy(gameObject); }
        DontDestroyOnLoad(gameObject);

        _photonView = GetComponent<PhotonView>();
        Pooler = GetComponent<ObjectPooling>();

        PlayerStats = new List<PlayerStatHandler>();
    }

    private void Start()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "LOAD_SCENE", true } });

        StartCoroutine(CoLoading());
    }

    private IEnumerator CoLoading()
    {
        while (!AllHasTag("LOAD_SCENE")) { yield return null; }

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "LOAD_PLAYER", true } });

        while (!AllHasTag("LOAD_PLAYER")) { yield return null; }

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
}
