using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class WeaponRotation : MonoBehaviourPun
{
    [SerializeField] private SpriteRenderer _weaponRenderer;
    [SerializeField] private Transform _weaponPivot;
    private WeaponController _controller;

    private void Awake()
    {
        _controller = GetComponent<WeaponController>();
    }

    private void Start()
    {
        _controller.OnRotateEvent += RotateWeapon;
    }

    private void RotateWeapon(Vector2 dir)
    {
        float deg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _weaponPivot.rotation = Quaternion.Euler(0, 0, deg);
        _weaponRenderer.flipY = Mathf.Abs(deg) >= 90f;
    }
}