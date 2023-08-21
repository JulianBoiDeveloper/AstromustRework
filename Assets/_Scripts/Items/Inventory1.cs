using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory1 : MonoBehaviour
{
    [SerializeField] GameObject inventorySlotGO;

    [SerializeField] int slotsAmount;
    [SerializeField] bool generateSlotsOnRuntime = true;
    public List<InventorySlot1> slots;

    [SerializeField] List<UnityEngine.UI.Image> bgImages;
    [HideInInspector] public int selectedSlot = 0;
    [SerializeField] Transform player;
    [HideInInspector] public bool canUseScroll = false;
    bool isInit = false;

    [SerializeField] bool dropStarted = false;
    bool canDrop = false;
    GameObject ghostObj;
    public bool dropPressedUI = false;

    private void Start()
    {
        if(!isInit) {
            Init();
        }
    //    SelectSlot(0);
    }

    public void Init(int _slotAmount = -1)
    {
        if(_slotAmount != -1) slotsAmount = _slotAmount;

        if (isInit) return;

        isInit = true;
        bgImages = new List<UnityEngine.UI.Image>();

        if (generateSlotsOnRuntime)
        {
            for (int i = 0; i < slotsAmount; i++)
            {
                GameObject go = Instantiate(inventorySlotGO, this.transform);
                slots.Add(go.GetComponent<InventorySlot1>());
            }
        }

        for (int i = 0; i < slots.Count; i++)
        {
            bgImages.Add(slots[i].GetComponent<Image>());
        }
    }

    void Update()
    {
        // Update the selected slot based on mouse wheel input
        if(canUseScroll) {
            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
            if (mouseWheel > 0f)
            {
                SelectSlot(selectedSlot-1);
            }
            else if (mouseWheel < 0f)
            {
                SelectSlot(selectedSlot+1);
            }

            if(Input.GetKeyDown(KeyCode.C))
            {
                if(slots[selectedSlot].item != null && slots[selectedSlot].item.gameObject.GetComponent<Consumable1>() != null)
                {
                    Consumable1 consumable = slots[selectedSlot].item.gameObject.GetComponent<Consumable1>();
                    if (consumable.type == Consumable1.ConsumableType.drink) {
                        player.GetComponent<HungerSystem>().AddThrist(consumable.amount);
                    }
                    else if (consumable.type == Consumable1.ConsumableType.food)
                    {
                        player.GetComponent<HungerSystem>().AddHunger(consumable.amount);
                    }
                    else if (consumable.type == Consumable1.ConsumableType.potion)
                    {
                        player.GetComponent<HungerSystem>().AddHealth(consumable.amount);
                    }
                    slots[selectedSlot].RemoveOne();
                }
            }

        }
        
        if (Input.GetKeyDown(KeyCode.G) && canUseScroll && !dropStarted)
        {
            player.GetComponent<ThirdPersonControllerV2>().testCharController.AttachToolToCharacter();

            if (slots[selectedSlot].item != null && slots[selectedSlot].item.GetComponent<Turret>() != null) {
                DestroyGhostObject();

                Ray ray = new Ray(player.position + Camera.main.transform.forward * 4f + Vector3.up, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    // Get the collider bounds of the hit object
                    Collider collider = hit.collider;
                    Bounds bounds = collider.bounds;

                    // Calculate the target position by subtracting half of the object's height from the hit point
                    float yOffset = bounds.extents.y;
                    Vector3 targetPosition = hit.point - new Vector3(0f, yOffset, 0f);

                    ghostObj = Instantiate(GameItems.Instance.GetItemByID(slots[selectedSlot].item.id), targetPosition, Quaternion.identity);
                    CleanGameObject(ghostObj);
                    CheckBoundsCollision(ghostObj);
                    dropStarted = true;
                    Debug.Log("Drop started");
                }
            }
            else
            {
                DropItem();
            }
        }
        else if(dropStarted)
        {
            Debug.Log("Drop has started");
            Ray ray = new Ray(player.position + Camera.main.transform.forward * 4f + Vector3.up, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Get the collider bounds of the hit object
                Collider collider = hit.collider;
                Bounds bounds = collider.bounds;

                // Calculate the target position by subtracting half of the object's height from the hit point
                float yOffset = bounds.extents.y;
                Vector3 targetPosition = hit.point - new Vector3(0f, yOffset, 0f);
                ghostObj.transform.position = targetPosition;
            }
            canDrop = !CheckBoundsCollision(ghostObj);

            if(canDrop && Input.GetKeyDown(KeyCode.G) && !player.GetComponent<ThirdPersonControllerV2>().jumped) {
                Debug.Log("Drop 2 ended");
                DropItem();
                DestroyGhostObject();
            }
        }
    }

    void DestroyGhostObject()
    {
        if(ghostObj != null) ghostObj.transform.position = new Vector3(100f, 100f, -100f);
        Destroy(ghostObj);
        canDrop = false;
        dropStarted = false;
        Debug.Log("Destroyed ghost obj");
    }

    public void AddMaterial(Item1 material)
    {
        if (material.amount <= 0) {
            Debug.LogWarning("Material amount should be higher than 0. Check material settings on inspector");
            return;
        }

        StackMaterial(material);
        player.GetComponent<ThirdPersonControllerV2>().testCharController.AttachToolToCharacter(true);
    }

    public void StackMaterial(Item1 material)
    {
        for(int i = 0; i < slots.Count; i++)
        {
            if (slots[i].CanAdd(material))
            {
                slots[i].SetItem(material);
                break;
            }
            else
            {
                Debug.Log("Cant add material");
            }
        }
    }

    public bool IsInventoryFull(Item1 itemToAdd)
    {
        if (itemToAdd.amount <= 0)
        {
            Debug.LogWarning("Material amount should be higher than 0. Check material settings on inspector");
            return true;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].CanAdd(itemToAdd))
            {
                return false;
            }
            else
            {
                Debug.Log("Cant add material");
            }
        }
        return true;
    }

    public void DropMaterial(int slotIndex)
    {
        Debug.Log("Dropping");

        if (slots[slotIndex].item == null) return;

        GameObject go = Instantiate(GameItems.Instance.GetItemByID(slots[slotIndex].item.id), player.position + Camera.main.transform.forward * 4f + Vector3.up, Quaternion.identity);
        
        if(slots[slotIndex].item.GetComponent<ToolStats>() != null) {
            if(slots[slotIndex].item.GetComponent<ToolStats>().levelStats.Count > 0)
            {
                go.GetComponent<ToolStats>().SetLevel(slots[slotIndex].item.GetComponent<ToolStats>().currentLevel);
            }
        }

        if (slots[slotIndex].item.GetComponent<Turret>() != null)
        {
            Debug.Log("inside");
            Debug.Log("Current health: " + slots[slotIndex].item.GetComponent<Turret>().currentHealth);
            go.GetComponent<Turret>().SavePreviousHealth(slots[slotIndex].item.GetComponent<Turret>().currentHealth);
        }

        slots[slotIndex].amount--;
        slots[slotIndex].UpdateUI();
        if(slots[slotIndex].amount <= 0)
            slots[slotIndex].Clear();
    }

   [HideInInspector] public Item1 itemToDrop;

    public void DropItem()
    {
        player.GetComponent<ThirdPersonControllerV2>().testCharController.AttachToolToCharacter();

        if (!canUseScroll) return;
        /*
        int id = -1;
        for(int i = 0; i < slots.Count; i++)
        {
            if(slots[i].item == itemToDrop)
            {
                id = i;
                break;
            }
        }

        if (id == -1) return;*/

        if (slots[selectedSlot].item == null) return;

        //   SelectSlot(id);
        GameObject go = null;
        // If item is Turret or Gun
        if (slots[selectedSlot].item.GetComponent<ToolStats>() != null || slots[selectedSlot].item.GetComponent<Turret>() != null) {

            if(ghostObj == null) {
                go = PhotonNetwork.Instantiate(GameItems.Instance.GetItemByID(slots[selectedSlot].item.id).name, player.position + Camera.main.transform.forward * 4f + Vector3.up, Quaternion.identity);
            }
            else
            {
                go = PhotonNetwork.Instantiate(GameItems.Instance.GetItemByID(slots[selectedSlot].item.id).name, ghostObj.transform.position, Quaternion.identity);
            }
        }
        else
        {
            go = Instantiate(GameItems.Instance.GetItemByID(slots[selectedSlot].item.id), player.position + Camera.main.transform.forward * 4f + Vector3.up, Quaternion.identity);
        }

        // IF TOOL STATS, COPY LEVEL  OF THE ONE IN YOUR INVENTORY
        if (slots[selectedSlot].item.GetComponent<ToolStats>() != null)
        {
            if (slots[selectedSlot].item.GetComponent<ToolStats>().levelStats.Count > 0)
            {
                go.GetComponent<ToolStats>().SetLevel(slots[selectedSlot].item.GetComponent<ToolStats>().currentLevel);
            }
        }

        // IF CONSUMABLE, COPY STATUS OF CONSUMABLE IN YOUR INVENTORY
        if (slots[selectedSlot].item.GetComponent<Consumable1>() != null && slots[selectedSlot].item.GetComponent<Consumable1>().type == Consumable1.ConsumableType.drink)
        {
            go.GetComponent<Consumable1>().myItem = go.GetComponent<Item1>();
            go.GetComponent<Consumable1>().canConsume = slots[selectedSlot].item.GetComponent<Consumable1>().canConsume;
            go.GetComponent<Consumable1>().currentConsumedLevel = slots[selectedSlot].item.GetComponent<Consumable1>().currentConsumedLevel;
            go.GetComponent<Consumable1>().myItem.icon = go.GetComponent<Consumable1>().icons[go.GetComponent<Consumable1>().currentConsumedLevel];
        }

        if (slots[selectedSlot].item.GetComponent<Turret>() != null)
        {
            Debug.Log("inside");
            Debug.Log("Current health: " + slots[selectedSlot].item.GetComponent<Turret>().currentHealth);
            go.GetComponent<Turret>().SavePreviousHealth(slots[selectedSlot].item.GetComponent<Turret>().currentHealth);
        }

        slots[selectedSlot].amount--;
        slots[selectedSlot].UpdateUI();
        if (slots[selectedSlot].amount <= 0) {
            Debug.Log("Clear ");
            slots[selectedSlot].Clear();
            GlobalItemDescription.Instance.HideMobileDropUI();
        }
    }

    private bool CheckBoundsCollision(GameObject obj)
    {
        Collider[] colliders = Physics.OverlapBox(obj.GetComponent<Collider>().bounds.center, obj.GetComponent<Collider>().bounds.extents);
        // Check if any colliders overlap with the bounds
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.name != obj.name) // Ignore self-collision
            {
                Debug.Log("Collision with: " + collider.gameObject.name);
                SetMaterialsRecursively(obj.transform, GlobalItemDescription.Instance.cantBuildMaterial);
                return true; // Colliding with at least one collider
            }
        }

        SetMaterialsRecursively(obj.transform, GlobalItemDescription.Instance.canBuildMaterial);
        Debug.Log("No collision!");
        return false; // No collisions detected
    }

    private void SetMaterialsRecursively(Transform transform, UnityEngine.Material mat)
    {
        // Set materials on the MeshRenderer of the current object, if present
        MeshRenderer meshRenderer = transform.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            /*
            meshRenderer.materials.
            for(int i = 0; i < meshRenderer.materials.Length; i++) {
                meshRenderer.setma = mat;
            }*/

            Material[] materials = new Material[meshRenderer.sharedMaterials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = mat;
            }
            meshRenderer.sharedMaterials = materials;
        }

        // Recursively set materials on child objects
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            SetMaterialsRecursively(child, mat);
        }
    }

    public void CleanGameObject(GameObject gameObject)
    {
        // Remove all components except MeshRenderer and Collider
        Component[] components = gameObject.GetComponentsInChildren<Component>();
        foreach (Component component in components)
        {
            if (!(component is MeshRenderer) && !(component is Collider) && !(component is MeshFilter))
            {
                DestroyImmediate(component);
            }
            else if (component is Collider)
            {
                ((Collider)component).isTrigger = true;
            }
        }

        // Remove empty child GameObjects recursively
        CleanEmptyChildGameObjects(gameObject.transform);
    }

    private void CleanEmptyChildGameObjects(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            CleanEmptyChildGameObjects(child);

            // Remove empty child GameObject if it has no components (except Transform)
            if (child.childCount == 0 && child.GetComponents<Component>().Length == 1)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    public void SetSlotSelectionToItem(Item1 itemToSelect)
    {
        if (!canUseScroll) return;


        int id = -1;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == itemToSelect)
            {
                id = i;
                break;
            }
        }

        if (id == -1) return;
        SelectSlot(id);
    }

    public void ItemDropUI()
    {
        dropPressedUI = true;
        if (dropPressedUI && canUseScroll && !dropStarted)
        {
            player.GetComponent<ThirdPersonControllerV2>().testCharController.AttachToolToCharacter();

            if (slots[selectedSlot].item != null && slots[selectedSlot].item.GetComponent<Turret>() != null)
            {
                DestroyGhostObject();

                Ray ray = new Ray(player.position + Camera.main.transform.forward * 4f + Vector3.up, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    // Get the collider bounds of the hit object
                    Collider collider = hit.collider;
                    Bounds bounds = collider.bounds;

                    // Calculate the target position by subtracting half of the object's height from the hit point
                    float yOffset = bounds.extents.y;
                    Vector3 targetPosition = hit.point - new Vector3(0f, yOffset, 0f);

                    ghostObj = Instantiate(GameItems.Instance.GetItemByID(slots[selectedSlot].item.id), targetPosition, Quaternion.identity);
                    CleanGameObject(ghostObj);
                    CheckBoundsCollision(ghostObj);
                    dropStarted = true;
                    dropPressedUI = false;
                    Debug.Log("Drop startedUI");
                }
            }
            else
            {
                DropItem();
            }
        }
        else if (dropStarted)
        {
            Debug.Log("Drop has started");
            Ray ray = new Ray(player.position + Camera.main.transform.forward * 4f + Vector3.up, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Get the collider bounds of the hit object
                Collider collider = hit.collider;
                Bounds bounds = collider.bounds;

                // Calculate the target position by subtracting half of the object's height from the hit point
                float yOffset = bounds.extents.y;
                Vector3 targetPosition = hit.point - new Vector3(0f, yOffset, 0f);
                ghostObj.transform.position = targetPosition;
            }
            canDrop = !CheckBoundsCollision(ghostObj);

            if (canDrop && dropPressedUI && !player.GetComponent<ThirdPersonControllerV2>().jumped)
            {
                dropPressedUI = false;
                Debug.Log("Drop 2 ended");
                DropItem();
                DestroyGhostObject();
            }
        }
    }

    public void DropItemUI()
    {
        dropPressedUI = true;
        GlobalItemDescription.Instance.HideMobileDropUI();
    }

    [HideInInspector] public Item1 toConsume;

    public void Consume()
    {
        /*int id = -1;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == toConsume)
            {
                id = i;
                break;
            }
        }*/

        if (slots[selectedSlot].item != null && slots[selectedSlot].item.gameObject.GetComponent<Consumable1>() != null)
        {
            Consumable1 consumable = slots[selectedSlot].item.gameObject.GetComponent<Consumable1>();
            if (consumable.type == Consumable1.ConsumableType.drink && consumable.canConsume)
            {
                player.GetComponent<HungerSystem>().AddThrist(consumable.amount);
            }
            else if (consumable.type == Consumable1.ConsumableType.food)
            {
                player.GetComponent<HungerSystem>().AddHunger(consumable.amount);
            }
            else if (consumable.type == Consumable1.ConsumableType.potion)
            {
                player.GetComponent<HungerSystem>().AddHealth(consumable.amount);
            }
            if(consumable.type == Consumable1.ConsumableType.food)
                slots[selectedSlot].RemoveOne();
            else
            {
                consumable.ConsumeDrink();
                slots[selectedSlot].UpdateUI();
            }
        }

        if (slots[selectedSlot].amount <= 0) GlobalItemDescription.Instance.HideMobileDropUI();
    }

    public void Equip()
    {
        SelectSlot(0);
    //    player.GetComponent<ThirdPersonControllerV2>().testCharController.AttachToolToCharacter(true);
    }

    void SelectSlot(int index)
    {
        if (slots[index].item != null && slots[index].item.gameObject.GetComponent<Turret>() == null) DestroyGhostObject();
        
        bgImages[selectedSlot].color = Color.white;

        if (index < 0) index = slotsAmount - 1;
        else if (index >= slotsAmount) index = 0;

        bgImages[index].color = Color.red;
        selectedSlot = index;

        if (slots[index].item != null)
        {
            if (slots[index].item.gameObject.GetComponent<GunID>() != null)
            {
                player.GetComponent<ThirdPersonControllerV2>().testCharController.AttachToolToCharacter(true);
            }
            else
            {
                player.GetComponent<ThirdPersonControllerV2>().testCharController.AttachToolToCharacter();
            }
        }
        else
        {
            player.GetComponent<ThirdPersonControllerV2>().testCharController.AttachToolToCharacter();
        }

    }

    // Checks if we have an specific item on the inventory
    public bool HasItem(Item1 item)
    {
        for(int i = 0; i < slots.Count; i++)
        {
            if(slots[i].item != null) {
                if(slots[i].item.id == item.id)
                {
                    Debug.Log("WE HAVE REQUIRED TOOL");
                    return true;
                }
            }
        }
        return false;
    }

    public bool CanAdd(Item1 item)
    {
        for(int i = 0; i < slots.Count; i++)
        {
            if(slots[i].CanAdd(item))
            {
                return true;
            }
        }
        return false;
    }

    public void AddInventory(Inventory1 other)
    {
        for(int j = 0; j < other.slots.Count; j++)
        {
            int currentAmount = other.slots[j].amount;
            for (int k = 0; k < currentAmount; k++) {
                if(CanAdd(other.slots[j].item)) {
                    AddMaterial(other.slots[j].item);
                    other.slots[j].RemoveOne();
                }
            }
        }
    }
}
