using UnityEngine;

public class KillPlayer : MonoBehaviour {

    void OnCollisionEnter(Collision info) {
		if (info.collider.TryGetComponent(out Player player)) {
			player.Kill();
		}
	}

    void OnTriggerEnter(Collider other) {
		if (other.TryGetComponent(out Player player)) {
			player.Kill();
		}
	}
}