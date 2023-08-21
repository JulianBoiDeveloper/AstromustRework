using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using EasyBuildSystem.Packages.Addons.CircularBuildingMenu;
public class GlobalItemDescription : MonoBehaviour
{
    private static GlobalItemDescription _instance;

    public static GlobalItemDescription Instance { get { return _instance; } }

    [SerializeField] GameObject parent;
    [SerializeField] Image icon;
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text itemDescription;

    [SerializeField] GameObject mobileDropConsumeUI;
    [SerializeField] TMP_Text itemName2;
    [SerializeField] TMP_Text itemDescription2;
    [SerializeField] GameObject mobileConsumeButton;
    [SerializeField] GameObject mobileEquipButton;


    [SerializeField] GameObject buildingMenu;
    [SerializeField] Inventory1 buildingMenuCrafting;
    [SerializeField] UICircularBuildingMenu circularMenu;
    [HideInInspector] public string buildingName;

    public bool isBuilding = false;

    public Material canBuildMaterial;
    public Material cantBuildMaterial;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void SetItemUI(Item1 item)
    {
        icon.sprite = item.icon;
        itemName.text = item.itemName;
        itemDescription.text = item.itemDescription;
        parent.SetActive(true);
    }

    public void HideUI()
    {
        parent.SetActive(false);
        mobileDropConsumeUI.SetActive(false);
    }

    public void ShowMobileDropUI(Item1 item)
    {
        if (item.GetComponent<Consumable1>() != null) mobileConsumeButton.SetActive(true);
        else mobileConsumeButton.SetActive(false);

        if(item.GetComponent<GunID>() != null) mobileEquipButton.SetActive(true);
        else mobileEquipButton.SetActive(false);

        itemName2.text = item.itemName;
        itemDescription2.text = item.itemDescription;
        mobileDropConsumeUI.transform.root.GetComponent<ThirdPersonControllerV2>().BlockMovement();
        mobileDropConsumeUI.SetActive(true);
    }

    public void HideMobileDropUI()
    {
        mobileDropConsumeUI.transform.root.GetComponent<ThirdPersonControllerV2>().UnblockMovement();
        mobileDropConsumeUI.SetActive(false);
    }

    public void ShowBuildingMenu()
    {
        buildingMenu.SetActive(true);
    }

    public void ShowRecipe(string _buildingName, GameObject buildingPart)
    {
        ShowBuildingMenu();
        buildingName = _buildingName;
        BuildingMaterials bm = buildingPart.GetComponent<BuildingMaterials>();
        if(bm != null)
        {
            for(int i = 0; i < bm.recipe.Count; i++)
            {
                buildingMenuCrafting.slots[i].SetItem(bm.recipe[i].material);
                buildingMenuCrafting.slots[i].amount = bm.recipe[i].amount;
                buildingMenuCrafting.slots[i].UpdateUI();
            }
        }
        else {
            for(int i = 0; i < buildingMenuCrafting.slots.Count; i++)
            {
                if(buildingMenuCrafting.slots[i].item != null)
                    buildingMenuCrafting.slots[i].Clear();
            }
        }

    }

    public void SelectBuildingPart()
    {
        circularMenu.SelectBuildingPartCraft(buildingName);
        buildingMenu.SetActive(false);
    }

    public void SetBuildingMode(bool enable)
    {
        isBuilding = enable;
    }
}