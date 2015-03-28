using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	
	public string gameName;

	// For user to input his own string
	public string listenPortString;
	public int listenPort;
	public string playerName;
	
    public GameObject masterclient;
    public GameObject client;
	
	// If a player disconnects, turn on the main camera
	public Camera mainCamera;
		
	public GUIStyle titleStyle;
	public GUIStyle textStyle;
	public GUIStyle buttonStyle;
	
	private bool refreshing;
	private HostData[] hostData; 

	private bool showGUI = true;
	private int menuLevel = 0;

	private float btnX;
	private float btnY;
	private float btnW;
	private float btnH;
	
	// Use this for initialization
	void Start () {
		btnX = Screen.width/20;
		btnY = Screen.height/14;
		btnW = Screen.width * 0.4f;
		btnH = Screen.height * 0.2f;
		
		gameName = "GameJam_Project";
		listenPortString = "51001";
		listenPort = 51001;
		playerName = "Guest" + Random.Range(1, 9999);
		
		titleStyle.fontSize = Screen.height/6;
		textStyle.fontSize = Screen.height/16;
		buttonStyle.fontSize = Screen.height/16;
	}
	
	// Update is called once per frame
	void Update () {
		if (refreshing) {
			if (MasterServer.PollHostList().Length > 0) {
				refreshing = false;
				hostData = MasterServer.PollHostList();
			}
		}

		// Hide GUI
		if (Network.isServer) {
			if (Input.GetKeyDown("h")) {
				if (showGUI == true) showGUI = false;
				else showGUI = true;
			}
		}
	}
	
	void startServer () {
		bool useNat = !Network.HavePublicAddress();
		Network.InitializeServer(32, listenPort, useNat);
		MasterServer.RegisterHost(gameName, "Test Server", "This is the server");
	}
	
	void refreshHostList() {
		MasterServer.RequestHostList(gameName);
		refreshing = true;
	}
	
	// Called when the master starts the server
	void OnServerInitialized () {
		Debug.Log("Server initialized");
	}
	
	// Called on a client when he joins the server
	void OnConnectedToServer () {
		Debug.Log("Player joined");
//		string player = playerName;
//		networkView.RPC ("TellServerPlayerHasJoined", RPCMode.Server, playerName);
		
		InstantiatePlayer(playerName);
	}
	
	void InstantiatePlayer (string player) {
		if (!Network.isServer) {
			// When a player wants to spawn an array of gameobjects with the tag "SpawnPoint" is created
//			GameObject[] spawnPoints;
//			spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
//			
//			// Then a random gameobject in the array is chosen to be the variable spawnPoint
//			if (spawnPoints.Length > 0) {
//				GameObject randomSpawnPoint = spawnPoints[Random.Range(0,spawnPoints.Length)];
//				spawnPoint = randomSpawnPoint;
//			}
//			else {
//				// If the array is empty a default spawn point is set to be a gameobject with the name "Spawn Point 1"
//				GameObject defaultSpawnPoint = GameObject.Find("Spawn Point 1");
//				spawnPoint = defaultSpawnPoint;
//			}
//			
//			Vector3 spawnPoint = new Vector3(0,0,0);
//			// When a player joins the gamemanager is told to instantiate a pirate prefab
//			GameObject balloonShip = Network.Instantiate(Resources.Load("Player"), spawnPoint, Quaternion.identity, 3) as GameObject;
//			
//			// The pirate prefab is renamed in order to compare it to the players name
//			playerName = player;
//			balloonShip.SendMessage("SetName", player);
		}
	}

    void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Player disconnected");
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }
	
	void OnMasterServerEvent(MasterServerEvent mse) {
		if(mse == MasterServerEvent.RegistrationSucceeded) {
			Debug.Log("Registered Server");
		}
	}
	
	void OnGUI () {
		if (!Network.isClient && !Network.isServer) {
			if (menuLevel == 0) {
				// Title
				GUI.Label(new Rect(btnX, btnY * 2f, Screen.width * 0.9f, Screen.height/6), "Balloon Vikings", titleStyle);

				// Play Game
				if (GUI.Button(new Rect (Screen.width * 0.3f, btnY * 6f, btnW, btnH), "Play", buttonStyle)) {

					// One level up in the menu
					menuLevel++; 
				}

				// Exit Game
				if (GUI.Button(new Rect (Screen.width * 0.3f, btnY * 9f, btnW, btnH), "Exit", buttonStyle)) {
					
					// Destroy the players ship prefab
					
					// Disconnect from server
					Network.Disconnect();
					
					// Close application
					Application.Quit();
				}
			}

			if (menuLevel == 1) {
				// Player name
				playerName = GUI.TextField(new Rect(btnX, btnY * 1f, btnW, btnH), playerName, 10, buttonStyle);

				// Listen Port
				listenPortString = GUI.TextField(new Rect(Screen.width/2 + btnX, btnY * 1f, btnW, btnH), listenPortString, 10, buttonStyle);

				// Start server
				if (GUI.Button(new Rect(Screen.width/2 + btnX, btnY * 4f, btnW, btnH), "Start Server", buttonStyle)) {
					if (listenPortString.Length >= 1) {
						listenPort = int.Parse(listenPortString);
						startServer();
					}
				}

				// Refresh hostlist
				if (GUI.Button(new Rect(btnX, btnY * 4f, btnW, btnH), "Find Server", buttonStyle)) {
					refreshHostList();
				}
				
				// Hostlist
				if (hostData != null) {
					for (int i = 0; i < hostData.Length; i++) {
						// For each created server, stored in hostData, a button will be created
						if (GUI.Button(new Rect(btnX, btnY * 6f + (btnH * i), btnW, btnH), hostData[i].gameName, buttonStyle)) {
							// Playername has to be longer than 4 characters
	//						if (playerName.Length >= 4) {
								// Connect to game
								Network.Connect(hostData[i]);
	//						}
						}
					}
				}

				// Back
				if (GUI.Button(new Rect (Screen.width * 0.3f, btnY * 10f, btnW, btnH), "Back", buttonStyle)) {
					
					// One level up in the menu
					menuLevel--; 
				}
			}
		}else {
			if (showGUI == true) {
				// Disconnect from Server
				if (GUI.Button(new Rect (0, 0, Screen.width/5, Screen.height/10), "Disconnect", buttonStyle)) {

					// Disconnect from server if you are the host
					Network.Disconnect();
				}
			}
		}
	}
}