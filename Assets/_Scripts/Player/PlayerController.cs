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
    public static event BubblePickUp OnBubblePickup;

    public delegate void ShootBubble(Vector3 dir, Transform firePos);
    public static event ShootBubble OnShootBubble;

    public delegate void MovingRing();
    public static event MovingRing OnMovingRing;

    public delegate void StopMovingRing();
    public static event StopMovingRing OnStopMovingRing;
    #endregion

    [SerializeField] private Transform _firePoint;
    private Transform _t;
    private Rigidbody _rb;
    private Collider _coll;
    private GameObject _ground;

    private float _horizontal;
    private bool _canThrow = true;

    void FixedUpdate() {
        _horizontal = _playerActionMap.Movement.Walk.ReadValue<Vector2>().x;

        Movement();
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Bubble b)) {
            //OnBubblePickup.Invoke(); // event to signify a bubble has been collided with
        }
    }

    protected void Movement() {

        _rb.velocity = new Vector3(_horizontal * _moveSpeed, _rb.velocity.y, 0);
    }

    protected void Jump(InputAction.CallbackContext context) {
        if (!IsGrounded()) { return; }

        float _currJumpPow = _jumpPower;

        if (_ground.TryGetComponent<Bubble>(out Bubble bubble)) {
            bubble.PopBubble();
            _currJumpPow *= 1.5f;
        }

        _rb.velocity = new Vector2(_rb.velocity.x, _currJumpPow);
    }

    public bool IsGrounded() {
        RaycastHit hit;
        float rayLength = 1.1f; // Adjust based on your character's size
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength)) {
            _ground = hit.collider.gameObject;
            return true;
        }
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
