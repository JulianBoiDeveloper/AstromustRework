using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TechTree : MonoBehaviour
{
    private static TechTree _instance;
    public static TechTree Instance { get { return _instance; } }

    [SerializeField] List<TechTreeButton> techTreeBranches;

    [HideInInspector] public List<TechTreeButton> queueToUnlock;

    public Canvas canvas;

    [SerializeField] Image loadingBar1;
    [SerializeField] Image loadingBar1Icon;
    [SerializeField] TMP_Text timeleft1;

    [SerializeField] Image loadingBar2Icon;
    [SerializeField] TMP_Text timeleft2;

    [SerializeField] Image loadingBar3Icon;
    [SerializeField] TMP_Text timeleft3;

    [SerializeField] GameObject[] loadingBars;
    [HideInInspector] public bool canResearch = false;

    [SerializeField] List<GameObject> level1Covers;
    [SerializeField] List<GameObject> level2Covers;
    [SerializeField] List<GameObject> level3Covers;

    public Item1[] researchCenters;
    public Item1 builtResearchCenter;
    public bool canBuildResearchCenter = true;


    int upgradeLevel = 0;

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

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
        queueToUnlock = new List<TechTreeButton>();
        assignedRobots = new GameObject[3];
        UpdateTechs();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            UpdateTechs();
        }

        if(canResearch || isAssigned[0] || isAssigned[1] || isAssigned[2]) {
            if(queueToUnlock.Count > 0 && queueToUnlock[0] != null)
            {
                timeleft1.text = (queueToUnlock[0].timeToUnlock - queueToUnlock[0].timer).ToString("F2");
                loadingBar1.fillAmount = queueToUnlock[0].timer / queueToUnlock[0].timeToUnlock;
             //   loadingBar1Icon.fillAmount = queueToUnlock[0].timeToUnlock / queueToUnlock[0].timer;
                queueToUnlock[0].Unlock();
            }
        }
    }

    public void CheckResearchCenterCreated(Item1 selectedItem)
    {
        // Block the building of more than 1 researchbuilding
        if (selectedItem.id == TechTree.Instance.builtResearchCenter.id)
        {
            TechTree.Instance.canBuildResearchCenter = false;
            TechTree.Instance.UpdateTechs();
        }
    }

    public void OnItemFinishedUnlock()
    {
        TechTree.Instance.UpdateTechs();
        UpdateLoadingBars();
        // clear up loading bars
    }

    public void AddItemQueue(TechTreeButton item)
    {
        queueToUnlock.Add(item);
        UpdateLoadingBars();
    }

    void UpdateLoadingBars()
    {
        if (queueToUnlock.Count > 0 && queueToUnlock[0] != null)
        {
            loadingBars[0].SetActive(true);
            loadingBar1Icon.sprite = queueToUnlock[0].slot.sprite;
            timeleft1.text = (queueToUnlock[0].timeToUnlock - queueToUnlock[0].timer).ToString();
            loadingBar1.fillAmount = queueToUnlock[0].timer / queueToUnlock[0].timeToUnlock;
        }
        else
        {
            loadingBars[0].SetActive(false);
        }


        if (queueToUnlock.Count > 1 && queueToUnlock[1] != null)
        {
            loadingBars[1].SetActive(true);
            loadingBar2Icon.sprite = queueToUnlock[1].slot.sprite;
            timeleft2.text = (queueToUnlock[1].timeToUnlock - queueToUnlock[1].timer).ToString();
        }
        else
        {
            loadingBars[1].SetActive(false);
        }

        if (queueToUnlock.Count > 2 && queueToUnlock[2] != null)
        {
            loadingBars[2].SetActive(true);
            loadingBar3Icon.sprite = queueToUnlock[2].slot.sprite;
            timeleft3.text = (queueToUnlock[2].timeToUnlock - queueToUnlock[2].timer).ToString();
        }
        else
        {
            loadingBars[2].SetActive(false);
        }
    }

    bool IsItemInTechTree(string _itemID)
    {
        for (int i = 0; i < techTreeBranches.Count; i++)
        {
            if (_itemID == techTreeBranches[i].techID)
            {
                if (techTreeBranches[i].isResearched) return false;
                else return true;
            }
        }
        return false;
    }

    public void UpdateTechs()
    {
        // Find all GameObjects with SomeScript attached
        Crafting[] scripts = FindObjectsOfType<Crafting>();

        // Iterate through the found scripts
        foreach (Crafting script in scripts)
        {
            // Do something with each script
            Debug.Log("Found script attached to GameObject: " + script.gameObject.name);
            foreach (InventorySlot1 inventorySlot in script.itemSlots)
            {
                if (IsItemInTechTree(inventorySlot.item.id))
                {
                    inventorySlot.gameObject.SetActive(false);
                }
                else if(inventorySlot.item.id != builtResearchCenter.id || (inventorySlot.item.id == builtResearchCenter.id && canBuildResearchCenter))
                {
                    inventorySlot.gameObject.SetActive(true);
                }
                else
                {
                    inventorySlot.gameObject.SetActive(false);
                }
            }
        }
    }

    public void Upgrade(int levelUpgrade)
    {
        if(levelUpgrade > upgradeLevel) {
            upgradeLevel = levelUpgrade;
            if(upgradeLevel > 2)
            {
                for(int i = 0; i < level3Covers.Count; i++)
                {
                    level3Covers[i].SetActive(false);
                }
            }
            if (upgradeLevel > 1)
            {
                for (int i = 0; i < level2Covers.Count; i++)
                {
                    level2Covers[i].SetActive(false);
                }
            }
            if (upgradeLevel > 0)
            {
                for (int i = 0; i < level1Covers.Count; i++)
                {
                    level1Covers[i].SetActive(false);
                }
            }
        }
    }

    GameObject[] assignedRobots;
    [SerializeField] public bool[] isAssigned;
    [SerializeField] TMP_Text[] assignedTexts;
    public void Assign(int id)
    {
        Debug.Log("Assign");
        TestCharacterController player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<TestCharacterController>();
        if(player.assignedRobot != null && !isAssigned[id])
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
        else if(isAssigned[id])
        {
            isAssigned[id] = false;
            assignedTexts[id].text = "Assign";
            assignedRobots[id].SetActive(true);
        }
    }
}
