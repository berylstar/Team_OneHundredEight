using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Boom : MonoBehaviourPunCallbacks, IPunObservable
{
    private LineRenderer _lineRenderer;
    private CircleCollider2D _circleCollider;

    [SerializeField] private int _size;
    [SerializeField] private float _distance = 0.5f;
    [SerializeField] private float _maxDistance = 3f; //최대범위를 좀 설정할수도 있으니까..
    [SerializeField] private float _width;
    [SerializeField] private float _distanceMulValue;
    [SerializeField] private float _widthMulValue;
    [SerializeField] private GameObject _user;
    [SerializeField] private float _angleGap;
    private Vector2 _baseDir = new Vector2(1, 0);


    private LayerMask _playerCollisionLayer;
    private LayerMask _bulletCollisionLayer;
    private LayerMask _wallCollisionLayer;

    private PhotonView _PV;
    public static Boom Create(GameObject user, Vector3 position, float maxDistance = 3f)
    {
        GameObject go = PhotonNetwork.Instantiate("Boom", position, Quaternion.identity);
        Boom p = go.GetComponent<Boom>();

        //이렇게한이유는 IsMine인 녀석이 딱 한번 이걸 부를테니까 Init이 일반함수면 안될것같아서.
        p.Init(maxDistance);
        p._PV.RPC(nameof(WidthSettingRPC), RpcTarget.All);

        return p;
    }
    private void Awake()
    {
        _PV = GetComponent<PhotonView>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = _size;

        _wallCollisionLayer = LayerMask.GetMask("Water");
        _playerCollisionLayer = LayerMask.GetMask("Player");
        _bulletCollisionLayer = LayerMask.GetMask("Bullet");
        //Test
        _angleGap = 360f / _size;
    }

    public void Init(float maxDistance)
    {
        if (!_PV.IsMine)
            return;

        _maxDistance = maxDistance;
        _angleGap = 360f / _size;
    }
    // Update is called once per frame
    void Update()
    {
        if (!_PV.IsMine)
            return;

        Vector3[] a = new Vector3[_size];
        //레이를 36번 박아야되는데? 사이즈가 36이면..
        for (int i = 0; i < _size; ++i)
        {
            Vector2 dir = Quaternion.Euler(0, 0, _angleGap * i) * _baseDir;
            RaycastHit2D ray = Physics2D.Raycast(transform.position, dir, _distance, _wallCollisionLayer);
            if (ray)
            {
                a.SetValue((Vector3)ray.point - transform.position, i);
            }
            else
            {
                a.SetValue((Vector3)dir * _distance, i);
            }
        }
        _PV.RPC(nameof(WidthSettingRPC), RpcTarget.All);
        _PV.RPC(nameof(LineSettingRPC), RpcTarget.All, a);
        _PV.RPC(nameof(CircleSettingRPC), RpcTarget.All, _distance);

        _distance += Time.deltaTime * _distanceMulValue;
        if (_distance > _maxDistance)
        {
            _distance = _maxDistance;
            _width -= Time.deltaTime * _widthMulValue;
            if (_width < 0)
                _PV.RPC(nameof(DestroyRPC), RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void CircleSettingRPC(float distance)
    {
        _circleCollider.radius = distance;
    }

    [PunRPC]
    public void LineSettingRPC(Vector3[] a)
    {
        _lineRenderer.SetPositions(a);
    }

    [PunRPC]
    public void WidthSettingRPC()
    {
        _lineRenderer.startWidth = _width;
        _lineRenderer.endWidth = _width;
    }
    [PunRPC]
    public void DestroyRPC()
    {
        Destroy(gameObject);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //????
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //느린쪽에 맞춘 히트판정 , 포톤을 쓰는 녀석들만 히트판정.(안쓰는게있나?)
        if (_PV.AmOwner || !col.TryGetComponent<PhotonView>(out PhotonView pv) || !pv.IsMine)
            return;
        if (col.gameObject == _user)
            return;

        if (0 != (_playerCollisionLayer.value & (1 << col.gameObject.layer)))
        {
            //플레이어 충돌 시 플레이어를 뒤로 밀고 데미지를 준다 근데 벽을 끼고있으면 데미지를 안주나?
            //그럼 이방법밖에 생각이 안납니다..

            //사실 플레이어의 콜라이더의 크기를 가져와서 각도랑 라디우스랑 잘 계산해가지고 좌측끝과 우측끝을 구한다음에
            //양쪽에 레이를 쏴서 맞은것중에 콜라이더에서 맞은게 있는지 판단해야됨
            Vector2 dir = (col.ClosestPoint(transform.position) - (Vector2)transform.position).normalized;
            RaycastHit2D[] rays = Physics2D.RaycastAll(transform.position, dir, float.MaxValue, _wallCollisionLayer | _playerCollisionLayer);
            if (rays.Length > 0)
            {
                for (int i = 0; i < rays.Length; ++i)
                {
                    if (1 << rays[i].transform.gameObject.layer == _wallCollisionLayer)
                    {
                        return;
                    }
                    else if (rays[i].transform.gameObject == col.gameObject)
                    {
                        //이때 진짜 충돌판정
                        if (col.transform.TryGetComponent<Rigidbody2D>(out var com))
                        {
                            com.AddForce(dir * 20, ForceMode2D.Impulse);
                        }
                        if(col.transform.TryGetComponent<PlayerController>(out var pc))
                        {
                            pc.SetStun(1.5f);
                        }
                        return;
                    }
                }
            }

            if (0 != (_bulletCollisionLayer.value & (1 << col.gameObject.layer)))
            {
                ////불릿 충돌 시 불릿 삭제
                //pv.RPC("DestroyRPC", RpcTarget.AllBuffered);
            }
        }
    }
}
