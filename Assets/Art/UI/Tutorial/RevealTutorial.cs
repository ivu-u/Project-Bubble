using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealTutorial : MonoBehaviour {

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeSpeed;

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player _)) {
            StopAllCoroutines();
            StartCoroutine(IDoFade(1));
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Player _)) {
            StopAllCoroutines();
            StartCoroutine(IDoFade(0));
        }
    }

    private IEnumerator IDoFade(float target) {
        float delta;
        while (Mathf.Abs(canvasGroup.alpha - target) > Mathf.Epsilon) {
            delta = Mathf.Min(Time.unscaledDeltaTime * fadeSpeed, 0.1f);
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, delta);
            yield return null;
        }
    }
}