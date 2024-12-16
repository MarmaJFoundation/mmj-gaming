using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public ParticleSystem particle;
    private EffectType effectType;
    private bool disposed;
    public void Setup(EffectType effectType, Vector3 goPosition, SpriteRenderer spriteRenderer, float scaleModifier)
    {
        this.effectType = effectType;
        disposed = false;
        transform.localScale = Vector3.one * scaleModifier;
        transform.position = goPosition;
        foreach (ParticleSystemRenderer renderer in GetComponentsInChildren<ParticleSystemRenderer>())
        {
            renderer.sortingOrder = spriteRenderer.sortingOrder;
        }
        particle.Play();
    }
    public void Setup(EffectType effectType, Vector3 goPosition, float scaleModifier)
    {
        this.effectType = effectType;
        transform.localScale = Vector3.one * scaleModifier;
        transform.position = goPosition;
        particle.Play();
    }
    public void Setup(EffectType effectType, Vector3 fromPosition, Vector3 gotoPosition, float scaleModifier)
    {
        this.effectType = effectType;
        transform.localScale = Vector3.one * scaleModifier;
        transform.position = fromPosition;
        particle.Play();
        StartCoroutine(TravelAndDestroy(gotoPosition));
    }
    private IEnumerator TravelAndDestroy(Vector3 gotoPosition)
    {
        float timer = 0;
        Vector3 fromPos = transform.position;
        while (timer <= 1)
        {
            transform.position = Vector3.Lerp(fromPos, gotoPosition, timer);
            timer += Time.deltaTime * 5;
            yield return null;
        }
        Dispose();
    }
    private void Update()
    {
        if (!disposed  && !particle.isPlaying)
        {
            Dispose();
        }
    }
    public void Dispose()
    {
        particle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        BaseUtils.effectPool[effectType].Enqueue(this);
        disposed = true;
        transform.position = Vector3.right * 10000;
    }
}
