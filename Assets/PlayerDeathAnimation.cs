using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class PlayerDeathAnimation : MonoBehaviour {
    [SerializeField] private Player player;
    [SerializeField] private Transform visualAsset;
    [SerializeField] private GameObject visualEffectPrefab;
    [SerializeField] private GameObject ragdollPrefab;
    [SerializeField] private HairAnimator animator;
    [SerializeField] private List<QuadAnimator> quadAnimators;

    [SerializeField] private float deathTime = 0.5f;

    private List<MeshRenderer> _meshes;
    private Dictionary<MeshRenderer, MaterialPropertyBlock> propertyBlocks;

    private void Awake() {
        player.OnDeath += RunDeathAnimation;
        player.OnSpawn += Respawn;
    }

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
    public void RunDeathAnimation() {
        StartCoroutine(DeathEventAction());
    }

    public void Respawn() {
        animator.enabled = true;
        foreach (QuadAnimator qAnimator in quadAnimators) {
            qAnimator.enabled = true;
        }
        foreach (var kvp in propertyBlocks)
        {
            MeshRenderer rend = kvp.Key;
            MaterialPropertyBlock mpb = kvp.Value;

            mpb.SetFloat("_Dissolve", -0f);
            rend.SetPropertyBlock(mpb);
        }
        visualAsset.DOScale(1f, 1.3f).SetEase(Ease.OutBounce);
    }

    private IEnumerator DeathEventAction() {
        animator.enabled = false;
        foreach (QuadAnimator qAnimator in quadAnimators) {
            qAnimator.enabled = false;
        }
        visualAsset.DOShakeRotation(deathTime * 2, new Vector3(15f, 0, 0), 20);
        yield return new WaitForSeconds(deathTime / 3);
        visualAsset.DOScale(0.01f, deathTime * 2).SetEase(Ease.InBounce);
        VisualEffect effect = Instantiate(visualEffectPrefab, visualAsset.transform.position, Quaternion.identity)
            .GetComponent<VisualEffect>();
        //effect.gameObject.SetActive(false);
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
        float currDissolve = 0f;

        DOTween.To(() => currDissolve, x => {
                currDissolve = x;
                mpb.SetFloat("_Dissolve", currDissolve);
                renderer.SetPropertyBlock(mpb);
            }, 1.2f, deathTime * 2)
            .SetEase(Ease.InOutQuad);
    }
}
