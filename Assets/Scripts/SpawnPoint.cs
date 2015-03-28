using UnityEngine;
using System.Collections;

public class SpawnPoint : Photon.MonoBehaviour {

	public bool nearbyPlayer;
	private bool checkForPlayer = true;

	// If trigger has not been called for 5 sec, set nearbyPlayer to false
	void OnTriggerStay2D(Collider2D collision) {
		if (collision.gameObject.tag == "Viking") {
			if (checkForPlayer) {

				nearbyPlayer = true;
				StartCoroutine(Timer(5.0f));
				checkForPlayer = false;
			}
		}
	}

	IEnumerator Timer(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		checkForPlayer = true;
		nearbyPlayer = false;
	}	
}