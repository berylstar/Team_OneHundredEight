using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
public class ItemSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private float _spawnDelay;
    private float _spawnTime = 0.0f;
    PhotonView _PV;
    [SerializeField]Transform spawnPoint;
    private void Awake()
    {
        _PV = GetComponent<PhotonView>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!_PV.IsMine)
            return;
        _spawnTime += Time.deltaTime;

        if (_spawnTime < _spawnDelay)
            return;
        _spawnTime = 0.0f;

        Pickup.Create(spawnPoint.position, Define.ItemType.Random, 15.0f);
    }
}
