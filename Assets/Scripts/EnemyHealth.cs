using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{

    public int health = 1;

    public delegate void HitDetected();
    public event HitDetected OnHitDetected;
    public delegate void DeathDetected();
    public event DeathDetected OnDeathDetected;

    void OnTriggerEnter(Collider other)
    {
        //
        //      Change layer name to match the real project's layer name
        //
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerAttack"))
        {
            health -= 1;
            if (health == 0)
            {
                OnDeathDetected();
                return;
            }
            OnHitDetected();
        }
    }

	void OnCollisionEnter(Collision other)
	{
		//
		//      Change layer name to match the real project's layer name
		//
		if (other.gameObject.layer == LayerMask.NameToLayer("PlayerAttack"))
		{
			health -= 1;
			if (health == 0)
			{
				OnDeathDetected();
				return;
			}
			OnHitDetected();
		}
	}
	
}
