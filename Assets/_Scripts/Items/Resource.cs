using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : Item1
{
    public GameObject material;
    public GameObject[] otherMaterials;

    public bool isFinite;
    public int amountResource;
    public bool canRespawn;
    public float respawnTime;
    public bool ejectsMaterialAutomatically;
    public float ejectMaterialAutomaticallyTime;
    public bool needsToolToBeMined;
    public bool ejectOnDepleted = false;
    public Item1 toolNeeded;


    public float health = 1;
    private float initialHealth = 1;

    bool isFinished;
    float respawnTimer = 0f;
    float ejectTimer = 0f;
    int initialAmount = 0;
    public int currentAmount;

    [SerializeField] GameObject hitVFX;

    private GameObject visual;
    private Collider col;

    public ToolStats.ToolType toolTypeNeeded;


    protected override void Start()
    {
        base.Start();
        initialAmount = amountResource;
        currentAmount = amountResource;
        visual = transform.GetChild(0).gameObject;
        col = GetComponent<Collider>();
        initialHealth = health;
    }

    void Update()
    {
        if(isFinished)
        {
            respawnTimer += Time.deltaTime;
            if(respawnTimer > respawnTime)
            {
                respawnTimer = 0f;
                isFinished = false;
                Respawn();
            }
        }

        if(ejectsMaterialAutomatically)
        {
            ejectTimer += Time.deltaTime;
            if(ejectTimer > ejectMaterialAutomaticallyTime)
            {
                ejectTimer = 0f;
                EjectMaterial(null);
            }
        }
    }

    public override void Interact(GameObject go)
    {
        if(needsToolToBeMined) {
            if (toolNeeded == null) {
                Debug.LogError("No tool found. You need to add a [toolNeeded] to " + gameObject.name);
                return;
            }
            if(go.GetComponent<TestCharacterController>().otherInventory.slots[0].item == null) {
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
        }
        else
        {
            health--;
        }
        

        if (hitVFX != null) {
            GameObject goVFX = Instantiate(hitVFX, transform.position, Quaternion.identity);
            Destroy(goVFX, 3f);
        }

        if (health < 0) {
            for(int i = 0; i < Mathf.Abs(health); i++) {
                EjectMaterial(go);
            }
            health = initialHealth;
        }
    }

    void EjectMaterial(GameObject player)
    {
        if (isFinite)
        {
            currentAmount--;
            if (currentAmount < 0)
            {
                OnResourceFinished(player);
            }
        }
        GlobalResourcesMined.Instance.UnlockResourceRobot(this);

        if(!ejectOnDepleted) {
            GameObject go = Instantiate(material, transform.position + new Vector3(Random.Range(-1.3f, 1.3f), 2f, Random.Range(-1.3f, 1.3f)), Quaternion.identity);
            if (player != null) player.GetComponent<TestCharacterController>().flyToMeItems.Add(go.transform);

            if(otherMaterials != null && otherMaterials.Length > 0) {
                for(int i = 0; i < otherMaterials.Length; i++) {
                    GameObject go2 = Instantiate(otherMaterials[i], transform.position + new Vector3(Random.Range(-1.3f, 1.3f), 2f, Random.Range(-1.3f, 1.3f)), Quaternion.identity);
                    if (player != null) player.GetComponent<TestCharacterController>().flyToMeItems.Add(go2.transform);
                }
            }
        }
    }

    void OnResourceFinished(GameObject player)
    {
        if(ejectOnDepleted)
        {
            int rand = Random.Range(1, 5);
            for(int i = 0; i < rand; i++)
            {
                GameObject go = Instantiate(material, transform.position + new Vector3(Random.Range(-1.3f, 1.3f), 2f, Random.Range(-1.3f, 1.3f)), Quaternion.identity);
                if (player != null) player.GetComponent<TestCharacterController>().flyToMeItems.Add(go.transform);
            }
        }

        visual.SetActive(false);
        col.enabled = false;

        if (!canRespawn) Destroy(this.gameObject);

        isFinished = true;
    }

    void Respawn()
    {
        visual.SetActive(true);
        col.enabled = true;
        currentAmount = initialAmount;
    }
}
