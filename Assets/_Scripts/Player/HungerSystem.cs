using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Photon.Realtime;

public class HungerSystem : MonoBehaviourPunCallbacks
{
    public float health = 100f;
    public float hunger = 100f;
    public float thrist = 100f;

    public float maxHealth = 100f;
    public float maxHunger = 100f;
    public float maxThrist = 100f;

    private float hungerTimer = 0f;
    public float hungerTickDuration = 20f;

    private float thristTimer = 0f;
    public float thristTickDuration = 17f;

    private float healthTimer = 0f;
    public float healthTickDuration = 10f;

    private float healthRegenTimer = 0f;
    public float healthRegenTickDuration = 10f;

    [SerializeField] Image healthBar;
    [SerializeField] Image hungerBar;
    [SerializeField] Image thristBar;

    [SerializeField] bool healOverTime = true;
    public float healingRate = 1.5f;
    float healTimer = 0f;
    public float healDuration = 2f;
    private WaveSpawner waveSpawner;

    [SerializeField] GameObject gameOverScreen;

    private void Start()
    {
        if(hungerBar != null) {
            if(healOverTime)
            {
                hungerBar.gameObject.SetActive(false);
                thristBar.gameObject.SetActive(false);
            }
        }

        waveSpawner = GameObject.FindObjectOfType<WaveSpawner>();
    }

    private void Update()
    {
        if (health <= 0)
        {
            if(GetComponent<PhotonView>().IsMine) {
                if (CloudSave.Instance.maxWave == 0 || CloudSave.Instance.maxWave < waveSpawner.currentWave) {
                    CloudSave.Instance.SaveEntry("MaxWave", waveSpawner.currentWave.ToString());
                }   
                Debug.Log("you die");
                StartCoroutine("GameOver");
            //    PhotonNetwork.Disconnect();
            //    PhotonNetwork.LoadLevel("MenuMain");
            }
        }
        if(!healOverTime) {
            UpdateHunger();
            UpdateThirst();
            UpdateHealth();
        }
        else
        {
            hungerBar.transform.parent.gameObject.SetActive(false);
            thristBar.transform.parent.gameObject.SetActive(false);

            healTimer += Time.deltaTime;
            if(healTimer > healDuration)
            {
                AddHealth(healingRate);
                healTimer = 0f;
            }
        }
    }

    void UpdateHunger()
    {
        hungerTimer += Time.deltaTime;
        if(hungerTimer > hungerTickDuration)
        {
            hungerTimer = 0f;
            hunger--;
            hungerBar.fillAmount = hunger / maxHunger;

            if (hunger < 0) hunger = 0;
        }
    }

    void UpdateThirst()
    {
        thristTimer += Time.deltaTime;
        if (thristTimer > thristTickDuration)
        {
            thristTimer = 0f;
            thrist--;
            thristBar.fillAmount = thrist / maxThrist;

            if (thrist < 0) thrist = 0;
        }
    }

    void UpdateHealth()
    {
        if(hunger < 20f)
        {
            healthTimer += Time.deltaTime;
        }
        if(thrist < 20f)
        {
            healthTimer += Time.deltaTime;
        }

        if (healthTimer > healthTickDuration)
        {
            healthTimer = 0f;
            health -= 5 / (Mathf.Max(hunger, 1) / 4f) + (5 / (Mathf.Max(thrist,1) / 4f));

            healthBar.fillAmount = health / maxHealth;

            if (health < 0) health = 0f;
        }

        if(health < maxHealth) {
            if (hunger > 80f)
            {
                healthRegenTimer += Time.deltaTime;
            }
            if (thrist > 80f)
            {
                healthRegenTimer += Time.deltaTime;
            }

            if (healthRegenTimer > healthRegenTickDuration)
            {
                healthRegenTimer = 0f;
                health += hunger / 65f + thrist / 65f;

                healthBar.fillAmount = health / maxHealth;
            }
        }
    }

    public void AddHealth(float toAdd)
    {
        health += toAdd;
        if (health > maxHealth) health = maxHealth;

        healthBar.fillAmount = health / maxHealth;
    }

    public void AddHunger(float toAdd)
    {
        hunger += toAdd;
        if (hunger > maxHunger) hunger = maxHunger;

        hungerBar.fillAmount = hunger / maxHunger;
    }

    public void AddThrist(float toAdd)
    {
        thrist += toAdd;
        if (thrist > maxThrist) thrist = maxThrist;

        thristBar.fillAmount = thrist / maxThrist;
    }

    IEnumerator GameOver()
    {
        gameOverScreen.SetActive(true);
        yield return new WaitForSeconds(1.4f);
        PhotonNetwork.Disconnect();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Damage")
        {
            Debug.Log("HIT BY DAMAGE");
            health -= other.transform.parent.GetComponent<EnemyAI>().damage;
            if (health < 0f) health = 0f;
            healthBar.fillAmount = health / maxHealth;
        }
    }
}
