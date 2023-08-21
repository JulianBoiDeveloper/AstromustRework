using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class RandomGunBox : MonoBehaviour
{
    public GameObject[] prefabsToSpawn;
    private GameObject[] spawnedObjects;

    [SerializeField] GameObject UI;
    [SerializeField] TMP_Text priceText;
    [SerializeField] GameObject[] weapons;
    TestCharacterController playerController;
    bool playerInsideArea = false;
    [SerializeField] int pointsPrice;

    public List<Transform> boxPositions;
    int currentBoxPos = 0;

    [SerializeField] Animator animator;

    PhotonView view;

    List<int> playersInAreaViewID;
    int interactingPlayerViewID;

    private AudioSource randomSoundManage;
    public AudioClip randomSound;
    public AudioClip randomOpenSound;
    bool interacting = false;
    IAPsTracker iAPsTracker;
    bool gunTaken = false;

    private void OnEnable()
    {
        // Subscribe to the event when the script is enabled
        ThirdPersonControllerV2.OnInteractDungeon += OnInteractDungeon;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event when the script is disabled
        ThirdPersonControllerV2.OnInteractDungeon -= OnInteractDungeon;
    }


    private void Start()
    {
        view = GetComponent<PhotonView>();

        transform.position = boxPositions[0].position;

        priceText.text = pointsPrice.ToString() + " points";
        UI.SetActive(false);
        spawnedObjects = new GameObject[prefabsToSpawn.Length];
        SpawnObjects();
        interactingPlayerViewID = -1;
        playersInAreaViewID = new List<int>();
        randomSoundManage = GetComponent<AudioSource>();
        iAPsTracker = GameObject.FindObjectOfType<IAPsTracker>();
    }

    private void Update()
    {
        Interact(-1);
    }

    public void Interact(int viewID)
    {
        if (iAPsTracker != null)
        {
            if (!iAPsTracker.unlockGame)
            {
                return;
            }
        }

        if (interactingPlayerViewID == -1 && playersInAreaViewID.Contains(viewID))
            interactingPlayerViewID = viewID;

        if (playerInsideArea && interactingPlayerViewID != -1)
        {
            if (!isGunSpawned)
            {
                randomSoundManage.clip = randomOpenSound;
                randomSoundManage.PlayOneShot(randomOpenSound);
                randomSoundManage.clip = randomSound;
                randomSoundManage.PlayOneShot(randomSound);
                animator.Play("openTreasureChest");
                gunIndex = Random.Range(0, spawnedObjects.Length);
                //HideGuns();
                interacting = false;
                isFinished = false;
            }
            else if (isGunSpawned && !gunTaken)
            {
                if ((Input.GetKeyDown(KeyCode.R) || interacting))
                {
                    TakeGun();
                    interacting = false;
                    gunTaken = true;
                }
            }
        }

        LoopGuns();
    }

    [PunRPC]
    public void SetInteractingPlayer(int viewID)
    {
        interactingPlayerViewID = viewID;
    }

    public void SetInteractingPlayerPUN(int myViewID)
    {
        if (interactingPlayerViewID != -1) return;

        view.RPC("SetInteractingPlayer", RpcTarget.All, myViewID);
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < prefabsToSpawn.Length; i++)
        {
            spawnedObjects[i] = Instantiate(prefabsToSpawn[i], transform.position + new Vector3(0f, 1.5f, 0f), transform.rotation);
            spawnedObjects[i].transform.localScale /= 6.5f;
            spawnedObjects[i].SetActive(false);
        }
    }

    float loopGunDuration = 4.2f;
    float loopTimer = 0f;
    float showGunTimer = 0f;
    float showGunDuration = 0.12f;
    int gunIndex;
    bool isFinished = true;

    bool isGunSpawned = false;
    float spawnedGunTimer = 0f;
    float spawnedGunDuration = 5f;
    GameObject latestObject = null;

    float openBoxDelayTimer = 0f;
    float openBoxDelayDuration = 0.5f;

    void LoopGuns()
    {
        if (isGunSpawned)
        {
            spawnedGunTimer += Time.deltaTime;
            if (spawnedGunTimer > spawnedGunDuration)
            {
                Destroy(latestObject);
                spawnedGunTimer = 0f;
                animator.Play("closeTreasureChest");
                openBoxDelayTimer = 0f;
            }
        }

        if (isFinished) return;

        loopTimer += Time.deltaTime;
        if (loopTimer > loopGunDuration)
        {


            // spawnGun
            loopTimer = 0f;
         //   spawnedObjects[gunIndex].SetActive(false);

            latestObject = Instantiate(prefabsToSpawn[0], transform.position + new Vector3(0f, 1.5f, 0f), Quaternion.identity);
            latestObject.transform.localScale /= 6.5f;
            latestObject.SetActive(true);

            isFinished = true;
            isGunSpawned = true;
            openBoxDelayTimer = 0f;
        }
        else
        {
            openBoxDelayTimer += Time.deltaTime;

            if(openBoxDelayTimer > openBoxDelayDuration) {
                showGunTimer += Time.deltaTime;
                if (showGunTimer > showGunDuration)
                {
                    HideGuns();
                    gunIndex++;
                    if (gunIndex >= spawnedObjects.Length)
                    {
                        gunIndex = 0;
                    }
                    spawnedObjects[gunIndex].SetActive(true);
                    showGunTimer = 0;
                }
            }
        }
    }

    public void TakeGun()
    {
        GameObject go = PhotonNetwork.Instantiate(weapons[gunIndex].name, new Vector3(100, 100, 100), Quaternion.identity);
        go.SetActive(false);
        playerController.inventory.AddMaterial(go.GetComponent<Item1>());
        latestObject.SetActive(false);
        HideGuns();
        //   MoveBoxPosition();
        animator.Play("closeTreasureChest");
    }

    public void MoveBoxPosition()
    {
        gunTaken = false;
        isGunSpawned = false;
        currentBoxPos++;
        if (currentBoxPos >= boxPositions.Count) currentBoxPos = 0;
        transform.position = boxPositions[currentBoxPos].position;

        for (int i = 0; i < spawnedObjects.Length; i++)
        {
            spawnedObjects[i].transform.position = transform.position + new Vector3(0f, 1.5f, 0f);
        }

        interactingPlayerViewID = -1;
    }

    void HideGuns()
    {
        for (int i = 0; i < spawnedObjects.Length; i++)
        {
            spawnedObjects[i].SetActive(false);
        }
    }

    public void ActivateWallGun(TestCharacterController player)
    {
        Debug.Log("Player in area");
        playerInsideArea = true;
        playerController = player;
        playersInAreaViewID.Add(player.mainController.GetComponent<PhotonView>().ViewID);
        UI.SetActive(true);
    }

    public void DeactivateWallGun(TestCharacterController player)
    {
        Debug.Log("Player left area");
        playerInsideArea = false;
        playerController = null;
        UI.SetActive(false);
        playersInAreaViewID.Remove(player.mainController.GetComponent<PhotonView>().ViewID);
    }

    private void OnInteractDungeon(int viewID)
    {
        if (iAPsTracker != null)
        {
            if (!iAPsTracker.unlockGame)
            {
                return;
            }
        }

        if (PhotonNetwork.LocalPlayer.GetScore() >= pointsPrice) {
            PhotonNetwork.LocalPlayer.AddScore(-pointsPrice);
            interacting = true;
            Interact(viewID);
        }
        else if(isGunSpawned && !gunTaken)
        {
            interacting = true;
            Interact(viewID);
        }
    }
}