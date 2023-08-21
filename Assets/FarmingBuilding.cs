using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FarmingBuilding : MonoBehaviour
{
    [SerializeField] Item1 water;

    [SerializeField] InventorySlot1 waterSlot;
    [SerializeField] List<InventorySlot1> seedSlots;

    [SerializeField] Image waterBar;
    [SerializeField] List<Image> seedGrowBars;

    [SerializeField] float waterGrowPerBucket = 0.3f;
    [SerializeField] List<Transform> plantPositions;

    TestCharacterController player;

    public GameObject[] plantsGrown;
    private bool[] isGrown;
    

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<TestCharacterController>();
        plantsGrown = new GameObject[3];
        isGrown = new bool[3];
        isGrown[0] = false;
        isGrown[1] = false;
        isGrown[2] = false;

    }

    private void Update()
    {
        Grow();
    }

    void Grow()
    {
        if (waterBar.fillAmount > 0) {
            for(int i = 0; i < seedSlots.Count; i++)
            {
                if(seedSlots[i].item != null && seedSlots[i].item.gameObject.GetComponent<Seed>() != null && seedGrowBars[i].fillAmount < 1f)
                {
                    Seed currentSeed = seedSlots[i].item.gameObject.GetComponent<Seed>();
                    seedGrowBars[i].fillAmount += currentSeed.growSpeed * Time.deltaTime;

                    if (plantsGrown[i] != currentSeed.plantGrowSteps[i])
                    {
                        if (plantsGrown[i] != null)
                            Destroy(plantsGrown[i].gameObject);

                        int stageIndex = Mathf.FloorToInt(seedGrowBars[i].fillAmount * (currentSeed.plantGrowSteps.Length - 1));
                        // Spawn the appropriate growth stage prefab at the spawn point
                        GameObject go = Instantiate(currentSeed.plantGrowSteps[stageIndex], plantPositions[i].position + new Vector3(0f, 0.25f, 0f), Quaternion.identity);
                        go.transform.parent = null;
                        plantsGrown[i] = go;
                    }

                    if(seedGrowBars[i].fillAmount >= 1f)
                    {
                        // can collect?
                        isGrown[i] = true;
                        seedSlots[i].RemoveOne();
                        Debug.Log("CanCollect");
                    }

                    // Reduce the water if there is any plant growing
                    if (waterBar.fillAmount > 0)
                    {
                        waterBar.fillAmount -= currentSeed.waterUsageRate * Time.deltaTime;
                    }
                }
            }
        }

        for (int i = 0; i < isGrown.Length; i++)
        {
            if(isGrown[i] && (plantsGrown[i] == null || !plantsGrown[i].activeInHierarchy)) {
                seedGrowBars[i].fillAmount = 0f;
                isGrown[i] = false;
                plantsGrown[i] = null;
            }
        }

        if (waterSlot.item != null && waterSlot.item.id == water.id && waterSlot.item.GetComponent<Consumable1>().canConsume)
        {
            Debug.Log("YEYEYE");
            Consumable1 waterConsumable = waterSlot.item.gameObject.GetComponent<Consumable1>();

            waterConsumable.currentConsumedLevel = waterConsumable.icons.Count;
            waterConsumable.ConsumeDrink();
            waterConsumable.canConsume = false;
            waterSlot.SetItem(waterSlot.item);
            waterBar.fillAmount += waterGrowPerBucket;
        }
    }


    GameObject[] assignedRobots;
    [SerializeField] public bool[] isAssigned;
    [SerializeField] TMP_Text[] assignedTexts;
    public void Assign(int id)
    {
        Debug.Log("Assign");
        if (player.assignedRobot != null && !isAssigned[id])
        {
            player.assignedRobot.target = player.currentOutlineInteraction.transform.root;
            //player.assignedRobot.SetActionFollow();
            player.assignedRobot.SetActionStop();
            player.assignedRobot.transform.position = player.assignedRobot.target.position - new Vector3(1.5f, 0f, -1.5f);
            isAssigned[id] = true;
            assignedTexts[id].text = player.assignedRobot.target.name;
            player.assignedRobot.gameObject.SetActive(false);
            assignedRobots[id] = player.assignedRobot.gameObject;
            player.assignedRobot = null;
        }
        else if (isAssigned[id])
        {
            isAssigned[id] = false;
            assignedTexts[id].text = "Assign";
            assignedRobots[id].SetActive(true);
        }
    }
}
