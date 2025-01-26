using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class FirePointRot : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _moveRadius = 0.1f;

    private Transform _c;   // parent transform
    private Transform _t;
    private Vector3 _startingPos;

    void Start() {
        _t = transform;
        _c = GetComponentInParent<Transform>();
        _startingPos = _c.localPosition;
    }

    void Update() {
        MoveFirePoint();
    }

    private void MoveFirePoint() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0;

        // Convert the mouse position from world space to local space relative to the bubble ring's parent
        Vector3 localMousePosition = _c.InverseTransformPoint(mousePosition);

        // Calculate the direction from the object to the mouse
        Vector3 directionToMouse = localMousePosition.normalized;

        Vector3 targetPosition = _startingPos + directionToMouse * _moveRadius;
        //Debug.Log(targetPosition);

        // Smoothly move the object towards the clamped position
        _t.DOLocalMove(targetPosition, 0.1f);
    }
}
