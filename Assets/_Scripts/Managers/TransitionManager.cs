using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum RoomTag { L1, L2, L3 }

public class TransitionManager : MonoBehaviour {

    [SerializeField] private TransitionPH transitionController;
    [SerializeField] private AnimationCurve timeScaleInCurve, timeScaleOutCurve;
    [SerializeField] private float softLoadTime;
    [SerializeField] private SceneIdentifier[] sceneIDs;

    private readonly Dictionary<RoomTag, SceneRef> sceneMap = new();

    void Awake() {
        foreach (SceneIdentifier sceneID in sceneIDs) {
            sceneMap[sceneID.roomTag] = sceneID.sceneRef;
        }
    }

    public void LoadLevel(RoomTag roomTag) {
        TimeScaleCore tsCore = GM.TimeScaleManager.AddTimeScaleShift(-1, 0.5f, timeScaleInCurve);
        tsCore.OnCoreDeath += () => GM.TimeScaleManager.GlobalTimeScale = 0;
        transitionController.OnTransitionEnd += () => StartCoroutine(ILoadLevel(roomTag));
        transitionController.Fade();
    }

    private IEnumerator ILoadLevel(RoomTag roomTag) {
        if (sceneMap.TryGetValue(roomTag, out SceneRef sceneRef)) {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneRef.BuildIndex);
            yield return new WaitForSeconds(softLoadTime);
            while (!asyncOperation.isDone) yield return null;
        }
        transitionController.Clear();
        transitionController.OnTransitionEnd += () => {
            GM.TimeScaleManager.GlobalTimeScale = 1;
            GM.TimeScaleManager.AddTimeScaleShift(-1, 0.5f, timeScaleOutCurve);
        };
    }
}

[System.Serializable]
public class SceneIdentifier {
    public RoomTag roomTag;
    public SceneRef sceneRef;
}