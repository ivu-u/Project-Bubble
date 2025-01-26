using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CamLookPoint : MonoBehaviour {
    [SerializeField] private Transform _targetObject; // Reference to the other GameObject
    [SerializeField] private float _maxDistance = 10f;

    private Transform _t;

    void Awake() {
        _t = transform;
    }

    void Update() {
        CalculateDistanceAndMidpoint();
    }

    private void CalculateDistanceAndMidpoint() {
        // Get the mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure it's on the same plane as the target object (if in 2D)

        // Get the target object's position
        Vector3 targetPosition = _targetObject.position;

        // Calculate the distance
        float distance = Vector3.Distance(mousePosition, targetPosition);

        // Calculate the midpoint
        Vector3 midpoint = (mousePosition + targetPosition) / 2f;

        // Restrict the camera's position to stay within maxDistance from the target
        if (distance > _maxDistance) {
            // Calculate the direction vector from the target to the mouse
            Vector3 direction = (mousePosition - targetPosition).normalized;

            // Clamp the mouse position to be within maxDistance from the target
            mousePosition = targetPosition + direction * _maxDistance;

            // Recalculate the midpoint after clamping
            midpoint = (mousePosition + targetPosition) / 2f;
        }

        _t.DOMove(midpoint, 0.15f);
    }
}
