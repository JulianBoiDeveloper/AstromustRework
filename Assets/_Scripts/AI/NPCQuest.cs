using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCQuest : MonoBehaviour, IInteractable
{
    /*
    public Transform target;
    public float moveSpeed = 5f;
    public TestCharacterController controller;

    [SerializeField] GameObject robotUI;
    [SerializeField] GameObject robotInventory;
    [SerializeField] GameObject resourcesMenu;*/
    [SerializeField] GameObject robotUI;
    [SerializeField] GameObject otherUI;
    [SerializeField] List<Item1> itemsNeededQuest;
    [SerializeField] Inventory1 inventoryItemsNeeded;
    [SerializeField] Inventory1 inventoryItemsReceived;

    private void Start()
    {
        for(int i = 0; i < itemsNeededQuest.Count; i++) {
            inventoryItemsNeeded.AddMaterial(itemsNeededQuest[i]);
        }
    }

    private void OnEnable()
    {
        GlobalResourcesMined.NewResourceMinedEvent += NewResourceMined;
    }

    private void OnDisable()
    {
        GlobalResourcesMined.NewResourceMinedEvent -= NewResourceMined;
    }

    void NewResourceMined()
    {

    }

    private void Update()
    {
        if (isInteracting && Input.GetKeyDown(KeyCode.Escape))
        {
            StopInteracting();
        }
    }

    public void OpenResourcesMenu()
    {
     //   resourcesMenu.SetActive(true);
        robotUI.SetActive(false);
    }

   public void CheckGivenItems()
    {
        bool allExist = true;
        foreach (Item1 obj in itemsNeededQuest)
        {
            if (!inventoryItemsReceived.HasItem(obj))
            {
                allExist = false;
                break;
            }
        }

        if (allExist)
        {
            Debug.Log("All objects in list1 exist in list2");
        }
        else
        {
            Debug.Log("Not all objects in list1 exist in list2");
        }
    }

    Transform interactingPlayer;
    bool isInteracting = false;

    public void Interact(GameObject go)
    {
    //    target = go.transform;
        robotUI.SetActive(true);
        otherUI.SetActive(true);
        interactingPlayer = go.transform;
        interactingPlayer.GetComponent<TestCharacterController>().canMove = false;
        interactingPlayer.GetComponent<TestCharacterController>().mainController.BlockMovement();
        isInteracting = true;
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
    }

    public void StopInteracting()
    {
        robotUI.SetActive(false);
        otherUI.SetActive(false);
    //    robotInventory.SetActive(false);
    //   resourcesMenu.SetActive(false);

        // maybe close more menus
        interactingPlayer.GetComponent<TestCharacterController>().canMove = true;
        interactingPlayer.GetComponent<TestCharacterController>().mainController.UnblockMovement();
        interactingPlayer = null;

        isInteracting = false;
    }

}