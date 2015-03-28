using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RespawnManager : Photon.MonoBehaviour {

	// Player prefab that should be instantiated
	public Transform playerPrefab;

	// Array of all the spawnPoints in the map
	public GameObject[] spawnPoints;

	// A list of spawnPoints, where there is no players nearby
	public List<GameObject> validSpawnPoints = new List<GameObject>();

	// The chosen spawnPoint for the player who wants to spawn/respawn
	private GameObject spawnPoint;

	// Health manager script
	public HealthManager health;
	
	void Awake () {
		// Reference to health manager script
		health = GameObject.FindGameObjectWithTag("Managers").GetComponent<HealthManager>();
		
		// Array of ALL spawnPoints
		spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
	}

	// When a player joins the server (from GameManager), instaintiate him
	public void OnPlayerJoined (PhotonPlayer player) {

		// Only the MasterClient is allowed to find and set spawn points
		// So, when a player wants to spawn, request a spawn point from the masterclient
		photonView.RPC("RequestSpawnPoint", PhotonTargets.MasterClient, player);

	}

	// Our masterClient is told to run this function
	[RPC]
	public void RequestSpawnPoint (PhotonPlayer player) {
		
		// Create a list of VALID spawnPoints
		foreach (GameObject spawnPoint in spawnPoints) {
			if (spawnPoint.GetComponent<SpawnPoint>().nearbyPlayer == false) {
				validSpawnPoints.Add(spawnPoint);
//				Debug.Log("Valid point found: " + spawnPoint);
			} else {}
		}

		// Then a random spawnPoint in the list is chosen
		if (validSpawnPoints.Count > 0) {
			GameObject randomSpawnPoint = validSpawnPoints[Random.Range(0,validSpawnPoints.Count)];
			spawnPoint = randomSpawnPoint;

			// Spawn point found, now tell the player who requested to join, where he may spawn
			string spawnPointName = spawnPoint.name;
			photonView.RPC("InstantiatePlayer", player, spawnPointName);
		}
		else {
//			Debug.Log("There were no valid points");
			// If there are no valid spawn points, chose a random of all of them
			GameObject randomSpawnPoint = spawnPoints[Random.Range(0,spawnPoints.Length)];
			spawnPoint = randomSpawnPoint;

			// Spawn point found, now tell the player who requested to join, where he may spawn
			string spawnPointName = spawnPoint.name;
			photonView.RPC("InstantiatePlayer", player, spawnPointName);
		}

		// Empty the list of valid GOs
		validSpawnPoints.Clear();
//		for (int i = 0; i < validSpawnPoints.Count; i++) {
//			validSpawnPoints.RemoveAt(i);
//		}
	}

	// Function run by the player that should be instantiated
	[RPC]
	public void InstantiatePlayer (string spawnPointName) {

		GameObject thisSpawnPoint = GameObject.Find(spawnPointName);
		PhotonNetwork.Instantiate(this.playerPrefab.name, thisSpawnPoint.transform.position, Quaternion.identity, 0);

		// When our player is spawned, set his healthManager up
		health.InstantiateHealth();
	}
}
