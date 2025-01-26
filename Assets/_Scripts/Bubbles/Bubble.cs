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
    [SerializeField] private float _boostForce = 5f;

    private Rigidbody _rb;
    private Transform _t;
    private float _contactTime = 0f; // Tracks how long the player has been in contact

    private bool _isPartOfRing = true;
    private Player _p;

    void Start() {
        _rb = GetComponent<Rigidbody>();
        _t = GetComponent<Transform>();
    }

    void FixedUpdate() {
        BubbleDrag();
    }

    void OnCollisionEnter(Collision collision) {
        if (!(collision.collider.CompareTag("Ground"))) { return; }

        Vector3 collisionPoint = collision.contacts[0].point; // Get the point of contact
        Vector3 directionToPlayer = (_t.position - collisionPoint).normalized; // Direction of boost
        _p.BoostPlayer(directionToPlayer, _boostForce);
        PopBubble();
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player p)) {
            if (_isPartOfRing) { return; }
            _contactTime = 0f;
        }
    }

    void OnTriggerStay(Collider other) {
        if (_isPartOfRing) { return; }

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
    public void IsPartOfRing(bool b, Player p) {
        _isPartOfRing = b;

        if(_p == null && _isPartOfRing) { _p = p; } // get ref to player to store for later
    }
}
