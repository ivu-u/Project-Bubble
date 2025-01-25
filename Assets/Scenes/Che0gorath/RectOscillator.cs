using UnityEngine;

public class RectOscillator : MonoBehaviour {

    [SerializeField] private Vector2 amplitude, phaseSpeed, moveSpeed;
    private RectTransform rect;
    private Vector2 anchor, offset;
    private float toggleMult = 1;

    void Awake() {
        rect = transform as RectTransform;
        anchor = rect.anchoredPosition;
        offset = new (Random.Range(-10, 10),
                      Random.Range(-10, 10));
        rect.anchoredPosition = new Vector2(anchor.x + Mathf.Sin(Time.time * phaseSpeed.x + offset.x) * amplitude.x,
                                            anchor.y + Mathf.Sin(Time.time * phaseSpeed.y + offset.y) * amplitude.y);
    }

    void Update() {
        rect.anchoredPosition = new Vector2(Mathf.MoveTowards(rect.anchoredPosition.x,
                                                  anchor.x + Mathf.Sin(Time.time * phaseSpeed.x + offset.x) * amplitude.x,
                                                  Time.deltaTime * moveSpeed.x * toggleMult),
                                            Mathf.MoveTowards(rect.anchoredPosition.y,
                                                  anchor.y + Mathf.Sin(Time.time * phaseSpeed.y + offset.y) * amplitude.y,
                                                  Time.deltaTime * moveSpeed.y * toggleMult));
    }

    public void Toggle(bool on) {
        toggleMult = on ? 1 : 0;
    }
}