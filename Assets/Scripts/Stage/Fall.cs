using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall : MonoBehaviour
{
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.mass = 50f;
            _rb.gravityScale = 0.02f;
            Destroy(gameObject, 8f);
        }
    }
}
