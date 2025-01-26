using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXSourceGroup : MonoBehaviour {

    [SerializeField] private AudioSource[] sfxSources;

    private readonly Dictionary<AudioSource, float> useMap = new();

    void Awake() {
        foreach (AudioSource src in sfxSources) {
            useMap.TryAdd(src, 0);
        }
    }

    public void Play(AudioClip clip) {
        AudioSource lowSrc = null;
        foreach (KeyValuePair<AudioSource, float> kvp in useMap) {

            if (lowSrc == null || kvp.Value < useMap[lowSrc]) {
                lowSrc = kvp.Key;
            }

            if (kvp.Value <= 0) {
                AudioSource src = kvp.Key;
                src.PlayOneShot(clip);
                useMap[kvp.Key] = clip.length;
                StartCoroutine(ISourceTimer(src));
                return;
            }
        }
        if (lowSrc) {
            lowSrc.PlayOneShot(clip);
            useMap[lowSrc] = clip.length;
            StartCoroutine(ISourceTimer(lowSrc));
        }
    }

    private IEnumerator ISourceTimer(AudioSource src) {
        float srcVal = useMap[src];
        while (srcVal > 0) {
            srcVal = useMap[src];
            useMap[src] = Mathf.MoveTowards(srcVal, 0, Time.unscaledDeltaTime);
            yield return null;
        }
    }
}