using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalResourcesMined : MonoBehaviour
{
    private static GlobalResourcesMined _instance;

    public static GlobalResourcesMined Instance { get { return _instance; } }


    public delegate void NewResourceMinedEventHandler();
    public static event NewResourceMinedEventHandler NewResourceMinedEvent;
    [HideInInspector] public List<string> allResourcesIDs;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        allResourcesIDs = new List<string>();
    }
    public void UnlockResourceRobot(Resource resource)
    {
        if(!allResourcesIDs.Contains(resource.id))
        {
            allResourcesIDs.Add(resource.id);
            Debug.Log("New resource mined!");
            NewResourceMinedEvent?.Invoke();
            // send signal robot?
        }
    }
}