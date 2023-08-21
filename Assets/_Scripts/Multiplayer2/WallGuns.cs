using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;

public class WallGuns : MonoBehaviour
{
    [SerializeField] GameObject UI;
    [SerializeField] TMP_Text priceText;
    [SerializeField] GameObject weapon;

    [SerializeField] int pointsPrice;
    bool playerInsideArea = false;
    TestCharacterController playerController;

    [SerializeField] int boostType = 0;

    private AudioSource cashSoundManage;
    public AudioClip cashSound;

    bool interacting = false;

    public bool isOpen;
    public bool unlock_game;
    public bool powerupA;
    public bool powerupB;

    IAPsTracker iapstracker;

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
        priceText.text = pointsPrice.ToString() + " points";
        UI.SetActive(false);
        cashSoundManage = GetComponent<AudioSource>();

        iapstracker = GameObject.FindObjectOfType<IAPsTracker>();
    }


    private void Update()
    {
        if (!isOpen)
        {
            if (iapstracker != null)
            {
                Debug.Log("Have a shop");
                if (unlock_game)
                {
                    if (!iapstracker.unlockGame)
                    {
                        return;
                    }
                }
                else if (powerupA)
                {
                    if (!iapstracker.unlockBoost1)
                    {
                        return;
                    }
                }
                else if (powerupB)
                {
                    if (!iapstracker.unlockBoost2)
                    {
                        return;
                    }
                }
            }
        }

        if (playerInsideArea && (Input.GetKeyDown(KeyCode.R) || interacting) && PhotonNetwork.LocalPlayer.GetScore() >= pointsPrice)
        {
            interacting = false;

            cashSoundManage.clip = cashSound;
            cashSoundManage.PlayOneShot(cashSound);
            PhotonNetwork.LocalPlayer.AddScore(-pointsPrice);
            Debug.Log("Spawning gun");

            if (weapon != null)
            {
                // Destroy previous gun if you have one in your inventory
                if (weapon.GetComponent<GunID>() != null) {
                    for(int i = 0; i < playerController.inventory.slots.Count; i++)
                    {
                        if (playerController.inventory.slots[i].item != null)
                        {
                            if (playerController.inventory.slots[i].item.gameObject.GetComponent<GunID>() != null)
                            {
                                playerController.inventory.slots[i].amount = 0;
                                playerController.inventory.slots[i].UpdateUI();
                                playerController.inventory.slots[i].Clear();
                            }
                        }
                    }
                }

                GameObject go = PhotonNetwork.Instantiate(weapon.name, new Vector3(100, 100, 100), Quaternion.identity);
                go.SetActive(false);
                playerController.inventory.AddMaterial(go.GetComponent<Item1>());
            }
            else
            {
                playerController.mainController.SetPowerUP(boostType);
            }
        }
    }


    public void ActivateWallGun(TestCharacterController player)
    {
        Debug.Log("Player in area");
        playerInsideArea = true;
        playerController = player;
        UI.SetActive(true);
    }

    public void DeactivateWallGun()
    {
        Debug.Log("Player left area");
        playerInsideArea = false;
        playerController = null;
        UI.SetActive(false);
    }

    private void OnInteractDungeon(int viewID)
    {
        if (!playerInsideArea) return;

        if (!isOpen)
        {
            if (iapstracker != null)
            {
                Debug.Log("Have a shop");
                if (unlock_game)
                {
                    if (!iapstracker.unlockGame)
                    {
                        return;
                    }
                }
                else if (powerupA)
                {
                    if (!iapstracker.unlockBoost1)
                    {
                        return;
                    }
                }
                else if (powerupB)
                {
                    if (!iapstracker.unlockBoost2)
                    {
                        return;
                    }
                }
            }
        }

        interacting = true;
    }
}
