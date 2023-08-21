using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Furnace : MonoBehaviour
{
    public int nfuelSlots = 1;
    public int ninputSlots = 2;
    public int noutputSlots = 3;

    [SerializeField] public List<InventorySlot1> fuelSlots;
    [SerializeField] public List<InventorySlot1> inputSlots;
    [SerializeField] public List<InventorySlot1> outputSlots;

    [SerializeField] Transform fuelSlotsParent;
    [SerializeField] Transform inputSlotsParent;
    [SerializeField] Transform outputSlotsParent;

    [SerializeField] GameObject inventorySlotGO;
    [SerializeField] Inventory1 inventoryFuel;
    [SerializeField] Inventory1 inventoryInput;
    [SerializeField] Inventory1 inventoryOutput;

    float burnTimer = 0f;
    [SerializeField] float burnDuration = 10f;
    [SerializeField] bool isRunning = true;
    [SerializeField] TMP_Text turnOnOffText;

    [SerializeField] Item1 exceptionAlloy1;
    [SerializeField] Item1 exceptionAlloy2;
    [SerializeField] Item1 steelAlloy;

    [SerializeField] ParticleSystem fireVFX;

    private void OnEnable()
    {
        Crafting.buildingUpgradeEvent += BuildingUpgrade;
    }

    private void OnDisable()
    {
        Crafting.buildingUpgradeEvent -= BuildingUpgrade;
    }

    private void Awake()
    {
        fireVFX.Stop();
        turnOnOffText.text = "Turn ON";

        inventoryFuel.Init(nfuelSlots);
        inventoryInput.Init(ninputSlots);
        inventoryOutput.Init(noutputSlots);


        fuelSlots = new List<InventorySlot1>();
        inputSlots = new List<InventorySlot1>();
        outputSlots = new List<InventorySlot1>();

        for(int i = 0; i < fuelSlotsParent.childCount; i++)
        {
            fuelSlots.Add(fuelSlotsParent.GetChild(i).GetComponent<InventorySlot1>());
        }

        for (int i = 0; i < inputSlotsParent.childCount; i++)
        {
            inputSlots.Add(inputSlotsParent.GetChild(i).GetComponent<InventorySlot1>());
        }

        for (int i = 0; i < outputSlotsParent.childCount; i++)
        {
            outputSlots.Add(outputSlotsParent.GetChild(i).GetComponent<InventorySlot1>());
        }
    }

    private void Update()
    {
        Run();
    }

    void Run()
    {
        int outputfull = outputSlots.Count;
        int needToAdd = 0;

        if(isRunning)
        {
            // CHECK IF OUTPUT SLOTS ARE FULL
            // Check how many byproducts will be produced
            // Usually one per fuel + one per input
            // Exception: Allow, 1 total from multiple inputs

            // get outputs from inputs availables into output inventory
            // reduce input and fuel by 1
            bool hasItem1 = false;
            bool hasItem2 = false;

            for (int i = 0; i < inputSlots.Count; i++)
            {
                if (inputSlots[i].item != null)
                {
                    if (inputSlots[i].item.id == exceptionAlloy1.id)
                    {
                        hasItem1 = true;
                    }
                    if (inputSlots[i].item.id == exceptionAlloy2.id)
                    {
                        hasItem2 = true;
                    }
                }
            }

            // EXCEPTION
            if(hasItem1 && hasItem2)
            {
                if (inventoryOutput.IsInventoryFull(steelAlloy))
                {
                    outputfull--;
                }
                needToAdd++;

                for (int i = 0; i < fuelSlots.Count; i++)
                {
                    if (fuelSlots[i].item != null && fuelSlots[i].item.byProduct != null)
                    {
                        if (inventoryOutput.IsInventoryFull(fuelSlots[i].item.byProduct))
                        {
                            outputfull--;
                        }
                        needToAdd++;
                    }
                }
            }
            else {
                for (int i = 0; i < inputSlots.Count; i++)
                {
                    if(inputSlots[i].item != null && inputSlots[i].item.byProduct != null)
                    {
                        if (inventoryOutput.IsInventoryFull(inputSlots[i].item.byProduct)) {
                            outputfull--;
                        }
                        needToAdd++;
                    }
                }

                for (int i = 0; i < fuelSlots.Count; i++)
                {
                    if (fuelSlots[i].item != null && fuelSlots[i].item.byProduct != null)
                    {
                        if (inventoryOutput.IsInventoryFull(fuelSlots[i].item.byProduct))
                        {
                            outputfull--;
                        }
                        needToAdd++;
                    }
                }

                if (inputSlots[0].item != null && inputSlots[1].item != null)
                {
                    if (inputSlots[0].item.id == inputSlots[1].item.id) needToAdd--;
                }
            }

            // take 1 fuel
            if (HasFuel() && outputfull > needToAdd) {             
                burnTimer += Time.deltaTime;

                for(int i = 0; i < fuelSlots.Count; i++)
                {
                    if(fuelSlots[i].item != null)
                    {
                        fuelSlots[i].transform.GetChild(1).GetComponent<Image>().fillAmount = burnTimer / burnDuration;
                    }
                    else
                    {
                        fuelSlots[i].transform.GetChild(1).GetComponent<Image>().fillAmount = 0f;
                    }
                }
                for (int i = 0; i < inputSlots.Count; i++)
                {
                    if (inputSlots[i].item != null)
                    {
                        inputSlots[i].transform.GetChild(1).GetComponent<Image>().fillAmount = burnTimer / burnDuration;
                    }
                    else
                    {
                        inputSlots[i].transform.GetChild(1).GetComponent<Image>().fillAmount = 0f;
                    }
                }

                if (burnTimer > burnDuration)
                {
                    // If we have alloy exception...
                    if(hasItem1 && hasItem2) {
                        Debug.Log("Making alloy!!");
                        for (int i = 0; i < fuelSlots.Count; i++)
                        {
                            if (fuelSlots[i].amount > 0)
                            {
                                inventoryOutput.AddMaterial(fuelSlots[i].item.byProduct);
                                fuelSlots[i].RemoveOne();
                            }
                        }

                        for (int i = 0; i < inputSlots.Count; i++)
                        {
                            if (inputSlots[i].amount > 0)
                            {
                                inputSlots[i].RemoveOne();
                            }
                        }

                        inventoryOutput.AddMaterial(steelAlloy);
                    }
                    // Run normally, reducing 1 from fuel and inputs, and generating the outputs for all of them
                    else {
                        for (int i = 0; i < fuelSlots.Count; i++)
                        {
                            if (fuelSlots[i].amount > 0)
                            {
                                if (fuelSlots[i].item.byProduct != null)
                                {
                                    inventoryOutput.AddMaterial(fuelSlots[i].item.byProduct);
                                    fuelSlots[i].RemoveOne();
                                }
                            }
                        }

                        for (int i = 0; i < inputSlots.Count; i++)
                        {
                            if(inputSlots[i].amount > 0)
                            {
                                if(inputSlots[i].item.byProduct != null) {
                                    inventoryOutput.AddMaterial(inputSlots[i].item.byProduct);

                                    inputSlots[i].RemoveOne();
                                }
                            }
                        }
                    }
                    burnTimer = 0f;
                }
            }
        }
        else
        {
            burnTimer = 0f;
        }

        if(!HasFuel() || outputfull <= needToAdd)
        {
            isRunning = false;
            turnOnOffText.text = "Turn ON";
            fireVFX.Stop();

            for (int i = 0; i < fuelSlots.Count; i++)
            {
                fuelSlots[i].transform.GetChild(1).GetComponent<Image>().fillAmount = 0f;
            }
            for (int i = 0; i < inputSlots.Count; i++)
            {
                inputSlots[i].transform.GetChild(1).GetComponent<Image>().fillAmount = 0f;
            }
        }
        if(!GetComponent<Building>().interacting)
        {
            if(upgradeCanvas != null) upgradeCanvas.SetActive(false);
        }
    }

    bool HasFuel()
    {
        for (int i = 0; i < fuelSlots.Count; i++)
        {
            if(fuelSlots[i].item != null && fuelSlots[i].item.amount > 0)
            {
                return true;
            }
        }

        return false;
    }

    public void TurnOnOff()
    {
        if (!HasFuel() && !isRunning) return;

        isRunning = !isRunning;

        if (isRunning) {
            turnOnOffText.text = "Turn OFF";
            fireVFX.Play();
        }
        else {
            turnOnOffText.text = "Turn ON";
            fireVFX.Stop();

            for (int i = 0; i < fuelSlots.Count; i++)
            {
                fuelSlots[i].transform.GetChild(1).GetComponent<Image>().fillAmount = 0f;
            }
            for (int i = 0; i < inputSlots.Count; i++)
            {
                inputSlots[i].transform.GetChild(1).GetComponent<Image>().fillAmount = 0f;
            }
        }
    }

    [SerializeField] GameObject furnaceCanvas;
    [SerializeField] GameObject upgradeCanvas;

    public void Upgrade()
    {
        furnaceCanvas.SetActive(false);
        upgradeCanvas.SetActive(true);
    }

    public void GoBack()
    {
        furnaceCanvas.SetActive(true);
        upgradeCanvas.SetActive(false);
    }

    public void CopyInventories()
    {

    }

    void BuildingUpgrade(GameObject currentBuilding, GameObject upgradeBuilding)
    {
        if (currentBuilding != this.gameObject) return;

        Debug.Log("Upgrading building: " + gameObject.name);

        Furnace other = upgradeBuilding.GetComponent<Furnace>();

        // **Maybe the upgrade has a different number of slots in the future, will need fixing

        for (int i = 0; i < fuelSlots.Count; i++)
        {
            if (fuelSlots[i].amount > 0) other.fuelSlots[i].CopySlot(fuelSlots[i]);
        }
        for (int i = 0; i < inputSlots.Count; i++)
        {
            if (inputSlots[i].amount > 0) other.inputSlots[i].CopySlot(inputSlots[i]);
        }
        for (int i = 0; i < outputSlots.Count; i++)
        {
            if (outputSlots[i].amount > 0) other.outputSlots[i].CopySlot(outputSlots[i]);
        }
    }
}
