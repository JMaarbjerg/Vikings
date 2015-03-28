using UnityEngine;
using System.Collections;

public class PlayerName : MonoBehaviour {
	
	public string player;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void MyName (string playerName) {
		if (GetComponent<NetworkView>().isMine) {
			player = playerName;
		}
	}
}
