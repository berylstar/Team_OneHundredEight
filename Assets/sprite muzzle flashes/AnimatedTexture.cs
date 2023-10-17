using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class AnimatedTexture : MonoBehaviourPunCallbacks
{
    [SerializeField] private float now = 0.0f;
    public float delay;
    public Sprite[] frames;

    private int frameIndex;
    private SpriteRenderer rendererMy;
   

    private PhotonView _PV;
    void Awake()
    {
        _PV = GetComponent<PhotonView>();
        rendererMy = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (!_PV.IsMine)
            return;

        now += Time.deltaTime;

        if (now <  delay)
            return;

        now = 0.0f;

        _PV.RPC("UpdateSpriteRPC", RpcTarget.All);
        
        if (frameIndex >= frames.Length)
            _PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void UpdateSpriteRPC()
    {
        rendererMy.sprite = frames[frameIndex];
        frameIndex = frameIndex + 1;
    }

    [PunRPC]
    void DestroyRPC()
    {
        Destroy(gameObject);
    }
}