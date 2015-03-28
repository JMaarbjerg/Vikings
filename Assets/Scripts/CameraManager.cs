using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	public Transform mainCamera;
	public Transform target;

	public float dampTime = 0.3f;
	private Vector3 velocity = Vector3.zero;

//	public float smoothTime = 0.3f;
//	private float yVelocity = 0.0f;
//
//	// Update is called once per frame
//	void Update () {
//		if (target) {
//			float newPosition = Mathf.SmoothDamp(mainCamera.transform.position.y, target.position.y, ref yVelocity, smoothTime);
//			mainCamera.transform.position = new Vector3(target.transform.position.x, newPosition, mainCamera.transform.position.z);
//		}
//	}

	// Update is called once per frame (in sync with physics)
	void FixedUpdate () {
		if (target) {
			Vector3 point = mainCamera.GetComponent<Camera>().WorldToViewportPoint(target.position);
			Vector3 delta = target.position - mainCamera.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = mainCamera.position + delta;
			mainCamera.position = Vector3.SmoothDamp(mainCamera.position, destination, ref velocity, dampTime);
		}
		
	}
}