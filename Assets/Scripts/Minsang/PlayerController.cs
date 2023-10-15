using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private SpriteRenderer _playerRenderer;
    [SerializeField] private Transform _footPivot;
    [SerializeField] private Camera _playerCamera;

    // private Animator _animator;
    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput;
    private PlayerStat _stat;
    
    [Header("Weapon")]
    [SerializeField] private Transform _weaponTransform;
    [SerializeField] private SpriteRenderer _weaponRenderer;
    public event Action EventShoot = null;

    [Header("Canvas")]
    [SerializeField] private GameObject _canvas;
    [SerializeField] private TextMeshProUGUI _textNickname;
    [SerializeField] private Image _hpBar ;

    private Vector2 _moveInput;
    private float _rotZ;
    private PhotonView _photonView;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _photonView = GetComponent<PhotonView>();
        _stat = GetComponent<PlayerStat>();
    }

    private void Start()
    {
        _textNickname.text = _photonView.Owner.ActorNumber.ToString();

        if (!_photonView.IsMine)
        {
            _playerInput.enabled = false;
        }
        else
        {
            _playerCamera.gameObject.SetActive(true);
        }
    }

    #region InputAction

    private void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>().normalized * _stat.MoveSpeed;
        _moveInput.y = _rigidbody.velocity.y;
        
        _rigidbody.velocity = _moveInput;
    }

    private void OnJump(InputValue value)
    {
        RaycastHit2D rayHit = Physics2D.Raycast(_footPivot.position, Vector3.down, 0.125f, LayerMask.GetMask("Water"));

        if (rayHit.collider == null)
            return;

        _rigidbody.AddForce(Vector2.up * _stat.JumpForce, ForceMode2D.Impulse);
    }

    private void OnAim(InputValue value)
    {
        Vector2 worldPos = _playerCamera.ScreenToWorldPoint(value.Get<Vector2>());
        Vector2 newAim = (worldPos - (Vector2)transform.position).normalized;

        if (newAim.magnitude >= 0.5f)
        {
            _rotZ = Mathf.Atan2(newAim.y, newAim.x) * Mathf.Rad2Deg;
            _weaponRenderer.flipY = Mathf.Abs(_rotZ) > 90f;
            _playerRenderer.flipX = Mathf.Abs(_rotZ) > 90f;
            _weaponTransform.rotation = Quaternion.Euler(0, 0, _rotZ);
        }
    }

    private void OnShoot(InputValue value)
    {
        EventShoot?.Invoke();
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_photonView.IsMine)
            return;

        if (collision.CompareTag("Item"))
        {
            Debug.Log("ITEM");
        }
    }
}
