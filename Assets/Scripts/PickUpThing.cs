using UnityEngine;
using System.Collections;

public class PickUpThing : MonoBehaviour {

	public GameObject frank;

	private FrankController frankController;

	// Use this for initialization
	void Start () {
		frankController = frank.GetComponent<FrankController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		Debug.Log ("Triggered Arm");
		if(other.CompareTag("PickUp")){
			frankController.Grab(other.gameObject);
		}
	}
}
