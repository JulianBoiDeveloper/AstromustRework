using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMaterials : MonoBehaviour
{
    [SerializeField] public List<Recipe> recipe;

    [System.Serializable]
    public struct Recipe
    {
        public Item1 material;
        public int amount;
    }

    public bool UseMaterials(Inventory1 inventory)
    {
        List<Recipe> clonedRecipe = new List<Recipe>();
        for(int i = 0; i < recipe.Count; i++)
        {
            clonedRecipe.Add(recipe[i]);
        }

        for(int i = 0; i < clonedRecipe.Count; i++)
        {
            for(int j = 0; j < inventory.slots.Count; j++)
            {
                if(inventory.slots[j].item != null) {
                    if(clonedRecipe[i].material.id == inventory.slots[j].item.id)
                    {
                        if(inventory.slots[j].item.amount > clonedRecipe[i].amount)
                        {
                            Recipe updatedRecipe = clonedRecipe[i]; // Create a copy of the struct
                            updatedRecipe.amount = 0; // Modify the amount field of the copied struct
                            clonedRecipe[i] = updatedRecipe; // Assign the updated struct back to the list
                            break;
                        }
                        else
                        {
                            int copyAmount = clonedRecipe[i].amount;
                            for (int k = 0; k < inventory.slots[j].amount; k++)
                            {
                                copyAmount--;
                            }
                            Recipe updatedRecipe = clonedRecipe[i]; // Create a copy of the struct
                            updatedRecipe.amount = copyAmount; // Modify the amount field of the copied struct
                            clonedRecipe[i] = updatedRecipe; // Assign the updated struct back to the list
                        }
                    }
                }
            }
        }

        // CHECK IF CLONED RECIPE AMOUNTS IS ZERO, IF SO, ALLOW BUILDING

        for(int i = 0; i < clonedRecipe.Count; i++)
        {
            if (clonedRecipe[i].amount > 0) return false;
        }

        for (int i = 0; i < recipe.Count; i++)
        {
            for (int j = 0; j < inventory.slots.Count; j++)
            {
                int amountToRemove = recipe[i].amount;

                if (inventory.slots[j].item != null)
                {
                    if (recipe[i].material.id == inventory.slots[j].item.id && amountToRemove > 0)
                    {

                        if (inventory.slots[j].item.amount > recipe[i].amount)
                        {
                            for(int k = 0; k < recipe[i].amount; k++) {
                                inventory.slots[j].RemoveOne();
                                amountToRemove--;
                                if (amountToRemove <= 0) break;
                            }
                            break;
                        }
                        else
                        {
                            for (int k = 0; k < inventory.slots[j].amount; k++)
                            {
                                inventory.slots[j].RemoveOne();
                                amountToRemove--;
                                if (amountToRemove <= 0) break;
                            }
                        }
                    }
                }
            }
        }

        return true;
    }
}
