using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astromust.Saving
{
    [Serializable]
    public class DataObjectMapSaving 
    {
        public GameObject[] objWithInstanceId;
        public Vector3[] position;
        public Vector3[] rotation;
    }
}