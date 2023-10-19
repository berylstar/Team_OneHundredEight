using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Pickup : MonoBehaviourPunCallbacks, IPunObservable
{
    private Vector3 _curPos;
    private Quaternion _curRotation;
    private PhotonView _PV;
    private LayerMask _playerCollisionLayer;
    private float _lifeTime = 9999;
    private Define.ItemType _itemType;

    public static GameObject Create(Vector3 position, Define.ItemType pickupType = Define.ItemType.Random, float lifeTime = 9999f)
    {
        GameObject go = PhotonNetwork.Instantiate("Pickup", position, Quaternion.identity);
        Pickup p = go.GetComponent<Pickup>();

        //이렇게한이유는 IsMine인 녀석이 딱 한번 이걸 부를테니까 Init이 일반함수면 안될것같아서.
        p._PV.RPC("InitRPC", RpcTarget.AllBuffered, pickupType, lifeTime);

        return go;
    }

    [PunRPC]
    public void InitRPC(Define.ItemType pickupType, float lifeTime)
    {
        _lifeTime = lifeTime;
        _itemType = pickupType;
    }
    private void Awake()
    {
        _PV = GetComponent<PhotonView>();
        _playerCollisionLayer = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (_PV.IsMine)
        {
            _lifeTime -= Time.deltaTime;
            if (_lifeTime < 0)
            {
                _PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
            }
        }
        else
        {
            if ((transform.position - _curPos).sqrMagnitude >= 100)
            {
                transform.position = _curPos;
                transform.rotation = _curRotation;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, _curPos, Time.deltaTime * 10);
                transform.rotation = Quaternion.Lerp(transform.rotation, _curRotation, Time.deltaTime * 10);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //느린쪽에 맞춘 히트판정 , 포톤을 쓰는 녀석들만 히트판정.(안쓰는게있나?)

        //-> 아이템은 상대방콜리즌인데 내가 만든 아이템(Player)이 아니잖아 생각해보니까?
        //->그럼 
         if(!col.TryGetComponent<PhotonView>(out PhotonView pv))
            return;
        if (!pv.IsMine)
            return;

        if (0 != (_playerCollisionLayer.value & (1 << col.gameObject.layer)))
        {
            //플레이어 충돌 시
            Item.Create(col.gameObject, _itemType);

            _PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    //TODO(KDM) : 매니저를 통한 생성삭제로 바뀌면 수정
    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            _curPos = (Vector3)stream.ReceiveNext();
            _curRotation = (Quaternion)stream.ReceiveNext();
        }
    }


}
