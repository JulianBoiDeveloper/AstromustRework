using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPsTracker : MonoBehaviour
{
    public bool unlockGame = false;
    public bool unlockBoost1 = false;
    public bool unlockBoost2 = false;

    public int boost1amount = 0;
    public int boost2amount = 0;

    public static IAPsTracker _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
