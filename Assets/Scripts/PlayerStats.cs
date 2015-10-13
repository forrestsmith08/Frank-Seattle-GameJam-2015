using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour{

	public GameObject uiCanvas;

	private Animator healthAnimator;
	private int health = 5;
	private int score = 0;

	void Start ()
	{
		healthAnimator = uiCanvas.GetComponent<Animator>();
	}

	public void Hurt(){
		health--;
		healthAnimator.SetInteger("health",health);
	}

	public void Heal(){
		if(health < 5){
			health++;
			healthAnimator.SetInteger("health",health);
		}
	}

	public int Health(){return health;}

}
