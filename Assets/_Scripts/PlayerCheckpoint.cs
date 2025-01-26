using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour {

	[SerializeField] private Transform checkpointTransform;

	void OnTriggerEnter(Collider other) {
		if (other.TryGetComponent(out Player player)) {
			player.SetCheckpoint(checkpointTransform);
		}
	}
}