using UnityEngine;
using System.Collections;

public class CharacterManager : Photon.MonoBehaviour {

	// Variables
	public float accelerationSpeed = 10f;

	public float minSpeed = 0f;
	public float maxSpeed = 20f; // Ikke integreret

	public float currentSpeed, accelerationMultiplier; // Ikke integreret

	public Rigidbody2D rb; //Den rigidbody, som skal bevæges "Player"
	public float myGravity;
	private Vector2 v, vel;

	public RespawnManager respawn;
	
	public CameraManager characterCamera;
	public GameObject[] players;

	// Use this for initialization
	void Start () {

		respawn = GameObject.FindGameObjectWithTag("Managers").GetComponent<RespawnManager>();

		players = GameObject.FindGameObjectsWithTag("Player");

		// Set the camera to the correct player
		foreach (GameObject player in players) {
			if (player.GetComponent<PhotonView>().isMine) {
				characterCamera = GameObject.FindGameObjectWithTag("Managers").GetComponent<CameraManager>();

				// The camera should be fixed on the first child of the player, which is the balloons
				characterCamera.target = player.transform.GetChild(0).transform;
			}
		}
		// Gravity is multiplied by 13, because the total amount of mass = 13
		myGravity = (Physics2D.gravity.y * 13) * -1;
		rb = GetComponent<Rigidbody2D> ();
	}
	
	void Update () {
		accelerationMultiplier = 1 - (vel.magnitude / maxSpeed); //Søger for at styrer at accelerationen bliver 0, når topfarten er nået
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Movement (); // Styrer spillerens bevægelser
		rb.AddForce (new Vector2 (0, myGravity)); // Gør at spilleren ikke bliver trukket ned af tyngdekraften

	}
	
	void Movement ()
	{
		if (gameObject.GetComponent<PhotonView>().isMine) {
			v = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
			minSpeed = (accelerationSpeed / maxSpeed);

			rb.velocity += (v * accelerationSpeed - (minSpeed * rb.velocity)) * Time.deltaTime;
			vel = rb.velocity;
			currentSpeed = vel.magnitude;

//			if (Input.GetMouseButton(0)) {
//				PhotonNetwork.Destroy(gameObject);
//				respawn.OnPlayerJoined();
//			}
		}
	}
}
