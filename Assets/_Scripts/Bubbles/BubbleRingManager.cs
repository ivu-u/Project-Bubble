using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BubbleRingManager : MonoBehaviour
{
    [SerializeField] private GameObject _bubble;    // use this to add bubbles to the ring
    [SerializeField] private int _maxNumOfBubbles;
    [SerializeField] private float _ringRadius;
    [SerializeField] private int _currNumBubbles;
    [SerializeField] private Transform _p;   // bubble ring center

    // SF so i can see it in unity editor
    [SerializeField] private List<GameObject> _heldBubbles;

    void Start() {
        StartCoroutine(InitialBubbleSpawn());
    }

    private void AddBubble() {
        if (!(_heldBubbles.Count < _maxNumOfBubbles)) { return; }
        GameObject obj = Instantiate(_bubble, _p);   // jank for nows
        _heldBubbles.Add(obj);
        UpdateRing();
    }

    private void RemoveBubble() {

    }

    private void UpdateRing() {
        // update positions of bubbles in ring with Trig
        for (int i = 0; i < _heldBubbles.Count; ++i) {
            // angle for each object
            float angle = i * Mathf.PI * 2f / _heldBubbles.Count;

            // position in ring (XY plane)
            float x = _p.position.x + _ringRadius * Mathf.Cos(angle);
            float y = _p.position.y + _ringRadius * Mathf.Sin(angle);
            Vector3 newPos = new Vector3(x, y, _p.position.z);

            //_heldBubbles[i].transform
        }
    }

    private IEnumerator InitialBubbleSpawn() {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < _currNumBubbles; ++i) {
            AddBubble();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
