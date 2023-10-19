using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DeadZone : MonoBehaviourPunCallbacks
{
    private PhotonView _PV;
    private LayerMask _playerCollisionLayer;

    private void Awake()
    {
        _PV = GetComponent<PhotonView>();
        _playerCollisionLayer = LayerMask.GetMask("Player");
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.TryGetComponent<PhotonView>(out PhotonView pv))
            return;
        if (!pv.IsMine)
            return;

        if (0 != (_playerCollisionLayer.value & (1 << col.gameObject.layer)))
        {
            //플레이어 충돌 시
            Item.Create(col.gameObject, Define.ItemType.DeadZone);
        }
    }
}
