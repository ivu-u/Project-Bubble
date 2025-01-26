using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Bubble Projectile
/// </summary>
public class Bubble : MonoBehaviour
{
    [SerializeField] private float _drag = 0.98f; // Factor to slow down the bubble
    [SerializeField] private float _minSpeed = 0.5f; // Minimum speed before stopping the bubble
    [SerializeField] private float _jumpWindow = 0.5f;
    private Rigidbody _rb;
    private float _contactTime = 0f; // Tracks how long the player has been in contact

    private bool _ignorePlayerColls = true;

    void Start() {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        BubbleDrag();
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player p)) {
            if (_ignorePlayerColls) { return; }
            _contactTime = 0f;
        }
    }

    void OnTriggerStay(Collider other) {
        if (_ignorePlayerColls) { return; }

        _contactTime += Time.deltaTime;

        if (_contactTime > _jumpWindow) {
            PopBubble();
        }
    }

    // so it slows down after being shot and stops
    private void BubbleDrag() {
        _rb.velocity *= _drag;

        if (_rb.velocity.magnitude < _minSpeed) {
            _rb.velocity = Vector3.zero;
            _rb.isKinematic = true;
        }
    }

    public void PopBubble() {
        Destroy(this.gameObject);
    }

    // more jank
    public void IgnorePlayerColls(bool b) {
        _ignorePlayerColls = b;
    }
}
