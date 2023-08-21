using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Item1
{
    [SerializeField] GameObject inventoryCanvas;
    [HideInInspector] public Transform interactingPlayer;
    [HideInInspector] public bool interacting = false;
    public Inventory1 inventory;

    public bool needsEnergy;
    public bool needsWorker;

    public bool isUpgrade = false;

    [SerializeField] List<GameObject> nonUpgradeMenus;
    [SerializeField] GameObject upgradeMenu;

    public enum BuildingType
    {
        storage,
        other
    }

    public BuildingType buildingType = BuildingType.other;

    protected override void Start()
    {
        base.Start();
        inventory = inventoryCanvas.GetComponentInChildren<Inventory1>();
    }

    void Update()
    {
        if(interacting)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                interacting = false;
                interactingPlayer.GetComponent<TestCharacterController>().canMove = true;
                interactingPlayer.GetComponent<TestCharacterController>().mainController.UnblockMovement();
                inventoryCanvas.SetActive(false);
                interactingPlayer = null;
             //   Cursor.lockState = CursorLockMode.Locked;
             //   Cursor.visible = false;

                if(nonUpgradeMenus != null && nonUpgradeMenus.Count > 0) {
                    for (int i = 0; i < nonUpgradeMenus.Count; i++)
                    {
                        nonUpgradeMenus[i].SetActive(false);
                    }
                }

                if(upgradeMenu != null)
                    upgradeMenu.SetActive(false);
            }
        }
    }

    public override void Interact(GameObject go)
    {
        interactingPlayer = go.transform;
        interactingPlayer.GetComponent<TestCharacterController>().canMove = false;
        interactingPlayer.GetComponent<TestCharacterController>().mainController.BlockMovement();
        inventoryCanvas.SetActive(true);
        interacting = true;
    //    Cursor.lockState = CursorLockMode.None;
    //    Cursor.visible = true;

        if (nonUpgradeMenus != null && nonUpgradeMenus.Count > 0)
        {
            for (int i = 0; i < nonUpgradeMenus.Count; i++)
            {
                nonUpgradeMenus[i].SetActive(true);
            }
        }

        if (upgradeMenu != null)
            upgradeMenu.SetActive(false);
    }

    public void Upgrade()
    {
        for(int i = 0; i < nonUpgradeMenus.Count; i++)
        {
            nonUpgradeMenus[i].SetActive(false);
        }

        upgradeMenu.SetActive(true);
    }

    public void GoBack()
    {
        for (int i = 0; i < nonUpgradeMenus.Count; i++)
        {
            nonUpgradeMenus[i].SetActive(true);
        }

        upgradeMenu.SetActive(false);
    }

    public void CloseInventory()
    {
        interacting = false;
        interactingPlayer.GetComponent<TestCharacterController>().canMove = true;
        interactingPlayer.GetComponent<TestCharacterController>().mainController.UnblockMovement();
        inventoryCanvas.SetActive(false);
        interactingPlayer = null;
    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;

        if (nonUpgradeMenus != null && nonUpgradeMenus.Count > 0)
        {
            for (int i = 0; i < nonUpgradeMenus.Count; i++)
            {
                nonUpgradeMenus[i].SetActive(false);
            }
        }

        GlobalItemDescription.Instance.HideUI();

        if (upgradeMenu != null)
            upgradeMenu.SetActive(false);
    }
}
