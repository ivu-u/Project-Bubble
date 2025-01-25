using UnityEngine;

public class VectorOscillator : MonoBehaviour {

    [SerializeField] private float amplitude, speed;
    [SerializeField] private Vector3 up = Vector3.up;
    private Vector3 anchor;

    void Awake() {
        anchor = transform.localPosition;
    }

    void Update() {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition,
            anchor + Mathf.Sin(Time.time * speed) * amplitude * up,
            Time.deltaTime * speed);
    }
}