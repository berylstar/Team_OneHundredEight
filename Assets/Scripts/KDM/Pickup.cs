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

    //����� ���� ������ ������?
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
        
        //TODO : ���⼭ ���� �������� SO�� �ҷ����� �ɵ�?
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
            //���̰ɷ��� �����ǰ����� Ȯ Ʋ������(10�̻�) ������ ������ ȸ���̵� �����Ǻ�ȯ�̵� �̰� ���������� �ͳ� ���ٽᵵ ����.
            //Ȥ�� ����ϸ� ȸ������ �ٷ� �����ϴ°ɷ� �ٲ߽ô�. ���忣 �׽�Ʈ�� �����ϱ�.
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
        //�����ʿ� ���� ��Ʈ���� , ������ ���� �༮�鸸 ��Ʈ����.(�Ⱦ��°��ֳ�?)
        if (_PV.IsMine || !col.TryGetComponent<PhotonView>(out PhotonView pv) || !pv.IsMine)
            return;

        if (0 != ( _playerCollisionLayer.value & ( 1 << col.gameObject.layer ) ))
        {
            //�÷��̾� �浹 ��

            //TODO : �÷��̾� ���� ����

            _PV.RPC("DestroyRPC" , RpcTarget.AllBuffered);
        }
    }

    //TODO : �Ŵ����� ���� ���������� �ٲ�� ����
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
