using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodDraw : MonoBehaviour
{
    [SerializeField] Texture2D Image;
    private Color drawColor = Color.red;
    private int brushSize = 100;
    private Texture2D texture;
    private SpriteRenderer spriteRenderer;
    Texture2D originalTexture;

    void Start()
    {
        originalTexture = GetComponent<SpriteRenderer>().sprite.texture;
        texture = new Texture2D(originalTexture.width , originalTexture.height , TextureFormat.ARGB32 , false);
        texture.SetPixels(originalTexture.GetPixels());
        texture.Apply();

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite.Create(texture , new Rect(0 , 0 , texture.width , texture.height) , Vector2.one * 0.5f, spriteRenderer.sprite.pixelsPerUnit);
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticleSystem ptc = other.GetComponent<ParticleSystem>();
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        int numofCollision = ptc.GetCollisionEvents(this.gameObject , collisionEvents);

        while (numofCollision > 0)
        {
            --numofCollision;
            RaycastHit2D hit = Physics2D.Raycast(collisionEvents[numofCollision].intersection , collisionEvents[numofCollision].velocity.normalized);
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
                    DrawTexture(localPos);
                    UpdateSprite();
                }
            }
        }
    }

    public void DrawTexture(Vector2 uv)
    {
        int x = (int)uv.x;
        int y = (int)uv.y;

        int startX = Mathf.Max(0 , x - (int)( brushSize * transform.localScale.y / 2 ));
        int startY = Mathf.Max(0 , y - (int)( brushSize * transform.localScale.x / 2 ));
        int endX = Mathf.Min(texture.width , x + (int)( brushSize * transform.localScale.y / 2)) ;
        int endY = Mathf.Min(texture.height , y + (int)(brushSize * transform.localScale.x / 2));

        for (int i = startX ; i < endX ; i++)
        {
            for (int j = startY ; j < endY ; j++)
            {
                //현재i값 - x값의 절댓값
                if (Random.Range(0 , x - startX) >= Mathf.Abs(x - i)
                || Random.Range(0 , y - startY) >= Mathf.Abs(y - j))
                {
                    texture.SetPixel(i , j , drawColor);
                }
            }
        }

        texture.Apply();
    }

    private void UpdateSprite()
    {
        spriteRenderer.sprite = Sprite.Create(texture , new Rect(0 , 0 , texture.width , texture.height) , Vector2.one * 0.5f, spriteRenderer.sprite.pixelsPerUnit);
    }
}