using UnityEngine;
using System.Collections;

[RequireComponent(typeof(FrankPhysics))]
public class FrankController : MonoBehaviour {
	
	//Player Handling
	public float acceleration = 30;
	public float gravity = 20;
	public float speed = 8;
	public float jumpHeight = 12;
	public float wallJumpStickTime = 0.5f;
	public float invulnerabilityTime = 1.5f;
	public GameObject carryZone;
	public float throwHeight;
	public float throwDistance;

	public AudioClip jumpSound;
	public AudioClip dieSound;
	public AudioClip whipSound;
	public AudioClip hurtSound;
	
	private Vector2 amountToMove;
	private float currentSpeed;
	private float targetSpeed;

	private float facing = 1;
	private FrankPhysics playerPhysics;
	private FrankAttacks playerAttacks;
	private Animator animator;
	private PlayerStats playerStats;
	private SpriteRenderer spriteRenderer;

	private bool canWallJump = false;
	private int wallJumpsLeft = 1;
	private bool invulnerable = false;
	private bool facingRight = true;
	private bool frankDead = false;
	private int health = 5;
	private AudioSource audioSource;
	private SpriteRenderer carryRenderer;
	private bool carryingSomething = false;
	private GameObject carriedObject;

	void Start () {
		playerPhysics = GetComponent<FrankPhysics>();
		playerAttacks = GetComponent<FrankAttacks>();
		animator = GetComponent<Animator>();
		playerStats = GetComponent<PlayerStats>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		audioSource = GetComponent<AudioSource>();
		carryRenderer = carryZone.GetComponent<SpriteRenderer>();
	}
	
	void Update () {
		if(frankDead){return;}
		if(playerPhysics.movementStopped){
			targetSpeed = 0;
			currentSpeed = 0;
		}

		//If you cant currently wall jump, your movement is blocked by a wall, and you aren't grounded, then turn on wall jumping
		if(!canWallJump && playerPhysics.movementStopped && !playerPhysics.grounded){
			animator.SetBool("blocked", true);
			StartCoroutine(WallJumpOn(wallJumpStickTime));
		}


		//Input
		targetSpeed = Input.GetAxisRaw("Horizontal") * speed;
		currentSpeed = IncrementTowards(currentSpeed, targetSpeed, acceleration);
		if(currentSpeed != 0)facing = Mathf.Sign (currentSpeed);


		if(playerPhysics.grounded){
			amountToMove.y = 0;			
			//jump
			if(Input.GetButtonDown("Jump")){
				amountToMove.y = jumpHeight;
				audioSource.PlayOneShot(jumpSound, 0.1f);
			}
		} else if(canWallJump && (wallJumpsLeft > 0)){
			if(Input.GetButtonDown("Jump")){
				amountToMove.y = jumpHeight;
				wallJumpsLeft--;
				audioSource.PlayOneShot(jumpSound, 0.1f);
			}
		}

		if((facingRight && (targetSpeed < 0)) || (!facingRight && (targetSpeed > 0))){
			Flip();
		}

		if(Input.GetButtonDown("Fire1")){
			if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Frank-Punch")){
				animator.SetTrigger("punch");
				playerAttacks.Punch(facingRight);
				audioSource.PlayOneShot(whipSound, 0.07f);
			}
			if(carryingSomething){Throw();}
		}



		amountToMove.x = currentSpeed;
		amountToMove.y -= gravity * Time.deltaTime;
		playerPhysics.Move(amountToMove * Time.deltaTime);
		audioSource.Play();
	}

	IEnumerator WallJumpOn(float time){
		canWallJump = true;
		yield return new WaitForSeconds(time);
		canWallJump = false;
		animator.SetBool("blocked", false);
		wallJumpsLeft = 1;
	}

	IEnumerator HitInvulnerability(float time){
		playerStats.Hurt();
		health--;
		animator.SetInteger("health",health);
		if(playerStats.Health() > 0){
			audioSource.PlayOneShot(hurtSound, 0.15f);
			animator.SetTrigger("hurt");
			invulnerable = true;
			for(int i = 0; i < 10; i++){
				spriteRenderer.color = new Color(0,0,0);
				yield return new WaitForSeconds(time);
				spriteRenderer.color = new Color(255,255,255);
				yield return new WaitForSeconds(time);
			}
			invulnerable = false;
		} else {
			animator.SetTrigger("hurt");
			audioSource.PlayOneShot(dieSound, 0.1f);
			FrankDie ();
		}
	}

	//Increase n towards target by speed
	private float IncrementTowards(float n, float target, float a) {
		if (n == target){
			return n;
		} else {
			float dir = Mathf.Sign (target - n);
			n += a * Time.deltaTime * dir;
			return (dir == Mathf.Sign (target - n))? n: target;
		}
	}

	void FrankDie(){
		frankDead = true;
		StartCoroutine(waitDie (2.0f));

	}

	IEnumerator waitDie(float time){
		yield return new WaitForSeconds(time);
		Application.LoadLevel("Main Menu");
	}
	void Flip()
	{
		// Switch the way the player is labelled as facing
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void checkEnemy(Collider other){
		if(frankDead){return;}
		if(other.CompareTag("Enemy") && !invulnerable){
			StartCoroutine(HitInvulnerability(0.08f));
		}
	}

	void OnTriggerEnter(Collider other)
	{
		checkEnemy(other);
	}

	void OnTriggerStay(Collider other)
	{
		checkEnemy(other);
	}

	public void Grab(GameObject grabbed){
		if(!carryingSomething){
			carriedObject = Instantiate(grabbed, carryZone.transform.position, Quaternion.identity) as GameObject;
			SpriteRenderer renderer = carriedObject.GetComponent<SpriteRenderer>();
			carryRenderer.sprite = renderer.sprite;
			carryingSomething = true;
			carriedObject.SetActive(false);
			Destroy(grabbed);
		}
	}

	void Throw(){
		if(carryingSomething){
			int face = (facingRight? 1:-1);
			carriedObject.SetActive(true);
			carriedObject.transform.position = carryZone.transform.position;
			carriedObject.GetComponent<Rigidbody>().velocity = new Vector3(throwDistance * face, throwHeight, 0);
			carriedObject.layer = LayerMask.NameToLayer("PlayerAttack");
			carryingSomething = false;
			carryRenderer.sprite = null;
		}
	}
}