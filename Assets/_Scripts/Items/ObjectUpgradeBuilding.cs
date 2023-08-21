using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectUpgradeBuilding : MonoBehaviour
{
    [SerializeField] public InventorySlot1 slot;
    [SerializeField] List<Item1> items;

    [SerializeField] public List<InventorySlot1> itemSlots;
    [SerializeField] Transform itemSlotsParent;
    [SerializeField] GameObject inventorySlotGO;

    [SerializeField] Transform recipeSlotsParent;
    [SerializeField] private List<InventorySlot1> recipeSlots;

    [SerializeField] Inventory1 inventory;

    Item1 selectedRecipeItem;
    [SerializeField] GameObject loadingBarParent;
    [SerializeField] Image loadingBar;
    [SerializeField] GameObject blockInput;

    private void Awake()
    {
        inventory.Init();
        loadingBar.fillAmount = 0f;
    }

    private void Start()
    {
        slot.isUpgradeSlot = true;
        itemSlots = new List<InventorySlot1>();

        for (int i = 0; i < items.Count; i++)
        {
            GameObject go = Instantiate(inventorySlotGO, itemSlotsParent);
            itemSlots.Add(go.GetComponent<InventorySlot1>());
            itemSlots[i].SetItemCrafting(items[i], 1);
        }
    }

    public void UpgradeTool()
    {
        if(slot.item != null && slot.item.GetComponent<ToolStats>() != null)
        {
            if(slot.item.GetComponent<ToolStats>().levelStats.Count > 0 && GetMaxCraftsUpgrade() > 0 && canMakeMaterial && !slot.item.GetComponent<ToolStats>().IsMaxLevel())
            {
                CraftUpgrade();
                slot.item.GetComponent<ToolStats>().IncreaseLevel();
                RefreshSlot();
            }
        }
    }

    public void RefreshSlot()
    {
        slot.icon.sprite = slot.item.icon;
    }

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

        for (int i = 0; i < item.upgradeRecipe.Count; i++)
        {
            GameObject go = Instantiate(inventorySlotGO, recipeSlotsParent);
            recipeSlots.Add(go.GetComponent<InventorySlot1>());
            recipeSlots[i].SetItemCrafting(item.upgradeRecipe[i].material, item.upgradeRecipe[i].amount);
            recipeSlots[i].isCrafting = false;
        }
    }

    float recipeTime = 0f;
    float recipeTimer = 0f;

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

        for (int j = 0; j < recipeMaterials.Count; j++)
        {
            recipeMaterials[j].Amount -= selectedRecipeItem.upgradeRecipe[j].amount;
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
                GameObject go = Instantiate(GameItems.Instance.GetItemByID(recipeMaterials[i].myitem.id));
                go.SetActive(false);
            }
        }

        // Add items to inventory
        if (selectedRecipeItem.gameObject.GetComponent<Building>() == null)
        {
            GetComponent<ObjectUpgradeBuilding>().RefreshSlot();
        }
        else if (!selectedRecipeItem.gameObject.GetComponent<Building>().isUpgrade)
        {
            selectedRecipeItem.amount = 1;
            GameObject go = Instantiate(GameItems.Instance.GetItemByID(selectedRecipeItem.id));
            go.SetActive(false);
            inventory.AddMaterial(go.GetComponent<Item1>());
        }
        else
        {
            // Copy inventory after upgrading the building
            GameObject go = Instantiate(GameItems.Instance.GetItemByID(selectedRecipeItem.id), transform.position, transform.rotation);

            // FINISH THIS, CORRECTLY ITERATE THE SLOTS, INVENTORY SLOTS AMOUNT MIGHT NOT BE THE SAME
            for (int i = 0; i < inventory.slots.Count; i++)
            {
                if (inventory.slots[i].amount > 0)
                    go.GetComponent<ObjectUpgradeBuilding>().inventory.slots[i].CopySlot(inventory.slots[i]);
            }

            go.GetComponent<Building>().Interact(GetComponent<Building>().interactingPlayer.gameObject);
            Destroy(this.gameObject);
        }

        yield return null;
    }

    bool canMakeMaterial = true;
    int maxItems = 99999;


    public void CraftUpgrade()
    {
        List<MaterialAmount> recipeMaterials = new List<MaterialAmount>();

        InitRecipeMaterials(recipeMaterials);

        recipeTime = selectedRecipeItem.craftingTime;
        // Crafts as many items as possible and adds them to the inventory

        if (canMakeMaterial)
            StartCoroutine(StartCrafting(recipeMaterials));
    }

    public int GetMaxCraftsUpgrade()
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
            if (recipeMaterials[i].Amount < selectedRecipeItem.upgradeRecipe[i].amount)
            {
                canMakeMaterial = false;
                Debug.Log("not enough materials on the inventory");
            }
            else
            {
                int maxAmount = recipeMaterials[i].Amount / selectedRecipeItem.upgradeRecipe[i].amount;
                if (maxItems > maxAmount) maxItems = maxAmount;
                if (selectedRecipeItem.gameObject.GetComponent<Building>() != null && selectedRecipeItem.gameObject.GetComponent<Building>().isUpgrade) maxItems = 1;
            }
        }

        if (maxItems == 99999) maxItems = 0;
        if (!canMakeMaterial) maxItems = 0;
    }

    public class MaterialAmount
    {
        public Item1 myitem { get; set; }
        public int Amount { get; set; }
    }
}
