using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Turret : MonoBehaviour
{
    public Transform cannon;            // Reference to the cannon GameObject
    public float attackRange = 10f;     // Range within which the turret can attack enemies
    public float attackInterval = 1f;   // Time between each attack
    public float attackDamage = 5f;     // Damage dealt per attack
    public float collisionDamage = 3f; // Damage taken when colliding with enemies
    public float maxHealth = 100f;      // Maximum health of the turret

    public float currentHealth;        // Current health of the turret

    [SerializeField] Image healthbar;
    [SerializeField] PhotonView view;

    float shotTimer = 0f;
    [SerializeField] float shotDuration = 0.5f;

    public float detectionAngle = 60f; // The angle in degrees to detect enemies within

    [SerializeField] Animator animator;
    
    private AudioSource attackSoundManage;
    public AudioClip attackSound;

    private void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("Adding max health");
        attackSoundManage = GetComponent<AudioSource>();
        StartCoroutine(AttackCoroutine());
    }

    private void Update()
    {
        FindClosestEnemy();

        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void FindClosestEnemy()
    {
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();

        if (enemies.Length == 0)
            return;

        EnemyAI closestEnemy = enemies[0];
        float closestDistance = Vector3.Distance(transform.position, closestEnemy.transform.position);

        for (int i = 1; i < enemies.Length; i++)
        {
            if (!enemies[i].isDead) {
                float distance = Vector3.Distance(transform.position, enemies[i].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemies[i];
                }
            }
        }

        shotTimer += Time.deltaTime;
        if(IsEnemyInFront(closestEnemy) && shotTimer > shotDuration) {
            // Attack the closest enemy if within range
            if (closestDistance <= attackRange)
            {
                animator.Play("shoot");
                animator.Play("shoot");
                attackSoundManage.clip = attackSound;
                attackSoundManage.PlayOneShot(attackSound);
                if (view.IsMine) closestEnemy.GetComponent<IInteractable>().Interact(this.gameObject);
                shotTimer = 0f;
            }
        }

        // Turn the cannon to face the closest enemy
        if (cannon != null && closestEnemy != null)
        {
            Vector3 direction = closestEnemy.transform.position - cannon.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            cannon.rotation = Quaternion.Lerp(cannon.rotation, targetRotation, Time.deltaTime);
        }
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<EnemyAI>() != null)
        {
            TakeDamage(collisionDamage);
        }
    }

    [PunRPC]
    void UpdateHealth(float newHealth)
    {
        if(currentHealth > 0) {
            currentHealth = newHealth;
            healthbar.fillAmount = newHealth / maxHealth;
        }
    }

    private void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Taking dmg");

        view.RPC("UpdateHealth", RpcTarget.All, currentHealth);

        if (currentHealth <= 0f)
        {
            // Turret destroyed, do something
            Debug.Log("TURRET DEATH");
            PhotonNetwork.Destroy(gameObject);
            
        }
    }

    public void SavePreviousHealth(float health)
    {
        view.RPC("UpdateHealth", RpcTarget.All, health);
    }

    private IEnumerator AttackCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);
            FindClosestEnemy();
        }
    }

    private bool IsEnemyInFront(EnemyAI closestEnemy)
    {
        Vector3 playerForward = cannon.forward;
        Vector3 enemyDirection = closestEnemy.transform.position - cannon.position;

        float angle = Vector3.Angle(playerForward, enemyDirection);

        if (angle <= detectionAngle / 2f)
        {
            return true;
        }

        return false;
    }
}
