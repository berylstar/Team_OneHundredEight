using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class WeaponController : MonoBehaviourPun
{
    private Camera _camera;
    public event Action<Vector2> OnRotateEvent;
    public event Action<Vector2> OnFireEvent;

    private float _deg;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected)
        {
            return;
        }

        //todo remove
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 dir = MousePositionToDirection(mousePos);
            InvokeFire(dir);
        }
    }

    public void InvokeRotate(Vector2 dir)
    {
        //todo screen position??
        OnRotateEvent?.Invoke(dir);
    }

    public void InvokeFire(Vector2 dir)
    {
        OnFireEvent?.Invoke(dir);
    }

    private Vector2 MousePositionToDirection(Vector2 mousePos)
    {
        Vector2 pos = transform.position;
        Vector2 worldMousePos = _camera.ScreenToWorldPoint(mousePos);
        return worldMousePos - pos;
    }

    private void CallRotateOnMouseInput()
    {
        Vector2 pos = transform.position;
        Vector2 mousePos = Input.mousePosition;
        Vector2 worldMousePos = _camera.ScreenToWorldPoint(mousePos);
        Vector2 dir = worldMousePos - pos;
        InvokeRotate(dir.normalized);
    }
}