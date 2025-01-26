using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class PlayerDeathAnimation : MonoBehaviour {
    [SerializeField] private Transform visualAsset;
    [SerializeField] private GameObject visualEffectPrefab;
    [SerializeField] private GameObject ragdollPrefab;

    [SerializeField] private float deathTime = 0.5f;

    private List<MeshRenderer> _meshes;
    private Dictionary<MeshRenderer, MaterialPropertyBlock> propertyBlocks;

    private void Start() {
        propertyBlocks = new Dictionary<MeshRenderer, MaterialPropertyBlock>();
        _meshes = visualAsset.GetComponentsInChildren<MeshRenderer>().ToList();
        
        foreach (MeshRenderer rend in _meshes)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            rend.GetPropertyBlock(mpb);
            propertyBlocks.Add(rend, mpb);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.K)) { RunDeathAnimation(); }
    }

    public void RunDeathAnimation() {
        StartCoroutine(DeathEventAction());
    }

    private IEnumerator DeathEventAction() {
        visualAsset.DOShakeRotation(deathTime * 2, new Vector3(15f, 0, 0), 20);
        yield return new WaitForSeconds(deathTime / 3);
        visualAsset.DOScale(0f, deathTime * 2).SetEase(Ease.InBounce);
        VisualEffect effect = Instantiate(visualEffectPrefab, visualAsset.transform.position, Quaternion.identity)
            .GetComponent<VisualEffect>();
        Destroy(effect, 4f);
        foreach (var kvp in propertyBlocks)
        {
            MeshRenderer rend = kvp.Key;
            MaterialPropertyBlock mpb = kvp.Value;

            Dissolve(rend, mpb);
            rend.SetPropertyBlock(mpb);
        }

        yield return new WaitForSeconds(deathTime / 2);
        Transform wand = Instantiate(ragdollPrefab, transform.position, Quaternion.Euler(new Vector3(-70f, -90f, 0f))).transform;
        wand.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(45f, 90f),Random.Range(45f, 90f), Random.Range(45f, 90f)), ForceMode.Impulse);
        wand.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0f, 5f), Random.Range(4f, 6f), Random.Range(0f, 5f)), ForceMode.Impulse);
    }
    
    void Dissolve(MeshRenderer renderer, MaterialPropertyBlock mpb)
    {
        float currDissolve = -0.5f;

        DOTween.To(() => currDissolve, x => {
                currDissolve = x;
                mpb.SetFloat("_Dissolve", currDissolve);
                renderer.SetPropertyBlock(mpb);
            }, 1.2f, deathTime * 2)
            .SetEase(Ease.InOutQuad);
    }
}
