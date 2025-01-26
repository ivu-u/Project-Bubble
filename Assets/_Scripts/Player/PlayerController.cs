using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

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
    #endregion

    [SerializeField] private float groundCastTolerance;
    [SerializeField] private Transform _firePoint;
    private Transform _t;
    private Rigidbody _rb;
    private Collider _coll;
    private GameObject _ground;

    private Vector2 _currDirection; // use this if you need player direction
    private bool _canThrow = true;

    [SerializeField] private AnimationCurve _movementCurve;
    private float _accelerationTime = 0f;

    void FixedUpdate() {
        _currDirection = _playerActionMap.Movement.Walk.ReadValue<Vector2>();
        Movement();
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Bubble _)) {
            //OnBubblePickup.Invoke(); // event to signify a bubble has been collided with
        }
    }

    public void BoostPlayer(Vector3 dir, float force) {
        _rb.AddForce(dir * force, ForceMode.Impulse);
    }

    protected void Movement() {
        float horizontal = _currDirection.x;

        // Ground Movement
        IsGrounded();
        if (_ground != null && _ground.tag == "GroundSAFE") {
            if (horizontal != 0) { _accelerationTime += Time.deltaTime; }
            else if (_accelerationTime > 0) { _accelerationTime -= Time.deltaTime * 2.5f; }
            //else { _accelerationTime = 0;}

            float curveValue = _movementCurve.Evaluate(_accelerationTime);
            float finalMoveSpeed = _moveSpeed * curveValue;

            _rb.velocity = new Vector3(horizontal * finalMoveSpeed, _rb.velocity.y, 0);
            return;
        }

        _rb.velocity = new Vector3(horizontal * _moveSpeed, _rb.velocity.y, 0);
    }

    protected void Jump(InputAction.CallbackContext context) {

        if (!IsGrounded()) { return; }

        float _currJumpPow = _jumpPower;

        if (_ground.TryGetComponent(out Bubble bubble)
                && !bubble.isPartOfRing) {
            bubble.PopBubble();
            _currJumpPow *= 1.5f;
        }

        _rb.velocity = new Vector2(_rb.velocity.x, _currJumpPow);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Vector3 limit = transform.position + Vector3.down * groundCastTolerance;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCastTolerance);
        Gizmos.DrawSphere(limit, 0.1f);
        Gizmos.color = Color.white;
    }
    #endif

    public bool IsGrounded() {
        RaycastHit hit;
        float rayLength = groundCastTolerance; // Adjust based on your character's size

        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength)) {
            _ground = hit.collider.gameObject;
            return true;
        }
        _ground = null;
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
    private IEnumerator IThrowTimer() {
        _canThrow = false;
        yield return new WaitForSeconds(_throwDelay);
        _canThrow = true;
    }
}
