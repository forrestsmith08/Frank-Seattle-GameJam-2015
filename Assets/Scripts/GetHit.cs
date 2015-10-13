using UnityEngine;
using System.Collections;

public class GetHit : MonoBehaviour {
	
	private Rigidbody rigidBody;
	void Start ()
	{
		rigidBody = this.GetComponent<Rigidbody>();
	}

	void OnTriggerEnter(Collider other){
		Destroy(this.gameObject);
	}
}
