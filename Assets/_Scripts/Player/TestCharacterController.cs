using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Newtonsoft.Json.Serialization;

public class TestCharacterController : MonoBehaviour
{
    public Inventory1 inventory;
    float raycastDistance = 15f;
    public bool canMove = true;

    public Inventory1 otherInventory;
    public GameObject personalCrafting;
    public LayerMask IgnoreMe;

    [SerializeField] public ThirdPersonControllerV2 mainController;
    [SerializeField] GameObject buildingUI;

    [SerializeField] Camera cam;
    [SerializeField] GameObject handle;
    [SerializeField] Animator animator;

    public GameObject mobileUI;

    [SerializeField] bool isAI = false;

    [HideInInspector] public List<Transform> flyToMeItems;

    [HideInInspector] public AICharacters assignedRobot;

    [SerializeField] PhotonView view;

    [SerializeField] GameObject powerUP;
    public int extraDamage = 1;

    Vector3 defaultPos = new Vector3(0f, -0.145f, 0f);
    void Start()
    {
        if (!view.IsMine) this.gameObject.SetActive(false);

        flyToMeItems = new List<Transform>();

        if(!isAI) {
        //    Cursor.lockState = CursorLockMode.Locked;
            if (buildingUI != null)
                buildingUI.SetActive(false);
            else
                Debug.LogWarning("Building UI is null. You might want to add the reference to it");
        }

        if (IAPsTracker._instance == null || IAPsTracker._instance.boost1amount <= 0)
        {
            useSwordButton.SetActive(false);
        }
        if (IAPsTracker._instance == null || IAPsTracker._instance.boost2amount <= 0)
        {
            useShieldButton.SetActive(false);
        }
    }

    void Update()
    {
        //        if (!view.IsMine) return;

        transform.localPosition = defaultPos;

        if (!isAI)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                personalCrafting.SetActive(true);
                canMove = false;
            }
            else if(Input.GetKeyDown(KeyCode.Escape)) {
                personalCrafting.SetActive(false);
                mobileUI.SetActive(true);
                canMove = true;
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                animator.SetBool("Jump", true);
            }

            if(Input.GetKeyDown(KeyCode.B))
            {
                buildingUI.SetActive(!buildingUI.activeInHierarchy);
            }

            Interact();

            // Count down the shoot timer.
            if (isShooting && shootTimer > 0f)
            {
                shootTimer -= Time.deltaTime;

                if (shootTimer <= 0f)
                {
                    // After the delay, if the player is still shooting, transition to the "Idle" state.
                    if (isShooting)
                    {
                        animator.SetBool("isShooting", false);
                    }
                    shootTimer = shootDuration;
                    isShooting = false;
                }
            }
        }

//#if UNITY_ANDROID
 //       AttachToolToCharacter(true);
