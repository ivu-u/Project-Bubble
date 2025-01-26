using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyBubble : MonoBehaviour
{
    [SerializeField] float scaleInDuration = 0.25f;
    [SerializeField] AnimationCurve scaleInAnimCurve;
    [SerializeField] float popDuration = 0.2f;
    [SerializeField] Renderer renderer;

    [SerializeField] Vector3 bubbleVelocity = new Vector3(0, 0.25f, 0);
    [SerializeField] float bubbleTurbulence = 0.25f;
    [SerializeField] float amplitude = 2f;

    [SerializeField] float maxLifetime = 5f;
    float lifeTime = 0f;
    
    MaterialPropertyBlock mpb;

    bool collectedByPlayer = false;
    
    public MaterialPropertyBlock Mpb
    {
        get
        {
            if (mpb == null)
                mpb = new MaterialPropertyBlock();
            return mpb;
        }
    }
    
    
    void Start()
    {
        StartCoroutine(PopIn());
    }

    void Update()
    {
        if (collectedByPlayer) return;
        
        transform.position += bubbleVelocity + new Vector3(Mathf.Sin(Time.time * Mathf.PI * 2 * amplitude) * bubbleTurbulence, 0, 0);

        lifeTime += Time.deltaTime;
        if (lifeTime >= maxLifetime)
        {
            StartCoroutine(PopAndDestroy());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (collectedByPlayer) return;
        
        if (other.TryGetComponent(out Player player))
        {
            player.AddBubble();

            collectedByPlayer = true;
            
            // Pop pubble and destroy
            StartCoroutine(PopAndDestroy());
        }
    }
    
    IEnumerator PopIn()
    {
        float t = 0f;
        while (t < scaleInDuration)
        {
            t += Time.deltaTime;

            float alpha = t / scaleInDuration;
            float scale = scaleInAnimCurve.Evaluate(alpha);
            
            // Override local scale
            transform.localScale = Vector3.one * scale;
            
            yield return null;
        }
    }
    
    IEnumerator PopAndDestroy()
    {
        float t = 0f;
        while (t < scaleInDuration)
        {
            t += Time.deltaTime;

            float alpha = t / scaleInDuration;
            
            // Override local scale
            ApplyDissolve(alpha);
            
            yield return null;
        }

        Destroy(this.gameObject);
    }
    
    void ApplyDissolve(float t)
    {
        if (!renderer) return;
        
        renderer.GetPropertyBlock(Mpb);
        
        Mpb.SetFloat("_Dissolve", t);
        
        renderer.SetPropertyBlock(Mpb);
    }
}
