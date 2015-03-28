using UnityEngine;
using System.Collections;

public class HealthManager : Photon.MonoBehaviour {
	
	public RespawnManager respawn;
	public HighscoreManager highscore;

	public GameObject[] players;

	public GameObject playerPrefab;
	public GameObject balloons;
	public PhotonPlayer player;
	
	public bool deathCheck;

	// Use this for initialization
	public void Start () {
		respawn = GameObject.FindGameObjectWithTag("Managers").GetComponent<RespawnManager>();
		highscore = GameObject.FindGameObjectWithTag("Managers").GetComponent<HighscoreManager>();
	}

	// Function called by respawnManager, when a player has been spawned
	public void InstantiateHealth () {
		players = GameObject.FindGameObjectsWithTag("Player");

		foreach (GameObject player in players) {
			if (player.GetComponent<PhotonView>().isMine) {
				balloons = player.transform.GetChild(0).transform.gameObject;
				playerPrefab = player.transform.gameObject;
			}
		}
	}
	
	// The player that was hit, is told to to run this function
	[RPC] void RunDeathCheck (PhotonPlayer attackingPlayer, int id) {

		// Remove the balloon which was hit
		GameObject balloon = PhotonView.Find(id).gameObject;
		Destroy(balloon);

		player = PhotonNetwork.player;
		players = GameObject.FindGameObjectsWithTag("Player");

		foreach (GameObject _player in players) {
			if (_player.GetComponent<PhotonView>().isMine) {
				balloons = _player.transform.GetChild(0).transform.gameObject;
				playerPrefab = _player.transform.gameObject;
			}
		}
		
		if (balloons != null) {
//			Debug.Log("Balloons left: " + balloons.transform.childCount);
			if(balloons.transform.childCount <= 1) {

				// Give the attacking player a score point
				highscore.UpdateScore(attackingPlayer);

				// Remove all the players RPC calls
				PhotonNetwork.RemoveRPCs(player);

				// Destroy the players prefab
				PhotonNetwork.Destroy(playerPrefab);

				// Request a new instantiation and spawnPoint
				respawn.OnPlayerJoined(player);
			}
		}
	}	
}