//#endif
        FlyItems();
    }

    public bool HasGun()
    {
        if (hasGunWindows) {
            return true;
        }

        if (otherInventory.slots[0].item != null)
        {
            if (otherInventory.slots[0].item.gameObject.GetComponent<ToolStats>().tooltype == ToolStats.ToolType.gun)
            {
                animator.SetBool("hasGun", true);
                return true;
            }
        }
        animator.SetBool("hasGun", false);
        //   if(mainController.isGrounded) animator.Play("Idle Walk Run Blend");
        return false;
    }

    public List<GunID> gunsInHandle;
    bool hasItemHandle = false;
    bool hasGunWindows = false;

    public void AttachToolToCharacter(bool isMouseWheel = false)
    {
        if(isMouseWheel)
        {
            // Hide all guns in case one was visible
            if (handle.transform.childCount > 0)
            {
                hasItemHandle = false;
                for (int i = 0; i < gunsInHandle.Count; i++)
                {
                    gunsInHandle[i].gameObject.SetActive(false);
                }
            }

            // Show current gun


            if (inventory.slots[inventory.selectedSlot].item != null && inventory.slots[inventory.selectedSlot].item.gameObject.GetComponent<GunID>() != null) {
                int gunID = inventory.slots[inventory.selectedSlot].item.gameObject.GetComponent<GunID>().ID;
                mainController.AttachGun(gunID);

                Debug.Log("gun attached!!!");

                //   if (inventory.slots[inventory.selectedSlot].item.gameObject.GetComponent<ToolStats>().tooltype == ToolStats.ToolType.gun)
                //   {
                animator.SetBool("hasGun", true);

                animator.Play("RifleIdle");
                //  }
                hasGunWindows = true;
                hasItemHandle = true;
                return;
            }
            else
            {
                hasGunWindows = false;
                animator.SetBool("hasGun", false);
            }
        }
        else {
            hasGunWindows = false;
            if (handle != null)
            {
                if (otherInventory.slots[0].item != null)
                {
                    if (!hasItemHandle) {
                        int gunID = otherInventory.slots[0].item.gameObject.GetComponent<GunID>().ID;
                        mainController.AttachGun(gunID);

                        if (otherInventory.slots[0].item.gameObject.GetComponent<ToolStats>().tooltype == ToolStats.ToolType.gun)
                        {
                            animator.Play("RifleIdle");
                        }
                        hasItemHandle = true;
                    }
                    /*
                    if (handle.transform.childCount == 0)
                    {
                        GameObject go = GameObject.Instantiate(GameItems.Instance.GetItemByID(otherInventory.slots[0].item.id));

                        if (go.GetComponent<ToolStats>().tooltype == ToolStats.ToolType.gun)
                        {
                            animator.Play("RifleIdle");
                        }

                        // Get all components attached to the GameObject
                        Component[] components = go.GetComponents<Component>();

                        // Iterate through the components and destroy them
                        for (int i = 0; i < components.Length; i++)
                        {
                            // Skip Transform component to avoid removing the GameObject itself
                            if (components[i] is Transform)
                                continue;

                            Destroy(components[i]);
                        }

                        RemoveCollidersRecursively(go.transform);

                        go.transform.parent = handle.transform;
                        go.transform.localPosition = Vector3.zero;
                        go.transform.localRotation = Quaternion.identity;
                    }*/
                }
                else if (handle.transform.childCount > 0)
                {
                    hasItemHandle = false;
                    for (int i = 0; i < gunsInHandle.Count; i++)
                    {
                        gunsInHandle[i].gameObject.SetActive(false);
                    }
                  //  Destroy(handle.transform.GetChild(0).gameObject);
                }
            }
        }
    }

    public void SetPowerUP()
    {
     //   Debug.Log("Set power up");
     //   view.RPC("ActivatePowerUp", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void ActivatePowerUp()
    {
        powerUP.SetActive(true);
    }

    private void RemoveCollidersRecursively(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            Collider[] colliders = child.GetComponents<Collider>();
            for (int j = 0; j < colliders.Length; j++)
            {
                Destroy(colliders[j]);
            }

            RemoveCollidersRecursively(child);
        }
    }

    void FlyItems()
    {
        for(int i = 0; i < flyToMeItems.Count; i++)
        {
            Vector3 flyDir = transform.position - flyToMeItems[i].position;
            flyToMeItems[i].position += flyDir * 4f * Time.deltaTime;
        }
        for (int i = 0; i < flyToMeItems.Count; i++)
        {
            if (!flyToMeItems[i].gameObject.activeInHierarchy) flyToMeItems.RemoveAt(i);
        }
    }

    public void OpenCraftingMenu()
    {
        personalCrafting.SetActive(true);
        canMove = false;
        mainController.BlockMovement();
    }

    public void CloseCraftingMenu()
    {
        personalCrafting.SetActive(false);
        mobileUI.SetActive(true);
        canMove = true;
        mainController.UnblockMovement();
        GlobalItemDescription.Instance.HideUI();
    }

    //[SerializeField] FixedTouchField fixedTouchField;

    public void TryInteract(bool isUse = true)
    {
        // Gun shooting
        if(!isUse) {
            if (!isAI && HasGun())
            {
                animator.SetBool("isShooting", true);

                //     cam.GetComponent<ScreenShake>().Shake();
                int gunID = -1;
                shootTimer = shootDuration;
                isShooting = true;

                for (int i = 0; i < gunsInHandle.Count; i++)
                {
                    // Play VFX always if gun has them
                    if (gunsInHandle[i].gameObject.activeInHierarchy)
                    {
                        gunID = i;
                        Debug.Log("gunid found: " + gunID);
                        if (gunsInHandle[i].transform.childCount > 0)
                        {
                            mainController.GunVFX(i);
                            mainController.PlayShot(i);
                            Debug.Log("Playing shot fx");
                        }
                        break;
                    }
                }

                if (gunID == -1) Debug.Log("NO GUN ID FOUND");

                RaycastHit hit;
                Camera mainCamera = Camera.main;

                // Calculate the middle screen position
                Vector3 middleScreenPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
                
                // Create a ray from the camera towards the middle of the screen
                Ray ray = mainCamera.ScreenPointToRay(middleScreenPosition);
                Debug.DrawRay(ray.origin, ray.direction, Color.green);


                // Hit enemies on point
                // add gun distance here
                if (gunID != -1)
                {
                    if (Physics.Raycast(ray, out hit, gunsInHandle[gunID].attackRange, ~IgnoreMe))
                    {
                        IInteractable guninteractable = hit.transform.gameObject.GetComponent<IInteractable>();

                        if (guninteractable != null && hit.transform.gameObject.GetComponent<EnemyAI>() != null)
                        {
                            hit.transform.gameObject.GetComponent<IInteractable>().Interact(this.gameObject);
                        }
                    }
                }
                //      }
            }
        }
        else {
            if (currentInteractable == null) return;
                currentInteractable.Interact(this.gameObject);

            if(currentOutlineInteraction != null)
            {
                if(currentOutlineInteraction.gameObject.GetComponent<Resource>() != null)
                {
                    animator.Play("RobotMine");
                }
            }
        }
    }


    float interactTimer = 0f;
    float interactDuration = 1f;
    [HideInInspector] public Outline currentOutlineInteraction;
    IInteractable currentInteractable;
    private bool isShooting = false;
    private float shootDuration = 0.3f; // Adjust this value to set the delay before transitioning back to Idle.
    private float shootTimer = 0f;

    public void Interact(GameObject target = null)
    {
        RaycastHit hit;
        Camera mainCamera = Camera.main;

        // Calculate the middle screen position
        Vector3 middleScreenPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        // Create a ray from the camera towards the middle of the screen
        Ray ray = mainCamera.ScreenPointToRay(middleScreenPosition);
        Debug.DrawRay(ray.origin, ray.direction,  Color.green);

        if (Physics.Raycast(ray, out hit, 15f, ~IgnoreMe))
        {
            //Debug.Log("raycast hit: " + hit.transform.gameObject.name);

            if(!GlobalItemDescription.Instance.isBuilding) {
                if(hit.transform.gameObject.GetComponent<IInteractable>() != null && hit.transform.gameObject.GetComponent<AICharacters>() == null && hit.transform.gameObject.GetComponent<NPCQuest>() == null)
                {
                    if (currentOutlineInteraction != null && currentOutlineInteraction != hit.transform.gameObject.GetComponent<Item1>().outlineEffect) currentOutlineInteraction.enabled = false;
                    currentOutlineInteraction = hit.transform.gameObject.GetComponent<Item1>().outlineEffect;
                    if(currentOutlineInteraction != null) currentOutlineInteraction.enabled = true;
                }
                else if (currentOutlineInteraction != null)
                {
                    currentOutlineInteraction.enabled = false;
                    currentInteractable = null;
                }

                if(Vector3.Distance(hit.transform.position, transform.position) > 3f && currentOutlineInteraction != null)
                {
                    currentOutlineInteraction.enabled = false;
                    currentInteractable = null;
                }
            }
            else
            {
                if(currentOutlineInteraction != null)
                    currentOutlineInteraction.enabled = false;
            }

            if (!isAI) {
            //    if (currentInteractable == null) return;

                currentInteractable = hit.transform.gameObject.GetComponent<IInteractable>();

                if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
                {
                    if (hit.transform.gameObject.GetComponent<IInteractable>() != null && hit.transform.gameObject.GetComponent<EnemyAI>() == null)
                    {
                        hit.transform.gameObject.GetComponent<IInteractable>().Interact(this.gameObject);

                        if(hit.transform.gameObject.GetComponent<Resource>() != null)
                        {
                            animator.Play("RobotMine");
                        }
                    }
                }

                if(Input.GetKeyDown(KeyCode.R))
                {
                    Debug.Log("hit: " + hit.transform.gameObject.name);
                    if(hit.transform.gameObject.GetComponent<RandomGunBox>() != null)
                    {
                        Debug.Log("Interacting box");
                        mainController.SetInteractingPlayer(hit.transform.gameObject.GetComponent<RandomGunBox>());
                    }
                }
            }
        }
        else if(currentOutlineInteraction != null)
        {
            currentInteractable = null;
            currentOutlineInteraction.enabled = false;
        }

#if UNITY_WIN_STANDALONE

        // Gun shooting
        if (!isAI && HasGun())
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                animator.SetBool("isShooting", true);

                //     cam.GetComponent<ScreenShake>().Shake();
                int gunID = -1;
                shootTimer = shootDuration;
                isShooting = true;

                for (int i = 0; i < gunsInHandle.Count; i++)
                {
                    // Play VFX always if gun has them
                    if (gunsInHandle[i].gameObject.activeInHierarchy)
                    {
                        gunID = i;
                        Debug.Log("gunid found: " + gunID);
                        if (gunsInHandle[i].transform.childCount > 0)
                        {
                            mainController.GunVFX(i);
                            mainController.PlayShot(i);
                            Debug.Log("Playing shot fx");
                        }
                        break;
                    }
                }

                if(gunID == -1) Debug.Log("NO GUN ID FOUND");

                // Hit enemies on point
                // add gun distance here
                if (gunID != -1) {
                    if (Physics.Raycast(ray, out hit, gunsInHandle[gunID].attackRange, ~IgnoreMe))
                    {
                        IInteractable guninteractable = hit.transform.gameObject.GetComponent<IInteractable>();

                        if (guninteractable != null && hit.transform.gameObject.GetComponent<EnemyAI>() != null)
                        {
                            hit.transform.gameObject.GetComponent<IInteractable>().Interact(this.gameObject);
                        }
                    }
                }
            }
        }
