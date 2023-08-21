using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchBuilding : MonoBehaviour
{
    Building building;
    TechTree techtree;
    GameObject player;

    bool isUpgrading = false;
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] GameObject otherMenu;
    [SerializeField] int levelToUnlock;

    [SerializeField] GameObject []robotsInChairs;

    void Start()
    {
        building = GetComponent<Building>();
        player = GameObject.FindGameObjectWithTag("Player");
        techtree = TechTree.Instance;

        techtree.Upgrade(levelToUnlock);
    }

    // Update is called once per frame
    void Update()
    {
        if(building.interacting && building.interactingPlayer.root.gameObject == player.transform.root.gameObject && (upgradeMenu == null || !upgradeMenu.activeInHierarchy))
        {
            techtree.canvas.enabled = true;
            techtree.canResearch = true;
        }
        else if(building.interacting && (upgradeMenu == null || !upgradeMenu.activeInHierarchy)) // if interecting is robot
        {
            techtree.canResearch = true;
            techtree.canvas.enabled = false;
        }
        else
        {
            techtree.canResearch = false;
            techtree.canvas.enabled = false;
        }

        for(int i = 0; i < robotsInChairs.Length; i++)
        {
            if(TechTree.Instance.isAssigned[i])
            {
                robotsInChairs[i].SetActive(true);
            }
            else
            {
                robotsInChairs[i].SetActive(false);
            }
        }
    } 

    public void Upgrade()
    {
        otherMenu.SetActive(false);
        upgradeMenu.SetActive(true);
        techtree.canvas.enabled = false;
    }

    public void GoBack()
    {
        otherMenu.SetActive(true);
        upgradeMenu.SetActive(false);
        techtree.canvas.enabled = true;
    }

    public void Close()
    {
        otherMenu.SetActive(true);
        upgradeMenu.SetActive(false);
        techtree.canvas.enabled = true;
    }
}
