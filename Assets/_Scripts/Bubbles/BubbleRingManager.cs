using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BubbleRingManager : MonoBehaviour
{
    [SerializeField] private GameObject _bubble;    // use this to add bubbles to the ring
    [SerializeField] private int _maxNumOfBubbles;
    [SerializeField] private float _ringRadius;
    [SerializeField] private int _currNumBubbles;
    [SerializeField] private Transform _c;   // bubble ring center

    // might not need this?
    private Player _p;

    // SF so i can see it in unity editor
    [SerializeField] private List<GameObject> _heldBubbles;

    void Start() {
        _p = GetComponent<Player>();

        // subscribe to events
        Player.OnShootBubble += ShootBubble;

        StartCoroutine(IInitialBubbleSpawn());
    }

    private void ShootBubble(Vector3 dir, Transform firePos) {
        if (_heldBubbles.Count <= 0) { return; }
        RemoveBubble();

        GameObject obj = Instantiate(_bubble, firePos.position, Quaternion.identity);
        obj.GetComponent<Bubble>().IgnorePlayerColls(false);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = dir * _p.ThrowSpeed;
    }

    private void AddBubble() {
        if (!(_heldBubbles.Count < _maxNumOfBubbles)) { return; }
        GameObject obj = Instantiate(_bubble, _c);   // jank for nows
        obj.GetComponent<Bubble>().IgnorePlayerColls(true);
        obj.GetComponent<Rigidbody>().isKinematic = true;
        _heldBubbles.Add(obj);
        UpdateRing();
    }

    private void RemoveBubble() {
        GameObject obj = _heldBubbles[_heldBubbles.Count - 1];
        _heldBubbles.RemoveAt(_heldBubbles.Count - 1);
        Destroy(obj);
        UpdateRing();
    }

    private void UpdateRing() {
        // update positions of bubbles in ring with Trig
        for (int i = 0; i < _heldBubbles.Count; ++i) {
            // angle for each object
            float angle = i * Mathf.PI * 2f / _heldBubbles.Count;

            // position in ring (XY plane)
            float x = _c.localPosition.x + _ringRadius * Mathf.Cos(angle);
            float y = _c.localPosition.y + _ringRadius * Mathf.Sin(angle);
            Vector3 newPos = new Vector3(x, y, _c.localPosition.z);

            _heldBubbles[i].transform.DOLocalMove(newPos, 0.2f);
        }
    }

    private IEnumerator IInitialBubbleSpawn() {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < _currNumBubbles; ++i) {
            AddBubble();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
