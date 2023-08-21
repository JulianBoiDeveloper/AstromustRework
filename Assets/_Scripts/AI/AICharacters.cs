using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacters : MonoBehaviour, IInteractable
{
    public Transform target;
    private Rigidbody enemyRigidbody;
    public float moveSpeed = 5f;
    public float chaseRadius = 10f;
    public float minDistance = 3f;
    public TestCharacterController controller;
    [SerializeField] Animator animator;

    [SerializeField] GameObject robotUI;
    [SerializeField] GameObject robotInventory;
    [SerializeField] GameObject resourcesMenu;

    public enum RobotAction
    {
        gather,
        stop,
        follow,
        storage
    }

    public RobotAction robotAction = RobotAction.stop;
    public RobotAction secondaryAction = RobotAction.stop;


    private void OnEnable()
    {
        GlobalResourcesMined.NewResourceMinedEvent += NewResourceMined
;
    }

    private void OnDisable()
    {
        GlobalResourcesMined.NewResourceMinedEvent -= NewResourceMined;
    }

    void NewResourceMined()
    {
        for(int i = 0; i < GlobalResourcesMined.Instance.allResourcesIDs.Count; i++)
        {
            for(int j = 0; j < resourcesSlots.Count; j++)
            {
                if(resourcesSlots[j].item.id == GlobalResourcesMined.Instance.allResourcesIDs[i])
                {
                    resourcesSlots[j].transform.GetChild(2).gameObject.SetActive(false);
                }
            }
        }
    }
    private void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody>();
        LoadResources();
    }

    private void Update()
    {
        if(isInteracting && Input.GetKeyDown(KeyCode.Escape)) {
            StopInteracting();
        }

        if(robotAction == RobotAction.follow) {
            Follow(target);
        }
        else if(robotAction == RobotAction.stop)
        {
            Stop();
        }
        else if (robotAction == RobotAction.gather)
        {
            Gather();
        }
        else if (robotAction == RobotAction.stop)
        {
            Stop();
        }

        if(robotAction != RobotAction.gather)
        {
            //transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.I)) FindStorage();
    }

    void Follow(Transform followTarget)
    {
        float distanceToTarget = Vector3.Distance(transform.position, followTarget.position);

        if (distanceToTarget <= chaseRadius && distanceToTarget > minDistance)
        {
            animator.Play("RobotWalkAnim2");
            // Play move animation
            Vector3 direction = (followTarget.position - transform.position).normalized;
            direction.y = 0f;
            enemyRigidbody.constraints = RigidbodyConstraints.None;
            enemyRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            enemyRigidbody.velocity = direction * moveSpeed;
            enemyRigidbody.angularVelocity = Vector3.zero;
            // Look at the target
            transform.LookAt(followTarget);
            Quaternion currentRotation = transform.rotation;
            // Reset the X rotation to zero
            Quaternion newRotation = Quaternion.Euler(0f, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z);
            // Apply the new rotation to the GameObject
            transform.rotation = newRotation;
        }
        else
        {
            Stop();
            transform.LookAt(followTarget);
            Quaternion currentRotation = transform.rotation;
            // Reset the X rotation to zero
            Quaternion newRotation = Quaternion.Euler(0f, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z);
            // Apply the new rotation to the GameObject
            transform.rotation = newRotation;
        }
    }

    void Stop()
    {
        animator.Play("RobotIdle");
        enemyRigidbody.velocity = Vector3.zero;
        enemyRigidbody.angularVelocity = Vector3.zero;
        // Play Idle animation
    }

    [SerializeField] Item1 gatheringItem;
    [SerializeField] Transform targetResource;
    [SerializeField] Transform targetStorage;

    Building[] buildingsInScene;
    bool dropItemsAttempted = false;

    public void OpenGatherMenu()
    {

    }

    float startGatherRotation = 0f;
    bool doOnce = false;

    public void Gather()
    {
        // Find target resource
        // target = FindClosestResource();
        if(secondaryAction == RobotAction.gather) {
            if (Vector3.Distance(targetResource.position, transform.position) < minDistance)
            {
                if (targetResource.GetComponent<Resource>() != null)
                {
                    if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "RobotMine") {
                        Quaternion currentRotation = transform.rotation;
                        Quaternion newRotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y + 37f, currentRotation.eulerAngles.z);
                        transform.rotation = newRotation;
                        enemyRigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; ;
                    }

                    animator.Play("RobotMine");

                    gatherTimer += Time.deltaTime;
                    if (gatherTimer > gatherDuration)
                    {
                        targetResource.GetComponent<Resource>().Interact(this.gameObject);
                        if (targetResource.GetComponent<Resource>().currentAmount < 0) FindResource(toFind);
                        if (toFind == null) secondaryAction = RobotAction.storage;
                        gatherTimer = 0f;


                    }
                }
            }
            else
            {
                Follow(targetResource);
            }
        }

        // If inventory is full -> Go to storage
        if (secondaryAction != RobotAction.storage && controller.inventory.IsInventoryFull(targetResource.GetComponent<Resource>().material.GetComponent<Item1>()))
        {
            gatheringItem = targetResource.GetComponent<Resource>().material.GetComponent<Item1>();

            targetStorage = FindStorage();
            // target = FindClosestStorage();
            Debug.Log("Robot inventory is full");
            secondaryAction = RobotAction.storage;
        }

        if(secondaryAction == RobotAction.storage)
        {
            if(targetStorage == null)
            {
                robotAction = RobotAction.stop;
                secondaryAction = RobotAction.stop;
                return;
            }

            Follow(targetStorage);

            if (Vector3.Distance(targetStorage.position, transform.position) < minDistance)
            {
                InteractStorage();

                // If the inventory we tried to drop to is full -> go back to gathering
                if(dropItemsAttempted) {
                    if (!controller.inventory.IsInventoryFull(gatheringItem))
                    {
                        secondaryAction = RobotAction.gather;
                    }
                    else if(targetStorage.GetComponent<Building>().inventory.IsInventoryFull(gatheringItem))
                    {
                        // If both our inventory and the storage inventory are full, stop gathering
                        Debug.Log("INVENTORIES ARE FULL");
                        robotAction = RobotAction.stop;
                        secondaryAction = RobotAction.stop;
                    }
                    else
                    {
                        robotAction = RobotAction.stop;
                        secondaryAction = RobotAction.stop;
                    }
                }
            }
        }
    }

    public Transform FindStorage()
    {
        buildingsInScene = GameObject.FindObjectsByType<Building>(FindObjectsSortMode.None);

        Building closest = null;

        float dist = 9999f;

        for(int i = 0; i < buildingsInScene.Length; i++)
        {
            if(buildingsInScene[i].buildingType == Building.BuildingType.storage && buildingsInScene[i].inventory.CanAdd(targetResource.GetComponent<Resource>().material.GetComponent<Item1>())) {
                float newDist = Vector3.Distance(buildingsInScene[i].transform.position, transform.position);
                if (newDist < dist)
                {
                    closest = buildingsInScene[i];
                    dist = newDist;
                }
            }
        }


        if(closest != null) {
            Debug.Log("Closest storage: " + closest.gameObject.name);
            return closest.transform;
        }

        Debug.Log("No storage found");
        return null;
    }

    public void InteractStorage()
    {
        if (targetStorage.GetComponent<Building>() != null)
        {
            // Drop all your items in some storage

            Debug.Log("Storage isnt null");

            if (targetStorage.GetComponent<Building>().buildingType == Building.BuildingType.storage)
            {

                Debug.Log("Is storage type");
                dropItemsAttempted = true;
                Building targetBuilding = targetStorage.GetComponent<Building>();
                
                // int i = 1 (to skip the pickaxe slot, or move that slot somewhere else to a different inventory
                targetBuilding.inventory.AddInventory(controller.inventory);
            }
        }
    }


    float gatherTimer = 0f;
    [SerializeField] float gatherDuration = 0.5f;

    void Interactions()
    {
        if(target.GetComponent<IInteractable>() != null)
        {
            if(targetResource.GetComponent<Resource>() != null)
            {
                gatherTimer += Time.deltaTime;
                if (gatherTimer > gatherDuration)
                {
                    targetResource.GetComponent<Resource>().Interact(this.gameObject);
                    gatherTimer = 0f;


                }
                //controller.Interact(target.gameObject);
            }
            /*
            else if(targetStorage.GetComponent<Building>() != null)
            {
                // Drop all your items in some storage

                Debug.Log("Storage isnt null");

                if(targetStorage.GetComponent<Building>().buildingType == Building.BuildingType.storage)
                {

                    Debug.Log("Is storage type");
                    dropItemsAttempted = true;
                    Building targetBuilding = targetStorage.GetComponent<Building>();

                    for (int i = 0; i < controller.inventory.slots.Count; i++)
                    {
                        if(targetBuilding.inventory.CanAdd(controller.inventory.slots[i].item))
                        {
                            Debug.Log("Addingggg");
                            for(int j = 0; j < controller.inventory.slots[i].item.amount; j++) {
                                targetBuilding.inventory.AddMaterial(controller.inventory.slots[i].item);
                                controller.inventory.slots[i].RemoveOne();
                            }
                        }
                    }
                }
            }*/
            else if(target.GetComponent<Building>() != null)
            {
                // target.GetComponent<Building>().AssignRobot(this);
            }
        }
    }

    public void SetActionFollow() {
        robotAction = RobotAction.follow;
    }
    public void SetActionStop()
    {
        robotAction = RobotAction.stop;
        secondaryAction = RobotAction.stop;
    }
    public void SetActionGather()
    {
        if (targetResource == null) return;

        if(GetComponent<TestCharacterController>().otherInventory.slots[0].item != null) { // only if we have a tool
            robotAction = RobotAction.gather;
            secondaryAction = RobotAction.gather;
        }
        else
        {
            Debug.LogWarning("ROBOT NEEDS TOOL BEFORE GATHER");
        }
    }
    public void OpenRobotStorage()
    {
        robotInventory.SetActive(true);
        robotUI.SetActive(false);
    }

    public void OpenResourcesMenu()
    {
        resourcesMenu.SetActive(true);
        robotUI.SetActive(false);
    }

    Transform interactingPlayer;
    bool isInteracting = false;

    public void Interact(GameObject go)
    {
        target = go.transform;
        robotUI.SetActive(true);
        interactingPlayer = go.transform;
        interactingPlayer.GetComponent<TestCharacterController>().canMove = false;
        interactingPlayer.GetComponent<TestCharacterController>().mainController.BlockMovement();
        isInteracting = true;
    //    Cursor.lockState = CursorLockMode.None;
    //    Cursor.visible = true;
    }

    void StopInteracting()
    {
        robotUI.SetActive(false);
        robotInventory.SetActive(false);
        resourcesMenu.SetActive(false);

        // maybe close more menus

        interactingPlayer.GetComponent<TestCharacterController>().canMove = true;
        interactingPlayer.GetComponent<TestCharacterController>().mainController.UnblockMovement();
        interactingPlayer = null;

        isInteracting = false;
    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;
    }

    [SerializeField] List<Item1> resources;
    [SerializeField] List<InventorySlot1> resourcesSlots;
    [SerializeField] GameObject resourceSlotGO;
    [SerializeField] Transform resourceSlotsParent;

    void LoadResources()
    {
   //     resourcesSlots = new List<InventorySlot1>();
        for (int i = 0; i < resources.Count; i++)
        {
        //    GameObject go = Instantiate(resourceSlotGO, resourceSlotsParent);
       //     resourcesSlots.Add(go.GetComponent<InventorySlot1>());
            resourcesSlots[i].SetItemCrafting(resources[i], resources[i].amount);
            resourcesSlots[i].isRobot = true;
        }
    }

    public Resource[] resourcesInScene;
    private Item1 toFind;
    public Transform FindResource(Item1 resourceToFind)
    {
        toFind = resourceToFind;
        resourcesInScene = GameObject.FindObjectsByType<Resource>(FindObjectsSortMode.None);

        Resource closest = null;

        float dist = 9999f;

        for (int i = 0; i < resourcesInScene.Length; i++)
        {
            if (resourcesInScene[i].id == resourceToFind.id && resourcesInScene[i].currentAmount >= 0)
            {
                float newDist = Vector3.Distance(resourcesInScene[i].transform.position, transform.position);
                if (newDist < dist)
                {
                    closest = resourcesInScene[i];
                    dist = newDist;
                }
            }
        }


        if (closest != null)
        {
            Debug.Log("Closest resource: " + closest.gameObject.name);
            targetResource = closest.transform;
            target = closest.transform;
            gatheringItem = targetResource.GetComponent<Resource>().material.GetComponent<Item1>();
            return closest.transform;
        }

        Debug.Log("No resource found");
        SetActionStop();
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject.GetComponent<IInteractable>() != null)
        {
            if (gatheringItem == null || other.transform.root.GetComponent<Item1>().id != gatheringItem.id) return;

            if (other.transform.root.gameObject.GetComponent<Item1>() != null && other.transform.root.gameObject.GetComponent<Crafting>() == null
                && other.transform.root.gameObject.GetComponent<Resource>() == null && other.transform.root.gameObject.GetComponent<Furnace>() == null && other.transform.root.gameObject.GetComponent<Building>() == null)
                other.transform.root.gameObject.GetComponent<IInteractable>().Interact(this.gameObject);
        }
    }

    public void CloseGatherMenu()
    {
        robotUI.SetActive(true);
        resourcesMenu.SetActive(false);
    }

    public void CloseInventoryMenu()
    {
        robotUI.SetActive(true);
        robotInventory.SetActive(false);
    }

    public void Assign()
    {
        interactingPlayer.GetComponent<TestCharacterController>().assignedRobot = this;
        target = interactingPlayer;
        SetActionFollow();
    }

    public void CloseRobotMenus()
    {
        robotUI.SetActive(false);
        robotInventory.SetActive(false);
        resourcesMenu.SetActive(false);

        interactingPlayer.GetComponent<TestCharacterController>().canMove = true;
        interactingPlayer.GetComponent<TestCharacterController>().mainController.UnblockMovement();
        interactingPlayer = null;

        isInteracting = false;
    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;
    }
}