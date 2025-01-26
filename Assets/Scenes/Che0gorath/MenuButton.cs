using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MainMenuBType { Start, Credits, End }

public abstract class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
                                                  IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {

    private event System.Action OnScaleEnd;

    private const string PLAYBACK_PARAM = "PlaybackTime";

    [SerializeField] private Animator animator;
    [SerializeField] private RectOscillator oscillator;
    [SerializeField] private AnimationCurve hoverCurve, downCurve,
                                            upCurve, popCurve, exitCurve;
    [SerializeField] private float scaleTime, scaleSpeed, popTime, popAwaitTime;
    [SerializeField] private bool popImmediate = true;
    [SerializeField] private ParticleSystem parSystem;

    public bool IsPopped => !interactable;

    private Vector3 baseScale;
    private float scaleMult = 1;
    private bool pointerIsDown,
                 interactable = true;
    private int playbackParam;

    void Awake() {
        baseScale = transform.localScale;
        playbackParam = Animator.StringToHash(PLAYBACK_PARAM);
    }

    public void OnPointerEnter(PointerEventData _) {
        if (interactable && !pointerIsDown) {
            StopAllCoroutines();
            StartCoroutine(IDoScale(hoverCurve));
        }
        oscillator.Toggle(false);
    }

    public void OnPointerDown(PointerEventData _) {
        if (interactable) {
            StopAllCoroutines();
            StartCoroutine(IDoScale(downCurve));
            pointerIsDown = true;
        }
    }

    public void OnPointerClick(PointerEventData _) {
        if (interactable) PopButton();
    }

    public void PopButton() {
        interactable = false;
        StopAllCoroutines();
        if (popImmediate) {
            StartCoroutine(IDoScale(popCurve));
            StartCoroutine(IPlayPopAnimation());
        } else {
            StartCoroutine(IDoScale(popCurve, popAwaitTime));
            OnScaleEnd += () => StartCoroutine(IPlayPopAnimation());
        }
    }

    public void OnPointerUp(PointerEventData _) {
        if (interactable) {
            StopAllCoroutines();
            StartCoroutine(IDoScale(upCurve));
            pointerIsDown = false;
        }
    }

    public void OnPointerExit(PointerEventData _) {
        if (interactable && !pointerIsDown) {
            StopAllCoroutines();
            StartCoroutine(IDoScale(exitCurve));
        }
        oscillator.Toggle(true);
    }

    private IEnumerator IDoScale(AnimationCurve curve, float extraScaleTime = 0) {
        float lerpVal, target, timer = 0,
              scaleTime = this.scaleTime + extraScaleTime;
        while (timer < scaleTime) {
            timer = Mathf.MoveTowards(timer, scaleTime, Time.unscaledDeltaTime);
            lerpVal = timer / scaleTime;
            target = curve.Evaluate(lerpVal);
            scaleMult = Mathf.MoveTowards(scaleMult, target, Time.unscaledDeltaTime * scaleSpeed);
            transform.localScale = baseScale * scaleMult;
            yield return null;
        }

        OnScaleEnd?.Invoke();
        OnScaleEnd = null;
    }

    private IEnumerator IPlayPopAnimation() {
        float lerpVal, timer = 0;
        while (timer < popTime) {
            timer = Mathf.MoveTowards(timer, popTime, Time.unscaledDeltaTime);
            lerpVal = Mathf.Min(0.98f, timer / popTime);
            animator.SetFloat(playbackParam, lerpVal);
            yield return null;
        }

        DoPopBehavior();
    }

    protected abstract void DoPopBehavior();
}