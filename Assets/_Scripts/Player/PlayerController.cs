using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player Movement Behavior
/// </summary>
public partial class Player : MonoBehaviour
{
    #region Events
    public delegate void BubblePickUp();
    public event BubblePickUp OnBubblePickup;

    public delegate void ShootBubble(Vector3 dir, Transform firePos);
    public event ShootBubble OnShootBubble;

    public delegate void MovingRing();
    public event MovingRing OnMovingRing;

    public delegate void StopMovingRing();
    public event StopMovingRing OnStopMovingRing;

    public event System.Action OnAddBubble;
    public event System.Action OnJump;
    public event System.Action OnSpawn;
    public event System.Action OnDeath;
    #endregion

    [SerializeField] private float deathRespawnTime;
    [SerializeField] private float groundCastTolerance;
    [SerializeField] private Transform _firePoint;

    // fuck fuck fuck i copy paste it crunching
    [SerializeField] private float minForce = 5f;
    [SerializeField] private float maxForce = 20f;

    public float WalkSpeed => _rb.velocity.x;
    public float VerticalSpeed => _rb.velocity.y;

    private Transform _t;
    private Rigidbody _rb;
    private Collider _coll;
    private GameObject _ground;

    private Transform checkpoint;

    private Vector2 _currDirection; // use this if you need player direction
    private bool _canThrow = true;

    [SerializeField] private AnimationCurve _movementCurve;
    private float _accelerationTime = 0f;

    // input buffering
    [SerializeField] private float inputBufferTime = 0.2f; // Duration of the input buffer
    private float jumpBufferTimer = 0f; // Tracks time remaining in the buffer
    private bool isJumpBuffered = false; // Tracks if a jump input is buffered
    [SerializeField] private float capsuleRadius = 1f;

    void Awake() {
        _t = transform;
    }

    void FixedUpdate() {
        _currDirection = _playerActionMap.Movement.Walk.ReadValue<Vector2>();
        Movement();
        ProcessBufferedInput();
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Bubble _)) {
            //OnBubblePickup.Invoke(); // event to signify a bubble has been collided with
        }
    }

    public void BoostPlayer(Vector3 dir, float force) {
        Vector3 newforce = dir.normalized * force;
        _rb.velocity = new Vector2(Mathf.Clamp(_rb.velocity.x + newforce.x, minForce, maxForce),
                                    Mathf.Clamp(_rb.velocity.y + newforce.y, minForce, maxForce));
        //_rb.AddForce(dir * force, ForceMode.Impulse);

        //Debug.Log(_rb.velocity);
    }

    protected void Movement() {
        float horizontal = _currDirection.x;

        // Ground Movement
        //IsGrounded();
        if (IsGrounded()) {
            if (horizontal != 0) { _accelerationTime += Time.deltaTime; }
            else if (_accelerationTime > 0) { _accelerationTime -= Time.deltaTime * 2.5f; }
            //else { _accelerationTime = 0;}

            float curveValue = _movementCurve.Evaluate(_accelerationTime);
            float finalMoveSpeed = _moveSpeed * curveValue;

            _rb.velocity = new Vector3(horizontal * finalMoveSpeed, _rb.velocity.y, 0);
            return;
        }

        _rb.velocity = new Vector3(horizontal * _airMoveSpeed, _rb.velocity.y, 0);
    }

    protected void Jump(InputAction.CallbackContext context) {
        // Buffer the jump input if conditions aren't valid yet
        if (!IsGrounded()) {
            isJumpBuffered = true;
            jumpBufferTimer = inputBufferTime;
            return;
        }

        PerformJump();
    }

    private void PerformJump() {
        float _currJumpPow = _jumpPower;

        if (_ground.TryGetComponent(out Bubble bubble) && !bubble.isPartOfRing) {
            bubble.PopBubble();
            _currJumpPow *= 1.5f;
        }

        _rb.velocity = new Vector2(_rb.velocity.x, _currJumpPow);
        OnJump?.Invoke();

        // Reset buffered jump input
        isJumpBuffered = false;
        jumpBufferTimer = 0f;
    }

    private void ProcessBufferedInput() {
        // Reduce the timer if a jump is buffered
        if (isJumpBuffered) {
            jumpBufferTimer -= Time.fixedDeltaTime;

            // If grounded and buffer timer is active, process the jump
            if (jumpBufferTimer > 0 && IsGrounded()) {
                PerformJump();
            }
            else if (jumpBufferTimer <= 0) {
                // Clear the buffer if the time expires
                isJumpBuffered = false;
            }
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Vector3 limit = transform.position + Vector3.down * groundCastTolerance;
        Gizmos.DrawLine(transform.position, limit);
        Gizmos.DrawSphere(limit, capsuleRadius);
        Gizmos.color = Color.white;
    }
    #endif

    public bool IsGrounded() {
        // Perform the capsule cast (same as a "thicker" ray)
        RaycastHit hit;
        if (Physics.SphereCast(_t.position, capsuleRadius, Vector3.down, out hit, groundCastTolerance, 1 << 4 | 1 << 7)) {
            _ground = hit.collider.gameObject;
            Debug.Log("Ground detected: " + hit.collider.name);  // Debugging line to show the object hit
            return true;
        }

        // No ground detected, reset _ground
        _ground = null;
        Debug.Log("No ground detected");
        return false;
    }

    protected void MoveRing(InputAction.CallbackContext context) {
        if (context.performed) { OnMovingRing.Invoke(); }
        else if (context.canceled) { OnStopMovingRing.Invoke(); }
    }

    protected void Shoot(InputAction.CallbackContext context) {
        if (!_canThrow) { return; }

        // calc bubble shoot direction
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0;
        Vector3 shootDirection = (mousePosition - _firePoint.position).normalized;

        OnShootBubble.Invoke(shootDirection, _firePoint);
    }

    public void AddBubble() => OnAddBubble?.Invoke();

    private IEnumerator IThrowTimer() {
        _canThrow = false;
        yield return new WaitForSeconds(_throwDelay);
        _canThrow = true;
    }

    public void Kill() {
        enabled = false;
        OnDeath?.Invoke();
        StartCoroutine(IRespawn());
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.K)) {
            Kill();
        }
    }

    public void SetCheckpoint(Transform checkpoint) {
        this.checkpoint = checkpoint;
    }

    private IEnumerator IRespawn() {
        yield return new WaitForSecondsRealtime(deathRespawnTime);
        if (checkpoint != null) {
            enabled = true;
            transform.position = checkpoint.position;
            OnSpawn?.Invoke();
        } 
    }
}
