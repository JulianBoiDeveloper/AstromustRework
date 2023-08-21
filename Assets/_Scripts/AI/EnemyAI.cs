using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using System.Linq;

public class EnemyAI : Item1
{
    public Transform player;

    public float minMoveDistance = 8f;
    public float attackDistance = 3f;

    public float movementSpeed = 3f;
    public float rotationSpeed = 5f;
    Animator animator;
    [SerializeField] GameObject damageCollider;
    [SerializeField] GameObject hitVFX;

    public int health;

    PhotonView view;

    NavMeshAgent agent;

    [SerializeField] WaveSpawner waveSpawner;
    bool isRemovedFromWave = false;
    public float damage;
    Collider collider;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        view = GetComponent<PhotonView>();
        agent = GetComponent<NavMeshAgent>();
        waveSpawner = GameObject.FindObjectOfType<WaveSpawner>();
        collider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;


        if (!hasCalledFunction && animator.GetCurrentAnimatorStateInfo(0).IsName("Two Handed Sword Death")
        && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= normalizedEndTime)
        {
            hasCalledFunction = true;
            view.RPC("DestroyResource", RpcTarget.All);
        }

        if (isDead) {
            if(!isRemovedFromWave)
            {
                animator.SetTrigger("Die");
                waveSpawner.OnEnemyKilled();
                isRemovedFromWave = true;
            }
            return;
         }

        FindClosestTarget();

        if (player == null) return;
        Move();
    }

    public string targetStateName; // The name of the state in which the animation ends
    public float normalizedEndTime = 0.95f; // The normalized time (0.0 to 1.0) at which the function should be called

    private bool hasCalledFunction = false;
    public bool isDead = false;
    int damageToTake = 0;


    public override void Interact(GameObject go)
    {
        /*
        if (toolNeeded == null)
        {
            Debug.LogError("No tool found. You need to add a [toolNeeded] to " + gameObject.name);
            return;
        }
        if (go.GetComponent<TestCharacterController>().otherInventory.slots[0].item == null)
        {
            return;
        }
        if (go.GetComponent<TestCharacterController>().otherInventory.slots[0].item.gameObject.GetComponent<ToolStats>() == null)
        {
            Debug.LogError("No tool found. You need to add a TOOLSTATS script to " + gameObject.name);
            return;
        }
        if (go.GetComponent<TestCharacterController>().otherInventory.slots[0].item.gameObject.GetComponent<ToolStats>().tooltype != toolTypeNeeded)
        {
            Debug.Log("You don't have the correct tool on your inventory");
            return;
        }

        health -= go.GetComponent<TestCharacterController>().otherInventory.slots[0].item.gameObject.GetComponent<ToolStats>().currentStats.miningSpeed;
        */

        //    else
        //    {
        //  }

        if (isDead) return;

        Debug.Log("Enemy hit");
        if (go.GetComponent<TestCharacterController>() != null)
        {

            if (go.GetComponent<TestCharacterController>().HasGun())
            {
                for (int i = 0; i < go.GetComponent<TestCharacterController>().gunsInHandle.Count; i++)
                {
                    if (go.GetComponent<TestCharacterController>().gunsInHandle[i].gameObject.activeInHierarchy)
                    {
                        damageToTake = go.GetComponent<TestCharacterController>().gunsInHandle[i].gunDMG * go.GetComponent<TestCharacterController>().extraDamage;
                    }
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            damageToTake = 1;
        }

        if (hitVFX != null)
        {
            view.RPC("OnEnemyHit", RpcTarget.All, damageToTake);
        }

        if (health < 0)
        {
            if (go.GetComponent<TestCharacterController>() != null)
                go.GetComponent<TestCharacterController>().mainController.AddScore(100);
            
            // waveSpawner.OnEnemyKilled();
        }
    }

    [PunRPC]
    void OnEnemyHit(int _damgeToTake)
    {
        GameObject goVFX = Instantiate(hitVFX, transform.position + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(0.3f, 0.85f), 0f), Quaternion.identity);
        Destroy(goVFX, 3f);

        health -= _damgeToTake;
        if(health < 0)
        {
            isDead = true;
            collider.enabled = false;
        }
    }

    void Move()
    {
        if (Vector3.Distance(transform.position, player.position) < minMoveDistance)
        {
            if (Vector3.Distance(transform.position, player.position) < attackDistance)
            {
                animator.SetBool("Move", false);
                animator.SetTrigger("Attack");

                // Get the current state of the specified animation
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                // Check if the specified animation is playing and it is in the middle
                if (stateInfo.IsName("attack") && stateInfo.normalizedTime >= 0.5f && stateInfo.normalizedTime < 0.52f)
                {
                    // Animation is in the middle
                    Debug.Log("Animation is in the middle");
                    view.RPC("EnemyAttack", RpcTarget.All, true);
                }
                else
                {
                    view.RPC("EnemyAttack", RpcTarget.All, false);
                }
            }
            else
            {
                animator.SetBool("Move", true);
                // Move towards the player

                agent.destination = player.position;
                //transform.position += transform.forward * movementSpeed * Time.deltaTime;

                // Rotate to look at the player
            }
            Vector3 direction = player.position - transform.position;
            direction.y = 0f;
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void FindClosestTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Turret[] turrets = GameObject.FindObjectsOfType<Turret>();

        GameObject[] combinedArray = players.Concat(turrets.Select(turret => (GameObject)turret.gameObject)).ToArray();

        GameObject closestObject = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject obj in combinedArray)
        {
            float distance = Vector3.Distance(obj.transform.position, transform.position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestObject = obj;
            }
        }

        if (closestObject != null)
        {
            player = closestObject.transform;
        }
    }

    [PunRPC]
    void EnemyAttack(bool attack)
    {
        damageCollider.SetActive(attack);
    }

    [PunRPC]
    void DestroyResource()
    {
        this.gameObject.SetActive(false);
        Destroy(this.gameObject, 3f);
    }
}
