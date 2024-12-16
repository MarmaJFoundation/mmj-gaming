using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public Sprite[] sprites;
    public SpriteRenderer spriteRenderer;
    public float animSpeed;
    public bool reverseLoop;
    public void Setup()
    {
        StartCoroutine(AnimateSprite());
    }
    private void OnEnable()
    {
        Setup();
    }
    private IEnumerator AnimateSprite()
    {
        yield return new WaitForSeconds(BaseUtils.RandomFloat(0, .25f));
        while (true)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                spriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(animSpeed);
            }
            if (reverseLoop)
            {
                for (int i = sprites.Length - 1; i >= 0; i--)
                {
                    spriteRenderer.sprite = sprites[i];
                    yield return new WaitForSeconds(animSpeed);
                }
            }
        }
    }
}
