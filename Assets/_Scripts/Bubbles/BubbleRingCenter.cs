using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

/// <summary>
/// Allows the center of the bubble ring to be slighly controlled by mouse pos
/// </summary>
public class BubbleRingCenter : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _moveRadius = 0.1f;
    private Transform _c;   // parent transform
    private Transform _t;
    private Vector3 _targetPos;
    private Vector3 _startingPos;
    private bool _canMoveRing;

    void Start() {
        _t = transform;
        _c = GetComponentInParent<Transform>();
        _startingPos = _t.localPosition;
        Player.OnMovingRing += MoveRing;
        Player.OnStopMovingRing += StopMoveRing;
    } 

    void Update() {
        MoveBubbleRing();
    }

    private void MoveBubbleRing() {
        if (!_canMoveRing) { return; }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0;

        // Convert the mouse position from world space to local space relative to the bubble ring's parent
        Vector3 localMousePosition = _c.InverseTransformPoint(mousePosition);

        // Calculate the direction from the object to the mouse
        Vector3 directionToMouse = localMousePosition.normalized;

        // Calculate the potential new position
        Vector3 targetPosition = _t.localPosition + directionToMouse * _moveSpeed * Time.deltaTime;

        // Clamp the position within the movement radius around the starting position
        Vector3 offsetFromStart = targetPosition - _startingPos;

        if (offsetFromStart.magnitude > _moveRadius) {
            targetPosition = _startingPos + offsetFromStart.normalized * _moveRadius;
        }

        // Smoothly move the object towards the clamped position
        _t.DOLocalMove(targetPosition, 0.1f);
    }

    private void MoveRing() {  _canMoveRing = true; }

    private void StopMoveRing() { 
        _canMoveRing = false;
        _t.DOLocalMove(_startingPos, 0.3f);
    }
}
