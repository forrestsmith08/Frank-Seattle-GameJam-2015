using UnityEngine;
using System.Collections;

public class FrankAttacks : MonoBehaviour {

	public GameObject arm;
	public float hitLength = 1;

	private CapsuleCollider hitCollider;
	private float facing = 1;

	// Use this for initialization
	void Start(){
		hitCollider = arm.GetComponent<CapsuleCollider>();
		hitCollider.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Fire1 was pressed.  Use assigned punch.  Called from player controller
	public void Punch (bool facingRight) {
		//if(facingRight){facing = 1;} else {facing = -1;}
		//Debug.Log(facingRight + " " + facing);
		StartCoroutine(PunchCoroutine(0.4F));
	}

	IEnumerator PunchCoroutine(float waitTime){
		yield return new WaitForSeconds(0.02f);
		hitCollider.center = hitCollider.center + new Vector3(facing * hitLength, 0, 0);
		hitCollider.height = .40f;
		hitCollider.enabled = true;
		yield return new WaitForSeconds(waitTime);
		hitCollider.center = hitCollider.center - new Vector3(facing * hitLength, 0, 0);
		hitCollider.height = .20f;
		hitCollider.enabled = false;
	}
}