#endif

        if (isAI)
        {
            interactTimer += Time.deltaTime;
            if (interactTimer > interactDuration)
            {
                if (target.transform.gameObject.GetComponent<IInteractable>() != null)
                {
                    target.transform.gameObject.GetComponent<IInteractable>().Interact(this.gameObject);
                }
                interactTimer = 0f;
            }
        }
    }

    [SerializeField] GameObject useSwordButton;
    [SerializeField] GameObject useShieldButton;

    public void UseSword()
    {
        if (IAPsTracker._instance != null && IAPsTracker._instance.boost1amount > 0)
        {
            IAPsTracker._instance.boost1amount--;
            mainController.SetPowerUP(0);
     //       IAPsTracker._instance.unlockBoost1 = true;
            CloudSave.Instance.SaveEntry("iap_boost1", IAPsTracker._instance.boost1amount.ToString());
        }
        useSwordButton.SetActive(false);
    }

    public void UseShield()
    {
        if (IAPsTracker._instance != null && IAPsTracker._instance.boost2amount > 0)
        {
            IAPsTracker._instance.boost2amount--;
            mainController.SetPowerUP(1);
        //    IAPsTracker._instance.unlockBoost2 = true;
            CloudSave.Instance.SaveEntry("iap_boost2", IAPsTracker._instance.boost2amount.ToString());
        }
        useShieldButton.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!view.IsMine) return;

        if (isAI) return;
        if(other.transform.root.gameObject.GetComponent<IInteractable>() != null && other.gameObject.GetComponent<Turret>() == null)
        {
            if(other.transform.root.gameObject.GetComponent<Item1>() != null && other.transform.root.gameObject.GetComponent<Crafting>() == null 
                && other.transform.root.gameObject.GetComponent<Resource>() == null && other.transform.root.gameObject.GetComponent<Furnace>() == null && other.transform.root.gameObject.GetComponent<Building>() == null && other.transform.root.gameObject.GetComponent<EnemyAI>() == null) {
                other.transform.root.gameObject.GetComponent<IInteractable>().Interact(this.gameObject);
            }
        }

        if(other.gameObject.GetComponent<WallGuns>() != null)
        {
            other.gameObject.GetComponent<WallGuns>().ActivateWallGun(this);
        }

        if (other.gameObject.GetComponent<RandomGunBox>() != null)
        {
            other.gameObject.GetComponent<RandomGunBox>().ActivateWallGun(this);
        }

        if (other.gameObject.GetComponent<PowerUp>() != null)
        {
            SetPowerUP();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!view.IsMine) return;

        if (isAI) return;

        if (other.transform.root.gameObject.GetComponent<Water>() != null)
        {
            for (int i = 0; i < inventory.slots.Count; i++)
            {
                if (inventory.slots[i].item != null)
                {
                    Consumable1 consumable = inventory.slots[i].item.gameObject.GetComponent<Consumable1>();
                    if (consumable != null)
                    {
                        if (consumable.type == Consumable1.ConsumableType.drink)
                        {
                            consumable.Refill();
                            inventory.slots[i].UpdateUI();
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!view.IsMine) return;

        if (isAI) return;

        if (other.gameObject.GetComponent<WallGuns>() != null)
        {
            other.gameObject.GetComponent<WallGuns>().DeactivateWallGun();
        }

        if (other.gameObject.GetComponent<RandomGunBox>() != null)
        {
            other.gameObject.GetComponent<RandomGunBox>().DeactivateWallGun(this);
        }
    }
}