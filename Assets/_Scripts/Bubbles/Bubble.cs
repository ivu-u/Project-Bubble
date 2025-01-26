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

    public bool isPartOfRing {  get; private set; }
    private Player _p;
    private BubbleRingManager _ringManager;

    void Start() {
        _rb = GetComponent<Rigidbody>();
        _t = GetComponent<Transform>();
    }

    void FixedUpdate() {
        BubbleDrag();
    }

    void OnTriggerEnter(Collider other) {
        // player check
        if (other.TryGetComponent(out Player p)) {
            if (isPartOfRing) { }
            else { _contactTime = 0f; }
        }

        // ground check
        if (!(other.CompareTag("Ground"))) { return; }
        Vector3 collisionPoint = other.ClosestPointOnBounds(transform.position);
        // Calculate the direction from the collision point to the player
        Vector3 directionToPlayer = (transform.position - collisionPoint).normalized;

        // Boost the player in the direction of the collision
        _p.BoostPlayer(directionToPlayer, _boostForce);

        // Pop the bubble after boosting the player
        PopBubble();
    }

    void OnTriggerStay(Collider other) {
        if (isPartOfRing) { return; }

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
        if (isPartOfRing) {
            _ringManager.RemoveBubble(this.gameObject);
            return;
        }
        Destroy(this.gameObject);
    }

    // more jank
    public void IsPartOfRing(bool b, Player p, BubbleRingManager ring) {
        isPartOfRing = b;

        if(_p == null && isPartOfRing) { _p = p; } // get ref to player to store for later
        if (_ringManager == null && isPartOfRing) { _ringManager = ring;  }
    }
}
