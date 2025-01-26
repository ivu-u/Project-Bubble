using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;
using Cinemachine;

public enum RoomTag { L1, L2, L3 }

public class TransitionManager : MonoBehaviour {

    [SerializeField] private TransitionEffector transitionController;
    [SerializeField] private VisualEffect bubbleVFX;
    [SerializeField] private Vector3 fxCameraOffset;
    [SerializeField] private AnimationCurve timeScaleInCurve, timeScaleOutCurve;
    [SerializeField] private float softLoadTime, bubbleWait;
    [SerializeField] private SceneIdentifier[] sceneIDs;
    [SerializeField] private FullScreenPassRendererFeature feature;

    private readonly Dictionary<RoomTag, SceneIdentifier> sceneMap = new();

    void Awake() {
        bubbleVFX.Stop();
        foreach (SceneIdentifier sceneID in sceneIDs) {
            sceneMap[sceneID.roomTag] = sceneID;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.U)) LoadLevel(RoomTag.L1);
    }

    public void LoadLevel(RoomTag roomTag) => StartCoroutine(IBubbleLoad(roomTag));

    private IEnumerator IBubbleLoad(RoomTag roomTag) {
        if (Camera.main.TryGetComponent(out CinemachineBrain cameraBrain)) {
            Vector3 camPos = cameraBrain.OutputCamera.transform.position;
            bubbleVFX.transform.position = camPos + fxCameraOffset;
            bubbleVFX.Play();
            yield return new WaitForSecondsRealtime(bubbleWait);
        }

        TimeScaleCore tsCore = GM.TimeScaleManager.AddTimeScaleShift(-1, 0.5f, timeScaleInCurve);
        tsCore.OnCoreDeath += () => GM.TimeScaleManager.GlobalTimeScale = 0;
        transitionController.OnTransitionEnd += () => StartCoroutine(ILoadLevel(roomTag));
        transitionController.Fade();
    }

    private IEnumerator ILoadLevel(RoomTag roomTag) {
        if (sceneMap.TryGetValue(roomTag, out SceneIdentifier sceneID)) {
            feature.passMaterial = sceneID.renderMat;
            SceneRef sceneRef = sceneID.sceneRef;
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneRef.BuildIndex);
            yield return new WaitForSecondsRealtime(softLoadTime);
            while (!asyncOperation.isDone) yield return null;
        }
        transitionController.Clear();
        transitionController.OnTransitionEnd += () => {
            GM.TimeScaleManager.GlobalTimeScale = 1;
            GM.TimeScaleManager.AddTimeScaleShift(-1, 0.5f, timeScaleOutCurve);
            bubbleVFX.Stop();
        };
    }
}

[System.Serializable]
public class SceneIdentifier {
    public RoomTag roomTag;
    public SceneRef sceneRef;
    public Material renderMat;
}