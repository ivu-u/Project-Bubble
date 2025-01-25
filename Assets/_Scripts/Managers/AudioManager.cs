using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private SFXSourceGroup sfxSource;
    [SerializeField] private SoundClip[] musicClips, sfxClips;

    private readonly Dictionary<string, AudioClip> musicMap, sfxMap;

    public float MusicVolume { get; private set; }
    public float SFXVolume { get; private set; }

    void Awake() {
        foreach (SoundClip soundClip in musicClips) {
            musicMap[soundClip.key] = soundClip.clip;
        }
        foreach (SoundClip soundClip in sfxClips) {
            sfxMap[soundClip.key] = soundClip.clip;
        }
    }

    public void PlayMusic(string clipName) {
        if (musicMap.ContainsKey(clipName)) {
            AudioClip clip = musicMap[clipName];
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string clipName) {
        if (sfxMap.ContainsKey(clipName)) {
            AudioClip clip = sfxMap[clipName];
            sfxSource.Play(clip);
        }
    }
}

[System.Serializable]
public class SoundClip {
    public string key;
    public AudioClip clip;
}