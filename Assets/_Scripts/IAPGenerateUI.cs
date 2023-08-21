using System;
using System.Collections;
using System.Collections.Generic;
using Samples.Purchasing.Core.BuyingConsumables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IAPGenerateUI : MonoBehaviour
{
    public string[] shop_id_list;
    public Sprite[] imageShop;
    public bool[] newProduct;
    public string[] name;
    public GameObject cardPrefab;
    public Transform contentTransform;
    public BuyingConsumables consumableScript;
    public GameObject imgPrefabs;
    public string[] price;
    public string[] quantity;
    
    public void Start()
    {
        for (int i = 0; i < shop_id_list.Length; i++)
        {
            GenerateCard(i,shop_id_list[i]);
        }
       
    }

    private void GenerateCard(int index,string id_shop)
    {
        GameObject cardObject = Instantiate(cardPrefab);
        Transform cardTransform = cardObject.transform;
        cardTransform.SetParent(contentTransform);
        Vector3 newPosition = new Vector3(0f, -index * 100f, 0f); 
        cardTransform.localPosition = newPosition;
        cardTransform.localScale = Vector3.one;
        cardObject.transform.name = ""+name[index];
        consumableScript = cardObject.transform.GetChild(1).GetComponent<BuyingConsumables>();
        
        cardObject.transform.GetChild(3).gameObject.SetActive(newProduct[index]);
        cardObject.transform.GetChild(4).GetComponent<TMP_Text>().text = name[index];
        cardObject.transform.GetChild(5).GetComponent<TMP_Text>().text = quantity[index];
        
    
        cardObject.transform.GetChild(2).GetComponent<TMP_Text>().text = price[index];
        if (imageShop.Length != 0)
        {
            GenerateCardImage(index);
            imgPrefabs = cardObject.transform.GetChild(0).GetChild(1).gameObject;
        }
        else
        {
            Debug.LogWarning("Aucune image n'est mis dans le store !");
            imgPrefabs = cardObject.transform.GetChild(0).GetChild(0).gameObject;
        }

         
       
        if (id_shop == "unlock_game")
        {
           cardObject.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(consumableScript.BuyUnlockGame);
        }else if(id_shop == "powerup")
        {
          cardObject.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(consumableScript.BuyPowerUp1);
        }
        else if (id_shop == "powerup2")
        {
           cardObject.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(consumableScript.BuyPowerUp2);
        }
        else
        {
            Debug.LogWarning("Le produit demandé n'existe pas"); 
        } 
       
    }

    private void GenerateCardImage(int index)
    {
        try
        {
            imgPrefabs.GetComponent<Image>().sprite = imageShop[index];
        }
        catch (Exception e)
        {
            //Debug.LogError("Intégré les images pour chaque produit !"); 
        }
    }
}