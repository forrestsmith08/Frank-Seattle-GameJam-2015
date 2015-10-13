using UnityEngine;
using System.Collections;

public class QuickDroneController : MonoBehaviour {

    public Path path;
    public float speed = 1f;
    public float electricityDropRate = 2f;
    public float attackAnimTime = 1f;
    public float hitStunTime = 2f;
    public float timeSpentDying = 1f;
    public float stallTimeAtEndOfPath = 1f;

    private bool canMove;
    private Transform node1;
    private Transform node2;
    private Transform targetNode;

    private EnemyAttack enemyAttack;

    private Animator anim;
    private EnemyHealth health;
	
    void Awake()
    {
        node1 = path.nodes[0];
        node2 = path.nodes[1];
        transform.position = node1.position;
        targetNode = node2;

        anim = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();

        enemyAttack = GetComponentInChildren<EnemyAttack>();
        enemyAttack.gameObject.SetActive(false);

        canMove = true;
        AddListeners();
    }

    void Start()
    {
        StartCoroutine(Attack());
    }
	
	void Update()
    {
        if (canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetNode.position, speed * Time.deltaTime);

            if (transform.position == targetNode.position)
            {
                if (targetNode == node1)
                    targetNode = node2;
                else
                    targetNode = node1;

                StartCoroutine(WaitAtNode());
            }
        }
	}

    public void OnHit()
    {
        StopCoroutine("Attack");
        canMove = false;


        // Change anim


        StartCoroutine(Attack());
    }

    IEnumerator HitComplete()
    {
        yield return new WaitForSeconds(hitStunTime);

        // Change anim

        StartCoroutine(Attack());
        canMove = true;
    }

    public void OnDeath()
    {
        canMove = false;
        StopCoroutine("Attack");

        // Change anim
        anim.SetTrigger("die");

        StartCoroutine("DeathComplete");
    }

    IEnumerator DeathComplete()
    {
        yield return new WaitForSeconds(timeSpentDying);

        RemoveListeners();
        StopAllCoroutines();
        Destroy(gameObject);
    }

    IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(electricityDropRate);

            // Attack anim and game object drop
            anim.SetTrigger("attack");
            enemyAttack.gameObject.SetActive(true);

            yield return new WaitForSeconds(attackAnimTime);

            // Move anim
            enemyAttack.gameObject.SetActive(false);
            anim.SetTrigger("move");
        }
    }

    IEnumerator WaitAtNode()
    {
        canMove = false;
        yield return new WaitForSeconds(stallTimeAtEndOfPath);
        canMove = true;
    }

    private bool IsAtNode1()
    {
        return transform.position == node1.position;   
    }

    private bool IsAtNode2()
    {
        return transform.position == node2.position;
    }

    private void AddListeners()
    {
        if (health != null)
        {
            health.OnHitDetected += OnHit;
            health.OnDeathDetected += OnDeath;
        }
    }

    private void RemoveListeners()
    {
        if (health != null)
        {
            health.OnHitDetected -= OnHit;
            health.OnDeathDetected -= OnDeath;
        }
    }

}
