using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Photon.Pun;
using System.Collections;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Player")]
    [SerializeField] private SpriteRenderer _playerRenderer;
    [SerializeField] private Transform _footPivot;
    [SerializeField] private Animator _playerAni;
    private Camera _cam;

    [Header("Weapon")]
    [SerializeField] private Transform _weaponTransform;
    [SerializeField] private SpriteRenderer _weaponRenderer;

    public event Action<Vector2> OnFire;

    [Header("Canvas")]
    [SerializeField] private GameObject _canvas;
    [SerializeField] private TextMeshProUGUI _textNickname;
    [SerializeField] private Image _hpBar ;

    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput;
    private PlayerStatHandler _stat;

    private PhotonView _photonView;

    private Vector2 _moveInput;
    private Vector2 _newAim;

    private int _maxJumpCount = 1;
    private int _jumpCount = 1;
    private float _stun = 0f;
    
    private int _animWalk = Animator.StringToHash("IsWalk");
    private int _animJump = Animator.StringToHash("TrJump");
    private int _animAir = Animator.StringToHash("IsAir");


    [Header("Boom")]
    [SerializeField] private Transform _boomTransform;
    [SerializeField] private Image _boomImage;
    [SerializeField] private float _maxBoomDelay;
    private bool _boomReady = true;
    private float _boomDelay = 0f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _stat = GetComponent<PlayerStatHandler>();
        _photonView = GetComponent<PhotonView>();

        _cam = Camera.main;
    }

    private void Start()
    {
        _textNickname.text = _photonView.Owner.NickName;
        if (!_photonView.IsMine)
        {
            _playerInput.enabled = false;
        }
        else
        {
            //var cvc = _cam.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
            //cvc.Follow = transform;
            //cvc.LookAt = transform;
        }
    }
    private void Update()
    {
        if (_jumpCount != _maxJumpCount && _rigidbody.velocity.y <= 0)
        {
            RaycastHit2D[] rayHit = Physics2D.RaycastAll(_footPivot.position, Vector3.down, 0.125f);

            for (int i = 0; i < rayHit.Length; ++i)
            {
                if (rayHit[i].transform.gameObject != this.gameObject)
                {
                    _jumpCount = _maxJumpCount;
                    PhotonNetwork.Instantiate("Effects/Land", rayHit[i].point, Quaternion.identity);
                }
            }
        }

        if(_stun >= 0f)
        {
            _stun -= Time.deltaTime;
        }
        if(!_boomReady)
        {
            _boomDelay -= Time.deltaTime;
            if(_boomDelay < 0)
            {
                _boomDelay = 0f;
                _boomReady = true;
            }
            _boomImage.fillAmount = (_maxBoomDelay - _boomDelay) / _maxBoomDelay;
        }

        _hpBar.fillAmount = _stat.CurrentStat.HP / _stat.CurrentStat.MaxHp;
    }
    private void FixedUpdate()
    {
        _moveInput.y = _rigidbody.velocity.y;
        _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity , _moveInput, 0.3f);

        _playerAni.SetBool(_animAir, _rigidbody.velocity.y != 0);
    }

    #region InputAction

    private void OnMove(InputValue value)
    {
        if (_stun > 0)
        {
            _moveInput = Vector2.zero;
            _playerAni.SetBool(_animWalk, false);
        }
        else
        {
            _moveInput = value.Get<Vector2>().normalized * _stat.CurrentStat.MoveSpeed;

            _playerAni.SetBool(_animWalk, _moveInput != Vector2.zero);
        }
    }

    private void OnJump(InputValue value)
    {
        //RaycastHit2D rayHit = Physics2D.Raycast(_footPivot.position, Vector3.down, 0.125f, LayerMask.GetMask("Stage"));

        //if (rayHit.collider == null)
        //    return;
        if (_stun > 0)
        {
            return;
        }

        if (_jumpCount < 1)
            return;

        --_jumpCount;
        _rigidbody.AddForce(Vector2.up * _stat.CurrentStat.JumpForce, ForceMode2D.Impulse);
        _playerAni.SetTrigger(_animJump);
    }

    private void OnAim(InputValue value)
    {
        if (_stun > 0)
        {
            return;
        }

        Vector2 worldPos = _cam.ScreenToWorldPoint(value.Get<Vector2>());
        _newAim = (worldPos - (Vector2)transform.position).normalized;

        if (_newAim.magnitude >= 0.5f)
        {
            float _rotZ = Mathf.Atan2(_newAim.y, _newAim.x) * Mathf.Rad2Deg;
            _weaponTransform.rotation = Quaternion.Euler(0, 0, _rotZ);
            _photonView.RPC(nameof(RPCRendererFlip), RpcTarget.All, Mathf.Abs(_rotZ) > 90f);
        }
    }

    [PunRPC]
    private void RPCRendererFlip(bool flag)
    {
        _weaponRenderer.flipY = flag;
        _playerRenderer.flipX = flag;
        Vector3 pos = _boomTransform.localPosition;
        pos.x = flag ? 40f : -40f;
        _boomTransform.localPosition = pos;
    }

    private void OnShoot(InputValue value)
    {
        if (_stun > 0)
        {
            return;
        }

        OnFire?.Invoke(_newAim);
    }

    private void OnBoom(InputValue value)
    {
        if(_stun <= 0 &&  _photonView.IsMine && _boomReady)
        {
            _boomReady = false;
            _boomDelay = _maxBoomDelay;
            Boom.Create(this.gameObject, this.transform.position);
        }
    }

    public void SetStun(float stunTime)
    {
        _stun = stunTime;
    }
    #endregion

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (!_photonView.IsMine || collision.GetComponent<PhotonView>().IsMine)
    //        return;

    //    if (collision.CompareTag("Bullet"))
    //    {
    //        _stat.ChangeHealth(-1 * collision.GetComponent<Weapon.Controller.ProjectileController>().Damage);
    //        _photonView.RPC(nameof(ShowHP), RpcTarget.All);
    //    }
    //}
}
