using UnityEngine;
using System.Collections;

public class QuickTrashyController : MonoBehaviour
{

    public Path path;
    public Grenade grenade;
    public Transform grenadeLauncherHole;
    public Transform grenadeLauncherForce;
    public float launcherForceMultiplier = 10f;
    public float speed = 1f;
    public float fireRate = 2f;
    public float preAttackTime = 1f;
    public float postAttackTime = 1f;
    public float hitStunTime = 2f;
    public float timeSpentDying = 1f;
    public float stallTimeAtEndOfPath = 1f;

    private bool isLeft = true;
    private bool canMove;
    private Transform node1;
    private Transform node2;
    private Transform targetNode;

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
            Vector3 oldPosition = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetNode.position, speed * Time.deltaTime);

            if (transform.position == targetNode.position)
            {
                if (targetNode == node1)
                    targetNode = node2;
                else
                    targetNode = node1;

                StartCoroutine(WaitAtNode());
            }
            else
            {
                UpdateDirection(transform.position - oldPosition);
            }
        }
    }

    public void UpdateDirection(Vector3 direction)
    {
        if (Mathf.Sign(direction.x) == -1f)
        {
            isLeft = true;
            Vector3 scale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            transform.localScale = scale;
        }
        else
        {
            isLeft = false;
            Vector3 scale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            transform.localScale = scale;
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
            yield return new WaitForSeconds(fireRate);

            // Attack anim
            anim.SetTrigger("attack");
            canMove = false;
            yield return new WaitForSeconds(preAttackTime);


            Grenade g = Instantiate(grenade, grenadeLauncherHole.position, new Quaternion()) as Grenade;
            Rigidbody r = g.GetComponent<Rigidbody>();
            r.AddForce((grenadeLauncherForce.position - grenadeLauncherHole.position) * launcherForceMultiplier, ForceMode.Impulse);
           

            yield return new WaitForSeconds(postAttackTime);

            // Move anim
            canMove = true;
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
