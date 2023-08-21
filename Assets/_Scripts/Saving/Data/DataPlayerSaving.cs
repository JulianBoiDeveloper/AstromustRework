using System;
using UnityEngine;


namespace Astromust.Saving
{
     [Serializable]
    public class DataPlayerSaving
    {
        // Specific data of player 
        public Vector3 position;
        public Vector3 rotation;
        // Data for player UI
        public float healthy, hunger, thirsty;
        // Exp Level
        public int expLevel;
        // Level
        public int levelPlayer;
        // Slot blocked with 8 quantities
        public DataSlotsSaving[] inventory = new DataSlotsSaving[8];
        // Slot for the wearable inventory with 4 quantities
        public DataSlotsSaving[] wearableInventory = new DataSlotsSaving[4];
        // Robot Target Assigned
        public DataRobotSaving[] robotAssigned;
        // Story progression
        public float progressionStory;

    }
    
}
