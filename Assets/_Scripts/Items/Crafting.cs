using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Crafting : MonoBehaviour
{
    [SerializeField] List<Item1> items;

    [SerializeField] public List<InventorySlot1> itemSlots;
    [SerializeField] Transform itemSlotsParent;
    [SerializeField] GameObject inventorySlotGO;

    [SerializeField] Transform recipeSlotsParent;
    [SerializeField] private List<InventorySlot1> recipeSlots;

    [SerializeField] Inventory1 inventory;
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text currentCraftAmount;

    Item1 selectedRecipeItem;

    [SerializeField] GameObject loadingBarParent;
    [SerializeField] Image loadingBar;
    [SerializeField] GameObject blockInput;

    public delegate void OnBuildingUpgrade(GameObject currentBuilding, GameObject upgradeBuilding);
    public static event OnBuildingUpgrade buildingUpgradeEvent;


    private void Awake()
    {
        loadingBar.fillAmount = 0f;

        itemSlots = new List<InventorySlot1>();

        for (int i = 0; i < items.Count; i++)
        {
            GameObject go = Instantiate(inventorySlotGO, itemSlotsParent);
            itemSlots.Add(go.GetComponent<InventorySlot1>());
            itemSlots[i].SetItemCrafting(items[i], 1);
            itemSlots[i].crafting = this;
        }

        if(TechTree.Instance != null)
            TechTree.Instance.UpdateTechs();
    }

    private void OnEnable()
    {
        InventorySlot1.ItemDropEvent += UpdateSlider;
    }

    private void OnDisable()
    {
        InventorySlot1.ItemDropEvent -= UpdateSlider;
    }

    // If click -> make and setup more recipe slots
    public void ShowRecipe(Item1 item)
    {
        selectedRecipeItem = item;

        if (recipeSlots != null)
        {
            for (int i = 0; i < recipeSlots.Count; i++)
            {
                Destroy(recipeSlots[i].gameObject);
            }
            recipeSlots.Clear();
        }

        recipeSlots = new List<InventorySlot1>();
        for (int i = 0; i < item.recipe.Count; i++)
        {
            GameObject go = Instantiate(inventorySlotGO, recipeSlotsParent);
            recipeSlots.Add(go.GetComponent<InventorySlot1>());
            recipeSlots[i].SetItemCrafting(item.recipe[i].material, item.recipe[i].amount);
            recipeSlots[i].isCrafting = false;
        }

        GlobalItemDescription.Instance.SetItemUI(item);

        UpdateSlider();
    }

    bool isCrafting = false;
    int itemsToCreate = 1;
    float recipeTime = 0f;
    float recipeTimer = 0f;
    bool canMakeMaterial = true;
    int maxItems = 99999;

    IEnumerator StartCrafting(List<MaterialAmount> recipeMaterials)
    {
        loadingBarParent.SetActive(true);
        blockInput.SetActive(true);
        do
        {
            recipeTimer += Time.deltaTime;
            loadingBar.fillAmount = recipeTimer / recipeTime;
            yield return null;
        } while (recipeTimer < recipeTime);

        recipeTimer = 0f;
        loadingBar.fillAmount = 0f;
        loadingBarParent.SetActive(false);
        blockInput.SetActive(false);

        for (int i = 0; i < slider.value; i++)
        {
            for (int j = 0; j < recipeMaterials.Count; j++)
            {
                recipeMaterials[j].Amount -= selectedRecipeItem.recipe[j].amount;
            }
        }

        // Destroy materials used from the inventory and add remaining materials from List<MaterialAmount> recipeMaterials
        // Dont allow or check continuosly if player takes off materials from the inventory
        for (int i = 0; i < inventory.slots.Count; i++)
        {
            for (int j = 0; j < recipeMaterials.Count; j++)
            {
                if (inventory.slots[i].item != null)
                {
                    if (inventory.slots[i].item.id == recipeMaterials[j].myitem.id)
                    {
                        inventory.slots[i].Clear();
                    }
                }
            }
        }

        for (int i = 0; i < recipeMaterials.Count; i++)
        {
            for (int j = 0; j < recipeMaterials[i].Amount; j++)
            {
                inventory.AddMaterial(recipeMaterials[i].myitem);
                //    GameObject go = Instantiate(GameItems.Instance.GetItemByID(recipeMaterials[i].myitem.id));
                //    go.SetActive(false);
            }
        }

        // Add items to inventory
        if (selectedRecipeItem.gameObject.GetComponent<Building>() == null)
        {
            for (int i = 0; i < slider.value; i++)
            {
                selectedRecipeItem.amount = 1;
                if(selectedRecipeItem.gameObject.GetComponent<ToolStats>() != null) {

                    GameObject go = Instantiate(GameItems.Instance.GetItemByID(selectedRecipeItem.id));
                    go.SetActive(false);
                    inventory.AddMaterial(go.GetComponent<Item1>());
                }
                else {
                    inventory.AddMaterial(selectedRecipeItem);
                }
            }
        }
        else if (!selectedRecipeItem.gameObject.GetComponent<Building>().isUpgrade)
        {
            for (int i = 0; i < slider.value; i++)
            {
                selectedRecipeItem.amount = 1;
                //    GameObject go = Instantiate(GameItems.Instance.GetItemByID(selectedRecipeItem.id));
                //    go.SetActive(false);
                inventory.AddMaterial(selectedRecipeItem);
            }
        }
        else
        {
            // IF UPGRADING, instantiate the new building on the position of the current, copy the inventories and delete the old one
            GameObject go = Instantiate(GameItems.Instance.GetItemByID(selectedRecipeItem.id), transform.parent.position, transform.parent.rotation);

            buildingUpgradeEvent?.Invoke(transform.parent.gameObject, go);

            go.transform.parent = transform.parent.parent;

            // FINISH THIS, CORRECTLY ITERATE THE SLOTS, INVENTORY SLOTS AMOUNT MIGHT NOT BE THE SAME
            for (int i = 0; i < inventory.slots.Count; i++)
            {
                if (inventory.slots[i].amount > 0 && go.GetComponent<Crafting>() != null)
                    go.GetComponent<Crafting>().inventory.slots[i].CopySlot(inventory.slots[i]);
            }

            Debug.Log("OBJ NAME: " + transform.parent.name);
            if (transform.parent.GetComponent<Building>() == null) Debug.Log("NULL 1");

            if (transform.parent.GetComponent<Building>().interactingPlayer == null) Debug.Log("No interacting player");

            if (go.transform.GetComponent<Building>() == null) Debug.Log("NULL 2");

            go.transform.GetComponent<Building>().Interact(transform.parent.GetComponent<Building>().interactingPlayer.gameObject);
            //transform.root.gameObject.SetActive(false);
            GlobalItemDescription.Instance.HideUI();

            Destroy(transform.parent.gameObject);
        }

        // Block the building of more than 1 researchbuilding
        TechTree.Instance.CheckResearchCenterCreated(selectedRecipeItem);

        UpdateSlider();
        yield return null;
    }


    public void Craft()
    {
        if (selectedRecipeItem == null) return;
        if (inventory.IsInventoryFull(selectedRecipeItem)) return;


        List<MaterialAmount> recipeMaterials = new List<MaterialAmount>();

        InitRecipeMaterials(recipeMaterials);

        recipeTime = selectedRecipeItem.craftingTime;
        // Crafts as many items as possible and adds them to the inventory

        if (canMakeMaterial)
            StartCoroutine(StartCrafting(recipeMaterials));
    }

    int GetMaxCrafts()
    {
        List<MaterialAmount> recipeMaterials = new List<MaterialAmount>();
        InitRecipeMaterials(recipeMaterials);
        return maxItems;
    }

    void InitRecipeMaterials(List<MaterialAmount> recipeMaterials)
    {
        canMakeMaterial = true;
        maxItems = 99999;

        for (int i = 0; i < recipeSlots.Count; i++)
        {
            MaterialAmount ma = new MaterialAmount();
            ma.myitem = recipeSlots[i].item;
            ma.Amount = 0;
            recipeMaterials.Add(ma);
        }

        // Get the amount of each material
        for (int i = 0; i < inventory.slots.Count; i++)
        {
            for (int j = 0; j < recipeMaterials.Count; j++)
            {
                if (inventory.slots[i].item != null)
                {
                    if (inventory.slots[i].item.id == recipeMaterials[j].myitem.id)
                    {
                        recipeMaterials[j].Amount = (recipeMaterials[j].Amount + inventory.slots[i].amount);
                    }
                }
            }
        }

        // Get the amount of each material
        // Check if enough materials, and how many we can produce in total
        for (int i = 0; i < recipeMaterials.Count; i++)
        {
            if (recipeMaterials[i].Amount < selectedRecipeItem.recipe[i].amount)
            {
                canMakeMaterial = false;
                Debug.Log("not enough materials on the inventory");
            }
            else
            {
                int maxAmount = recipeMaterials[i].Amount / selectedRecipeItem.recipe[i].amount;
                if (maxItems > maxAmount) maxItems = maxAmount;
                if (selectedRecipeItem.gameObject.GetComponent<Building>() != null && selectedRecipeItem.gameObject.GetComponent<Building>().isUpgrade) maxItems = 1;
            }
        }

        if (maxItems == 99999) maxItems = 0;
        if (!canMakeMaterial) maxItems = 0;
    }

    void UpdateSlider()
    {
        slider.maxValue = GetMaxCrafts();
        slider.value = 1;
        slider.minValue = 0;

        if (slider.value > slider.maxValue) slider.value = slider.maxValue;
    }

    public void UpdateTextOnSlider()
    {
        currentCraftAmount.text = ((int)slider.value).ToString();
    }
}

public class MaterialAmount
{
    public Item1 myitem { get; set; }
    public int Amount { get; set; }
}