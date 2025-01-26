using UnityEngine;

public class RoomExit : MonoBehaviour {

	[SerializeField] private RoomTag roomTag;

	void OnTriggerEnter(Collider other) {
		if (other.TryGetComponent(out Player _)) {
			GM.TransitionManager.LoadLevel(roomTag);
		}
	}
}