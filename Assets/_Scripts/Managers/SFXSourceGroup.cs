using System.Collections.Generic;
using UnityEngine;

public class SFXSourceGroup : MonoBehaviour {

    [SerializeField] private AudioSource[] sfxSources;

    private readonly Dictionary<AudioSource, float> useMap;

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
                return;
            }
        }
        lowSrc.PlayOneShot(clip);
    }
}