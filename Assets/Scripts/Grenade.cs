using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour
{

    public float grenadeFuse = 2f;

    private Animator anim;

    void OnEnable()
    {
        anim = GetComponent<Animator>();
        StartCoroutine("Fuse");
    }

    public void Arm()
    {

		tag = "Enemy";
		transform.localScale = transform.localScale*2;
		GetComponent<SphereCollider>().radius = 0.09f;
		GetComponent<Rigidbody>().isKinematic = true;
		//gameObject.layer = LayerMask.NameToLayer("PlayerAttack");
		GetComponent<SphereCollider>().isTrigger = true;
		GetComponent<AudioSource>().Play();

    }

    void OnTriggerEnter(Collider other)
    {

    }

    IEnumerator Fuse()
    {
        yield return new WaitForSeconds(grenadeFuse);
        anim.SetTrigger("arm");
    }

    public void Kill()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }
}
