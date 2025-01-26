using UnityEngine;

public class AddPlayerBubble : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.TryGetComponent(out Player player)) {
			player.AddBubble();
		}
	}
}