using System;
using System.Linq;
using UnityEngine;

namespace Astromust.Saving
{
    [RequireComponent(typeof(DataGeneralSaving))]
    // All objects saving with tag = "SavingObject"
    public class SavingTransformObject : MonoBehaviour
    {
        public GameObject[] objsSaving;
        public  Vector3[] position;
        public  Vector3[] rotation;
        public bool checkSavingObject;

        private GameObject[] objs;
        

        private void Update()
        {
            if (checkSavingObject)
            {
                SavingObject();
                checkSavingObject = false;
            }
        }

        public void SavingObject()
        {
            objs = FindObjectsOfType<GameObject>();
            objsSaving = new GameObject[objs.Length]; // Resize the array to match the number of objects
            position = new Vector3[objs.Length]; 
            rotation = new Vector3[objs.Length]; 
            int index = 0;
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].tag == "SavingObject")
                {
                    objsSaving[index] = objs[i];
                    position[index] = objs[i].transform.position;
                    rotation[index] = objs[i].transform.rotation.eulerAngles;
                    index++;
                }
            }
    
            objsSaving = objsSaving.Where(obj => obj != null).ToArray(); // Remove null element
            position = position.Where(pos => pos != null).ToArray();
            rotation = rotation.Where(rot => rot != null).ToArray();

            GetComponent<DataGeneralSaving>().objAllPositionWithTag.objWithInstanceId = objsSaving;
            GetComponent<DataGeneralSaving>().objAllPositionWithTag.position = position;
            GetComponent<DataGeneralSaving>().objAllPositionWithTag.rotation = rotation;
        }
    }
}