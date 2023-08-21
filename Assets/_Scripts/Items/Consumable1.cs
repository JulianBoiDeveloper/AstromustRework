using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable1 : MonoBehaviour
{
    public List<Sprite> icons;
    [HideInInspector] public Item1 myItem;
    public bool canConsume = true;
    [SerializeField] public int currentConsumedLevel = 0;

    private void Start()
    {
        myItem = GetComponent<Item1>();
    }


    public enum ConsumableType
    {
        food,
        drink,
        potion
    }

    public ConsumableType type;
    public float amount;

    public void ConsumeDrink()
    {
        if (!canConsume) return;

        currentConsumedLevel++;
        if(currentConsumedLevel >= icons.Count)
        {
            canConsume = false;
            currentConsumedLevel = icons.Count - 1;
        }
        Debug.Log("here1233");
        myItem.icon = icons[currentConsumedLevel];
    }

    public void Refill()
    {
        canConsume = true;
        myItem.icon = icons[0];
        currentConsumedLevel = 0;
    }
}
