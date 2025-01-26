using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemy : MonoBehaviour
{
    [SerializeField] private List<Transform> _targetPoints; // List of points to move between
    [SerializeField] private float _moveDuration = 1f; // Duration to move to each point
    [SerializeField] private float _waitDuration = 2f; // Time to wait at each point

    private int _currentTargetIndex = 0; // Current index in the target points list

    private Transform _t;

    void Awake() {
        _t = transform;
    }

    private void Start() {
        StartCoroutine(MoveToPointsLoop());
    }

    private IEnumerator MoveToPointsLoop() {
        while (true) {
            // Get the current target point
            Transform targetPoint = _targetPoints[_currentTargetIndex];

            // Move to the target point using DoMove
            _t.DOMove(targetPoint.position, _moveDuration);

            // Wait for the move duration plus the wait duration
            yield return new WaitForSeconds(_moveDuration + _waitDuration);

            // Update the target index to loop through the list
            _currentTargetIndex = (_currentTargetIndex + 1) % _targetPoints.Count;
        }
    }
}
