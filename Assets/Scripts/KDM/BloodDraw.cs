using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BloodDraw : MonoBehaviour
{
    [SerializeField] Texture2D Image;
    private Color drawColor = Color.red;
    private int brushSize = 30;
    private Texture2D texture;
    private SpriteRenderer spriteRenderer;
   // private PhotonView _PV;
    Color[] pixels;
    Sprite originalTexture;
    void Awake()
    {
       // _PV = GetComponentInParent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {

        originalTexture = GetComponent<SpriteRenderer>().sprite;
        Rect r = originalTexture.rect;
        texture = new Texture2D((int)r.width, (int)r.height, TextureFormat.ARGB32, false);
        texture.SetPixels(originalTexture.texture.GetPixels((int)r.xMin, (int)r.yMin, (int)r.width, (int)r.height, 0));
        texture.Apply();
        brushSize = 30;
        pixels = texture.GetPixels();
        //_PV.RPC("UpdateSprite", RpcTarget.All);

    }

    private void OnParticleCollision(GameObject other)
    {
        //본체만 바꾸고 어찌저찌 해볼랬는데 뭔가 애매해서...
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
            //_PV.RPC("UpdateSpriteRPC", RpcTarget.All, texture.GetRawTextureData());
            UpdateSpriteRPC(texture.GetRawTextureData());
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
        int longX = Mathf.Max(Mathf.Abs(startX - x), Mathf.Abs(endX - x));
        int longY = Mathf.Max(Mathf.Abs(startY - y), Mathf.Abs(endY - y));

        // ... 픽셀 변경 ...
        for (int i = startX; i < endX; i++)
        {
            for (int j = startY; j < endY; j++)
            {
                //현재i값 - x값의 절댓값
                
                if (pixels[j * texture.width + i].a > 0.2f &&
                (Random.Range(0, longX) - 2 >= Mathf.Abs(x - i)
                && Random.Range(0, longY) - 2 >= Mathf.Abs(y - j)))
                {
                    pixels.SetValue(drawColor, j * texture.width + i);
                }
            }
        }
        return true;
    }

    //[PunRPC]
    private void UpdateSpriteRPC(byte[] receivedByte)
    {
        texture.LoadRawTextureData(receivedByte);
        Vector2 middlePos = new Vector2(originalTexture.pivot.x / originalTexture.rect.width , originalTexture.pivot.y / originalTexture.rect.height);
        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), middlePos, spriteRenderer.sprite.pixelsPerUnit);
    }

}