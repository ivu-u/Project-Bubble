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
    public static event BubblePickUp OnBubblePickup;

    public delegate void ShootBubble();
    public static event ShootBubble OnShootBubble;
    #endregion

    private Transform _t;
    private Rigidbody _rb;
    private Collider _coll;

    private float _horizontal;

    void FixedUpdate() {
        _horizontal = _playerActionMap.Movement.Walk.ReadValue<Vector2>().x;

        Movement();
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Bubble b)) {
            OnBubblePickup(); // event to signify a bubble has been collided with
        }
    }

    protected void Movement() {

        _rb.velocity = new Vector3(_horizontal * _moveSpeed, _rb.velocity.y, 0);
    }

    protected void Jump(InputAction.CallbackContext context) {
        _rb.velocity = new Vector2(_rb.velocity.x, _jumpPower);
    }

    protected void Attack(InputAction.CallbackContext context) {

    }

    protected void Shoot(InputAction.CallbackContext context) {
        OnShootBubble();
    }
}
