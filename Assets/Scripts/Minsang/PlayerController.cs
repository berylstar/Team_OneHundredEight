using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private SpriteRenderer _playerRenderer;
    [SerializeField] private Transform _footPivot;
    private Camera _cam;

    [Header("Weapon")]
    [SerializeField] private Transform _weaponTransform;
    [SerializeField] private SpriteRenderer _weaponRenderer;

    public event Action<Vector2> OnFire;

    [Header("Canvas")]
    [SerializeField] private GameObject _canvas;
    [SerializeField] private TextMeshProUGUI _textNickname;
    [SerializeField] private Image _hpBar ;

    // private Animator _animator;
    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput;
    private PlayerStatHandler _stat;

    private PhotonView _photonView;

    private Vector2 _moveInput;
    private Vector2 _newAim;
    
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

    private void FixedUpdate()
    {
        _moveInput.y = _rigidbody.velocity.y;
        _rigidbody.velocity = _moveInput;
    }

    #region InputAction

    private void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>().normalized * _stat.CurrentStat.MoveSpeed;
    }

    private void OnJump(InputValue value)
    {
        RaycastHit2D rayHit = Physics2D.Raycast(_footPivot.position, Vector3.down, 0.125f, LayerMask.GetMask("Stage"));

        if (rayHit.collider == null)
            return;

        _rigidbody.AddForce(Vector2.up * _stat.CurrentStat.JumpForce, ForceMode2D.Impulse);
    }

    private void OnAim(InputValue value)
    {
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
    }

    private void OnShoot(InputValue value)
    {
        OnFire?.Invoke(_newAim);
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

    public void Hit(int damage)
    {
        _photonView.RPC(nameof(ShowHP), RpcTarget.All, damage);
    }

    [PunRPC]
    private void ShowHP(int damage)
    {
        _stat.ChangeHealth(-1 * damage);
        _hpBar.fillAmount = _stat.CurrentStat.HP / _stat.CurrentStat.MaxHp;
    }
}
