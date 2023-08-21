using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItems : MonoBehaviour
{
    public List<GameObject> items; // unique list


    private static GameItems _instance;

    public static GameItems Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameItems>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public GameObject GetItemByID(string id)
    {
        foreach (GameObject element in items)
        {
            if(element != null)
                if (element.GetComponent<Item1>() != null && element.GetComponent<Item1>().id == id) 
                    return element;
        }

        return null;
    }
}
