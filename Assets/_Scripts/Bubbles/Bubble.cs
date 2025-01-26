using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Bubble Projectile
/// </summary>
public class Bubble : MonoBehaviour
{
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private float _popDuration;
    [SerializeField] private float _drag = 0.98f; // Factor to slow down the bubble
    [SerializeField] private float _minSpeed = 0.5f; // Minimum speed before stopping the bubble
    [SerializeField] private float _jumpWindow = 0.5f;
    [SerializeField] private float _boostForce = 5f;

    private MaterialPropertyBlock _mpb;
    private Player _p;
    private Rigidbody _rb;
    private float _contactTime = 0f; // Tracks how long the player has been in contact

    public bool isPartOfRing { get; private set; }
    private BubbleRingManager _ringManager;

    void Awake() {
        _rb = GetComponent<Rigidbody>();
        _mpb = new();
    }

    void FixedUpdate() {
        BubbleDrag();
    }

    void OnTriggerEnter(Collider other) {
        // ground check
        if (!other.CompareTag("Ground")) { return; }

        // player check
        if (other.TryGetComponent(out Player p)) {
            if (isPartOfRing) { }
            else { _contactTime = 0f; }
        }

        if (_p == null) { return; }

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
        if (_rb.isKinematic) return;
        _rb.velocity *= _drag;

        if (_rb.velocity.magnitude < _minSpeed) {
            _rb.velocity = Vector3.zero;
            _rb.isKinematic = true;
        }
    }

    public void PopBubble() {
        if (isPartOfRing) {
            _ringManager.DiscardBubbleRef(gameObject);
        }
        enabled = false;
        StartCoroutine(IPopAndDestroy());
    }

    // more jank
    public void IsPartOfRing(bool b, Player p, BubbleRingManager ring) {
        isPartOfRing = b;

        if (_p == null && isPartOfRing) { _p = p; } // get ref to player to store for later
        /// Carlos Note: reverted the caching to fix some null refs;
        if (_ringManager == null && isPartOfRing) { _ringManager = ring;  }
    }

    IEnumerator IPopAndDestroy() {
        float t = 0f;
        while (t < _popDuration) {
            t += Time.deltaTime;

            float alpha = t / _popDuration;

            // Override local scale
            ApplyDissolve(alpha);

            yield return null;
        }

        Destroy(gameObject);
    }

    void ApplyDissolve(float t) {
        Debug.Log(t);
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_Dissolve", t);
        _renderer.SetPropertyBlock(_mpb);
    }

    //public void InitBubble() {

    //}
}
