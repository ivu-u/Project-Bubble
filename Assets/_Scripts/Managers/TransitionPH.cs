using System.Collections;
using UnityEngine;

public class TransitionPH : MonoBehaviour {

    public event System.Action OnTransitionEnd;

    [SerializeField] private float duration;
    [SerializeField] private CanvasGroup canvasGroup;

    void Awake() {
        canvasGroup.alpha = 1;
        Clear();
    }

    public void Fade() {
        StopAllCoroutines();
        StartCoroutine(IFade(1));
    }

    public void Clear() {
        StopAllCoroutines();
        StartCoroutine(IFade(0));
    }

    private IEnumerator IFade(float target) {
        float delta, lerpVal, timer = 0;
        while (timer < duration) {
            delta = Mathf.Min(Time.unscaledDeltaTime, 0.1f);
            timer = Mathf.MoveTowards(timer, duration, delta);
            lerpVal = timer / duration;
            canvasGroup.alpha = Mathf.Lerp(1 - target, target, lerpVal);
            yield return null;
        }

        OnTransitionEnd?.Invoke();
        /// IMPORTANT! Passing asynchronous method. Clear invocation list after call!
        OnTransitionEnd = null;
        canvasGroup.interactable = target != 0;
        canvasGroup.blocksRaycasts = target != 0;
    }
}