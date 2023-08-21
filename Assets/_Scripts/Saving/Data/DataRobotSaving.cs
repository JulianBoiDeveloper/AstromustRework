
using System;
using UnityEngine;

namespace Astromust.Saving
{
    [Serializable]
    public class DataRobotSaving
    {
        public Vector3 position;
        public DataPlayerSaving assigned;
        public bool follow;
        public EnumChoiceTechUsed action;  
        public GameObject workRessources;
        public Item1 equiped;
        public DataChestRobotSaving inventory;
    }

}
