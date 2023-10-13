using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Pickup : MonoBehaviourPunCallbacks, IPunObservable
{
    private Vector3     _curPos;
    private Quaternion  _curRotation;
    private PhotonView  _PV;
    private LayerMask   _playerCollisionLayer;

    private float _lifeTime;
    private float _buffDuration;
    private bool _isTimed = true;
    private float _value;
    public enum Type
    {
        HpUp,
        HpDown,
        SpeedUp,
        SpeedDown,
        ReverseKey,
        Invinvible
    }

    //밸류는 역시 배율이 낫겠죠?
    public static GameObject CreatePickup(Vector3 position , Pickup.Type pickupType , float buffDuration = 0f , float lifeTime = 9999f )
    {
        GameObject go = PhotonNetwork.Instantiate("Pickup" , position, Quaternion.identity);
        Pickup p = go.GetComponent<Pickup>();

        p.Init(pickupType, lifeTime , buffDuration);

        return go;
    }

    public void Init(Pickup.Type pickupType , float lifeTime, float buffDuration)
    {
        _lifeTime = lifeTime;
        _buffDuration = buffDuration;
        
        //TODO : 여기서 만들 아이템의 SO를 불러오면 될듯?
        switch (pickupType)
        {
            case Pickup.Type.HpUp:
                _isTimed = false;
                break;
            case Pickup.Type.HpDown:
                _isTimed = false;
                break;
            case Pickup.Type.SpeedUp:
                _isTimed = true;
                break;
            case Pickup.Type.SpeedDown:
                _isTimed = true;
                break;
            case Pickup.Type.ReverseKey:
                _isTimed = true;
                break;
            case Pickup.Type.Invinvible:
                _isTimed = true;
                break;
        }
    }
    private void Awake()
    {
        _PV = GetComponent<PhotonView>();
        _playerCollisionLayer = LayerMask.GetMask("Player");
    }
    
    // Update is called once per frame
    void Update()
    {
        if(_PV.IsMine)
        {
            _lifeTime -= Time.deltaTime;
            if( _lifeTime < 0 )
            {
                _PV.RPC("DestroyRPC" , RpcTarget.AllBuffered);
            }
        }
        else
        {
            //렉이걸려서 포지션같은게 확 틀어지면(10이상) 텔포가 맞지만 회전이든 포지션변환이든 이게 맞지않을까 싶네 갖다써도 ㅇㅇ.
            //혹시 요상하면 회전값은 바로 적용하는걸로 바꿉시다. 당장엔 테스트가 어려우니까.
            if((transform.position - _curPos).sqrMagnitude >= 100)
            {
                transform.position = _curPos;
                transform.rotation = _curRotation;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position , _curPos , Time.deltaTime * 10);
                transform.rotation = Quaternion.Lerp(transform.rotation , _curRotation , Time.deltaTime * 10);
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //느린쪽에 맞춘 히트판정 , 포톤을 쓰는 녀석들만 히트판정.(안쓰는게있나?)
        if (_PV.IsMine || !col.TryGetComponent<PhotonView>(out PhotonView pv) || !pv.IsMine)
            return;

        if (0 != ( _playerCollisionLayer.value & ( 1 << col.gameObject.layer ) ))
        {
            //플레이어 충돌 시

            //TODO : 플레이어 스텟 증가

            _PV.RPC("DestroyRPC" , RpcTarget.AllBuffered);
        }
    }

    //TODO : 매니저를 통한 생성삭제로 바뀌면 수정
    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    public void OnPhotonSerializeView(PhotonStream stream , PhotonMessageInfo info)
    {
        if(stream.IsWriting)
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
