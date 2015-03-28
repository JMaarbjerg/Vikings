using UnityEngine;
using System.Collections;

public class RopeScript : MonoBehaviour {

	public LineRenderer rope;

	public Transform hinge01;
	public Transform hinge02;
	public Transform hinge03;
	public Transform hinge04;
	public Transform viking;

	// Use this for initialization
	void Start () {
		rope.SetWidth(0.1f, 0.1f);
	}
	
	// Update is called once per frame
	void Update () {
		rope.SetPosition(0, gameObject.transform.localPosition);
		rope.SetPosition(1, hinge01.localPosition);
		rope.SetPosition(2, hinge02.localPosition);
		rope.SetPosition(3, hinge03.localPosition);
		rope.SetPosition(4, hinge04.localPosition);
		rope.SetPosition(5, viking.localPosition);
	}
}
