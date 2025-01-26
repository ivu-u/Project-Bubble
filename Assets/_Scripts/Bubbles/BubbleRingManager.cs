using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BubbleRingManager : MonoBehaviour
{
    [SerializeField] private Player playerRef;
    [SerializeField] private GameObject _bubble;    // use this to add bubbles to the ring
    [SerializeField] private int _maxNumOfBubbles;
    [SerializeField] private float _ringRadius;
    [SerializeField] private int _currNumBubbles;
    [SerializeField] private Transform _c;   // bubble ring center
    [SerializeField] private float _rotationSpeed = 10f; // Rotation speed in degrees per second
    [SerializeField] private float _bubbleScale = 1.5f;
    private float _currentRotationAngle = 0f;

    // SF so i can see it in unity editor
    [SerializeField] private List<GameObject> _heldBubbles;

    void Awake() {
        // subscribe to events
        playerRef.OnShootBubble += ShootBubble;
        playerRef.OnAddBubble += AddBubble;
        playerRef.OnSpawn += PlayerRef_OnSpawn;
        playerRef.OnDeath += PlayerRef_OnDeath;

        InitRing();
    }

    void Update() {
        RotateRing();
    }

    private void ShootBubble(Vector3 dir, Transform firePos) {
        if (_heldBubbles.Count <= 0) { return; }
        RemoveBubble(_heldBubbles[_heldBubbles.Count - 1]);

        GameObject obj = Instantiate(_bubble, firePos.position, Quaternion.identity);
        obj.GetComponent<Bubble>().IsPartOfRing(false, playerRef, this);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = dir * playerRef.ThrowSpeed;

        obj.transform.DOScale(Vector3.one * _bubbleScale, 1f).SetEase(Ease.InOutSine);
    }

    private void AddBubble() {
        if (!(_heldBubbles.Count < _maxNumOfBubbles)) { return; }
        GameObject obj = Instantiate(_bubble, _c);   // jank for nows
        obj.layer = 0;
        Bubble bubble = obj.GetComponent<Bubble>();
        bubble.IsPartOfRing(true, playerRef, this);
        obj.GetComponent<Rigidbody>().isKinematic = true;
        _heldBubbles.Add(obj);
        UpdateRing();
    }

    public void RemoveBubble(GameObject bubble) {
        //GameObject obj = _heldBubbles[_heldBubbles.Count - 1];
        _heldBubbles.Remove(bubble);
        Destroy(bubble);
        UpdateRing();
    }

    private void RotateRing() {
        // Increment the rotation angle based on time and speed
        _currentRotationAngle += _rotationSpeed * Time.deltaTime;

        // Keep the angle between 0 and 360 to avoid overflow
        _currentRotationAngle %= 360;

        // Update the positions of the bubbles in the ring
        UpdateRing();
    }

    private void UpdateRing() {
        // update positions of bubbles in ring with Trig
        for (int i = 0; i < _heldBubbles.Count; ++i) {
            float baseAngle = i * Mathf.PI * 2f / _heldBubbles.Count;

            // based on rotation
            float angle = baseAngle + Mathf.Deg2Rad * _currentRotationAngle;

            // position in ring (XY plane)
            float x = _c.localPosition.x + _ringRadius * Mathf.Cos(angle);
            float y = _c.localPosition.y + _ringRadius * Mathf.Sin(angle);
            Vector3 newPos = new Vector3(x, y, _c.localPosition.z);

            _heldBubbles[i].transform.DOLocalMove(newPos, 0.2f);
        }
    }

    public void InitRing() => StartCoroutine(IInitialBubbleSpawn());

    private IEnumerator IInitialBubbleSpawn() {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < _currNumBubbles; ++i) {
            AddBubble();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void PlayerRef_OnSpawn() => InitRing();
    private void PlayerRef_OnDeath() => PopRing();

    private void PopRing() {
        while (_heldBubbles.Count > 0) {
            Bubble bubble = _heldBubbles[0].GetComponent<Bubble>();
            bubble.PopBubble();
        }
    }
}
