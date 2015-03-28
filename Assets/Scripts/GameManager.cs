using UnityEngine;

public class GameManager : Photon.MonoBehaviour
{
//    public Transform playerPrefab;
	RespawnManager respawnManager;
	HighscoreManager highscoreManager;

    public void Awake()
    {
        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.connected)
        {
            Application.LoadLevel(MenuManager.SceneNameMenu);
            return;
        }

		// Set reference to manager scripts
		respawnManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<RespawnManager>();
		highscoreManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<HighscoreManager>();

		// Tell our spawn Manager who should be spawned
		PhotonPlayer player = PhotonNetwork.player;
		// Spawn our player
		respawnManager.OnPlayerJoined(player);
		// Add our player to the highscore system
		highscoreManager.AddPlayer(player);

        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
//        PhotonNetwork.Instantiate(this.playerPrefab.name, transform.position, Quaternion.identity, 0);
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Return to Lobby"))
        {
            PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
        }
    }

    public void OnMasterClientSwitched(PhotonPlayer player)
    {
        Debug.Log("OnMasterClientSwitched: " + player);

        string message;
        InRoomChat chatComponent = GetComponent<InRoomChat>();  // if we find a InRoomChat component, we print out a short message

        if (chatComponent != null)
        {
            // to check if this client is the new master...
            if (player.isLocal)
            {
                message = "You are Master Client now.";
            }
            else
            {
                message = player.name + " is Master Client now.";
            }


            chatComponent.AddLine(message); // the Chat method is a RPC. as we don't want to send an RPC and neither create a PhotonMessageInfo, lets call AddLine()
        }
    }

    public void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom (local)");
        
        // back to main menu        
        Application.LoadLevel(MenuManager.SceneNameMenu);
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("OnDisconnectedFromPhoton");

        // back to main menu        
        Application.LoadLevel(MenuManager.SceneNameMenu);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
		// This player should request a spawn from the Master Client
        Debug.Log("OnPhotonPlayerConnected: " + player);
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("OnPlayerDisconneced: " + player);

		// It seems that only the MasterClient gets this information
		// Remove player to the highscore system
		highscoreManager.RemovePlayer(player);
    }

    public void OnFailedToConnectToPhoton()
    {
        Debug.Log("OnFailedToConnectToPhoton");

        // back to main menu        
        Application.LoadLevel(MenuManager.SceneNameMenu);
    }
}
