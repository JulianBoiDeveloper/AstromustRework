using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectIDInstantiate : MonoBehaviour
{
    public int instanceID;
    public GameObject obj;
    int targetInstanceID; // The instance ID of the target GameObject you want to find
    public void Start()
    {
        obj = FindGameObjectByInstanceID(instanceID);
    }
  

    GameObject FindGameObjectByInstanceID(int instanceID)
    {
        Object[] gameObjects = Object.FindObjectsOfType<GameObject>(); // Get all GameObjects in the scene

        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i].GetInstanceID() == instanceID)
            {
                return gameObjects[i] as GameObject;
            }
        }

        return null; // Return null if the GameObject is not found
    }
}
