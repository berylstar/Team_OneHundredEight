using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall : MonoBehaviourPun//, IPunObservable
{
    [SerializeField] private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            FallingPlatform(_rb);
        }  
    }
    private void FallingPlatform(Rigidbody2D rb)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.mass = 200f;
        rb.gravityScale = 0.001f;
        Destroy(gameObject, 30f);
    }

    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_rb.position);
            stream.SendNext(_rb.velocity);
        }
        else
        {
            _rb.position = (Vector2)stream.ReceiveNext();
            _rb.velocity = (Vector2)stream.ReceiveNext();
        }
    }
    */
}
