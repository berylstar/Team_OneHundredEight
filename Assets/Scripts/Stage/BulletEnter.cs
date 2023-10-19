using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon.Controller;

public class BulletEnter : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _bulletRb;

    private ProjectileController PC;
    private void Awake()
    {
        PC = GetComponent<ProjectileController>();
        _bulletRb = PC.GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            PC.Disapear();
        }
    }
}
