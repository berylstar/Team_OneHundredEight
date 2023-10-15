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
    private float _value;
    private Define.ItemType _itemType;
 
    public static GameObject Create(Vector3 position , Define.ItemType pickupType = Define.ItemType.Random, float lifeTime = 9999f)
    {
        GameObject go = PhotonNetwork.Instantiate("Pickup" , position, Quaternion.identity);
        Pickup p = go.GetComponent<Pickup>();

        //�̷����������� IsMine�� �༮�� �� �ѹ� �̰� �θ��״ϱ� Init�� �Ϲ��Լ��� �ȵɰͰ��Ƽ�.
        p._PV.RPC("InitRPC", RpcTarget.AllBuffered , pickupType, lifeTime);

        return go;
    }


    [PunRPC]
    public void InitRPC(Define.ItemType pickupType , float lifeTime)
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
            Item.Create(col.gameObject , _itemType);

            _PV.RPC("DestroyRPC" , RpcTarget.AllBuffered);
        }
    }

    //TODO(KDM) : �Ŵ����� ���� ���������� �ٲ�� ����
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
