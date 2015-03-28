using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// Added for sorting lists
using System.Linq;

public class HighscoreManager : Photon.MonoBehaviour {

	public List<HighscoreEntry> entries = new List<HighscoreEntry>();
	public GameObject entryPrefab;
	public GameObject highscorePanel;

	// Function used to add players to the list of players (From gameManager)
	// Called locally whenever a player joins the server
	public void AddPlayer (PhotonPlayer player) {

		// When the master creates a room, add him to the list
		if (PhotonNetwork.isMasterClient) {
			photonView.RPC("RequestAddNew", PhotonTargets.MasterClient, player);
		} else {
			// If a player joins, who is not the master, ask the master for a list of existing players
			photonView.RPC("ExistingPlayers", PhotonTargets.MasterClient, player);
		}
	}

	// MasterClient runs this function, to tell the new player who is already in the game
	[RPC]
	public void ExistingPlayers (PhotonPlayer player) {

		for(int i = 0; i < entries.Count; i++) {
			photonView.RPC("RequestAddExisting", player, entries[i].playerName, entries[i].playerKills);
		}

		photonView.RPC("RequestAddNew", PhotonTargets.All, player);
	}
	
	// Master Client tells all on the server, to add the newly joined player
	[RPC]
	public void RequestAddNew (PhotonPlayer player) {
		string playerName = player.name;
		int playerKills = 0;
		entries.Add(new HighscoreEntry(playerName, playerKills));

		// Calculate our position first, so our list is empty (meaning the first postion is the one at the very top)
		Vector3 pos = new Vector3(0, (-40 * entries.Count) + 130, 0);
		
		// Instantiate our prefab
		GameObject entry = (GameObject)Instantiate(entryPrefab);

		// Set it as a parent of the Highscore panel
		entry.transform.SetParent(highscorePanel.transform, false);
		
		// Set the items position
		entry.GetComponent<RectTransform>().localPosition = pos;
		
		// Set the name and kills
		entry.transform.GetChild(0).GetComponent<Text>().text = playerName;
		entry.transform.GetChild(1).GetComponent<Text>().text = playerKills.ToString();

		// Rename the GameObejct, so it is easier to sync to the list
		entry.name = playerName;
	}

	// Master Client asks a newly joined player, to run this function (so the player knows who are already in the game)
	[RPC]
	public void RequestAddExisting (string playerName, int playerKills) {
		entries.Add(new HighscoreEntry(playerName, playerKills));
		
		// Calculate our position first, so our list is empty (meaning the first postion is the one at the very top)
		Vector3 pos = new Vector3(0, (-40 * entries.Count) + 130, 0);
		
		// Instantiate our prefab
		GameObject entry = (GameObject)Instantiate(entryPrefab);
		
		// Set it as a parent of the Highscore panel
		entry.transform.SetParent(highscorePanel.transform, false);
		
		// Set the items position
		entry.GetComponent<RectTransform>().localPosition = pos;
		
		// Set the name and kills
		entry.transform.GetChild(0).GetComponent<Text>().text = playerName;
		entry.transform.GetChild(1).GetComponent<Text>().text = playerKills.ToString();
		
		// Rename the GameObejct, so it is easier to sync to the list
		entry.name = playerName;
	}

	// When a player disconnects, we hide him from the store, but keeps his data (Only after 5 mins, a hidden players data will be cleared)
	public void HidePlayer (PhotonPlayer player) {
		// Set the players highScoreEntry to setActive(false), and move him to the bottom of the list, make a entry variable called isActive
		// So we know if an entry should be visible or invisible
	}

	// Function called when we remove a player from the list of players
	public void RemovePlayer (PhotonPlayer player) {
		photonView.RPC("RequestRemove", PhotonTargets.AllBuffered, player);
	}

	[RPC]
	public void RequestRemove (PhotonPlayer player) {

		// If a client joins later, when this player has already left
		if (player != null) {
			string playerName = player.name;

			// Find the entry with the same playerName as the player who left, and remove him from the list
			for(int i=0; i < entries.Count; i++) {
				if (entries[i].playerName == playerName) {
					entries.Remove(entries[i]);

					// Find the entry Prefab with the same name as the player, and destroy it
					GameObject entry = GameObject.Find("/Canvas/Highscore/" + playerName);
					Destroy(entry);
				}
			}

			// Because a player was removed, sort the list
			for(int i = 0; i < entries.Count; i++) {

				GameObject entry = GameObject.Find("/Canvas/Highscore/" + entries[i].playerName);

				Vector3 pos = new Vector3(0, (-40 * (i+1)) + 130, 0);

				entry.GetComponent<RectTransform>().localPosition = pos;
			}
		}
	}
	
	// Function called add a score point to a player who killed another player (From HealthManager)
	public void UpdateScore (PhotonPlayer player) {
		photonView.RPC("RequestUpdate", PhotonTargets.AllBuffered, player);
	}

	[RPC]
	public void RequestUpdate (PhotonPlayer player) {
		string playerName = player.name;
		
		// Find the entry with the same playerName as the player killed a player, and add a score point to him
		for(int i=0; i < entries.Count; i++) {
			if (entries[i].playerName == playerName) {

				entries[i].playerKills++;

				// Find the entry Prefab with the same name as the player, and update the kills
				GameObject entry = GameObject.Find("/Canvas/Highscore/" + playerName);

				entry.transform.GetChild(1).GetComponent<Text>().text = entries[i].playerKills.ToString();
			}
		}
		// Sort the list by score (Does not seem to work?)
		entries = entries.OrderBy(x => x.playerKills).ToList();
//		entries.Sort(SortByScore);
	}

	// Used to sort the list (not working?)
	static int SortByScore(HighscoreEntry p1, HighscoreEntry p2) {
		return p1.playerKills.CompareTo(p2.playerKills);
	}
}
