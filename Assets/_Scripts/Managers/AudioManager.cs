using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private SFXSourceGroup sfxSource;
    [SerializeField] private SoundClip[] musicClips, sfxClips;

    private readonly Dictionary<string, AudioClip> musicMap = new(),
                                                   sfxMap = new();

    public float MusicVolume { get; private set; }
    public float SFXVolume { get; private set; }

    void Awake() {
        foreach (SoundClip soundClip in musicClips) {
            musicMap[soundClip.key] = soundClip.clip;
        }
        foreach (SoundClip soundClip in sfxClips) {
            sfxMap[soundClip.key] = soundClip.clip;
        }
        PlayMusic("Music");
    }

    public void PlayMusic(string clipName) {
        if (musicMap.TryGetValue(clipName, out AudioClip clip)) {
            musicSource.clip = clip;
            musicSource.Play();
        } else Debug.LogWarning($"Can't find Music Clip named '{clipName}'");
    }

    public void PlaySFX(string clipName) {
        if (sfxMap.TryGetValue(clipName, out AudioClip clip)) {
            sfxSource.Play(clip);
        } else Debug.LogWarning($"Can't find Sound Clip named '{clipName}'");
    }
}

[System.Serializable]
public class SoundClip {
    public string key;
    public AudioClip clip;
}