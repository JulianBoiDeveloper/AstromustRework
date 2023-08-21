using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckObjID : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Camera id: " + gameObject.GetInstanceID());
    }

}
