using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BloodDraw : MonoBehaviourPunCallbacks
{
    [SerializeField] Texture2D Image;
    private Color drawColor = Color.red;
    private int brushSize = 30;
    private Texture2D texture;
    private SpriteRenderer spriteRenderer;
    Texture2D originalTexture;
    private PhotonView _PV;
    Color[] pixels;
    void Awake()
    {
        _PV = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        if (_PV.IsMine)
        {
            originalTexture = GetComponent<SpriteRenderer>().sprite.texture;
            texture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.ARGB32, false);
            texture.SetPixels(originalTexture.GetPixels());
            texture.Apply();
            pixels = texture.GetPixels();
            //_PV.RPC("UpdateSprite", RpcTarget.All);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!_PV.IsMine)
            return;

        ParticleSystem ptc = other.GetComponent<ParticleSystem>();
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        int numofCollision = ptc.GetCollisionEvents(this.gameObject, collisionEvents);
        bool pixelUpdate = false;
        while (numofCollision > 0)
        {
            --numofCollision;
            RaycastHit2D hit = Physics2D.Raycast(collisionEvents[numofCollision].intersection, collisionEvents[numofCollision].velocity.normalized);
            if (hit)
            {
                if (hit.transform == transform)
                {
                    if (!GetComponent<SpriteRenderer>().enabled)
                        return;
                    Vector2 localPos = hit.point - (Vector2)transform.position;
                    localPos = Quaternion.Inverse(transform.rotation) * localPos;
                    localPos.x = localPos.x / transform.localScale.x + 0.5f;
                    localPos.y = localPos.y / transform.localScale.y + 0.5f;
                    localPos *= spriteRenderer.sprite.pixelsPerUnit;
                    pixelUpdate = DrawTexture(localPos);
                }
            }
        }
        if (pixelUpdate)
        {
            texture.SetPixels(pixels);
            texture.Apply();
            _PV.RPC("UpdateSpriteRPC", RpcTarget.All, texture.GetRawTextureData());
        }
    }

    bool DrawTexture(Vector2 uv)
    {
        int x = (int)uv.x;
        int y = (int)uv.y;

        int startX = Mathf.Max(0, x - (int)(brushSize * transform.localScale.y / 2));
        int startY = Mathf.Max(0, y - (int)(brushSize * transform.localScale.x / 2));
        int endX = Mathf.Min(texture.width, x + (int)(brushSize * transform.localScale.y / 2));
        int endY = Mathf.Min(texture.height, y + (int)(brushSize * transform.localScale.x / 2));

        // ... 픽셀 변경 ...
        for (int i = startX; i < endX; i++)
        {
            for (int j = startY; j < endY; j++)
            {
                //현재i값 - x값의 절댓값
                if (Random.Range(0, x - startX) - 2 >= Mathf.Abs(x - i)
                || Random.Range(0, y - startY) - 2 >= Mathf.Abs(y - j))
                {
                    pixels.SetValue(drawColor, j * texture.width + i);
                }
            }
        }
        return true;
    }
    [PunRPC]
    private void UpdateSpriteRPC(byte[] receivedByte)
    {
        texture.LoadRawTextureData(receivedByte);
        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, spriteRenderer.sprite.pixelsPerUnit);
    }

}