using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour {

    private static GM instance;
    public static GM Instance => instance;

    [SerializeField] private AudioManager audioManager;
    public static AudioManager AudioManager => instance.audioManager;

    [SerializeField] private TimeScaleManager timeScaleManager;
    public static TimeScaleManager TimeScaleManager => instance.timeScaleManager;

    [SerializeField] private TransitionManager transitionManager;
    public static TransitionManager TransitionManager => instance.transitionManager;

    [SerializeField] private CameraShakeManager cameraShakeManager;
    public static CameraShakeManager CameraShakeManager => instance.cameraShakeManager;

    void Awake() {
        if (instance) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}