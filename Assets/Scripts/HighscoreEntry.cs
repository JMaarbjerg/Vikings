using UnityEngine;
using System.Collections;

[System.Serializable]
public class HighscoreEntry {

	public string playerName;
	public int playerKills;
//	public int playerDeaths;

	// Function to set up an entry
	public HighscoreEntry (string name, int kills) {
		playerName = name;
		playerKills = kills;
	}
}