using UnityEngine;
using System.Collections;

public class FrankPhysics: MonoBehaviour{
	
	public LayerMask collisionMask;
	
	private BoxCollider collider;
	private Vector3 size;
	private Vector3 center;
	
	private float skin = .005f;
	
	private Ray ray;
	private RaycastHit hit;
	private Animator animator;
	
	[HideInInspector]
	public bool grounded;
	public bool movementStopped;
	
	void Start(){
		collider = GetComponent<BoxCollider>();
		animator = GetComponent<Animator>();
		size = collider.size;
		center = collider.center;
	}
	
	public void Move (Vector2 moveamount){
		
		float deltaY = moveamount.y;
		float deltaX = moveamount.x;
		Vector2 position = transform.position;
		
		grounded = false;
		movementStopped = false;
		
		for (int i = 0; i < 3; i++){
			float dir = Mathf.Sign(deltaY);
			float x = (position.x + center.x - size.x/2) + size.x/2 * i;  //Left, center, and rightmost point of collider
			float y = position.y + center.y + size.y/2 * dir;
			
			ray = new Ray(new Vector2(x, y), new Vector2(0, dir));
			Debug.DrawRay(new Vector2(x,y), new Vector2(0,dir));
			if (Physics.Raycast(ray, out hit, Mathf.Abs (deltaY) + skin, collisionMask)){
				//Get distance between player and ground
				float dist = Vector3.Distance (ray.origin, hit.point);
				
				//Stop player's movement comin within skin width of a collider
				if(dist > skin){
					deltaY = (dist * dir) + (skin * -dir);
				} else {
					deltaY = 0;	
				}
				grounded = true;
				animator.SetTrigger("grounded");
				break;
			}
		}
		
		for (int i = 0; i < 3; i++){
			float dir = Mathf.Sign(deltaX);
			float x = position.x + center.x + size.x/2 * dir;  //Leading edge
			float y = position.y + center.y - size.y/2 + size.y/2 * i;  //Bottom middle and top
			
			ray = new Ray(new Vector2(x, y), new Vector2(dir, 0));
			Debug.DrawRay(ray.origin, ray.direction);
			if (Physics.Raycast(ray, out hit, Mathf.Abs (deltaX) + skin, collisionMask)){
				//Get distance between player and ground
				float dist = Vector3.Distance (ray.origin, hit.point);
				
				//Stop player's movement comin within skin width of a collider
				if(dist > skin){
					deltaX = (dist * dir) + (skin * -dir);
				} else {
					deltaX = 0;	
				}
				movementStopped = true;
				break;
			}
		}
		
		if(!movementStopped && !grounded){
			Vector3 playerDir = new Vector3(deltaX, deltaY);
			Vector3 origin = new Vector3(position.x + center.x + size.x/2 * Mathf.Sign(deltaX), position.y + center.y + size.y/2 * Mathf.Sign (deltaY));
			ray = new Ray(origin, playerDir.normalized);
			Debug.DrawRay(ray.origin, ray.direction);
			
			if(Physics.Raycast(ray, Mathf.Sqrt(deltaX*deltaX + deltaY*deltaY), collisionMask)) {
				grounded = true;
				deltaY = 0;
			}
		}
		
		Vector2 finalTransform = new Vector2(deltaX, deltaY);
		transform.Translate(finalTransform);
		animator.SetFloat("speedx",Mathf.Abs (deltaX));
		animator.SetFloat("speedy",Mathf.Abs (deltaY));
	}
}
