using UnityEngine;
using System.Collections;

public class ColDetect : Photon.MonoBehaviour {

	public HealthManager health;
	
	// Use this for initialization
	void Start () {
		// Set reference to the HealthManager, as it handles our players health
		health = GameObject.FindGameObjectWithTag("Managers").GetComponent<HealthManager>();
	}

	// The players own client calls this function, but should tell the enemy to destory their balloon
	void OnCollisionEnter2D(Collision2D collision) {
		// Says i hit a Ballons GO (because if its rigidbody component), and not the actual balloon
//		Debug.Log(collision.collider.gameObject.name);

		if (collision.collider.gameObject.tag == "Balloon") {

			// Get the player we are colliding with
			PhotonPlayer attackedPlayer = collision.collider.gameObject.GetComponent<PhotonView>().owner;

			// Get the player who did the collision
			PhotonPlayer attackingPlayer = gameObject.GetComponent<PhotonView>().owner;

			// The ID of the gameObject we're hitting
			int id = collision.collider.gameObject.GetComponent<PhotonView>().viewID;

			// Tell him to destroy the gameObject with the view ID (No friendly fire)
			if (collision.collider.gameObject.GetComponent<PhotonView>().owner != gameObject.GetComponent<PhotonView>().owner) {
				photonView.RPC("DestroyBalloon", PhotonTargets.AllBuffered, id, attackedPlayer, attackingPlayer);
			}
		}
	}

	// All clients call this function, and then locally remove the hit balloon
	[RPC]
	public void DestroyBalloon (int id, PhotonPlayer attackedPlayer, PhotonPlayer attackingPlayer) {

		// Only look for the id, if it exists
		try {
			GameObject balloon = PhotonView.Find(id).gameObject;
			
			if (balloon != null) {
				
				// We only want one client to send the RunDeathCheck
				if (balloon.GetComponent<PhotonView>().isMine) {
					// Destroy balloon
//					Destroy(balloon);
					
					// Check the healthManager, to see if it was the last balloon
					health.GetComponent<PhotonView>().RPC("RunDeathCheck", attackedPlayer, attackingPlayer, id);
				} else {
					Destroy(balloon);
				}
			}
		} catch {}
	}
}
