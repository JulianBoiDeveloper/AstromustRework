using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot1 : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [SerializeField] GameObject itemUI;
    public Item1 item = null;
    public int amount = 0;

    [SerializeField] public Image icon;
    [SerializeField] TMP_Text itemsAmount;

    public bool isCrafting = false;
    public bool isFurnace = false;

    public Inventory1 myInventory;
    public Crafting crafting;
    public bool isRobot = false;
    public int toolLevel;

    public delegate void ItemDropEventHandler();
    public static event ItemDropEventHandler ItemDropEvent;

    public bool wearableOnly = false;
    public Weareable.WeareableType weareableTypeAllowed;

    [HideInInspector] public bool isUpgradeSlot = false;


    public void SetItem(Item1 _item, int _amount = 1)
    {
        item = _item;

        if (!item.canBeStacked)
        {
            amount++;
            UpdateUI();
            return;
        }

        int toAdd = _item.amount;
        int canAdd = _item.maxAmountStack - amount;
        if (toAdd < canAdd)
        {
            amount += toAdd;
            UpdateUI();
            return;
        }
        else
        {
            amount += canAdd;
            UpdateUI();
            return;
        }
    }

    public void UpdateUI()
    {
        if (!isFurnace)
        {
            if (transform.childCount < 2 || item == null)
            {
                Instantiate(itemUI, transform);
            }
            icon = transform.GetChild(1).GetComponent<Image>();
        }
        else
        {
            if (transform.childCount < 3 || item == null)
            {
                Instantiate(itemUI, transform);
            }
            icon = transform.GetChild(2).GetComponent<Image>();
        }

        icon.sprite = item.icon;
        itemsAmount.text = amount.ToString();
    }

    // Used by crafting to set the amount needed for each
    public void SetAmountUI(int _amount)
    {
        itemsAmount.text = _amount.ToString();
    }

    public void SetItemCrafting(Item1 _item, int _amount)
    {
        amount = _amount;
        item = _item;
        UpdateUI();
    }

    public bool CanAdd(Item1 _item)
    {
        if (item == null) {
            //Debug.Log("Can ADD: Slot is empty");
            return true;
        }
        else if (item.id == _item.id && item.maxAmountStack > amount) {
            //Debug.Log("Can ADD: Slot has same id, amount: " + amount);
            return true;
        }
        else {
            //Debug.Log("CANT ADD WTF");
            return false;
        }
    }

    public bool IsFull()
    {
        if (item == null) {
            //Debug.Log("Slot is empty");
            return false;
        }
        //    if (!item.canBeStacked && amount >= 1) return true;
        if (item.maxAmountStack == amount) Debug.Log("IS FULL");

        return item.maxAmountStack == amount;
    }

    // Empty the inventory slot
    public void Clear()
    {
        item = null;
        icon.sprite = null;
        amount = 0;
        DestroyImmediate(icon.gameObject);
        itemsAmount.text = amount.ToString();
    }

    public void CopySlot(InventorySlot1 other)
    {
        item = other.item;
        amount = other.amount;
        Debug.Log("Copying tool level");
        toolLevel = other.toolLevel;

        UpdateUI();
        other.Clear();

        ItemDropEvent?.Invoke();

        if (isUpgradeSlot)
        {
            transform.root.GetComponent<ObjectUpgradeBuilding>().ShowRecipe(item);
        }
    }

    public void RemoveOne()
    {
        amount--;
        UpdateUI();
        if (amount <= 0)
            Clear();
    }

    public void OnDrop(PointerEventData eventData)
    {

        if(item == null || (item != null && eventData.pointerDrag.GetComponent<ItemDragHandler>() != null && eventData.pointerDrag.GetComponent<ItemDragHandler>().mySlot.item != null && eventData.pointerDrag.GetComponent<ItemDragHandler>().mySlot.item.id != item.id)) {
            if (!isFurnace && transform.childCount > 1) return; // already some item on this slot
            else if (isFurnace && transform.childCount > 2) return;
        
            GameObject dropped = eventData.pointerDrag;

            if (wearableOnly)
            {
                Weareable weareable = dropped.GetComponent<ItemDragHandler>().mySlot.item.gameObject.GetComponent<Weareable>();
                if (weareable == null) return;
                if (weareable.type != weareableTypeAllowed) return;
            }


            ItemDragHandler itemDragged = dropped.GetComponent<ItemDragHandler>();
            if (itemDragged == null || this == itemDragged.mySlot) return; 

            itemDragged.parentAfterDrag = transform;
            CopySlot(itemDragged.mySlot);
        }
        else // stacking
        {
            GameObject dropped = eventData.pointerDrag;

            if (wearableOnly)
            {
                Weareable weareable = dropped.GetComponent<ItemDragHandler>().mySlot.item.gameObject.GetComponent<Weareable>();
                if (weareable == null) return;
                if (weareable.type != weareableTypeAllowed) return;
            }


            ItemDragHandler itemDragged = dropped.GetComponent<ItemDragHandler>();
            if (itemDragged == null || this == itemDragged.mySlot) return; // droping back on the same slot

            if(itemDragged.mySlot.amount + amount <= item.maxAmountStack)
            {
                for(int i = 0; i < itemDragged.mySlot.amount; i++)
                {
                    SetItem(itemDragged.mySlot.item, 1);
                }
                itemDragged.mySlot.Clear();
            }
            else
            {
                int maxAmountDrop = item.maxAmountStack - amount;
                for(int i = 0; i < maxAmountDrop; i++)
                {
                    SetItem(itemDragged.mySlot.item, 1);
                }
                itemDragged.mySlot.Clear();

                for (int i = 0; i < maxAmountDrop; i++)
                {
                    itemDragged.mySlot.SetItem(item, 1);
                }
            }
        }
    // copy the data from the dragged slot into the new one
    // maybe data should be on the item itself..? and update it looking at the child item
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null) return;

        if (transform.parent.GetComponent<Inventory1>() != null && transform.parent.GetComponent<Inventory1>().canUseScroll)
        {
            transform.parent.GetComponent<Inventory1>().SetSlotSelectionToItem(item);


            if (item.GetComponent<Consumable1>() != null)
                transform.parent.GetComponent<Inventory1>().toConsume = item;
                
            if(item.GetComponent<Turret>() != null)
            {
                Debug.Log("pointer click!");
                transform.parent.GetComponent<Inventory1>().ItemDropUI();
            }

            transform.parent.GetComponent<Inventory1>().itemToDrop = item;
           // GlobalItemDescription.Instance.ShowMobileDropUI(item);

        }

        if (isCrafting)
        {
            for (int i = 0; i < eventData.pointerClick.transform.parent.childCount; i++)
            {
                eventData.pointerClick.transform.parent.GetChild(i).GetComponent<Image>().color = Color.white;
            }

            eventData.pointerClick.GetComponent<Image>().color = Color.cyan;
            crafting.ShowRecipe(item);
        }

        if(isRobot)
        {
            if (transform.GetChild(2).gameObject.activeInHierarchy) return;

            for (int i = 0; i < eventData.pointerClick.transform.parent.childCount; i++)
            {
                eventData.pointerClick.transform.parent.GetChild(i).GetComponent<Image>().color = Color.white;
            }

            eventData.pointerClick.GetComponent<Image>().color = Color.cyan;
            this.transform.root.GetComponent<AICharacters>().FindResource(item);
        }
    }
}


/*

 - 
 
     
*/
