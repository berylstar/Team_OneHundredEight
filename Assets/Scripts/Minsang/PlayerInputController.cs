using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

using Photon.Pun;
using Photon.Realtime;

public class PlayerInputController : MonoBehaviour
{
    [Header("Player Stats")]
    public float jumpForce;
    public float speed;

    [Header("Player")]
    [SerializeField] private SpriteRenderer _playerRenderer;
    [SerializeField] private Transform _footPivot;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    [SerializeField] private Camera _playerCamera;
    
    [Header("Weapon")]
    [SerializeField] private Transform _weaponTransform;
    [SerializeField] private SpriteRenderer _weaponRenderer;
    public event Action EventShoot = null;

    [Header("Canvas")]
    [SerializeField] private GameObject _canvas;
    [SerializeField] TextMeshProUGUI textNickname;

    private Vector2 _moveInput;
    private float _rotZ;
    private PhotonView photonView;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            InitialMyPlayer();
        }
    }

    private void OnMove(InputValue value)
    {
        if (!photonView.IsMine)
            return;

        _moveInput = value.Get<Vector2>().normalized * speed;
        _moveInput.y = _rigidbody.velocity.y;
        
        _rigidbody.velocity = _moveInput;
    }

    private void OnJump(InputValue value)
    {
        if (!photonView.IsMine)
            return;

        RaycastHit2D rayHit = Physics2D.Raycast(_footPivot.position, Vector3.down, 0.125f, LayerMask.GetMask("Water"));

        if (rayHit.collider == null)
            return;

        _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnAim(InputValue value)
    {
        if (!photonView.IsMine)
            return;

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
        if (!photonView.IsMine)
            return;

        Debug.Log(value.isPressed);
        EventShoot?.Invoke();
    }

    private void InitialMyPlayer()
    {
        _playerCamera.gameObject.SetActive(true);
        _canvas.SetActive(true);
        textNickname.text = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
    }
}
