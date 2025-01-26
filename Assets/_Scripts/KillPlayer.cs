using UnityEngine;

public class KillPlayer : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.TryGetComponent(out Player player)) {
			player.Kill();
		}
	}
}