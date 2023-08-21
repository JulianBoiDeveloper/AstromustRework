using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroMaterial : MonoBehaviour, IInteractable
{
    public List<Recipe> recipe;
    [HideInInspector] public int amount = 1;
    public int maxAmountStack;

    public bool canBePicked = true;
    public bool canBeDropped = true;
    public bool canGoInInventory = true;
    public bool canBeStacked = true;
    public bool canBeTakenFromInventory= true;

    [System.Serializable]
    public struct Recipe
    {
        public AstroMaterial material;
        public int amount;
    }

    public void Interact(GameObject go)
    {
        Debug.Log("Interact2");
        //go.GetComponent<TestCharacterController>().inventory.AddMaterial(this);
        //    if (amount <= 0) this.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }
}